using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class TextPreview : MonoBehaviour {
    public GameObject content;
    public RectTransform backgroundRT, textRT;
    public string word;
    public TMP_Text text;

    [Header("")]
    public Color answerColor;
    public Color validColor;
    public Color wrongColor;
    public Color existColor;
    public Color defaultColor;

    public static TextPreview instance;

    // Design (.preview b): mỗi chữ một ô 38 css cao, rộng tối thiểu 32, lề 8, cách nhau 4, chữ 20 css nâu #5a4100.
    private const float DesignWidth = 390f, ChipHeight = 38f, ChipMinWidth = 32f, ChipPad = 8f, ChipGap = 4f, ChipFont = 22.2f;
    private readonly List<GameObject> chips = new List<GameObject>();

    private void Awake()
    {
        instance = this;
        backgroundRT.GetComponent<Image>().enabled = false;
        text.enabled = false;
    }

    private float LocalPerCss()
    {
        RectTransform canvasRt = (RectTransform)content.transform.GetComponentInParent<Canvas>(true).rootCanvas.transform;
        return canvasRt.rect.width / DesignWidth * canvasRt.lossyScale.x / content.transform.lossyScale.x;
    }

    private void BuildChips(string word)
    {
        foreach (var chip in chips) Destroy(chip);
        chips.Clear();
        float perCss = LocalPerCss();
        var widths = new float[word.Length];
        float total = ChipGap * perCss * Mathf.Max(0, word.Length - 1);
        for (int i = 0; i < word.Length; i++)
        {
            GameObject chip = new GameObject("Chip", typeof(RectTransform), typeof(Image));
            RectTransform rect = (RectTransform)chip.transform;
            rect.SetParent(content.transform, false);
            Image image = chip.GetComponent<Image>();
            image.sprite = backgroundRT.GetComponent<Image>().sprite;
            image.type = Image.Type.Sliced;
            image.color = backgroundRT.GetComponent<Image>().color;
            image.raycastTarget = false;

            GameObject label = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform labelRect = (RectTransform)label.transform;
            labelRect.SetParent(rect, false);
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = labelRect.offsetMax = Vector2.zero;
            TMP_Text chipText = label.GetComponent<TMP_Text>();
            chipText.font = text.font;
            chipText.fontSize = Mathf.RoundToInt(ChipFont * perCss);
            chipText.fontStyle = FontStyles.Bold;
            chipText.alignment = TextAlignmentOptions.Center;
            chipText.textWrappingMode = TextWrappingModes.NoWrap;
            chipText.color = new Color32(0x5a, 0x41, 0x00, 255);
            chipText.raycastTarget = false;
            chipText.text = word[i].ToString();

            widths[i] = Mathf.Max(ChipMinWidth * perCss, chipText.preferredWidth + ChipPad * 2 * perCss);
            rect.sizeDelta = new Vector2(widths[i], ChipHeight * perCss);
            total += widths[i];
            chips.Add(chip);
        }
        float x = -total / 2f;
        for (int i = 0; i < chips.Count; i++)
        {
            ((RectTransform)chips[i].transform).anchoredPosition = new Vector2(x + widths[i] / 2f, 0f);
            x += widths[i] + ChipGap * perCss;
        }
    }

    private void Tint(Color color)
    {
        backgroundRT.GetComponent<Image>().color = color;
        foreach (var chip in chips) chip.GetComponent<Image>().color = color;
    }

    public void SetIndexes(List<int> indexes)
    {
        StringBuilder sb = new StringBuilder();
        foreach(var i in indexes)
        {
            sb.Append(word[i]);
        }
        text.text = sb.ToString();
        BuildChips(text.text);
    }

    public void SetActive(bool isActive)
    {
        content.SetActive(isActive && text.text.Length > 0);
    }

    public void ClearText()
    {
        text.text = "";
        BuildChips("");
    }

    public void SetText(string textStr)
    {
        text.text = textStr;
        BuildChips(textStr);
    }

    public string GetText()
    {
        return text.text;
    }

    public void SetAnswerColor()
    {
        Tint(answerColor);
    }

    public void SetValidColor()
    {
        Tint(validColor);
    }

    public void SetWrongColor()
    {
        Tint(wrongColor);
    }

    public void SetDefaultColor()
    {
        Tint(defaultColor);
    }

    public void SetExistColor()
    {
        Tint(existColor);
    }

    public void FadeOut()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", 0.4f, "onupdate", "OnUpdate", "ontarget", gameObject));
    }

    public void FadeIn()
    {
        iTween.Stop(gameObject);
        SetDefaultColor();
        content.GetComponent<CanvasGroup>().alpha = 1;
    }

    private void OnUpdate(float value)
    {
        content.GetComponent<CanvasGroup>().alpha = value;
    }
}
