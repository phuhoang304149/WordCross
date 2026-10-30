using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterText : MonoBehaviour
{
    public Image selectDisc;
    public TMP_Text selectGlyph;
    public TMP_Text letter;

    public Color idleColor;

    private const float DesignWidth = 390f, GlyphDropCss = 1.3f;

    private bool selected;

    // Font game có chữ cái nằm cao hơn tâm dòng ~1,3 css (đo trên máy, tâm hộp bao chữ hoa so với vị trí chữ): hạ chữ để tâm chữ trùng tâm vị trí/đĩa.
    // Đĩa chọn và chữ trắng nằm trên nét nối (thiết kế: nét nối dưới các chữ); phải gắn sau khi đã vào canvas cha, nếu không Unity xoá overrideSorting.
    private void Start()
    {
        RaiseAboveLine(selectDisc.gameObject, 6);
        RaiseAboveLine(selectGlyph.gameObject, 7);
        RectTransform canvasRt = (RectTransform)letter.canvas.rootCanvas.transform;
        float localPerCss = canvasRt.rect.width / DesignWidth * canvasRt.lossyScale.x / letter.transform.lossyScale.x;
        RectTransform rect = letter.rectTransform;
        if (letter.preferredHeight > 0f)
        {
            rect.pivot = new Vector2(rect.pivot.x, 0.5f + GlyphDropCss * localPerCss / letter.preferredHeight);
        }
        // Rect chữ bị hạ cùng pivot nên đĩa (con của rect) cũng hạ theo: nâng đĩa lại để tâm đĩa trùng vị trí chữ, chữ trắng vẫn hạ như chữ thường.
        selectDisc.rectTransform.anchoredPosition = new Vector2(0f, GlyphDropCss * localPerCss);
        selectGlyph.rectTransform.anchoredPosition = new Vector2(0f, -GlyphDropCss * localPerCss);
    }

    private void RaiseAboveLine(GameObject target, int order)
    {
        Canvas canvas = target.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingLayerName = "MyLineRenderer";
        canvas.sortingOrder = order;
    }

    public bool IsSelected()
    {
        return selected;
    }

    public void SetSelected(bool isSelected)
    {
        selected = isSelected;
        selectGlyph.text = letter.text;
        selectDisc.enabled = isSelected;
        selectGlyph.enabled = isSelected;
        letter.color = isSelected ? Color.clear : idleColor;
    }
}
