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

// Trạng thái giao dịch báo cho Shop (đang xử lý / thành công / huỷ / lỗi / store chưa sẵn sàng).
public enum PurchaseUiState { Processing, Success, Cancelled, Error, StoreNotReady }

[Serializable]
public class IAPItem
{
    public string productID;
    public PType productType;
    public float price;
    public int value;
    public bool removeAds;
}

// Nối luồng mua với PurchaseService của catgamingmodule (Unity IAP 5, không cần Nakama).
// Catalog lấy từ ProductData đăng ký trong GameDatabase; storeId phải trùng iapItems[i].productID.
public class Purchaser : MonoBehaviour
{
    public IAPItem[] iapItems;

    public static Purchaser instance;
    public Action<IAPItem, int> onItemPurchased;
    public Action<PurchaseUiState, string> onPurchaseState;
    public Action onProductsReady;

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
        purchaseService.PurchaseRequested -= OnPurchaseRequested;
        purchaseService.ProductsReady -= OnProductsReady;
        purchaseService.purchaseFailure -= OnPurchaseFailed;
        purchaseService.failure -= OnPurchaseRejected;
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
        purchaseService.PurchaseRequested += OnPurchaseRequested;
        purchaseService.ProductsReady += OnProductsReady;
        purchaseService.purchaseFailure += OnPurchaseFailed;
        purchaseService.failure += OnPurchaseRejected;
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
            NotifyState(PurchaseUiState.StoreNotReady, iapItems[index].productID);
            return;
        }

        purchaseService.BuyProduct(iapItems[index].productID);
#else
        Debug.Log("Purchaser.BuyProduct: CATGAMING_IAP is not defined. Product: " + iapItems[index].productID);
#endif
    }

    // Giá hiển thị do store trả (localizedPriceString); null khi store chưa sẵn sàng hoặc sản phẩm chưa có trên store.
    public string LocalizedPrice(int index)
    {
#if CATGAMING_IAP
        if (purchaseService == null || !purchaseService.IsInitialized) return null;
        var product = purchaseService.GetProductById(iapItems[index].productID);
        if (product == null || product.metadata == null || string.IsNullOrEmpty(product.metadata.localizedPriceString)) return null;
        return product.metadata.localizedPriceString;
#else
        return null;
#endif
    }

    private void NotifyState(PurchaseUiState state, string productId)
    {
        if (onPurchaseState != null) onPurchaseState(state, productId);
    }

    public void RestorePurchases()
    {
#if CATGAMING_IAP
        if (purchaseService != null) purchaseService.FetchPurchases();
#endif
    }

#if CATGAMING_IAP
    private void OnPurchaseRequested(Product product)
    {
        NotifyState(PurchaseUiState.Processing, product == null ? null : product.definition.id);
    }

    private void OnProductsReady(System.Collections.Generic.List<Product> products)
    {
        if (onProductsReady != null) onProductsReady();
    }

    private void OnPurchaseFailed(FailedOrder failed)
    {
        bool cancelled = failed != null && failed.FailureReason == PurchaseFailureReason.UserCancelled;
        NotifyState(cancelled ? PurchaseUiState.Cancelled : PurchaseUiState.Error, null);
    }

    private void OnPurchaseRejected(PurchaseFailureReason reason)
    {
        NotifyState(reason == PurchaseFailureReason.StoreNotConnected ? PurchaseUiState.StoreNotReady : PurchaseUiState.Error, null);
    }

    // Ghi ledger + cộng ruby trước, rồi mới xác nhận đơn với store để đơn lỗi được giao lại ở lần mở sau.
    // Thiếu transactionId hoặc EconomyService chưa sẵn sàng thì không cộng, không xác nhận: đơn còn pending và được giao lại.
    private void OnPendingPurchase(PendingOrder order)
    {
        if (order == null || order.CartOrdered == null) return;

        string transactionId = order.Info == null ? null : order.Info.TransactionID;
        bool handled = true;
        foreach (CartItem cartItem in order.CartOrdered.Items())
        {
            if (cartItem == null || cartItem.Product == null || cartItem.Product.definition == null) continue;
            if (!ProcessPurchase(cartItem.Product.definition.id, transactionId)) handled = false;
        }

        if (handled) purchaseService.ConfirmPurchase(order);
        else NotifyState(PurchaseUiState.Error, null);
    }

    private void OnNoAdsRestored(ConfirmedOrder order)
    {
        GrantNoAds();
    }
#endif

    // Đường xử lý chung sau khi mua gói tắt quảng cáo (hoặc khôi phục): ghi cờ buy_item = cờ tắt quảng cáo duy nhất (người chơi cũ đã có cờ này vẫn được giữ; PromoteController không xoá được), ẩn banner ngay.
    public void GrantNoAds()
    {
        CUtils.SetBuyItem();
#if CATGAMING_ADS && CATGAMING_ADMOB
        CatGamingAds.RemoveAds(true);
#endif
        CUtils.CloseBannerAd();
    }

    // Trả true khi đơn đã được xử lý xong (kể cả đơn đã cộng trước đó hoặc product lạ) và có thể xác nhận với store.
    public bool ProcessPurchase(string productID, string transactionId)
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
            return true;
        }

        var econ = Script.Utils.Locator<WordCross.Gameplay.EconomyService>.Instance;
        if (econ == null || string.IsNullOrEmpty(transactionId))
        {
            Debug.LogWarning("Purchaser.ProcessPurchase: hold order " + productID + " (economy ready=" + (econ != null) + ", transactionId=" + transactionId + ")");
            return false;
        }

        // Cấp thưởng ở đây (không phụ thuộc Shop đang mở) để đơn giao lại lúc khởi động hay đơn chậm vẫn được cộng; ledger theo transactionId chặn cộng lặp.
        bool granted = econ.GrantPurchase(transactionId, productID);

        // Cờ tắt quảng cáo (buy_item) chỉ bật khi gói có removeAds: word.noads, word.item4, word.item5. Gói item1–3 chỉ cộng ruby. Idempotent nên đơn gửi lại vẫn an toàn.
        if (item.removeAds) GrantNoAds();

        if (granted && onItemPurchased != null) onItemPurchased(item, Array.IndexOf(iapItems, item));
        if (granted) NotifyState(PurchaseUiState.Success, productID);
        return true;
    }
}
