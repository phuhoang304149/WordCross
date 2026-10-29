using UnityEngine;

// Keeps a screen title centered in the gap between the Header back button and ruby pill (design: flex 1 with a 10 gap)
public class HeaderTitleFit : MonoBehaviour
{
    public string headerName = "Header";
    public float designGap = 10f, designWidth = 390f;

    private RectTransform self, parent, back, pill;

    private void Start()
    {
        self = transform as RectTransform;
        parent = transform.parent as RectTransform;
        Transform header = GameObject.Find(headerName).transform;
        back = header.Find("Back") as RectTransform;
        pill = header.Find("RubyBalance") as RectTransform;
    }

    private void LateUpdate()
    {
        float gap = designGap * parent.rect.width / designWidth;
        float left = parent.InverseTransformPoint(back.TransformPoint(new Vector3(back.rect.xMax, 0f, 0f))).x + gap;
        float right = parent.InverseTransformPoint(pill.TransformPoint(new Vector3(pill.rect.xMin, 0f, 0f))).x - gap;
        self.anchoredPosition = new Vector2(left - parent.rect.xMin, self.anchoredPosition.y);
        self.sizeDelta = new Vector2(Mathf.Max(0f, right - left), self.sizeDelta.y);
    }
}
