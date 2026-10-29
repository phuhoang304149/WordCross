using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : BaseController {
    public RectTransform mainUI, scrollContent;
    public HorizontalLayoutGroup contentLayoutGroup;
    public SnapScrollRect snapScroll;
    // Design screen is 390 wide with 22 side padding on each page: scale it with the canvas width
    public float designWidth = 390f, designSide = 22f;

    protected override void Start()
    {
        base.Start();
        UpdateUI();
        snapScroll.SetPage(Prefs.unlockedWorld);
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Each page is stretched to the screen width by the layout group so cards keep the design side padding
        scrollContent.sizeDelta = new Vector2(mainUI.rect.width * scrollContent.childCount, scrollContent.sizeDelta.y);
        contentLayoutGroup.spacing = 0;

        float side = designSide * mainUI.rect.width / designWidth;
        foreach (Transform page in scrollContent)
        {
            RectTransform list = page.Find("List") as RectTransform;
            if (list != null && !Mathf.Approximately(list.sizeDelta.x, -2f * side))
                list.sizeDelta = new Vector2(-2f * side, list.sizeDelta.y);
        }
    }
}
