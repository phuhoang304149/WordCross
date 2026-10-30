using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;
using TMPro;

// Popup Nhiệm vụ & Thưởng (design: screens/economy-year, bỏ phần mô phỏng): tab Hôm nay / Tuần / Tháng / Sự kiện / Năm,
// tiến độ và thưởng lấy từ EconomyService thật. Thẻ dựng từ các mẫu con của "Templates" (prefab QuestsDialog).
public class QuestsDialog : Dialog {
    public const int TAB_TODAY = 0;
    public const int TAB_WEEK = 1;
    public const int TAB_MONTH = 2;
    public const int TAB_EVENT = 3;
    public const int TAB_YEAR = 4;

    public TMP_Text titleText;
    public TMP_Text balanceText;
    public Button[] tabButtons;
    public TMP_Text[] tabTexts;
    public Image[] tabImages;
    public ScrollRect scroll;
    public RectTransform content;
    // Mẫu (inactive): Card, Section, Nav, Grid (lưới 7 cột), Cell (ô ngày).
    public RectTransform cardTemplate;
    public RectTransform sectionTemplate;
    public RectTransform navTemplate;
    public RectTransform gridTemplate;
    public RectTransform cellTemplate;

    private enum Btn { None, Claim, Claimed, Soon, Cost }

    private static readonly Color TEAL = new Color(0.071f, 0.710f, 0.651f, 1f);
    private static readonly Color TEAL_DEEP = new Color(0.043f, 0.549f, 0.502f, 1f);
    private static readonly Color TEAL_SOFT = new Color(0.847f, 0.961f, 0.945f, 1f);
    private static readonly Color CORAL = new Color(1f, 0.420f, 0.369f, 1f);
    private static readonly Color CORAL_DEEP = new Color(0.851f, 0.294f, 0.251f, 1f);
    private static readonly Color CORAL_SOFT = new Color(1f, 0.890f, 0.875f, 1f);
    private static readonly Color YELLOW = new Color(1f, 0.788f, 0.235f, 1f);
    private static readonly Color YELLOW_DEEP = new Color(0.878f, 0.643f, 0.075f, 1f);
    private static readonly Color YELLOW_SOFT = new Color(1f, 0.945f, 0.769f, 1f);
    private static readonly Color INK = new Color(0.122f, 0.176f, 0.239f, 1f);
    private static readonly Color INK_SOFT = new Color(0.365f, 0.431f, 0.502f, 1f);
    private static readonly Color LINE = new Color(0.875f, 0.914f, 0.918f, 1f);
    private static readonly Color WHITE = Color.white;
    private static readonly Color DARK_YELLOW_TEXT = new Color(0.353f, 0.255f, 0f, 1f);

    private const float CARD_HEIGHT = 132f;
    private const float CARD_HEIGHT_PLAIN = 108f;
    private const float DETAIL_HEIGHT = 210f;
    private const float GRID_CELL = 68f;
    private const float GRID_SPACING = 6f;
    private const int GRID_COLUMNS = 7;

    private LocalizationController Loc { get { return Locator<LocalizationController>.Instance; } }
    private EconomyService Econ { get { return Locator<EconomyService>.Instance; } }

    private int tab = TAB_TODAY;
    private int yearMonth;
    private int yearSelected;
    private TMP_Text dailyHeader;
    private int shownDay = -1;

    protected override void Start()
    {
        base.Start();
        Econ.Tick();
        yearMonth = Econ.DayRow.month;
        yearSelected = Econ.DayIndex;
        Econ.onChanged += OnEconChanged;
        Loc.onLanguageChanged += OnLanguageChanged;
        InvokeRepeating("OnSecond", 1f, 1f);
        Rebuild(true);
    }

    private void OnDestroy()
    {
        if (Econ != null) Econ.onChanged -= OnEconChanged;
        if (Loc != null) Loc.onLanguageChanged -= OnLanguageChanged;
    }

    private void OnEconChanged()
    {
        Rebuild(false);
    }

    private void OnLanguageChanged(string code)
    {
        Rebuild(false);
    }

    // Mỗi giây: cập nhật đồng hồ đếm ngược nhiệm vụ ngày; sang ngày mới (00:00 UTC) thì dựng lại.
    private void OnSecond()
    {
        Econ.Tick();
        if (shownDay != Econ.Today) Rebuild(false);
        else if (dailyHeader != null) dailyHeader.text = DailyHeaderText();
    }

    public void OnTabClick(int index)
    {
        Sound.instance.PlayButton();
        tab = index;
        Rebuild(true);
    }

    public void OnCloseClick()
    {
        Sound.instance.PlayButton();
        Close();
    }

    // ---------- dựng nội dung ----------

    private void Rebuild(bool resetScroll)
    {
        float keep = content.anchoredPosition.y;
        foreach (Transform child in content)
        {
            // Tắt ngay để layout bỏ qua trong khung hình này; Destroy chỉ có hiệu lực cuối khung hình.
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        dailyHeader = null;
        shownDay = Econ.Today;
        titleText.text = Loc.Get("econ.title");
        balanceText.text = CurrencyController.GetBalance().ToString();
        string[] names = { "econ.tab.today", "econ.tab.week", "econ.tab.month", "econ.tab.event", "econ.tab.year" };
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabTexts[i].text = Loc.Get(names[i]);
            tabTexts[i].color = i == tab ? WHITE : INK;
            tabImages[i].color = i == tab ? TEAL : LINE;
        }
        switch (tab)
        {
            case TAB_TODAY: BuildToday(); break;
            case TAB_WEEK: BuildWeek(); break;
            case TAB_MONTH: BuildMonth(); break;
            case TAB_EVENT: BuildEvent(); break;
            default: BuildYear(); break;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        content.anchoredPosition = new Vector2(0f, resetScroll ? 0f : keep);
    }

    private void BuildToday()
    {
        AddSection(Loc.Get("econ.cycle", "c", Econ.CycleNo, "d", Econ.DayIndex));
        BuildLogin();
        AddSection(Loc.Get("econ.streakMilestones"));
        foreach (var milestone in Econ.Data.streakMilestones) BuildStreakMilestone(milestone);
        AddSection(Loc.Get("econ.levelMilestones"));
        int cleared = Econ.LevelsCleared;
        foreach (var milestone in Econ.Data.levelMilestones) BuildLevelMilestone(milestone, cleared);
        dailyHeader = AddSection(DailyHeaderText());
        BuildQuests(EconomyService.SCOPE_DAILY);
    }

    private string DailyHeaderText()
    {
        int seconds = Econ.SecondsToNextDay();
        string time = string.Format("{0:00}:{1:00}:{2:00}", seconds / 3600, seconds / 60 % 60, seconds % 60);
        return Loc.Get("econ.dailyQuests", "t", time);
    }

    private void BuildLogin()
    {
        var login = Econ.TodayLogin;
        string sub = Loc.Get("econ.reward", "n", login.coin) + "  ·  " + Loc.Get("econ.streak", "n", Econ.Streak, "b", Econ.BestStreak);
        var state = Econ.IsLoginClaimed ? Btn.Claimed : Btn.Claim;
        AddCard(Loc.Get("econ.login"), sub, -1f, state, null, CARD_HEIGHT_PLAIN, () =>
        {
            if (Econ.ClaimLogin()) Reward(login.coin);
        });
        if (!Econ.CanRepairStreak) return;
        int cost = Econ.SinkCost(EconomyService.SINK_STREAK_REPAIR);
        AddCard(Loc.Get("econ.repair"), Loc.Get("sink.streak_repair"), -1f, Btn.Cost, cost.ToString(), CARD_HEIGHT_PLAIN, () =>
        {
            if (CurrencyController.GetBalance() < cost)
            {
                var low = (LowRubyDialog)DialogController.instance.GetDialog(DialogType.LowRuby);
                low.SetSink(EconomyService.SINK_STREAK_REPAIR);
                DialogController.instance.ShowDialog(low, DialogShow.STACK);
            }
            else Econ.RepairStreak();
        });
    }

    private void BuildStreakMilestone(EconomyData.Milestone milestone)
    {
        var state = Econ.GetStreakMilestoneState(milestone);
        AddCard(Loc.Get(milestone.locKey), Loc.Get("econ.progress", "a", Mathf.Min(Econ.Streak, milestone.days), "b", milestone.days, "n", milestone.coin),
            (float)Econ.Streak / milestone.days, ActionOf(state), null, CARD_HEIGHT, () =>
            {
                if (Econ.ClaimStreakMilestone(milestone.id)) Reward(milestone.coin);
            });
    }

    private void BuildLevelMilestone(EconomyData.Milestone milestone, int cleared)
    {
        var state = Econ.GetLevelMilestoneState(milestone, cleared);
        AddCard(Loc.Get(milestone.locKey), Loc.Get("econ.progress", "a", Mathf.Min(cleared, milestone.days), "b", milestone.days, "n", milestone.coin),
            (float)cleared / milestone.days, ActionOf(state), null, CARD_HEIGHT, () =>
            {
                if (Econ.ClaimLevelMilestone(milestone.id, cleared)) Reward(milestone.coin);
            });
    }

    private void BuildWeek()
    {
        var row = Econ.DayRow;
        if (row.week <= 0)
        {
            AddSection(Loc.Get("econ.weekN", "n", "-"));
            AddNote(Loc.Get("econ.noWeek"));
            return;
        }
        var week = Econ.CurrentPeriod(EconomyService.SCOPE_WEEKLY);
        AddSection(Loc.Get("econ.periodDays", "p", Loc.Get("econ.weekN", "n", row.week), "a", week.startDay, "b", week.endDay));
        BuildQuests(EconomyService.SCOPE_WEEKLY);
    }

    private void BuildMonth()
    {
        var month = Econ.CurrentPeriod(EconomyService.SCOPE_MONTHLY);
        AddSection(Loc.Get("econ.periodDays", "p", Loc.Get(month.locKey), "a", month.startDay, "b", month.endDay));
        BuildQuests(EconomyService.SCOPE_MONTHLY);
    }

    private void BuildEvent()
    {
        var current = Econ.CurrentEvent();
        if (current != null)
        {
            AddSection(Loc.Get("econ.periodDays", "p", Loc.Get(current.locKey), "a", current.startDay, "b", current.endDay));
            BuildQuests(EconomyService.SCOPE_EVENT);
            return;
        }
        AddSection(Loc.Get("econ.tab.event"));
        AddNote(Loc.Get("econ.noEvent"));
        var next = NextEvent();
        if (next != null) AddNote(Loc.Get("econ.nextEvent", "e", Loc.Get(next.locKey), "a", next.startDay));
    }

    // Sự kiện sắp tới trong chu kỳ (sau hôm nay); hết chu kỳ thì null.
    private EconomyData.Period NextEvent()
    {
        EconomyData.Period best = null;
        foreach (var ev in Econ.Data.events)
            if (ev.startDay > Econ.DayIndex && (best == null || ev.startDay < best.startDay)) best = ev;
        return best;
    }

    private void BuildQuests(string scope)
    {
        foreach (var id in Econ.QuestRefs(scope))
        {
            var quest = Econ.Quest(id);
            var state = Econ.GetQuestState(scope, quest);
            int progress = Econ.Progress(scope, quest);
            AddCard(Loc.Get(quest.locKey), Loc.Get("econ.progress", "a", progress, "b", quest.target, "n", quest.coin),
                (float)progress / quest.target, ActionOf(state), null, CARD_HEIGHT, () =>
                {
                    if (Econ.ClaimQuest(scope, id)) Reward(quest.coin);
                });
        }
        int clear = Econ.AllClearCoin(scope);
        if (clear > 0)
            AddCard(Loc.Get("econ.allClear"), Loc.Get("econ.reward", "n", clear), -1f, ActionOf(Econ.GetAllClearState(scope)), null, CARD_HEIGHT_PLAIN, () =>
            {
                if (Econ.ClaimAllClear(scope)) Reward(clear);
            });
        AddNote(Loc.Get("econ.free"));
    }

    private void BuildYear()
    {
        var data = Econ.Data;
        AddSection(Loc.Get("econ.cycle", "c", Econ.CycleNo, "d", Econ.DayIndex));
        var month = data.months[yearMonth - 1];
        AddNav(Loc.Get(month.locKey), () => ShiftMonth(-1), () => ShiftMonth(1));

        var grid = Instantiate(gridTemplate, content, false);
        grid.gameObject.SetActive(true);
        int offset = (month.startDay - 1) % GRID_COLUMNS;
        int cells = offset + (month.endDay - month.startDay + 1);
        int rows = (cells + GRID_COLUMNS - 1) / GRID_COLUMNS;
        grid.GetComponent<LayoutElement>().preferredHeight = rows * GRID_CELL + (rows - 1) * GRID_SPACING;
        for (int i = 0; i < offset; i++) AddCell(grid, 0, false);
        for (int d = month.startDay; d <= month.endDay; d++) AddCell(grid, d, true);

        AddNote(Loc.Get("econ.legend"));
        BuildDayDetail(yearSelected);
    }

    private void ShiftMonth(int step)
    {
        Sound.instance.PlayButton();
        int count = Econ.Data.months.Length;
        yearMonth = (yearMonth - 1 + step + count) % count + 1;
        Rebuild(false);
    }

    private void AddCell(RectTransform grid, int day, bool real)
    {
        var cell = Instantiate(cellTemplate, grid, false);
        cell.gameObject.SetActive(true);
        var outer = cell.GetComponent<Image>();
        var face = cell.Find("Face").GetComponent<Image>();
        var label = cell.Find("Text").GetComponent<TMP_Text>();
        var button = cell.GetComponent<Button>();
        if (!real)
        {
            outer.color = Color.clear;
            face.color = Color.clear;
            label.text = "";
            button.interactable = false;
            return;
        }
        var row = Econ.DayRowAt(day);
        var current = Econ.DayRow;
        Color tint = WHITE;
        if (row.bridge) tint = YELLOW_SOFT;
        else if (!string.IsNullOrEmpty(row.eventRef)) tint = CORAL_SOFT;
        else if (row.week > 0 && row.week == current.week) tint = TEAL_SOFT;
        bool selected = day == yearSelected;
        outer.color = day == Econ.DayIndex ? TEAL : LINE;
        face.color = selected ? TEAL : tint;
        label.text = day.ToString();
        label.color = selected ? WHITE : INK;
        int picked = day;
        button.onClick.AddListener(() =>
        {
            Sound.instance.PlayButton();
            yearSelected = picked;
            Rebuild(false);
        });
    }

    private void BuildDayDetail(int day)
    {
        var row = Econ.DayRowAt(day);
        string title = row.bridge ? Loc.Get("econ.bridge", "d", day) : Loc.Get("econ.dayWeek", "d", day, "w", row.week);
        var login = Econ.LoginReward(row.loginRef);
        var lines = new List<string>();
        lines.Add(Loc.Get(login.locKey) + " " + Loc.Get("econ.reward", "n", login.coin));
        var names = new List<string>();
        foreach (var id in row.dailyQuestRefs) names.Add(Loc.Get(Econ.Quest(id).locKey));
        lines.Add(string.Join(", ", names));
        var ev = string.IsNullOrEmpty(row.eventRef) ? null : Econ.EventById(row.eventRef);
        if (ev != null) lines.Add(Loc.Get("econ.periodDays", "p", Loc.Get(ev.locKey), "a", ev.startDay, "b", ev.endDay));
        AddCard(title, string.Join("\n", lines), -1f, Btn.None, null, DETAIL_HEIGHT, null);
    }

    // ---------- phần tử ----------

    private TMP_Text AddSection(string text)
    {
        var section = Instantiate(sectionTemplate, content, false);
        section.gameObject.SetActive(true);
        var label = section.GetComponent<TMP_Text>();
        label.text = text;
        return label;
    }

    private void AddNote(string text)
    {
        var note = AddSection(text);
        note.alignment = TextAlignmentOptions.Center;
        note.fontSize = note.fontSize * 0.9f;
        note.GetComponent<LayoutElement>().preferredHeight = 64f;
    }

    private void AddNav(string text, UnityEngine.Events.UnityAction previous, UnityEngine.Events.UnityAction next)
    {
        var nav = Instantiate(navTemplate, content, false);
        nav.gameObject.SetActive(true);
        nav.Find("Label").GetComponent<TMP_Text>().text = text;
        nav.Find("Prev").GetComponent<Button>().onClick.AddListener(previous);
        nav.Find("Next").GetComponent<Button>().onClick.AddListener(next);
    }

    // progress < 0: không có thanh tiến độ. costText chỉ dùng cho Btn.Cost (nút trả ruby).
    private void AddCard(string title, string sub, float progress, Btn action, string costText, float height, UnityEngine.Events.UnityAction onClick)
    {
        var card = Instantiate(cardTemplate, content, false);
        card.gameObject.SetActive(true);
        card.GetComponent<LayoutElement>().preferredHeight = height;
        var titleLabel = card.Find("Title").GetComponent<TMP_Text>();
        var subLabel = card.Find("Sub").GetComponent<TMP_Text>();
        var bar = (RectTransform)card.Find("Bar");
        var button = card.Find("Button").GetComponent<Button>();
        titleLabel.text = title;
        subLabel.text = sub;
        bar.gameObject.SetActive(progress >= 0f);
        bar.offsetMax = new Vector2(action == Btn.None ? -22f : -190f, bar.offsetMax.y);
        if (progress >= 0f)
        {
            var fill = (RectTransform)bar.Find("Fill");
            fill.anchorMax = new Vector2(Mathf.Clamp01(progress), 1f);
        }
        // Không có nút: chữ trải hết bề ngang thẻ.
        float right = action == Btn.None ? -22f : -190f;
        foreach (var label in new[] { titleLabel, subLabel })
            label.rectTransform.offsetMax = new Vector2(right, label.rectTransform.offsetMax.y);
        subLabel.rectTransform.offsetMin = new Vector2(subLabel.rectTransform.offsetMin.x, progress >= 0f ? 40f : 16f);
        button.gameObject.SetActive(action != Btn.None);
        if (action == Btn.None) return;

        var image = button.GetComponent<Image>();
        var shadow = button.GetComponent<Shadow>();
        var text = button.transform.Find("Text").GetComponent<TMP_Text>();
        switch (action)
        {
            case Btn.Claim:
                Style(image, shadow, text, YELLOW, YELLOW_DEEP, DARK_YELLOW_TEXT, Loc.Get("econ.claim"), true);
                break;
            case Btn.Cost:
                Style(image, shadow, text, CORAL, CORAL_DEEP, WHITE, costText, true);
                break;
            case Btn.Claimed:
                Style(image, shadow, text, LINE, LINE, INK_SOFT, Loc.Get("econ.claimed"), false);
                break;
            default:
                Style(image, shadow, text, LINE, LINE, INK_SOFT, Loc.Get("econ.soon"), false);
                break;
        }
        if (onClick != null) button.onClick.AddListener(onClick);
    }

    private void Style(Image image, Shadow shadow, TMP_Text text, Color fill, Color edge, Color ink, string label, bool enabled)
    {
        image.color = fill;
        shadow.effectColor = edge;
        text.color = ink;
        text.text = label;
        image.GetComponent<Button>().interactable = enabled;
    }

    // Thẻ nhiệm vụ: chưa tới mức nhận thì chỉ hiện thanh tiến độ, không có nút.
    private Btn ActionOf(QuestState state)
    {
        switch (state)
        {
            case QuestState.Claimable: return Btn.Claim;
            case QuestState.Claimed: return Btn.Claimed;
            case QuestState.Unavailable: return Btn.Soon;
            default: return Btn.None;
        }
    }

    private void Reward(int coin)
    {
        Sound.instance.PlayButton();
        if (Toast.instance != null) Toast.instance.ShowMessage(Loc.Get("econ.reward", "n", coin));
    }
}
