using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

namespace WordCross.Editor
{
    // T134 bước 4: thêm hàng gói tắt quảng cáo (word.noads), ghi chú quy tắc tắt ads, dòng "Đã tắt quảng cáo" và nối các tham chiếu mới vào ShopDialog.prefab.
    // Chạy lại nhiều lần vẫn an toàn (đã có hàng thứ 6 thì chỉ nối lại tham chiếu/bố cục).
    // Hàm static chỉ là điểm vào cho Unity (-executeMethod), phần việc nằm ở hàm instance.
    public class ShopPrefabSetup
    {
        private const string PrefabPath = "Assets/_Prefabs/Dialogs/ShopDialog.prefab";
        private const int RowCount = 6;
        // Toàn bộ nội dung dịch lên để nhường chỗ cho hàng thứ 6 + ghi chú; nền cao thêm tương ứng (canvas 720x1280).
        private const float Shift = 45f;
        private const float BackgroundHeight = 1260f;
        private const float RowSpacing = 4f;

        public static void Apply()
        {
            new ShopPrefabSetup().DoApply();
        }

        private void DoApply()
        {
            var root = PrefabUtility.LoadPrefabContents(PrefabPath);
            var dialog = root.GetComponent<ShopDialog>();
            var content = root.transform.Find("Content");
            var items = content.Find("Items");

            var rows = new Transform[RowCount];
            rows[0] = items.Find("ShopItem");
            for (int i = 1; i < 5; i++) rows[i] = items.Find("ShopItem (" + i + ")");
            rows[5] = items.Find("ShopItem (5)");
            if (rows[5] == null)
            {
                rows[5] = Object.Instantiate(rows[4].gameObject, items).transform;
                rows[5].name = "ShopItem (5)";
                rows[5].SetAsLastSibling();
            }

            var numTexts = new TMP_Text[RowCount];
            var priceTexts = new TMP_Text[RowCount];
            var buttons = new Button[RowCount];
            for (int i = 0; i < RowCount; i++)
            {
                numTexts[i] = FindText(rows[i], "Text");
                priceTexts[i] = rows[i].Find("Price").GetComponent<TMP_Text>();
                buttons[i] = rows[i].GetComponent<Button>();
            }
            // Hàng gói tắt quảng cáo không có biểu tượng ruby.
            rows[5].Find("Image").gameObject.SetActive(false);
            for (int i = 0; i < RowCount; i++) WireBuy(buttons[i], dialog, i);

            dialog.numRubyTexts = numTexts;
            dialog.priceTexts = priceTexts;
            dialog.buyButtons = buttons;
            dialog.noAdsRow = rows[5].gameObject;

            Layout(content, items);
            dialog.noteText = EnsureText(content, "Note", -566f, 72f, 20f, new Color32(0x5d, 0x6e, 0x80, 0xff), items);
            dialog.adsOffText = EnsureText(content, "AdsOff", -502f, 36f, 26f, new Color32(0x0b, 0x8c, 0x80, 0xff), items);
            dialog.adsOffText.gameObject.SetActive(false);

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[ShopPrefabSetup] done rows=" + RowCount);
        }

        // Text số ruby của hàng: con có tên bắt đầu bằng "Text" và là TMP (tên gốc lệch nhau: Text, Text (1)...).
        private TMP_Text FindText(Transform row, string prefix)
        {
            foreach (Transform child in row)
            {
                var text = child.GetComponent<TMP_Text>();
                if (text != null && child.name.StartsWith(prefix)) return text;
            }
            return null;
        }

        private void WireBuy(Button button, ShopDialog dialog, int index)
        {
            for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
                UnityEventTools.RemovePersistentListener(button.onClick, i);
            UnityEventTools.AddIntPersistentListener(button.onClick, dialog.OnBuyProduct, index);
        }

        private void Layout(Transform content, Transform items)
        {
            // Đã dịch ở lần chạy trước thì không dịch lại: nhận biết bằng vị trí Title (gốc 496).
            bool shifted = ((RectTransform)content.Find("Title")).anchoredPosition.y > 496f + 1f;
            foreach (Transform child in content)
            {
                if (shifted || child.name == "BG" || child.name == "Note" || child.name == "AdsOff" || child.name == "Items") continue;
                var rect = (RectTransform)child;
                rect.anchoredPosition += new Vector2(0f, Shift);
            }
            var bg = (RectTransform)content.Find("BG");
            bg.sizeDelta = new Vector2(bg.sizeDelta.x, BackgroundHeight);

            // Items neo mép trên để ẩn hàng gói tắt quảng cáo không làm các hàng còn lại nhảy vị trí.
            var rectItems = (RectTransform)items;
            rectItems.pivot = new Vector2(0.5f, 1f);
            rectItems.anchoredPosition = new Vector2(0f, 203f + Shift);
            items.GetComponent<VerticalLayoutGroup>().spacing = RowSpacing;
        }

        private TMP_Text EnsureText(Transform content, string name, float y, float height, float size, Color color, Transform items)
        {
            var existing = content.Find(name);
            GameObject go;
            if (existing != null) go = existing.gameObject;
            else
            {
                go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                go.transform.SetParent(content, false);
            }
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(530f, height);
            rect.anchoredPosition = new Vector2(0f, y);
            var text = go.GetComponent<TextMeshProUGUI>();
            var source = content.Find("Title").GetComponent<TMP_Text>();
            text.font = source.font;
            text.fontSharedMaterial = source.fontSharedMaterial;
            text.fontSize = size;
            text.enableAutoSizing = true;
            text.fontSizeMax = size;
            text.fontSizeMin = size * 0.7f;
            text.alignment = TextAlignmentOptions.Center;
            text.color = color;
            text.raycastTarget = false;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Truncate;
            return text;
        }
    }
}
