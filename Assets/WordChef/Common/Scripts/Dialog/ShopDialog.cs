using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

public class ShopDialog : Dialog
{
    public TMP_Text[] numRubyTexts, priceTexts;
    public Button[] buyButtons;
    // Hàng gói tắt quảng cáo (word.noads) ẩn khi đã tắt quảng cáo; thay bằng dòng "Đã tắt quảng cáo".
    public GameObject noAdsRow;
    public TMP_Text noteText, adsOffText;

    // Đơn không phản hồi (mất callback) thì mở lại nút mua sau thời gian này.
    private const float PROCESSING_TIMEOUT = 60f;
    private bool processing;

    protected override void Start()
    {
        base.Start();
#if CATGAMING_IAP
        Purchaser.instance.onItemPurchased += OnItemPurchased;
        Purchaser.instance.onPurchaseState += OnPurchaseState;
        Purchaser.instance.onProductsReady += UpdatePacks;
        Locator<LocalizationController>.Instance.onLanguageChanged += OnLanguageChanged;
        UpdatePacks();
#endif
    }

#if CATGAMING_IAP
    private void OnLanguageChanged(string code)
    {
        UpdatePacks();
    }

    // Ruby của gói lấy từ cấu hình kinh tế (GD shop.packs); giá lấy từ store (localizedPriceString), chưa có thì giữ giá cấu hình sẵn.
    private void UpdatePacks()
    {
        var loc = Locator<LocalizationController>.Instance;
        var econ = Locator<EconomyService>.Instance;
        for(int i = 0; i < numRubyTexts.Length; i++)
        {
            var item = Purchaser.instance.iapItems[i];
            var pack = econ == null ? null : econ.PackFor(item.productID);
            int ruby = pack != null ? pack.coin : item.value;
            SetPackLabel(numRubyTexts[i], loc, ruby);
            priceTexts[i].text = Purchaser.instance.LocalizedPrice(i) ?? (item.price + "$");
        }

        bool adsOff = CUtils.IsBuyItem();
        if (noAdsRow != null) noAdsRow.SetActive(!adsOff);
        if (adsOffText != null)
        {
            adsOffText.gameObject.SetActive(adsOff);
            adsOffText.text = loc.Get("shop.adsOff");
        }
        RefreshNote();
    }

    // Hàng gói có LocalizedText (key shop.pack) thì đặt key/tham số trên đó để đổi ngôn ngữ không ghi đè số cũ; ruby = 0 là hàng gói tắt quảng cáo.
    private void SetPackLabel(TMP_Text label, LocalizationController loc, int ruby)
    {
        var localized = label.GetComponent<LocalizedText>();
        if (localized != null)
        {
            localized.SetKey(ruby > 0 ? "shop.pack" : "shop.pk_noads");
            if (ruby > 0) localized.SetArg("n", ruby);
            return;
        }
        label.text = ruby > 0 ? loc.Get("shop.pack", "n", ruby) : loc.Get("shop.pk_noads");
    }

    private void RefreshNote()
    {
        if (noteText != null) noteText.text = Locator<LocalizationController>.Instance.Get(processing ? "shop.buying" : "shop.note");
    }

    private void SetProcessing(bool value)
    {
        processing = value;
        CancelInvoke("EndProcessing");
        if (value) Invoke("EndProcessing", PROCESSING_TIMEOUT);
        foreach (var button in buyButtons)
            if (button != null) button.interactable = !value;
        RefreshNote();
    }

    private void EndProcessing()
    {
        SetProcessing(false);
    }
#endif

    public void OnBuyProduct(int index)
	{
#if CATGAMING_IAP
        Sound.instance.PlayButton();
        if (processing) return;
        Purchaser.instance.BuyProduct(index);
#else
        Debug.LogError("Please enable, import and install Unity IAP to use this function");
#endif
    }

#if CATGAMING_IAP
    // Thưởng đã được Purchaser.ProcessPurchase cộng (ledger theo transactionId); Shop chỉ làm mới hàng gói (ẩn gói tắt quảng cáo khi đã mua).
    private void OnItemPurchased(IAPItem item, int index)
    {
        UpdatePacks();
    }

    private void OnPurchaseState(PurchaseUiState state, string productId)
    {
        SetProcessing(state == PurchaseUiState.Processing);
        string key = null;
        switch (state)
        {
            case PurchaseUiState.Success: key = "shop.success"; break;
            case PurchaseUiState.Cancelled: key = "shop.cancelled"; break;
            case PurchaseUiState.Error: key = "shop.error"; break;
            case PurchaseUiState.StoreNotReady: key = "shop.storeNotReady"; break;
        }
        if (key != null) Toast.instance.ShowMessage(Locator<LocalizationController>.Instance.Get(key));
    }
#endif

#if CATGAMING_IAP
    private void OnDestroy()
    {
        if (Purchaser.instance != null)
        {
            Purchaser.instance.onItemPurchased -= OnItemPurchased;
            Purchaser.instance.onPurchaseState -= OnPurchaseState;
            Purchaser.instance.onProductsReady -= UpdatePacks;
        }
        if (Locator<LocalizationController>.Instance != null)
            Locator<LocalizationController>.Instance.onLanguageChanged -= OnLanguageChanged;
    }
#endif
}
