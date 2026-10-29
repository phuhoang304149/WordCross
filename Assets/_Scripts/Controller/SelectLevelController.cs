using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

public class SelectLevelController : BaseController {
    public RectTransform mainUI, scrollContent;
    public ScrollRect scrollRect;
    public Text titleText;
    public GridLayoutGroup grid;
    public float cellShadow = 7f;
    // Design screen is 390 wide: grid padding 20 (8 above, 30 below) and gap 12 scale with the canvas width
    public float designWidth = 390f, designPadding = 20f, designGap = 12f, designTop = 8f, designBottom = 30f;

    protected override void Start()
    {
        base.Start();

        UpdateTitle();
        Locator<LocalizationController>.Instance.onLanguageChanged += OnLanguageChanged;
        UpdateCellSize();

        int numLevels = Superpow.Utils.GetNumLevels(GameState.currentWorld, GameState.currentSubWorld);
        for(int i =0; i < numLevels; i++)
        {
            GameObject levelButton = Instantiate(MonoUtils.instance.levelButton);
            levelButton.transform.SetParent(scrollContent);
            levelButton.transform.localScale = Vector3.one;
            levelButton.transform.SetLocalZ(0);
        }

		StartCoroutine(UpdateGrid());
    }

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
        titleText.text = Locator<LocalizationController>.Instance.Get("lvl.title", "n", GameState.currentSubWorld + 1);
    }

    // Square cells (aspect-ratio 1 + shadow step) filling the columns like the design grid
    private void UpdateCellSize()
    {
        float unit = scrollRect.viewport.rect.width / designWidth;
        int padding = Mathf.RoundToInt(designPadding * unit);
        grid.padding.left = padding;
        grid.padding.right = padding;
        grid.padding.top = Mathf.RoundToInt(designTop * unit);
        grid.padding.bottom = Mathf.RoundToInt(designBottom * unit);
        // The shadow strip is part of the cell height but not of the design grid step (cell + gap)
        grid.spacing = new Vector2(designGap * unit, designGap * unit - cellShadow);

        int columns = grid.constraintCount;
        float width = scrollRect.viewport.rect.width - grid.padding.left - grid.padding.right - grid.spacing.x * (columns - 1);
        float cell = width / columns;
        grid.cellSize = new Vector2(cell, cell + cellShadow);
    }

	private IEnumerator UpdateGrid()
	{
		yield return new WaitForEndOfFrame();
		UpdateCellSize();
		LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent);
		if (GameState.currentWorld == Prefs.unlockedWorld && GameState.currentSubWorld == Prefs.unlockedSubWorld)
		{
			Transform unlockedLevelTransform = scrollContent.transform.GetChild(Prefs.unlockedLevel);
			float newY = -unlockedLevelTransform.localPosition.y - scrollRect.GetComponent<RectTransform>().rect.height / 2f;
			newY = Mathf.Clamp(newY, 0, scrollContent.sizeDelta.y);
			scrollContent.localPosition = new Vector3(scrollContent.localPosition.x, newY, scrollContent.localPosition.z);
		}
	}
}
