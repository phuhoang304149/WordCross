using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

public class ShopDialog : Dialog
{
    public Text[] numRubyTexts, priceTexts;

    protected override void Start()
    {
        base.Start();
#if CATGAMING_IAP
        Purchaser.instance.onItemPurchased += OnItemPurchased;
        Locator<LocalizationController>.Instance.onLanguageChanged += OnLanguageChanged;
        UpdatePacks();
#endif
    }

#if CATGAMING_IAP
    private void OnLanguageChanged(string code)
    {
        UpdatePacks();
    }

    private void UpdatePacks()
    {
        for(int i = 0; i < numRubyTexts.Length; i++)
        {
            numRubyTexts[i].text = Locator<LocalizationController>.Instance.Get("shop.pack", "n", Purchaser.instance.iapItems[i].value);
            priceTexts[i].text = Purchaser.instance.iapItems[i].price + "$";
        }
    }
#endif

    public void OnBuyProduct(int index)
	{
#if CATGAMING_IAP
        Sound.instance.PlayButton();
        Purchaser.instance.BuyProduct(index);
#else
        Debug.LogError("Please enable, import and install Unity IAP to use this function");
#endif
    }

#if CATGAMING_IAP
    // Thưởng đã được Purchaser.ProcessPurchase cộng; Shop chỉ báo toast cho gói tiêu thụ.
    private void OnItemPurchased(IAPItem item, int index)
    {
        if (item.productType == PType.Consumable)
            Toast.instance.ShowMessage(Locator<LocalizationController>.Instance.Get("shop.success"));
    }
#endif

#if CATGAMING_IAP
    private void OnDestroy()
    {
        Purchaser.instance.onItemPurchased -= OnItemPurchased;
        if (Locator<LocalizationController>.Instance != null)
            Locator<LocalizationController>.Instance.onLanguageChanged -= OnLanguageChanged;
    }
#endif
}
