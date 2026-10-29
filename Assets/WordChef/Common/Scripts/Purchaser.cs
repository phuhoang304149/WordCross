using System;
using UnityEngine;

public enum PType { Consumable, NonConsumable, Subscription };

[Serializable]
public class IAPItem
{
    public string productID;
    public PType productType;
    public float price;
    public int value;
}

// Unity IAP đã được gỡ. Giữ API cũ làm điểm gắn để tích hợp lại SDK thanh toán
// (ví dụ module Purchase của catgamingmodule); hiện không thực hiện giao dịch.
public class Purchaser : MonoBehaviour
{
    public IAPItem[] iapItems;

    public static Purchaser instance;
    public Action<IAPItem, int> onItemPurchased;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializePurchasing();
    }

    // Điểm gắn: khởi tạo SDK thanh toán với danh sách iapItems.
    public void InitializePurchasing()
    {
        Debug.Log("Purchaser.InitializePurchasing: IAP SDK not integrated.");
    }

    // Điểm gắn: mở giao dịch cho iapItems[index].
    public void BuyProduct(int index)
    {
        Debug.Log("Purchaser.BuyProduct: IAP SDK not integrated. Product: " + iapItems[index].productID);
    }

    // Điểm gắn: khôi phục giao dịch (iOS).
    public void RestorePurchases()
    {
        Debug.Log("Purchaser.RestorePurchases: IAP SDK not integrated.");
    }

    // SDK mới gọi hàm này khi giao dịch thành công.
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

        if (onItemPurchased != null) onItemPurchased(item, Array.IndexOf(iapItems, item));
    }
}
