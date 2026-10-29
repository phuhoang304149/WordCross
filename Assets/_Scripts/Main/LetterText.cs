using UnityEngine;
using UnityEngine.UI;

public class LetterText : MonoBehaviour
{
    public Image selectDisc;
    public Text selectGlyph;
    public Text letter;

    public Color idleColor;

    private bool selected;

    // Đĩa chọn và chữ trắng nằm trên nét nối (thiết kế: nét nối dưới các chữ).
    private void Awake()
    {
        RaiseAboveLine(selectDisc.gameObject, 6);
        RaiseAboveLine(selectGlyph.gameObject, 7);
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
