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
    // Page dots: 10 wide, the current page is a 26 wide pill, 8 apart
    public float designDot = 10f, designActiveDot = 26f, designDotGap = 8f;

    protected override void Start()
    {
        base.Start();
        UpdateUI();
        snapScroll.SetPage(Prefs.unlockedWorld);
    }

    private void Update()
    {
        UpdateUI();
        UpdateDots();
    }

    private void UpdateDots()
    {
        float unit = mainUI.rect.width / designWidth;
        snapScroll.indicatorRoot.GetComponent<HorizontalLayoutGroup>().spacing = designDotGap * unit;
        int i = 0;
        foreach (RectTransform item in snapScroll.indicatorRoot)
        {
            bool current = i == snapScroll.index;
            Vector2 size = new Vector2((current ? designActiveDot : designDot) * unit, designDot * unit);
            item.sizeDelta = size;
            for (int c = 0; c < item.childCount; c++)
                (item.GetChild(c) as RectTransform).sizeDelta = size;
            item.GetChild(0).gameObject.SetActive(!current);
            i++;
        }
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
