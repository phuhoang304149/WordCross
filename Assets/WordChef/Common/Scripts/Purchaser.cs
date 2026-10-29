using System;
using UnityEngine;
#if CATGAMING_IAP
using catgamingmodule.Monetization;
using catgamingmodule.Purchase;
using catgamingmodule.Tool.Database;
using Script.Utils;
using UnityEngine.Purchasing;
using PurchaseService = catgamingmodule.Purchase.PurchaseService;
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
        if (Locator<GameDatabase>.Instance == null) Locator<GameDatabase>.Set(GameDatabase.LoadInstance());
        if (Locator<ProductService>.Instance == null) Locator<ProductService>.Set(new ProductService());
        purchaseService = new PurchaseService();
        purchaseService.PendingPurchase += OnPendingPurchase;
        purchaseService.NoAdsRestored += OnNoAdsRestored;
        purchaseService.Init();
#else
        Debug.Log("Purchaser.InitializePurchasing: CATGAMING_IAP is not defined.");
#endif
    }

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
#if CATGAMING_ADS && CATGAMING_ADMOB
        CatGamingAds.RemoveAds(true);
#endif
    }
#endif

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
