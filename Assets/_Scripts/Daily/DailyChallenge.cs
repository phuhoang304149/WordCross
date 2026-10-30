using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Script.Utils;
using WordCross.Gameplay;

public enum DayStatus { Future, Done, Today, Miss, Old }

public enum StreakState { Safe, Risk, Lost }

public class DailyResult
{
    public string date;
    public bool first;
    public int reward;
    public int milestone;
    public int milestoneDays;
    public int streak;
}

// Luật Daily Challenge (design: systems.js WC.daily*). Dữ liệu lưu PlayerPrefs, key mới daily_* (theo ngôn ngữ chơi qua ProgressKeys; en giữ khoá cũ); không đụng tiến độ chính.
// Ngày tính theo giờ máy (00:00), định dạng yyyy-MM-dd. Chống đổi giờ lùi: "hôm nay" = max(ngày máy, ngày lớn nhất từng thấy).
public class DailyChallenge
{
    // Kinh tế (THAY ĐỔI KINH TẾ, số đề xuất theo design ECON): chỉnh tại đây.
    // thưởng thắng bàn của hôm nay, một lần: dailyChallengeCoin của EconomyData (30); 3 chỉ là dự phòng khi chưa có EconomyService.
    public int baseReward { get { var econ = Locator<EconomyService>.Instance; return econ == null ? 3 : econ.Data.dailyChallengeCoin; } }
    // USER duyệt 2026-10-31 (econ-daily-old): chỉ còn số GD, bỏ thưởng bù hôm qua và mốc chuỗi Daily cũ (2/5/8).
    // Bàn hôm qua vẫn chơi bù được để giữ chuỗi, chỉ không có ruby; mảng mốc rỗng nên dòng "mốc kế" tự ẩn.
    public int backfillReward = 0;
    public int[] milestoneDays = { };
    public int[] milestoneRewards = { };

    // 12 level đầu (sub-world hướng dẫn) không vào pool Daily.
    private const int POOL_SKIP = 12;
    private const string SOLVED_KEY = "daily_solved";
    private const string MILESTONES_KEY = "daily_milestones";
    private const string BEST_KEY = "daily_best";
    private const string LAST_SEEN_KEY = "daily_last_seen";
    private const string DATE_FORMAT = "yyyy-MM-dd";
    private const string EPOCH = "2026-01-01";

    private HashSet<string> solved;
    private HashSet<string> milestones;

    public int best
    {
        get { return CPlayerPrefs.GetInt(Prefs.progressKeys.Of(BEST_KEY), 0); }
        private set { CPlayerPrefs.SetInt(Prefs.progressKeys.Of(BEST_KEY), value); }
    }

    // ---------- ngày ----------

    public string Today()
    {
        string device = DateTime.Now.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);
        string seen = CPlayerPrefs.GetString(LAST_SEEN_KEY, "");
        if (seen.Length == 0 || string.CompareOrdinal(device, seen) > 0)
        {
            CPlayerPrefs.SetString(LAST_SEEN_KEY, device);
            return device;
        }
        return seen;
    }

    public DateTime Parse(string date)
    {
        return DateTime.ParseExact(date, DATE_FORMAT, CultureInfo.InvariantCulture);
    }

    public string AddDays(string date, int days)
    {
        return Parse(date).AddDays(days).ToString(DATE_FORMAT, CultureInfo.InvariantCulture);
    }

    public int DaysBetween(string from, string to)
    {
        return (int)Math.Round((Parse(to) - Parse(from)).TotalDays);
    }

    public int DailyNo(string date)
    {
        return DaysBetween(EPOCH, date) + 1;
    }

    // Ngày ngắn theo ngôn ngữ UI ("29/10", "Oct 29", "29 oct.").
    public string ShortDate(string date)
    {
        var loc = Locator<LocalizationController>.Instance;
        DateTime d = Parse(date);
        string[] months = loc.Get("daily.monthsShort").Split('|');
        return loc.Get("daily.shortDate", "d", d.Day, "m", months[d.Month - 1]);
    }

    // ---------- bàn chơi ----------

    // FNV-1a như design (WC.hash): cùng ngày thì mọi máy ra cùng một bàn.
    public uint Hash(string text)
    {
        uint h = 2166136261;
        for (int i = 0; i < text.Length; i++)
        {
            h ^= text[i];
            h *= 16777619;
        }
        return h;
    }

    // Mã ngôn ngữ chơi ("en" khi chưa có Localization) làm hạt giống bàn Daily: mỗi ngôn ngữ một bàn riêng, en giữ nguyên.
    private string GameLang()
    {
        var loc = Locator<LocalizationController>.Instance;
        return loc == null ? "en" : loc.GameLanguage;
    }

    public int PoolSize()
    {
        int total = 0;
        for (int w = 0; w < Superpow.Utils.NUM_WORLDS; w++)
            for (int s = 0; s < Const.NUM_SUBWORLD; s++)
                total += Superpow.Utils.GetNumLevels(w, s);
        return total - POOL_SKIP;
    }

    // Chọn level thứ (POOL_SKIP + hash % pool) theo thứ tự world/sub/level.
    public void PickLevel(string date, out int world, out int subWorld, out int level)
    {
        int index = POOL_SKIP + (int)(Hash(GameLang() + "|" + date) % (uint)PoolSize());
        for (world = 0; world < Superpow.Utils.NUM_WORLDS; world++)
        {
            for (subWorld = 0; subWorld < Const.NUM_SUBWORLD; subWorld++)
            {
                int count = Superpow.Utils.GetNumLevels(world, subWorld);
                if (index < count)
                {
                    level = index;
                    return;
                }
                index -= count;
            }
        }
        world = 0;
        subWorld = 1;
        level = 0;
    }

    // ---------- trạng thái ----------

    private HashSet<string> Solved()
    {
        if (solved == null) solved = Load(SOLVED_KEY);
        return solved;
    }

    private HashSet<string> Milestones()
    {
        if (milestones == null) milestones = Load(MILESTONES_KEY);
        return milestones;
    }

    private HashSet<string> Load(string key)
    {
        string raw = CPlayerPrefs.GetString(Prefs.progressKeys.Of(key), "");
        return new HashSet<string>(raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
    }

    private void Save(string key, HashSet<string> set)
    {
        var sb = new StringBuilder();
        foreach (string item in set)
        {
            if (sb.Length > 0) sb.Append(',');
            sb.Append(item);
        }
        CPlayerPrefs.SetString(Prefs.progressKeys.Of(key), sb.ToString());
    }

    public bool IsSolved(string date)
    {
        return Solved().Contains(date);
    }

    public DayStatus Status(string date)
    {
        string today = Today();
        int diff = string.CompareOrdinal(date, today);
        if (diff > 0) return DayStatus.Future;
        if (IsSolved(date)) return DayStatus.Done;
        if (diff == 0) return DayStatus.Today;
        return date == AddDays(today, -1) ? DayStatus.Miss : DayStatus.Old;
    }

    // Thưởng lần đầu thắng bàn `date` (0 nếu đã quá hạn bù hoặc đã thắng).
    public int RewardFor(string date)
    {
        DayStatus status = Status(date);
        if (status == DayStatus.Today) return baseReward;
        if (status == DayStatus.Miss) return backfillReward;
        return 0;
    }

    // ---------- chuỗi ----------

    private int StreakFrom(string end)
    {
        int n = 0;
        string d = end;
        while (IsSolved(d))
        {
            n++;
            d = AddDays(d, -1);
        }
        return n;
    }

    // Điểm cuối chuỗi: hôm nay nếu đã xong, không thì hôm qua; lỡ hôm qua mà còn chuỗi trước đó thì vẫn tính (chờ bù).
    public string StreakEnd()
    {
        string today = Today(), yesterday = AddDays(today, -1);
        if (IsSolved(today)) return today;
        if (IsSolved(yesterday)) return yesterday;
        if (IsSolved(AddDays(today, -2))) return AddDays(today, -2);
        return yesterday;
    }

    public int Streak()
    {
        return StreakFrom(StreakEnd());
    }

    public StreakState GetStreakState()
    {
        string today = Today(), yesterday = AddDays(today, -1);
        if (IsSolved(today)) return StreakState.Safe;
        if (IsSolved(yesterday) || IsSolved(AddDays(today, -2))) return StreakState.Risk;
        return StreakState.Lost;
    }

    // Mốc kế tiếp sau chuỗi n; false nếu đã qua hết các mốc.
    public bool NextMilestone(int n, out int days, out int reward)
    {
        for (int i = 0; i < milestoneDays.Length; i++)
        {
            if (milestoneDays[i] > n)
            {
                days = milestoneDays[i];
                reward = milestoneRewards[i];
                return true;
            }
        }
        days = 0;
        reward = 0;
        return false;
    }

    // ---------- hoàn thành ----------

    // Ghi thắng bàn `date`: thưởng cơ bản một lần + mốc chuỗi một lần cho mỗi chuỗi. Lưu trước khi cộng ruby để không nhận lại.
    public DailyResult Complete(string date)
    {
        var result = new DailyResult { date = date };
        int baseAmount = 0;
        result.first = !IsSolved(date);
        if (result.first)
        {
            baseAmount = RewardFor(date);
            Solved().Add(date);
            Save(SOLVED_KEY, Solved());
        }

        int run = Streak();
        string start = AddDays(StreakEnd(), -(run - 1));
        int milestoneAmount = 0;
        for (int i = 0; i < milestoneDays.Length; i++)
        {
            string id = start + ":" + milestoneDays[i];
            if (run >= milestoneDays[i] && !Milestones().Contains(id))
            {
                Milestones().Add(id);
                milestoneAmount += milestoneRewards[i];
                result.milestoneDays = milestoneDays[i];
            }
        }
        if (milestoneAmount > 0) Save(MILESTONES_KEY, Milestones());
        if (run > best) best = run;
        CPlayerPrefs.Save();

        if (baseAmount + milestoneAmount > 0) CurrencyController.CreditBalance(baseAmount + milestoneAmount);
        result.reward = baseAmount;
        result.milestone = milestoneAmount;
        result.streak = run;
        return result;
    }
}
