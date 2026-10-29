using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
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
    private readonly Color32 headingColor = new Color32(31, 45, 61, 255);
    private readonly Color32 captionColor = new Color32(107, 122, 133, 255);

    // Thứ tự theo design: en chơi được, fr/es/pt sắp có, vi đề xuất.
    private readonly string[] gameCodes = { "en", "fr", "es", "pt", "vi" };
    private readonly List<string> gameRowCodes = new List<string>();
    private readonly List<Image> gameBorders = new List<Image>();
    private readonly List<Image> gameFaces = new List<Image>();

    protected override void Start()
    {
        base.Start();
        BuildGameSection();
        Refresh();
    }

    public void OnGameOptionClick(string code)
    {
        Sound.instance.PlayButton();
        Locator<LocalizationController>.Instance.SetGameLanguage(code);
        Refresh();
        savedNote.SetActive(true);
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

    // Ngôn ngữ chơi tách khỏi ngôn ngữ UI: heading/mô tả cho cả hai phần và các dòng ngôn ngữ chơi (nhân bản từ Option_en).
    private void BuildGameSection()
    {
        var content = transform.Find("Content") as RectTransform;
        var font = content.Find("Title").GetComponent<Text>().font;
        var template = content.Find("Option_en") as RectTransform;

        AddLabel(content, font, "lang.ui", 445, 34, 30, FontStyle.Bold, headingColor);
        AddLabel(content, font, "lang.uiDesc", 398, 58, 21, FontStyle.Normal, captionColor);
        AddLabel(content, font, "lang.game", 45, 34, 30, FontStyle.Bold, headingColor);
        AddLabel(content, font, "lang.gameDesc", -2, 58, 21, FontStyle.Normal, captionColor);
        AddLabel(content, font, "lang.noteSoon", -470, 70, 21, FontStyle.Normal, captionColor);

        var loc = Locator<LocalizationController>.Instance;
        for (var i = 0; i < gameCodes.Length; i++)
        {
            var code = gameCodes[i];
            var playable = loc.IsGamePlayable(code);
            var row = Instantiate(template.gameObject, content).GetComponent<RectTransform>();
            row.name = "GameOption_" + code;
            row.GetComponent<Button>().enabled = false;
            row.anchoredPosition = new Vector2(0, -70 - 78 * i);
            row.sizeDelta = new Vector2(540, 70);

            var name = row.Find("Text").GetComponent<RectTransform>();
            name.anchorMin = new Vector2(0, 0.5f);
            name.anchorMax = new Vector2(1, 1);
            name.offsetMin = new Vector2(24, 0);
            name.offsetMax = new Vector2(-24, -6);
            name.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            name.GetComponent<Text>().fontSize = 28;
            name.GetComponent<LocalizedText>().SetKey("lang." + code);

            var caption = Instantiate(name.gameObject, row).GetComponent<RectTransform>();
            caption.name = "Caption";
            caption.anchorMin = new Vector2(0, 0);
            caption.anchorMax = new Vector2(1, 0.5f);
            caption.offsetMin = new Vector2(24, 8);
            caption.offsetMax = new Vector2(-24, 0);
            var captionText = caption.GetComponent<Text>();
            captionText.fontSize = 20;
            captionText.color = captionColor;
            caption.GetComponent<LocalizedText>().SetKey(playable ? "lang.playable" : code == "vi" ? "lang.soonVi" : "lang.soon");

            if (playable)
            {
                var trigger = row.gameObject.AddComponent<EventTrigger>();
                var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
                entry.callback.AddListener(data => OnGameOptionClick(code));
                trigger.triggers.Add(entry);
            }
            else
            {
                row.gameObject.AddComponent<CanvasGroup>().alpha = 0.55f;
            }

            gameRowCodes.Add(code);
            gameBorders.Add(row.GetComponent<Image>());
            gameFaces.Add(row.Find("Face").GetComponent<Image>());
        }
    }

    private void AddLabel(RectTransform parent, Font font, string key, float y, float height, int size, FontStyle style, Color color)
    {
        var go = new GameObject("Label_" + key, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchoredPosition = new Vector2(0, y);
        rect.sizeDelta = new Vector2(540, height);
        var text = go.AddComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAnchor.MiddleLeft;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        go.AddComponent<LocalizedText>().SetKey(key);
    }

    private void Refresh()
    {
        var loc = Locator<LocalizationController>.Instance;
        for (var i = 0; i < gameRowCodes.Count; i++)
        {
            var selected = gameRowCodes[i] == loc.GameLanguage && loc.IsGamePlayable(gameRowCodes[i]);
            gameBorders[i].color = selected ? selectedBorder : normalBorder;
            gameFaces[i].color = selected ? selectedFace : normalFace;
        }
        var current = loc.Language;
        for (var i = 0; i < codes.Length; i++)
        {
            var selected = codes[i] == current;
            borders[i].color = selected ? selectedBorder : normalBorder;
            faces[i].color = selected ? selectedFace : normalFace;
        }
    }
}
