using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace WordCross.Gameplay
{
    // Kinh tế coin + chu kỳ 365 ngày (design/GD/economy-year.html, economy-config.json#rules).
    // Ngày theo UTC: dayNumber = số ngày từ anchorUtcDate, dayIndex = dayNumber % 365 + 1, cycleId = "C" + (dayNumber / 365 + 1).
    // Mọi phần thưởng đi qua khoá claim:{cycleId}:{scope}:{periodId}:{refId}: ghi ledger trước rồi mới cộng coin, trùng khoá thì bỏ qua.
    public class EconomyService
    {
        public const string SCOPE_DAILY = "daily";
        public const string SCOPE_WEEKLY = "weekly";
        public const string SCOPE_MONTHLY = "monthly";
        public const string SCOPE_EVENT = "event";
        public const string SCOPE_LOGIN = "login";
        public const string SCOPE_STREAK = "streak";

        public const string METRIC_WIN = "levels_won";
        public const string METRIC_WORDS = "words_found";
        public const string METRIC_DAILY = "daily_challenge_done";
        public const string METRIC_BOT_PLAYED = "bot_match_played";
        public const string METRIC_BOT_WON = "bot_match_won";
        public const string METRIC_EXTRA = "extra_word_found";
        public const string METRIC_LOGIN = "login_days";

        public const string AD_SHOP = "ad_shop_free";
        public const string AD_LOW_COIN = "ad_low_coin";
        public const string AD_WIN_X2 = "ad_win_x2";
        public const string AD_DAILY_X2 = "ad_daily_x2";

        public const string SINK_HINT_CELL = "sk_hint_cell";
        public const string SINK_HINT_WORD = "sk_hint_word";
        public const string SINK_STREAK_REPAIR = "sk_streak_repair";

        private const string INIT_KEY = "init:balance_v1";
        private const string LIFE_PREFIX = "claim:life:";
        private const string FIRST_WIN_KEY = "claim:life:win:first";
        private const string AD_PREFIX = "ad:";
        private const string ALL_CLEAR = "allclear";

        private static readonly string[] QUEST_SCOPES = { SCOPE_DAILY, SCOPE_WEEKLY, SCOPE_MONTHLY, SCOPE_EVENT };

        private readonly EconomyData data;
        private readonly IEconomyStore store;
        private readonly IEconomyClock clock;
        private readonly Dictionary<string, EconomyData.Quest> quests = new Dictionary<string, EconomyData.Quest>();
        private readonly Dictionary<string, EconomyData.LoginReward> logins = new Dictionary<string, EconomyData.LoginReward>();
        private readonly HashSet<string> claims = new HashSet<string>();
        private readonly Dictionary<string, int> progress = new Dictionary<string, int>();
        private readonly DateTime anchor;
        private EconomyState state;
        private int today;

        // Bot chưa có trên nhánh chính: nhiệm vụ bot hiện "Sắp có" và không chặn thưởng hoàn thành hết.
        public bool botAvailable;

        public event Action onChanged;

        public EconomyService(EconomyData data, IEconomyStore store, IEconomyClock clock)
        {
            this.data = data;
            this.store = store;
            this.clock = clock;
            foreach (var quest in data.quests) quests[quest.id] = quest;
            foreach (var login in data.loginRewards) logins[login.id] = login;
            anchor = DateTime.ParseExact(data.anchorUtcDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
        }

        public EconomyData Data { get { return data; } }

        // ---------- khởi tạo / lưu ----------

        public void Initialize()
        {
            string json = store.LoadState();
            state = json.Length > 0 ? JsonUtility.FromJson<EconomyState>(json) : null;
            if (state == null) state = new EconomyState();
            claims.Clear();
            foreach (var key in state.claims) claims.Add(key);
            progress.Clear();
            for (int i = 0; i < state.progressKeys.Count && i < state.progressValues.Count; i++) progress[state.progressKeys[i]] = state.progressValues[i];
            today = Mathf.Max(0, state.maxSeenDay);
            InitBalance();
            Tick();
        }

        private void Save()
        {
            state.claims = new List<string>(claims);
            state.progressKeys = new List<string>(progress.Keys);
            state.progressValues = new List<int>(progress.Values);
            store.SaveState(JsonUtility.ToJson(state));
        }

        private void Changed()
        {
            if (onChanged != null) onChanged();
        }

        // Ruby là tiền tệ duy nhất: người cũ giữ nguyên số dư, chưa có ví thì nhận startBalance của GD (một lần).
        private void InitBalance()
        {
            if (claims.Contains(INIT_KEY)) return;
            claims.Add(INIT_KEY);
            state.initialized = true;
            bool hasWallet = store.HasLegacyBalance();
            Save();
            if (!hasWallet) store.SetBalance(data.startBalance);
        }

        // ---------- thời gian ----------

        public DateTime UtcNow { get { return clock.UtcNow; } }

        public double NowSeconds { get { return (clock.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds; } }

        public int DayNumberAt(DateTime utc)
        {
            return Mathf.Max(0, (int)Math.Floor((utc.Date - anchor).TotalDays));
        }

        // Gọi khi mở popup/scene: sang ngày mới thì dọn kỳ cũ và reset bộ đếm ngày. Đồng hồ lùi không làm ngày lùi.
        public void Tick()
        {
            int now = DayNumberAt(clock.UtcNow);
            if (now > state.maxSeenDay)
            {
                state.maxSeenDay = now;
                today = now;
                Prune();
                Save();
                Changed();
            }
            today = state.maxSeenDay;
            ResetDayCounters();
        }

        public int Today { get { return today; } }
        public int DayIndex { get { return today % data.lengthDays + 1; } }
        public int CycleNo { get { return today / data.lengthDays + 1; } }
        public string CycleId { get { return "C" + CycleNo; } }
        public EconomyData.Day DayRow { get { return data.days[DayIndex - 1]; } }

        public EconomyData.Day DayRowAt(int dayIndex)
        {
            return data.days[dayIndex - 1];
        }

        // Giây còn lại tới 00:00 UTC kế tiếp (đếm ngược nhiệm vụ ngày).
        public int SecondsToNextDay()
        {
            DateTime now = clock.UtcNow;
            return Mathf.Max(0, (int)(now.Date.AddDays(1) - now).TotalSeconds);
        }

        public string PeriodId(string scope)
        {
            var row = DayRow;
            switch (scope)
            {
                case SCOPE_DAILY:
                case SCOPE_LOGIN:
                    return "d" + row.day.ToString("000");
                case SCOPE_WEEKLY:
                    return row.week > 0 ? "w" + row.week.ToString("00") : "bridge";
                case SCOPE_MONTHLY:
                    return "m" + row.month.ToString("00");
                case SCOPE_EVENT:
                    return string.IsNullOrEmpty(row.eventRef) ? "none" : row.eventRef;
                default:
                    return "cycle";
            }
        }

        private string Prefix(string scope)
        {
            return CycleId + ":" + scope + ":" + PeriodId(scope) + ":";
        }

        public string ClaimKey(string scope, string refId)
        {
            return "claim:" + Prefix(scope) + refId;
        }

        // Giữ khoá trọn đời, khoá khởi tạo ví, khoá claim/tiến độ của kỳ hiện tại và phiên quảng cáo hôm nay.
        private void Prune()
        {
            var keep = new List<string>();
            foreach (var scope in new[] { SCOPE_DAILY, SCOPE_WEEKLY, SCOPE_MONTHLY, SCOPE_EVENT, SCOPE_LOGIN, SCOPE_STREAK }) keep.Add(Prefix(scope));
            string adToday = AD_PREFIX + today + ":";
            claims.RemoveWhere(key => !(key.StartsWith(LIFE_PREFIX) || key == INIT_KEY || key.StartsWith(adToday) || StartsWithAny("claim:" , key, keep)));
            var stale = new List<string>();
            foreach (var key in progress.Keys)
                if (!StartsWithAny("", key, keep)) stale.Add(key);
            foreach (var key in stale) progress.Remove(key);
        }

        private bool StartsWithAny(string head, string key, List<string> prefixes)
        {
            foreach (var prefix in prefixes)
                if (key.StartsWith(head + prefix, StringComparison.Ordinal)) return true;
            return false;
        }

        private void ResetDayCounters()
        {
            if (state.counterDay == today) return;
            state.counterDay = today;
            state.adCountKeys.Clear();
            state.adCountValues.Clear();
            state.adCoinToday = 0;
            state.interCount = 0;
            state.botEarned = 0;
            state.pendingSession = "";
            state.pendingPlacement = "";
            Save();
        }

        // ---------- coin ----------

        public int Balance { get { return store.GetBalance(); } }

        public bool IsClaimed(string key)
        {
            return claims.Contains(key);
        }

        // Ghi ledger trước rồi mới cộng: app chết giữa chừng thì mất thưởng chứ không nhận đôi.
        public bool TryClaim(string key, int coin)
        {
            if (claims.Contains(key)) return false;
            claims.Add(key);
            Save();
            if (coin > 0) store.Credit(coin);
            Changed();
            return true;
        }

        public bool Spend(string sinkId)
        {
            var sink = Sink(sinkId);
            if (sink == null || !store.Debit(sink.coin)) return false;
            Changed();
            return true;
        }

        public EconomyData.Sink Sink(string id)
        {
            foreach (var sink in data.sinks)
                if (sink.id == id) return sink;
            return null;
        }

        public int SinkCost(string id)
        {
            var sink = Sink(id);
            return sink == null ? 0 : sink.coin;
        }

        public EconomyData.Pack PackFor(string productId)
        {
            foreach (var pack in data.packs)
                if (pack.productId == productId) return pack;
            return null;
        }

        // ---------- thắng / từ / daily / bot ----------

        // Thắng level chính: tính tiến độ nhiệm vụ mọi lần thắng; coin chỉ lần đầu (firstClear do tiến độ level quyết định) + 20 cho level đầu tiên của tài khoản.
        public int OnLevelWon(bool firstClear, bool firstLevel)
        {
            AddProgress(METRIC_WIN, 1);
            int coin = firstClear ? data.firstWinCoin : data.replayWinCoin;
            if (firstClear && firstLevel && !claims.Contains(FIRST_WIN_KEY))
            {
                claims.Add(FIRST_WIN_KEY);
                coin += data.firstEverWinBonusCoin;
            }
            Save();
            if (coin <= 0) return 0;
            store.Credit(coin);
            Changed();
            return coin;
        }

        public void OnWordFound()
        {
            AddProgress(METRIC_WORDS, 1);
        }

        // Từ thưởng mới: tiến độ nhiệm vụ; coin chỉ cho từ thưởng đầu tiên của level (tối đa extraWordCapPerLevel = 1).
        public int OnExtraWord(bool firstInLevel)
        {
            AddProgress(METRIC_EXTRA, 1);
            if (!firstInLevel || data.extraWordCapPerLevel <= 0) return 0;
            store.Credit(data.extraWordCoin);
            Changed();
            return data.extraWordCoin;
        }

        public void OnDailyChallengeDone()
        {
            AddProgress(METRIC_DAILY, 1);
        }

        // Bot (chưa nối: chế độ Bot chưa có trên nhánh chính). result: 1 thắng, 0 hoà, -1 thua; trần botDailyCap coin/ngày.
        public int OnBotMatch(string difficulty, int result)
        {
            Tick();
            AddProgress(METRIC_BOT_PLAYED, 1);
            if (result > 0) AddProgress(METRIC_BOT_WON, 1);
            int coin = result < 0 ? 0 : result == 0 ? data.botDrawCoin : difficulty == "hard" ? data.botHardCoin : difficulty == "medium" ? data.botMediumCoin : data.botEasyCoin;
            coin = Mathf.Clamp(coin, 0, data.botDailyCap - state.botEarned);
            if (coin <= 0) return 0;
            state.botEarned += coin;
            Save();
            store.Credit(coin);
            Changed();
            return coin;
        }

        // ---------- nhiệm vụ ----------

        public void AddProgress(string metric, int amount)
        {
            Tick();
            foreach (var scope in QUEST_SCOPES)
            {
                if (QuestRefs(scope).Length == 0) continue;
                string key = Prefix(scope) + metric;
                int value;
                progress.TryGetValue(key, out value);
                progress[key] = value + amount;
            }
            Save();
            Changed();
        }

        public string[] QuestRefs(string scope)
        {
            var row = DayRow;
            switch (scope)
            {
                case SCOPE_DAILY:
                    return row.dailyQuestRefs;
                case SCOPE_WEEKLY:
                    return row.week > 0 ? data.weeks[row.week - 1].questRefs : new string[0];
                case SCOPE_MONTHLY:
                    return data.months[row.month - 1].questRefs;
                case SCOPE_EVENT:
                    var ev = CurrentEvent();
                    return ev == null ? new string[0] : ev.questRefs;
            }
            return new string[0];
        }

        public EconomyData.Quest Quest(string id)
        {
            EconomyData.Quest quest;
            return quests.TryGetValue(id, out quest) ? quest : null;
        }

        public EconomyData.Period CurrentEvent()
        {
            var row = DayRow;
            if (string.IsNullOrEmpty(row.eventRef)) return null;
            return EventById(row.eventRef);
        }

        public EconomyData.Period EventById(string id)
        {
            foreach (var ev in data.events)
                if (ev.id == id) return ev;
            return null;
        }

        public EconomyData.Period CurrentPeriod(string scope)
        {
            var row = DayRow;
            if (scope == SCOPE_WEEKLY) return row.week > 0 ? data.weeks[row.week - 1] : null;
            if (scope == SCOPE_MONTHLY) return data.months[row.month - 1];
            if (scope == SCOPE_EVENT) return CurrentEvent();
            return null;
        }

        // Ngày cuối (dayIndex) của kỳ hiện tại; daily = hôm nay.
        public int PeriodEndDay(string scope)
        {
            var period = CurrentPeriod(scope);
            return period == null ? DayIndex : period.endDay;
        }

        public int Progress(string scope, EconomyData.Quest quest)
        {
            int value;
            progress.TryGetValue(Prefix(scope) + quest.metric, out value);
            return Mathf.Min(value, quest.target);
        }

        public bool IsQuestAvailable(EconomyData.Quest quest)
        {
            return botAvailable || (quest.metric != METRIC_BOT_PLAYED && quest.metric != METRIC_BOT_WON);
        }

        public QuestState GetQuestState(string scope, EconomyData.Quest quest)
        {
            if (IsClaimed(ClaimKey(scope, quest.id))) return QuestState.Claimed;
            if (!IsQuestAvailable(quest)) return QuestState.Unavailable;
            return Progress(scope, quest) >= quest.target ? QuestState.Claimable : QuestState.InProgress;
        }

        public bool ClaimQuest(string scope, string questId)
        {
            Tick();
            var quest = Quest(questId);
            if (quest == null || Array.IndexOf(QuestRefs(scope), questId) < 0) return false;
            if (GetQuestState(scope, quest) != QuestState.Claimable) return false;
            return TryClaim(ClaimKey(scope, questId), quest.coin);
        }

        public int AllClearCoin(string scope)
        {
            if (QuestRefs(scope).Length == 0) return 0;
            if (scope == SCOPE_DAILY) return data.dailyAllClearCoin;
            var period = CurrentPeriod(scope);
            return period == null ? 0 : period.allClearCoin;
        }

        public QuestState GetAllClearState(string scope)
        {
            if (IsClaimed(ClaimKey(scope, ALL_CLEAR))) return QuestState.Claimed;
            foreach (var id in QuestRefs(scope))
            {
                var quest = Quest(id);
                if (IsQuestAvailable(quest) && !IsClaimed(ClaimKey(scope, id))) return QuestState.InProgress;
            }
            return QuestState.Claimable;
        }

        public bool ClaimAllClear(string scope)
        {
            Tick();
            if (AllClearCoin(scope) <= 0 || GetAllClearState(scope) != QuestState.Claimable) return false;
            return TryClaim(ClaimKey(scope, ALL_CLEAR), AllClearCoin(scope));
        }

        // ---------- đăng nhập / chuỗi ----------

        public EconomyData.LoginReward LoginReward(string id)
        {
            EconomyData.LoginReward login;
            return logins.TryGetValue(id, out login) ? login : null;
        }

        public EconomyData.LoginReward TodayLogin { get { return LoginReward(DayRow.loginRef); } }

        public bool IsLoginClaimed { get { return IsClaimed(ClaimKey(SCOPE_LOGIN, DayRow.loginRef)); } }

        public bool ClaimLogin()
        {
            Tick();
            var login = TodayLogin;
            if (!TryClaim(ClaimKey(SCOPE_LOGIN, login.id), login.coin)) return false;
            state.streak = state.lastLoginDay == today - 1 ? state.streak + 1 : 1;
            state.lastLoginDay = today;
            state.bestStreak = Mathf.Max(state.bestStreak, state.streak);
            Save();
            AddProgress(METRIC_LOGIN, 1);
            return true;
        }

        // Chuỗi đang có hiệu lực: đứt khi lần claim cuối cũ hơn hôm qua.
        public int Streak { get { return state.lastLoginDay >= today - 1 ? state.streak : 0; } }
        public int BestStreak { get { return state.bestStreak; } }

        // Lỡ đúng 1 ngày (lần claim cuối là hôm kia) và hôm nay chưa claim: trả 50 coin để giữ chuỗi.
        public bool CanRepairStreak
        {
            get
            {
                var sink = Sink(SINK_STREAK_REPAIR);
                int missed = sink == null ? 1 : Mathf.Max(1, sink.maxMissedDays);
                return state.streak > 0 && !IsLoginClaimed && state.lastLoginDay < today - 1 && state.lastLoginDay >= today - 1 - missed;
            }
        }

        public bool RepairStreak()
        {
            Tick();
            if (!CanRepairStreak || !Spend(SINK_STREAK_REPAIR)) return false;
            state.lastLoginDay = today - 1;
            Save();
            Changed();
            return true;
        }

        public QuestState GetStreakMilestoneState(EconomyData.Milestone milestone)
        {
            if (IsClaimed(ClaimKey(SCOPE_STREAK, milestone.id))) return QuestState.Claimed;
            return Streak >= milestone.days ? QuestState.Claimable : QuestState.InProgress;
        }

        public bool ClaimStreakMilestone(string id)
        {
            Tick();
            foreach (var milestone in data.streakMilestones)
                if (milestone.id == id && GetStreakMilestoneState(milestone) == QuestState.Claimable)
                    return TryClaim(ClaimKey(SCOPE_STREAK, id), milestone.coin);
            return false;
        }

        public QuestState GetLevelMilestoneState(EconomyData.Milestone milestone, int levelsCleared)
        {
            if (IsClaimed(LIFE_PREFIX + "level:" + milestone.id)) return QuestState.Claimed;
            return levelsCleared >= milestone.days ? QuestState.Claimable : QuestState.InProgress;
        }

        public bool ClaimLevelMilestone(string id, int levelsCleared)
        {
            foreach (var milestone in data.levelMilestones)
                if (milestone.id == id && GetLevelMilestoneState(milestone, levelsCleared) == QuestState.Claimable)
                    return TryClaim(LIFE_PREFIX + "level:" + id, milestone.coin);
            return false;
        }

        // Chấm đỏ: còn thưởng chờ nhận ở bất kỳ tab nào.
        public bool HasClaimable(int levelsCleared)
        {
            Tick();
            if (!IsLoginClaimed) return true;
            foreach (var scope in QUEST_SCOPES)
            {
                foreach (var id in QuestRefs(scope))
                    if (GetQuestState(scope, Quest(id)) == QuestState.Claimable) return true;
                if (AllClearCoin(scope) > 0 && GetAllClearState(scope) == QuestState.Claimable) return true;
            }
            foreach (var milestone in data.streakMilestones)
                if (GetStreakMilestoneState(milestone) == QuestState.Claimable) return true;
            foreach (var milestone in data.levelMilestones)
                if (GetLevelMilestoneState(milestone, levelsCleared) == QuestState.Claimable) return true;
            return false;
        }

        // ---------- quảng cáo thưởng ----------

        public EconomyData.Rewarded RewardedFor(string id)
        {
            foreach (var ad in data.rewarded)
                if (ad.id == id) return ad;
            return null;
        }

        private int Count(string key)
        {
            int index = state.adCountKeys.IndexOf(key);
            return index < 0 ? 0 : state.adCountValues[index];
        }

        private void Increment(string key)
        {
            int index = state.adCountKeys.IndexOf(key);
            if (index < 0)
            {
                state.adCountKeys.Add(key);
                state.adCountValues.Add(1);
            }
            else state.adCountValues[index]++;
        }

        // Trần nhóm = dailyCap của vị trí đầu tiên trong nhóm (rw_coin 5, rw_x2 5); vị trí có trần riêng nhỏ hơn (ad_daily_x2 = 1) thì theo trần riêng.
        private int GroupCap(string group)
        {
            foreach (var ad in data.rewarded)
                if (ad.sharedCapGroup == group) return ad.dailyCap;
            return 0;
        }

        public int RewardedLeft(string id)
        {
            Tick();
            var ad = RewardedFor(id);
            if (ad == null) return 0;
            int left = Mathf.Min(GroupCap(ad.sharedCapGroup) - Count("g:" + ad.sharedCapGroup), ad.dailyCap - Count("p:" + id));
            if (state.adCoinToday + ad.coin > data.maxRewardedCoinPerDay) left = 0;
            return Mathf.Max(0, left);
        }

        public int CooldownRemain(string id)
        {
            var ad = RewardedFor(id);
            int index = state.cooldownKeys.IndexOf(id);
            if (ad == null || index < 0) return 0;
            double remain = state.cooldownUntil[index] - NowSeconds;
            // Đồng hồ bị chỉnh lùi: không để hồi chiêu dài hơn cooldownSec.
            if (remain > ad.cooldownSec) remain = ad.cooldownSec;
            return Mathf.Max(0, Mathf.CeilToInt((float)remain));
        }

        public EconomyAdState GetAdState(string id)
        {
            if (RewardedLeft(id) <= 0) return EconomyAdState.CapReached;
            if (CooldownRemain(id) > 0) return EconomyAdState.Cooldown;
            return EconomyAdState.Ready;
        }

        // Bắt đầu một phiên xem; thưởng chỉ cộng khi CompleteRewarded khớp phiên này.
        public string BeginRewarded(string id)
        {
            Tick();
            if (GetAdState(id) != EconomyAdState.Ready) return null;
            state.sessionCounter++;
            state.pendingSession = AD_PREFIX + today + ":" + state.sessionCounter;
            state.pendingPlacement = id;
            Save();
            return state.pendingSession;
        }

        public string PendingPlacement { get { return state.pendingPlacement; } }

        // Callback "xem xong" của SDK: idempotent theo sessionId (callback lặp, bấm đúp, kill app đều không nhận đôi).
        public int CompleteRewarded(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId) || sessionId != state.pendingSession) return 0;
            var ad = RewardedFor(state.pendingPlacement);
            state.pendingSession = "";
            state.pendingPlacement = "";
            if (ad == null || claims.Contains(sessionId)) { Save(); return 0; }
            Increment("g:" + ad.sharedCapGroup);
            Increment("p:" + ad.id);
            state.adCoinToday += ad.coin;
            int index = state.cooldownKeys.IndexOf(ad.id);
            double until = NowSeconds + ad.cooldownSec;
            if (index < 0)
            {
                state.cooldownKeys.Add(ad.id);
                state.cooldownUntil.Add(until);
            }
            else state.cooldownUntil[index] = until;
            state.lastRewarded = NowSeconds;
            TryClaim(sessionId, ad.coin);
            return ad.coin;
        }

        public string PendingSession { get { return state.pendingSession; } }

        // Huỷ/lỗi/no-fill: bỏ phiên, không thưởng, không trừ, không tính lượt.
        public void CancelRewarded()
        {
            if (state.pendingSession.Length == 0) return;
            state.pendingSession = "";
            state.pendingPlacement = "";
            Save();
        }

        // ---------- interstitial ----------

        // Chỉ sau khi thắng: bỏ 5 level đầu, cách nhau ≥ minGapSec, ≤ dailyCap/ngày, không trong blockedAfterRewardedSec sau rewarded, tắt khi đã mua gói tắt ads.
        public bool CanShowInterstitial(int levelsCleared, bool noAds)
        {
            Tick();
            var rule = data.interstitial;
            if (noAds || levelsCleared <= rule.graceLevels) return false;
            double now = NowSeconds;
            if (now - state.lastInterstitial < rule.minGapSec && now >= state.lastInterstitial) return false;
            if (state.interCount >= rule.dailyCap) return false;
            if (now - state.lastRewarded < rule.blockedAfterRewardedSec && now >= state.lastRewarded) return false;
            return true;
        }

        public void OnInterstitialShown()
        {
            state.interCount++;
            state.lastInterstitial = NowSeconds;
            Save();
        }
    }
}
