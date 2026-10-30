using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

public class WorldItem : MonoBehaviour {
    public MaskableGraphic itemName, itemNumber, itemNumberBack;
    public Image play;
    public Button button;
    public Sprite playUnactive;
    public TMP_Text processText, subWorldName;
    public TMP_Text titleText;
    public CanvasGroup canvasGroup;
    public float lockedAlpha = 0.55f;
    public Image indexBack;
    public TMP_Text indexText;
    public RectTransform progressFill;
    public Color lockedIndexColor = new Color(0.7216f, 0.7843f, 0.7843f, 1f);
    public Color lockedIndexTextColor = Color.white;

    public int world, subWorld;

    // Design card (390 wide frame): the title/bar column ends before the right-aligned count, which has 16 padding and a 14 gap
    public float designUnit = 1.6923f, designPadding = 16f, designGap = 14f;
    // Index chip colors per sub world, like the design (coral, yellow, teal repeating) with its text colors
    // Locked card: the design fades the whole card to 55% over the page (#f4fbfa); a group alpha would show the border through the card face
    public Color32 pageColor = new Color32(244, 251, 250, 255);
    private bool locked;
    private readonly Color32[] indexColors = { new Color32(255, 107, 94, 255), new Color32(255, 201, 60, 255), new Color32(18, 181, 166, 255) };
    private readonly Color32[] indexTextColors = { new Color32(255, 255, 255, 255), new Color32(90, 65, 0, 255), new Color32(255, 255, 255, 255) };

    private void OnDestroy()
    {
        if (Locator<LocalizationController>.Instance != null)
            Locator<LocalizationController>.Instance.onLanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(string code)
    {
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        titleText.text = Locator<LocalizationController>.Instance.Get("world.sub", "n", subWorld + 1);
    }

	private void Start()
    {
        world = transform.parent.parent.GetSiblingIndex();
        subWorld = transform.GetSiblingIndex();
        int numLevels = Superpow.Utils.GetNumLevels(world, subWorld);
        UpdateTitle();
        Locator<LocalizationController>.Instance.onLanguageChanged += OnLanguageChanged;

        int unlockedWorld = Prefs.unlockedWorld;
        int unlockedSubWorld = Prefs.unlockedSubWorld;
        int unlockedLevel = Prefs.unlockedLevel;

        indexText.text = (subWorld + 1).ToString();
        indexBack.color = indexColors[subWorld % 3];
        indexText.color = indexTextColors[subWorld % 3];

        if (world > unlockedWorld || (world == unlockedWorld && subWorld > unlockedSubWorld))
        {
            locked = true;
            button.interactable = false;
            play.sprite = playUnactive;
            indexBack.color = lockedIndexColor;
            indexText.color = lockedIndexTextColor;

            processText.text = "0" + "/" + numLevels;
            SetProgress(0, numLevels);
        }
        else if (world == unlockedWorld && subWorld == unlockedSubWorld)
        {
            processText.text = unlockedLevel + "/" + numLevels;
            SetProgress(unlockedLevel, numLevels);
        }
        else
        {
            processText.text = numLevels + "/" + numLevels;
            SetProgress(numLevels, numLevels);
        }

        LayoutCount();
        if (locked) FadeToPage();
        button.onClick.AddListener(OnButtonClick);
    }

    private void FadeToPage()
    {
        foreach (Graphic graphic in GetComponentsInChildren<Graphic>(true))
        {
            Color color = graphic.color;
            graphic.color = Color.Lerp(pageColor, new Color(color.r, color.g, color.b, 1f), lockedAlpha * color.a);
        }
    }

    // The count text is as wide as its digits: the right-aligned count box and the title/bar column follow it
    private void LayoutCount()
    {
        float width = processText.preferredWidth;
        RectTransform count = processText.transform.parent as RectTransform;
        count.sizeDelta = new Vector2(width, count.sizeDelta.y);

        float right = (designPadding + designGap) * designUnit + width;
        RectTransform title = titleText.rectTransform;
        title.sizeDelta = new Vector2(-(title.anchoredPosition.x + right), title.sizeDelta.y);
        RectTransform bar = progressFill.parent as RectTransform;
        bar.sizeDelta = new Vector2(-(bar.anchoredPosition.x + right), bar.sizeDelta.y);
    }

    private void SetProgress(int done, int total)
    {
        float ratio = total > 0 ? Mathf.Clamp01((float)done / total) : 0f;
        progressFill.anchorMax = new Vector2(ratio, 1f);
        progressFill.gameObject.SetActive(ratio > 0f);
    }

    private void OnButtonClick()
    {
        GameState.dailyDate = null;
        GameState.currentWorld = world;
        GameState.currentSubWorld = subWorld;
        GameState.currentSubWorldName = subWorldName.text;

        CUtils.LoadScene(2, true);
        Sound.instance.PlayButton();
    }
}
