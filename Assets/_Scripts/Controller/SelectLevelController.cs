using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelController : BaseController {
    public RectTransform mainUI, scrollContent;
    public ScrollRect scrollRect;
    public Text titleText;
    public GridLayoutGroup grid;
    public float cellShadow = 7f;

    protected override void Start()
    {
        base.Start();

        UpdateTitle();
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

    private void UpdateTitle()
    {
        // TODO l10n: design key 'lvl.title'
        titleText.text = "Subworld " + (GameState.currentSubWorld + 1);
    }

    // Square cells (aspect-ratio 1 + shadow step) filling the columns like the design grid
    private void UpdateCellSize()
    {
        int columns = grid.constraintCount;
        float width = scrollRect.viewport.rect.width - grid.padding.left - grid.padding.right - grid.spacing.x * (columns - 1);
        float cell = Mathf.Floor(width / columns);
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
