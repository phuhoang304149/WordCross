using System.Collections;
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
    public Text processText, subWorldName;
    public Text titleText;
    public CanvasGroup canvasGroup;
    public float lockedAlpha = 0.55f;
    public Image indexBack;
    public Text indexText;
    public RectTransform progressFill;
    public Color lockedIndexColor = new Color(0.7216f, 0.7843f, 0.7843f, 1f);
    public Color lockedIndexTextColor = Color.white;

    public int world, subWorld;

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

        if (world > unlockedWorld || (world == unlockedWorld && subWorld > unlockedSubWorld))
        {
            canvasGroup.alpha = lockedAlpha;
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

        button.onClick.AddListener(OnButtonClick);
    }

    private void SetProgress(int done, int total)
    {
        float ratio = total > 0 ? Mathf.Clamp01((float)done / total) : 0f;
        progressFill.anchorMax = new Vector2(ratio, 1f);
        progressFill.gameObject.SetActive(ratio > 0f);
    }

    private void OnButtonClick()
    {
        GameState.currentWorld = world;
        GameState.currentSubWorld = subWorld;
        GameState.currentSubWorldName = subWorldName.text;

        CUtils.LoadScene(2, true);
        Sound.instance.PlayButton();
    }
}
