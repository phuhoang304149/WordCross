using System;
using UnityEngine;
#if CATGAMING_IAP
using catgamingmodule.Monetization;
using catgamingmodule.Purchase;
using catgamingmodule.Tool.Database;
using Script.Utils;
using UnityEngine.Purchasing;
using PurchaseService = catgamingmodule.Purchase.PurchaseService;
#if CATGAMING_ADDRESSABLES
using CatGaming.Clr;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif
#endif

public enum PType { Consumable, NonConsumable, Subscription };

[Serializable]
public class IAPItem
{
    public string productID;
    public PType productType;
    public float price;
    public int value;
}

// Nối luồng mua với PurchaseService của catgamingmodule (Unity IAP 5, không cần Nakama).
// Catalog lấy từ ProductData đăng ký trong GameDatabase; storeId phải trùng iapItems[i].productID.
public class Purchaser : MonoBehaviour
{
    public IAPItem[] iapItems;

    public static Purchaser instance;
    public Action<IAPItem, int> onItemPurchased;

#if CATGAMING_IAP
    private PurchaseService purchaseService;
#if CATGAMING_ADDRESSABLES
    private AsyncOperationHandle<GameDatabase> gameDatabaseHandle;
#endif
#endif

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializePurchasing();
    }

    private void OnDestroy()
    {
#if CATGAMING_IAP
        if (purchaseService == null) return;
        purchaseService.PendingPurchase -= OnPendingPurchase;
        purchaseService.NoAdsRestored -= OnNoAdsRestored;
        purchaseService.Dispose();
        purchaseService = null;
#endif
    }

    public void InitializePurchasing()
    {
#if CATGAMING_IAP
        if (purchaseService != null) return;
        if (Locator<GameDatabase>.Instance == null)
        {
#if CATGAMING_ADDRESSABLES
            // Cùng address package với MainModule; ClrContent chỉ alias Assets/Resources/GameDatabase.asset.
            if (gameDatabaseHandle.IsValid()) return;
            gameDatabaseHandle = Addressables.LoadAssetAsync<GameDatabase>("GameDatabase/" + Application.identifier.ToLowerInvariant());
            ClrContent.Instance.RetainAddressable(gameDatabaseHandle);
            gameDatabaseHandle.Completed += OnGameDatabaseLoaded;
            return;
#else
            Locator<GameDatabase>.Set(GameDatabase.LoadInstance());
#endif
        }
        if (Locator<ProductService>.Instance == null) Locator<ProductService>.Set(new ProductService());
        purchaseService = new PurchaseService();
        purchaseService.PendingPurchase += OnPendingPurchase;
        purchaseService.NoAdsRestored += OnNoAdsRestored;
        purchaseService.Init();
#else
        Debug.Log("Purchaser.InitializePurchasing: CATGAMING_IAP is not defined.");
#endif
    }

#if CATGAMING_IAP && CATGAMING_ADDRESSABLES
    private void OnGameDatabaseLoaded(AsyncOperationHandle<GameDatabase> operation)
    {
        if (operation.Status != AsyncOperationStatus.Succeeded || operation.Result == null)
        {
            Debug.LogError("Purchaser: cannot load GameDatabase/" + Application.identifier.ToLowerInvariant());
            return;
        }
        if (Locator<GameDatabase>.Instance == null) Locator<GameDatabase>.Set(operation.Result);
        Debug.Log("Purchaser: GameDatabase ready, initializing purchasing.");
        InitializePurchasing();
    }
#endif

    public void BuyProduct(int index)
    {
#if CATGAMING_IAP
        if (purchaseService == null || !purchaseService.IsInitialized)
        {
            Debug.LogWarning("Purchaser.BuyProduct: store is not ready. Product: " + iapItems[index].productID);
            InitializePurchasing();
            return;
        }

        purchaseService.BuyProduct(iapItems[index].productID);
#else
        Debug.Log("Purchaser.BuyProduct: CATGAMING_IAP is not defined. Product: " + iapItems[index].productID);
#endif
    }

    public void RestorePurchases()
    {
#if CATGAMING_IAP
        if (purchaseService != null) purchaseService.FetchPurchases();
#endif
    }

#if CATGAMING_IAP
    // Cộng thưởng trước, rồi mới xác nhận đơn với store để đơn lỗi được giao lại ở lần mở sau.
    private void OnPendingPurchase(PendingOrder order)
    {
        if (order == null || order.CartOrdered == null) return;

        foreach (CartItem cartItem in order.CartOrdered.Items())
        {
            if (cartItem == null || cartItem.Product == null || cartItem.Product.definition == null) continue;
            ProcessPurchase(cartItem.Product.definition.id);
        }

        purchaseService.ConfirmPurchase(order);
    }

    private void OnNoAdsRestored(ConfirmedOrder order)
    {
        GrantNoAds();
    }
#endif

    // Đường xử lý chung sau khi mua gói tắt quảng cáo: ghi cờ buy_item (PromoteController không xoá được), ẩn banner ngay.
    public void GrantNoAds()
    {
        CUtils.SetBuyItem();
#if CATGAMING_ADS && CATGAMING_ADMOB
        CatGamingAds.RemoveAds(true);
#endif
        CUtils.CloseBannerAd();
    }

    public void ProcessPurchase(string productID)
    {
        IAPItem item = null;
        foreach (var i in iapItems)
        {
            if (String.Equals(productID, i.productID, StringComparison.Ordinal))
            {
                item = i;
            }
        }

        if (item == null)
        {
            Debug.LogWarning("Purchaser.ProcessPurchase: unknown product " + productID);
            return;
        }

        if (onItemPurchased != null) onItemPurchased(item, Array.IndexOf(iapItems, item));
    }
}
