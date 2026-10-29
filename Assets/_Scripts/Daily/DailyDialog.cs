using System;
using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

// Daily Challenge (design: screens/daily): hero, chuỗi, lịch tháng, thưởng, nút chơi. Mở từ nút DAILY ở Home.
public class DailyDialog : Dialog
{
    public const int DAY_CELLS = 42;

    public Text rubyText;
    public Text heroSmallText, heroTitleText, heroGoalText;
    public Image streakBackground;
    public Text streakTitleText, streakBestText, streakNextText, streakMessageText;
    public Text monthText;
    public Button prevButton, nextButton;
    public Text[] weekdayTexts;
    public Button[] dayButtons;
    public Image[] dayBorders;
    public Image[] dayFaces;
    public Text[] dayTexts;
    public Text rewardText;
    public Text playText;
    public Text legendText, timezoneText;

    private readonly Color32 teal = new Color32(18, 181, 166, 255);
    private readonly Color32 coral = new Color32(255, 107, 94, 255);
    private readonly Color32 coralSoft = new Color32(255, 227, 223, 255);
    private readonly Color32 coralDeep = new Color32(217, 75, 64, 255);
    private readonly Color32 yellow = new Color32(255, 201, 60, 255);
    private readonly Color32 yellowSoft = new Color32(255, 241, 196, 255);
    private readonly Color32 ink = new Color32(31, 45, 61, 255);
    private readonly Color32 line = new Color32(223, 233, 234, 255);
    private readonly Color32 white = new Color32(255, 255, 255, 255);
    private readonly Color32 lostSoft = new Color32(238, 243, 243, 255);

    private RectTransform selectionOuter, selectionInner;
    private DailyChallenge daily = new DailyChallenge();
    private string today;
    private string selected;
    private string viewMonth;
    private bool navigating;

    private LocalizationController Loc { get { return Locator<LocalizationController>.Instance; } }

    protected override void Start()
    {
        base.Start();
        selectionOuter = transform.Find("Content/Grid/SelOuter") as RectTransform;
        selectionInner = transform.Find("Content/Grid/SelInner") as RectTransform;
        today = daily.Today();
        selected = today;
        viewMonth = today.Substring(0, 7);
        for (int i = 0; i < dayButtons.Length; i++)
        {
            int index = i;
            dayButtons[i].onClick.AddListener(() => OnDayClick(index));
        }
        Loc.onLanguageChanged += OnLanguageChanged;
        CurrencyController.onBalanceChanged += RefreshBalance;
        Refresh();
    }

    private void OnDestroy()
    {
        if (Loc != null) Loc.onLanguageChanged -= OnLanguageChanged;
        CurrencyController.onBalanceChanged -= RefreshBalance;
    }

    private void OnLanguageChanged(string code)
    {
        Refresh();
    }

    private void RefreshBalance()
    {
        rubyText.text = CurrencyController.GetBalance().ToString();
    }

    public void OnCloseClick()
    {
        Sound.instance.PlayButton();
        Close();
    }

    public void OnPrevMonthClick()
    {
        Sound.instance.PlayButton();
        viewMonth = daily.AddDays(viewMonth + "-01", -1).Substring(0, 7);
        Refresh();
    }

    public void OnNextMonthClick()
    {
        Sound.instance.PlayButton();
        DateTime first = daily.Parse(viewMonth + "-01").AddMonths(1);
        viewMonth = first.ToString("yyyy-MM", System.Globalization.CultureInfo.InvariantCulture);
        Refresh();
    }

    private void OnDayClick(int index)
    {
        string date = DateOfCell(index);
        if (date == null || daily.Status(date) == DayStatus.Future) return;
        Sound.instance.PlayButton();
        selected = date;
        Refresh();
    }

    public void OnPlayClick()
    {
        if (navigating) return;
        navigating = true;
        Sound.instance.PlayButton();

        int world, subWorld, level;
        daily.PickLevel(selected, out world, out subWorld, out level);
        // Ngữ cảnh Daily chỉ ở bộ nhớ: current* đặt tạm cho Main, unlocked* và PlayerPrefs tiến độ chính không đổi.
        GameState.dailyDate = selected;
        GameState.currentWorld = world;
        GameState.currentSubWorld = subWorld;
        GameState.currentLevel = level;
        Close();
        CUtils.LoadScene(3, true);
        Music.instance.Play(CUtils.GetRandom(Music.Type.Main_0, Music.Type.Main_1, Music.Type.Main_2));
    }

    // Ô i của lưới 6x7 (thứ Hai đầu hàng); null nếu ô nằm ngoài tháng đang xem.
    private string DateOfCell(int index)
    {
        DateTime first = daily.Parse(viewMonth + "-01");
        int offset = ((int)first.DayOfWeek + 6) % 7;
        int day = index - offset + 1;
        if (day < 1 || day > DateTime.DaysInMonth(first.Year, first.Month)) return null;
        return viewMonth + "-" + day.ToString("00");
    }

    public void Refresh()
    {
        RefreshBalance();
        RefreshHero();
        RefreshStreak();
        RefreshCalendar();
        RefreshFooter();
    }

    private int CountWords(string date)
    {
        int world, subWorld, level;
        daily.PickLevel(date, out world, out subWorld, out level);
        GameLevel gameLevel = Superpow.Utils.Load(world, subWorld, level);
        return gameLevel == null ? 0 : CUtils.BuildListFromString<string>(gameLevel.answers).Count;
    }

    private void RefreshHero()
    {
        heroSmallText.text = Loc.Get("daily.today", "d", daily.ShortDate(today));
        heroTitleText.text = "#" + daily.DailyNo(selected) + " · " + daily.ShortDate(selected);
        heroGoalText.text = Loc.Get("daily.goal", "n", CountWords(selected));
    }

    private void RefreshStreak()
    {
        int streak = daily.Streak();
        StreakState state = daily.GetStreakState();
        streakBackground.color = state == StreakState.Risk ? coralSoft : state == StreakState.Lost ? lostSoft : yellowSoft;
        streakTitleText.text = Loc.Get("daily.streak", "n", streak);
        streakBestText.text = Loc.Get("daily.best", "n", daily.best);
        // Kỷ lục nằm ngay sau tiêu đề, cách 12 css px (20 canvas).
        float titleLeft = streakTitleText.rectTransform.anchoredPosition.x;
        streakBestText.rectTransform.anchoredPosition = new Vector2(titleLeft + streakTitleText.preferredWidth + 20f, streakBestText.rectTransform.anchoredPosition.y);

        int days, reward;
        bool hasNext = daily.NextMilestone(streak, out days, out reward);
        streakNextText.gameObject.SetActive(hasNext);
        if (hasNext) streakNextText.text = Loc.Get("daily.nextMilestone", "d", days, "n", reward);

        string message = state == StreakState.Risk ? Loc.Get("daily.streakRisk") : state == StreakState.Lost ? Loc.Get("daily.streakLost", "n", daily.best) : "";
        streakMessageText.gameObject.SetActive(message.Length > 0);
        streakMessageText.text = message;
    }

    private void RefreshCalendar()
    {
        DateTime month = daily.Parse(viewMonth + "-01");
        string[] names = Loc.Get("daily.months").Split('|');
        monthText.text = Loc.Get("daily.month", "m", names[month.Month - 1], "y", month.Year);
        bool hasNext = string.CompareOrdinal(viewMonth, today.Substring(0, 7)) < 0;
        nextButton.interactable = hasNext;
        foreach (Graphic graphic in nextButton.GetComponentsInChildren<Graphic>()) graphic.canvasRenderer.SetAlpha(hasNext ? 1f : 0.35f);
        selectionOuter.gameObject.SetActive(false);
        selectionInner.gameObject.SetActive(false);

        string[] weekdays = Loc.Get("daily.weekdays").Split('|');
        for (int i = 0; i < weekdayTexts.Length; i++) weekdayTexts[i].text = weekdays[i];

        for (int i = 0; i < DAY_CELLS; i++)
        {
            string date = DateOfCell(i);
            dayButtons[i].gameObject.SetActive(date != null);
            if (date == null) continue;
            DayStatus status = daily.Status(date);
            dayTexts[i].text = date.Substring(8).TrimStart('0');
            dayButtons[i].interactable = status != DayStatus.Future;

            Color32 border = line, face = white, text = ink;
            if (status == DayStatus.Done) { border = teal; face = teal; text = white; }
            else if (status == DayStatus.Miss) { border = coral; face = coralSoft; text = coralDeep; }
            else if (status == DayStatus.Today) { border = yellow; face = yellowSoft; }
            if (date == selected)
            {
                // Viền chọn nằm ngoài ô (outline 3px, cách 1px) nên vẽ vòng phía sau ô.
                RectTransform grid = (RectTransform)selectionOuter.parent;
                Vector2 center = new Vector2(-grid.rect.width / 2 + grid.rect.width / 7 * (i % 7 + 0.5f), grid.rect.height / 2 - 72f - 87f * (i / 7));
                selectionOuter.anchoredPosition = center;
                selectionInner.anchoredPosition = center;
                selectionOuter.gameObject.SetActive(true);
                selectionInner.gameObject.SetActive(true);
            }
            float alpha = status == DayStatus.Future ? 0.35f : 1f;
            dayBorders[i].color = WithAlpha(border, alpha);
            dayFaces[i].color = WithAlpha(face, alpha);
            dayTexts[i].color = WithAlpha(text, alpha);
        }
    }

    private Color WithAlpha(Color32 color, float alpha)
    {
        Color result = color;
        result.a = alpha;
        return result;
    }

    private void RefreshFooter()
    {
        DayStatus status = daily.Status(selected);
        int reward = daily.RewardFor(selected);
        rewardText.text = status == DayStatus.Done ? Loc.Get("daily.rewardDone")
            : reward == 0 ? Loc.Get("daily.noReward")
            : status == DayStatus.Miss ? Loc.Get("daily.backfill", "n", reward)
            : Loc.Get("daily.reward", "n", reward);
        playText.text = status == DayStatus.Today ? Loc.Get("daily.play")
            : status == DayStatus.Done ? Loc.Get("daily.replay")
            : Loc.Get("daily.playDay", "d", daily.ShortDate(selected));
        legendText.text = Loc.Get("daily.legend");
        timezoneText.text = Loc.Get("daily.tz");

        // Dòng thưởng: icon + chữ căn giữa cả cụm (icon rộng 27, cách chữ 13,5 canvas).
        RectTransform rewardIcon = transform.Find("Content/RewardIcon") as RectTransform;
        float rewardY = rewardText.rectTransform.anchoredPosition.y;
        float start = -(27f + 13.5f + rewardText.preferredWidth) / 2f;
        rewardIcon.anchoredPosition = new Vector2(start + 13.5f, rewardY);
        rewardText.rectTransform.anchoredPosition = new Vector2(start + 40.5f, rewardY);
    }
}
