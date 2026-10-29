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
    private readonly Color32 captionColor = new Color32(93, 110, 128, 255);
    private readonly Color32 disabledFace = new Color32(238, 243, 243, 255);

    // Thứ tự theo design (en, fr, es, pt, vi); ngôn ngữ chưa có level hiển thị "sắp có"/"đề xuất".
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

        // Vị trí và cỡ theo design (css px x 1,6914, gốc là tâm canvas 660x1397).
        AddLabel(content, font, "lang.ui", 519, 34, 27, FontStyle.Bold, headingColor);
        AddLabel(content, font, "lang.uiDesc", 449, 66, 24, FontStyle.Normal, captionColor);
        AddLabel(content, font, "lang.game", 34, 34, 27, FontStyle.Bold, headingColor);
        AddLabel(content, font, "lang.gameDesc", -20, 33, 24, FontStyle.Normal, captionColor);
        var loc = Locator<LocalizationController>.Instance;
        var allPlayable = true;
        foreach (var gameCode in gameCodes) allPlayable &= loc.IsGamePlayable(gameCode);
        if (!allPlayable) AddLabel(content, font, "lang.noteSoon", -694, 25, 20, FontStyle.Normal, captionColor, TextAnchor.MiddleCenter);

        for (var i = 0; i < gameCodes.Length; i++)
        {
            var code = gameCodes[i];
            var playable = loc.IsGamePlayable(code);
            var row = Instantiate(template.gameObject, content).GetComponent<RectTransform>();
            row.name = "GameOption_" + code;
            row.GetComponent<Button>().enabled = false;
            row.anchoredPosition = new Vector2(0, -112 - 123.5f * i);
            row.sizeDelta = new Vector2(599, 110);

            var name = row.Find("Text").GetComponent<RectTransform>();
            name.anchorMin = new Vector2(0.5f, 0.5f);
            name.anchorMax = new Vector2(0.5f, 0.5f);
            name.pivot = new Vector2(0, 0.5f);
            name.sizeDelta = new Vector2(545, 34);
            name.anchoredPosition = new Vector2(-272.5f, 14);
            name.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            name.GetComponent<Text>().fontSize = 27;
            name.GetComponent<LocalizedText>().SetKey("lang." + code);

            var caption = Instantiate(name.gameObject, row).GetComponent<RectTransform>();
            caption.name = "Caption";
            caption.sizeDelta = new Vector2(545, 25);
            caption.anchoredPosition = new Vector2(-272.5f, -19);
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
                row.gameObject.AddComponent<CanvasGroup>().alpha = 0.7f;
            }

            gameRowCodes.Add(code);
            gameBorders.Add(row.GetComponent<Image>());
            gameFaces.Add(row.Find("Face").GetComponent<Image>());
        }
    }

    private void AddLabel(RectTransform parent, Font font, string key, float y, float height, int size, FontStyle style, Color color, TextAnchor alignment = TextAnchor.MiddleLeft)
    {
        var go = new GameObject("Label_" + key, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchoredPosition = new Vector2(0, y);
        rect.sizeDelta = new Vector2(599, height);
        var text = go.AddComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.lineSpacing = 0.92f;
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
            gameFaces[i].color = selected ? selectedFace : (loc.IsGamePlayable(gameRowCodes[i]) ? normalFace : disabledFace);
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
