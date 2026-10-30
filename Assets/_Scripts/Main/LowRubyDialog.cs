using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;
using TMPro;

// Popup thiếu ruby (design: popups['low-ruby']): xem video +ruby (luồng RewardedVideoDialog), mở Shop, hoặc bỏ qua.
public class LowRubyDialog : Dialog {
    public RectTransform card;
    public RectTransform closeX;
    public TMP_Text titleText;
    public TMP_Text messageText;
    public RectTransform watchButton;
    public TMP_Text watchText;
    public RectTransform shopButton;
    public TMP_Text shopText;
    public RectTransform skipButton;
    public TMP_Text skipText;

    private LocalizationController Loc { get { return Locator<LocalizationController>.Instance; } }

    // Khoảng cách và cỡ theo design (css px x 1,6914 trên canvas 660x1397): padding trên 28, dưới/hai bên 22, gap 16, dòng title 32,5, dòng mô tả 22,4, nút 52.
    private const float PADDING_TOP = 47f;
    private const float PADDING_BOTTOM = 37f;
    private const float SPACING = 27f;
    private const float TITLE_LINE = 55f;
    private const float MESSAGE_LINE = 38f;
    private const float BUTTON_HEIGHT = 88f;
    private const float CLOSE_INSET = 10.3f;

    protected override void Start()
    {
        base.Start();
        Refresh();
    }

    // Quay lại từ popup video: đã đủ ruby thì đóng.
    private void OnEnable()
    {
        if (CurrencyController.GetBalance() >= Const.HINT_COST) Invoke("CloseWhenEnough", 0.1f);
    }

    private void CloseWhenEnough()
    {
        Close();
    }

    public void OnWatchClick()
    {
        Sound.instance.PlayButton();
        var dialog = (RewardedVideoDialog)DialogController.instance.GetDialog(DialogType.RewardedVideo);
        dialog.SetAmount(ConfigController.Config.rewardedVideoAmount);
        DialogController.instance.ShowDialog(dialog, DialogShow.STACK);
    }

    public void OnShopClick()
    {
        Sound.instance.PlayButton();
        DialogController.instance.ShowDialog(DialogType.Shop);
    }

    public void OnSkipClick()
    {
        Sound.instance.PlayButton();
        Close();
    }

    private void Refresh()
    {
        titleText.text = Loc.Get("low.title", "n", Const.HINT_COST);
        messageText.text = Loc.Get("low.desc", "a", CurrencyController.GetBalance());
        watchText.text = Loc.Get("low.watch", "n", ConfigController.Config.rewardedVideoAmount);
        shopText.text = Loc.Get("low.shop");
        skipText.text = Loc.Get("common.skip");
        Layout();
    }

    // Xếp title, mô tả, 3 nút theo chiều dọc rồi co giãn card; title/mô tả cao theo số dòng.
    private void Layout()
    {
        float titleHeight = TITLE_LINE * LineCount(titleText);
        float messageHeight = MESSAGE_LINE * LineCount(messageText);
        float total = PADDING_TOP + PADDING_BOTTOM + titleHeight + SPACING + messageHeight + 3 * (SPACING + BUTTON_HEIGHT);

        card.sizeDelta = new Vector2(card.sizeDelta.x, total);
        float y = total / 2 - PADDING_TOP;
        y = Place(titleText.rectTransform, y, titleHeight);
        y = Place(messageText.rectTransform, y - SPACING, messageHeight);
        y = Place(watchButton, y - SPACING, BUTTON_HEIGHT);
        y = Place(shopButton, y - SPACING, BUTTON_HEIGHT);
        Place(skipButton, y - SPACING, BUTTON_HEIGHT);
        closeX.anchoredPosition = new Vector2(closeX.anchoredPosition.x, total / 2 - CLOSE_INSET);
    }

    // Số dòng chữ sau khi xuống dòng theo bề rộng khung hiện tại.
    private int LineCount(TMP_Text text)
    {
        text.ForceMeshUpdate();
        return Mathf.Max(1, text.textInfo.lineCount);
    }

    // Đặt phần tử có mép trên tại top, trả về mép dưới.
    private float Place(RectTransform rt, float top, float height)
    {
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, top - height / 2);
        return top - height;
    }
}
