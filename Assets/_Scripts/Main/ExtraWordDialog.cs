using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

public class ExtraWordDialog : Dialog
{
    public Transform claimTr;
    public ExtraProgress extraProgress;
    public GameObject claimButton;
    public Text progressText;
    public Text claimText;

    // Toạ độ gốc trong prefab (1 dòng mô tả, 1 dòng ghi chú); chữ dài xuống dòng thì dịch các phần tử quanh các giá trị này.
    private const float DESC_Y = 57f, TITLE_Y = 126f, CLOSE_Y = 191f, NOTE_Y = -45f, CLAIM_Y = -120f, BG_HEIGHT = 402f, TEXT_HEIGHT = 30f;
    private const float NOTE_WIDTH = 412f;

    private int numWords, claimQuantity;
    private LocalizationController Loc { get { return Locator<LocalizationController>.Instance; } }

    protected override void Start()
    {
        base.Start();
        extraProgress.target = Prefs.extraTarget;
        extraProgress.current = Prefs.extraProgress;

        UpdateUI();
        FitTexts();
    }

    // Mô tả và ghi chú có thể xuống dòng (fr/es/pt): ghi chú chừa chỗ cho số tiến độ bên phải, card và các phần tử kế bên giãn theo số dòng.
    private void FitTexts()
    {
        Transform content = transform.Find("Content");
        Text desc = content.Find("Text").GetComponent<Text>();
        Transform progress = content.Find("VerticalGroup/Progress");
        RectTransform noteRect = (RectTransform)progress.Find("Note");
        Text note = noteRect.GetComponent<Text>();
        noteRect.sizeDelta = new Vector2(NOTE_WIDTH, noteRect.sizeDelta.y);

        float descExtra = ExtraHeight(desc);
        float noteExtra = ExtraHeight(note);
        Place((RectTransform)desc.transform, DESC_Y + descExtra / 2, TEXT_HEIGHT + descExtra);
        Place(noteRect, NOTE_Y - noteExtra / 2, TEXT_HEIGHT + noteExtra);
        Place((RectTransform)content.Find("Title"), TITLE_Y + descExtra, -1f);
        Place((RectTransform)content.Find("CloseButton"), CLOSE_Y + descExtra, -1f);
        Place((RectTransform)content.Find("VerticalGroup/Claim"), CLAIM_Y - noteExtra, -1f);
        Place((RectTransform)content.Find("BG"), (descExtra - noteExtra) / 2, BG_HEIGHT + descExtra + noteExtra);
    }

    // Chiều cao thêm so với một dòng.
    private float ExtraHeight(Text text)
    {
        TextGenerator generator = text.cachedTextGeneratorForLayout;
        generator.Populate(text.text, text.GetGenerationSettings(new Vector2(text.rectTransform.rect.width, 0f)));
        int lines = Mathf.Max(1, generator.lineCount);
        return text.preferredHeight / lines * (lines - 1);
    }

    // height < 0: giữ nguyên chiều cao.
    private void Place(RectTransform rt, float y, float height)
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
        if (height >= 0f) rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
    }

    public void Claim()
    {
        claimQuantity = GetClaimQuantity();
        
        extraProgress.current -= (int)extraProgress.target;
        Prefs.extraProgress = (int)extraProgress.current;
        UpdateUI();

        // Ruby cộng ngay; hiệu ứng bay chỉ trang trí, chạy trên MonoUtils để không chết theo dialog.
        CurrencyController.CreditBalance(claimQuantity);
        if (MonoUtils.instance != null) MonoUtils.instance.StartCoroutine(ClaimEffect());
        ExtraWord.instance.OnClaimed();

        if (Prefs.extraTarget == 5 && Prefs.totalExtraAdded > 10)
        {
            Prefs.extraTarget = 10;
            extraProgress.target = 10;
            UpdateUI();
        }
    }

    private IEnumerator ClaimEffect()
    {
        GameObject balanceObj = GameObject.FindWithTag("RubyBalance");
        if (balanceObj == null) yield break;
        Transform rubyBalance = balanceObj.transform;
        // Pivot của RubyBalance nằm ở mép phải pill nên bay về icon ruby con (nếu có) thay vì pivot
        Transform rubyIcon = rubyBalance.Find("Ruby");
        Vector3 targetPosition = rubyIcon != null ? rubyIcon.position : rubyBalance.position;
        var middlePoint = CUtils.GetMiddlePoint(claimTr.position, targetPosition, -0.4f);
        Vector3[] waypoints = { claimTr.position, middlePoint, targetPosition };
        int quantity = claimQuantity;

        for (int i = 0; i < quantity; i++)
        {
            GameObject gameObj = Instantiate(MonoUtils.instance.rubyFly);
            gameObj.transform.position = waypoints[0];
            gameObj.transform.localScale = 0.5f * Vector3.one;

            iTween.MoveTo(gameObj, iTween.Hash("path", waypoints, "speed", 30, "oncomplete", "OnMoveComplete"));
            iTween.ScaleTo(gameObj, iTween.Hash("scale", 0.7f * Vector3.one, "time", 0.3f));
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateUI()
    {
        // Theo design: nút luôn hiện, xám khi chưa đủ từ thưởng
        bool ready = extraProgress.current >= extraProgress.target;
        claimButton.GetComponent<Button>().interactable = ready;
        claimButton.GetComponent<Image>().color = ready ? new Color(0.0706f, 0.7098f, 0.651f) : new Color(0.8118f, 0.8627f, 0.8627f);
        claimButton.GetComponent<Shadow>().effectColor = ready ? new Color(0.0431f, 0.549f, 0.502f) : new Color(0.7098f, 0.7725f, 0.7725f);
        claimText.text = Loc.Get("extra.claim", "n", GetClaimQuantity());
        progressText.text = extraProgress.current + "/" + extraProgress.target;
    }

    private int GetClaimQuantity()
    {
        return (int)extraProgress.target / 5 * 2;
    }
}
