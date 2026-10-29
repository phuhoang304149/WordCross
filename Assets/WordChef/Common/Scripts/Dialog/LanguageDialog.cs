using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

public class LanguageDialog : Dialog
{
    public string[] codes;
    public Image[] borders;
    public Image[] faces;
    public GameObject savedNote;

    private readonly Color32 selectedBorder = new Color32(18, 181, 166, 255);
    private readonly Color32 selectedFace = new Color32(216, 245, 241, 255);
    private readonly Color32 normalBorder = new Color32(223, 233, 234, 255);
    private readonly Color32 normalFace = new Color32(255, 255, 255, 255);

    protected override void Start()
    {
        base.Start();
        Refresh();
    }

    public void OnOptionClick(string code)
    {
        Sound.instance.PlayButton();
        Locator<LocalizationController>.Instance.SetLanguage(code);
        Refresh();
        savedNote.SetActive(true);
    }

    public void OnCloseClick()
    {
        Sound.instance.PlayButton();
        Close();
    }

    private void Refresh()
    {
        var current = Locator<LocalizationController>.Instance.Language;
        for (var i = 0; i < codes.Length; i++)
        {
            var selected = codes[i] == current;
            borders[i].color = selected ? selectedBorder : normalBorder;
            faces[i].color = selected ? selectedFace : normalFace;
        }
    }
}
