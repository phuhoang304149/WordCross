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
    private void OnItemPurchased(IAPItem item, int index)
    {
        // A consumable product has been purchased by this user.
        if (item.productType == PType.Consumable)
        {
            CurrencyController.CreditBalance(item.value);
            Toast.instance.ShowMessage(Locator<LocalizationController>.Instance.Get("shop.success"));
            CUtils.SetBuyItem();
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (item.productType == PType.NonConsumable)
        {
            // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
        }
        // Or ... a subscription product has been purchased by this user.
        else if (item.productType == PType.Subscription)
        {
            // TODO: The subscription item has been successfully purchased, grant this to the player.
        }
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
