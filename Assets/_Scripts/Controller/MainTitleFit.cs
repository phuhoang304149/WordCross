using UnityEngine;
using UnityEngine.UI;

// Tên level Main theo design (.game-top .lvl-name): giữa khoảng trống nút pause và pill ruby, tên 19 css + dòng phụ 12 css.
public class MainTitleFit : MonoBehaviour
{
    public Text nameText, subText;
    public string headerName = "Header";
    public float designGap = 10f, designWidth = 390f;
    public float nameCss = 19f, subCss = 12f;
    public float nameDy, subDy = 1.5f;

    private RectTransform back, pill;
    private float nameY, subY;
    private Canvas canvas;

    private void Start()
    {
        Transform header = GameObject.Find(headerName).transform;
        back = header.Find("Back") as RectTransform;
        pill = header.Find("RubyBalance") as RectTransform;
        canvas = nameText.canvas.rootCanvas;
        nameY = nameText.transform.position.y;
        subY = subText.transform.position.y;
        SetFont(nameText, nameCss);
        SetFont(subText, subCss);
    }

    private float CanvasPerCss()
    {
        return ((RectTransform)canvas.transform).rect.width / designWidth;
    }

    private void SetFont(Text text, float css)
    {
        float localPerCanvas = canvas.transform.lossyScale.x / text.transform.lossyScale.x;
        int size = Mathf.RoundToInt(css * CanvasPerCss() * localPerCanvas);
        text.fontSize = size;
        text.resizeTextForBestFit = true;
        text.resizeTextMaxSize = size;
        text.resizeTextMinSize = Mathf.RoundToInt(size * 0.7f);
    }

    private void LateUpdate()
    {
        float worldPerCss = CanvasPerCss() * canvas.transform.lossyScale.x;
        float left = back.TransformPoint(new Vector3(back.rect.xMax, 0f, 0f)).x + designGap * worldPerCss;
        float right = pill.TransformPoint(new Vector3(pill.rect.xMin, 0f, 0f)).x - designGap * worldPerCss;
        Place(nameText.rectTransform, left, right, nameY + nameDy * worldPerCss);
        Place(subText.rectTransform, left, right, subY + subDy * worldPerCss);
    }

    private void Place(RectTransform target, float left, float right, float y)
    {
        target.anchorMin = target.anchorMax = new Vector2(0.5f, 0.5f);
        target.pivot = new Vector2(0.5f, 0.5f);
        target.sizeDelta = new Vector2(Mathf.Max(0f, (right - left) / target.lossyScale.x), target.sizeDelta.y);
        target.position = new Vector3((left + right) / 2f, y, target.position.z);
    }
}
