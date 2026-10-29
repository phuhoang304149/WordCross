using UnityEngine;
using UnityEngine.UI;

public class LetterText : MonoBehaviour
{
    public Image selectDisc;
    public Text selectGlyph;
    public Text letter;

    public Color idleColor;

    private bool selected;

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
