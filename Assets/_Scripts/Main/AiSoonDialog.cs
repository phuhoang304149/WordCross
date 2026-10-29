using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

// Popup Đấu AI sắp ra mắt (design: popups['ai-soon']): badge AI, tiêu đề, nhãn Sắp ra mắt, 2 đoạn mô tả, nút vô hiệu, nút Đấu Bot.
public class AiSoonDialog : Dialog {
    public RectTransform card;
    public RectTransform closeX;
    public RectTransform badge;
    public Text titleText;
    public RectTransform pill;
    public Text pillText;
    public Text descText;
    public Text noteText;
    public RectTransform soonButton;
    public Text soonText;
    public RectTransform botButton;
    public Text botText;

    private LocalizationController Loc { get { return Locator<LocalizationController>.Instance; } }

    // Khoảng cách và cỡ theo design (css px x 1,6914 trên canvas 660x1397): badge 84 nhô 62 trên mép card, title cách badge 16, nhãn cao 30, ngang = chữ + 28.
    private const float TOP_TO_TITLE = 111f;
    private const float PADDING_BOTTOM = 37f;
    private const float SPACING = 27f;
    private const float TITLE_LINE = 55f;
    private const float PILL_HEIGHT = 51f;
    private const float PILL_PADDING = 47f;
    private const float MESSAGE_LINE = 38f;
    private const float BUTTON_HEIGHT = 88f;
    private const float BADGE_TOP = -58f;
    private const float CLOSE_INSET = 10.3f;

    protected override void Start()
    {
        base.Start();
        Loc.onLanguageChanged += OnLanguageChanged;
        Refresh();
    }

    private void OnDestroy()
    {
        if (Loc != null) Loc.onLanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(string code)
    {
        Refresh();
    }

    public void OnCloseClick()
    {
        Sound.instance.PlayButton();
        Close();
    }

    // Nút "Đấu Bot": Bot chưa có màn riêng nên đóng popup và báo như nút Bot ở Home.
    public void OnBotClick()
    {
        Sound.instance.PlayButton();
        Close();
        Toast.instance.ShowMessage(Loc.Get("home.toast.bot"));
    }

    private void Refresh()
    {
        titleText.text = Loc.Get("ai.title");
        pillText.text = Loc.Get("ai.soon");
        descText.text = Loc.Get("ai.desc");
        noteText.text = Loc.Get("ai.note");
        soonText.text = Loc.Get("ai.soon");
        botText.text = Loc.Get("ai.toBot");
        Layout();
    }

    // Xếp tiêu đề, nhãn, 2 đoạn, 2 nút theo chiều dọc rồi co giãn card; chữ cao theo số dòng.
    private void Layout()
    {
        float titleHeight = TITLE_LINE * LineCount(titleText);
        float descHeight = MESSAGE_LINE * LineCount(descText);
        float noteHeight = MESSAGE_LINE * LineCount(noteText);
        float total = TOP_TO_TITLE + titleHeight + SPACING + PILL_HEIGHT + SPACING + descHeight + SPACING + noteHeight + 2 * (SPACING + BUTTON_HEIGHT) + PADDING_BOTTOM;

        card.sizeDelta = new Vector2(card.sizeDelta.x, total);
        float top = total / 2;
        badge.anchoredPosition = new Vector2(badge.anchoredPosition.x, top - BADGE_TOP - badge.sizeDelta.y / 2);
        float y = Place(titleText.rectTransform, top - TOP_TO_TITLE, titleHeight);
        pill.sizeDelta = new Vector2(pillText.preferredWidth + PILL_PADDING, PILL_HEIGHT);
        y = Place(pill, y - SPACING, PILL_HEIGHT);
        y = Place(descText.rectTransform, y - SPACING, descHeight);
        y = Place(noteText.rectTransform, y - SPACING, noteHeight);
        y = Place(soonButton, y - SPACING, BUTTON_HEIGHT);
        Place(botButton, y - SPACING, BUTTON_HEIGHT);
        closeX.anchoredPosition = new Vector2(closeX.anchoredPosition.x, top - CLOSE_INSET);
    }

    // Số dòng chữ sau khi xuống dòng theo bề rộng khung hiện tại.
    private int LineCount(Text text)
    {
        TextGenerator generator = text.cachedTextGeneratorForLayout;
        generator.Populate(text.text, text.GetGenerationSettings(new Vector2(text.rectTransform.rect.width, 0f)));
        return Mathf.Max(1, generator.lineCount);
    }

    // Đặt phần tử có mép trên tại top, trả về mép dưới.
    private float Place(RectTransform rt, float top, float height)
    {
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, top - height / 2);
        return top - height;
    }
}
