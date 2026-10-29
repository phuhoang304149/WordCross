using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

// Chừa vùng banner AdMob dưới vòng chữ ở màn Main (design: .banner cao 50 + lề dưới 12, chỉ khi chưa tắt quảng cáo).
// Bố cục trong scene là bố cục không banner; có banner thì đẩy cả cụm vòng chữ + nút công cụ lên đúng 62 css.
public class BannerSpace : MonoBehaviour
{
    private const float DesignWidth = 390f;
    private const float BannerCss = 50f, MarginCss = 12f;

    public RectTransform[] bottomItems;

    private float shift;
    private bool reserved;
    private GameObject placeholder;

    private void Start()
    {
        StartCoroutine(Watch());
    }

    private IEnumerator Watch()
    {
        while (true)
        {
            bool removed = CUtils.IsBuyItem() || CUtils.IsAdsRemoved();
            if (!removed && !reserved) Reserve();
            else if (removed && reserved) Release();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Reserve()
    {
        var canvas = MainController.instance.levelNameText.canvas.rootCanvas;
        float perCss = ((RectTransform)canvas.transform).rect.width / DesignWidth;
        float inset = Screen.safeArea.y / canvas.scaleFactor;
        shift = (BannerCss + MarginCss) * perCss + inset;
        reserved = true;
        Move(shift);
        ShowPlaceholder(canvas, perCss, inset);
    }

    private void Release()
    {
        Move(-shift);
        shift = 0;
        reserved = false;
        if (placeholder != null) Destroy(placeholder);
    }

    private void Move(float dy)
    {
        for (int i = 0; i < bottomItems.Length; i++)
        {
            bottomItems[i].anchoredPosition += new Vector2(0, dy);
        }
    }

    private void ShowPlaceholder(Canvas canvas, float perCss, float inset)
    {
#if GAME_DEBUG || DEVELOPMENT_BUILD || UNITY_EDITOR
        // Khung giữ chỗ theo design (.banner): chỉ ở bản debug, bản thật no-fill để trống vùng.
        float k = 3f;
        int w = Mathf.RoundToInt((DesignWidth - 32) * k), h = Mathf.RoundToInt(BannerCss * k);
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var line = new Color32(0xb8, 0xc8, 0xc8, 255);
        var clear = new Color32(0, 0, 0, 0);
        float border = 2 * k, radius = 10 * k;
        var pixels = new Color32[w * h];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float cx = Mathf.Clamp(x + 0.5f, radius, w - radius), cy = Mathf.Clamp(y + 0.5f, radius, h - radius);
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(cx, cy));
                bool edge = (x + 0.5f < radius || x + 0.5f > w - radius || y + 0.5f < radius || y + 0.5f > h - radius)
                    ? d <= radius && d > radius - border
                    : (x < border || x >= w - border || y < border || y >= h - border);
                float run = (x + 0.5f < radius || x + 0.5f > w - radius) && (y + 0.5f > radius && y + 0.5f < h - radius) ? y : x;
                bool dash = Mathf.FloorToInt(run / (2 * border)) % 2 == 0;
                pixels[y * w + x] = edge && dash ? line : clear;
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();

        placeholder = new GameObject("BannerPlaceholder", typeof(RectTransform), typeof(RawImage));
        var rect = (RectTransform)placeholder.transform;
        rect.SetParent(canvas.transform, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0);
        rect.pivot = new Vector2(0.5f, 0);
        rect.sizeDelta = new Vector2((DesignWidth - 32) * perCss, BannerCss * perCss);
        rect.anchoredPosition = new Vector2(0, inset + MarginCss * perCss);
        var img = placeholder.GetComponent<RawImage>();
        img.texture = tex;
        img.raycastTarget = false;

        var label = new GameObject("Label", typeof(RectTransform), typeof(Text));
        var lrect = (RectTransform)label.transform;
        lrect.SetParent(rect, false);
        lrect.anchorMin = Vector2.zero;
        lrect.anchorMax = Vector2.one;
        lrect.offsetMin = lrect.offsetMax = Vector2.zero;
        var text = label.GetComponent<Text>();
        var source = MainController.instance.levelNameText;
        text.font = source.font;
        text.fontSize = Mathf.RoundToInt(11 * perCss);
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color32(0x8a, 0xa0, 0xa0, 255);
        text.raycastTarget = false;
        text.text = Locator<LocalizationController>.Instance.Get("game.banner");
#endif
    }
}
