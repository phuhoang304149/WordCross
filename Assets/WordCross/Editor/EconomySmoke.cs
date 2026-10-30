using System;
using UnityEditor;
using UnityEngine;
using WordCross.Gameplay;

namespace WordCross.Editor
{
    // cli-smoke cho EconomyService (T134): lịch UTC (ngày 1, tuần 7/8, 364/365/366 sang C2), đồng hồ lùi, nhảy tiến, claim trùng,
    // ví ruby (giữ số dư cũ, người mới startBalance), hôm nay 2026-10-30 = ngày 303 C1, mở lại app, nhiệm vụ + thưởng hoàn thành hết, quảng cáo thưởng (phiên, hồi chiêu, trần nhóm), interstitial.
    // Dùng EconomyData.asset thật + store/clock trong bộ nhớ (không đụng PlayerPrefs). Thoát mã 1 nếu có FAIL.
    // Hàm static chỉ là điểm vào cho Unity (-executeMethod), phần việc nằm ở hàm instance.
    public class EconomySmoke
    {
        private const string AssetPath = "Assets/WordCross/Config/GameData/Economy/EconomyData.asset";

        public static void Run()
        {
            int failed = new EconomySmoke().RunAll();
            EditorApplication.Exit(failed > 0 ? 1 : 0);
        }

        private EconomyData data;
        private DateTime anchor;
        private int passed, failed;

        private void Check(string name, bool ok, string detail)
        {
            if (ok) passed++;
            else failed++;
            Debug.Log("[EconSmoke] " + (ok ? "PASS " : "FAIL ") + name + " :: " + detail);
        }

        private EconomyService Make(MemoryEconomyStore store, FixedEconomyClock clock)
        {
            var service = new EconomyService(data, store, clock);
            service.Initialize();
            return service;
        }

        private DateTime Day(int dayNumber, int hour)
        {
            return anchor.AddDays(dayNumber).AddHours(hour);
        }

        public int RunAll()
        {
            data = AssetDatabase.LoadAssetAtPath<EconomyData>(AssetPath);
            if (data == null)
            {
                Debug.LogError("[EconSmoke] FAIL missing " + AssetPath);
                return 1;
            }
            anchor = DateTime.SpecifyKind(DateTime.Parse(data.anchorUtcDate + "T00:00:00"), DateTimeKind.Utc);
            Wallet();
            Calendar();
            Today();
            Reopen();
            ClockGuard();
            Streak();
            Quests();
            Rewarded();
            Interstitial();
            Debug.Log("[EconSmoke] RESULT passed=" + passed + " failed=" + failed);
            return failed;
        }

        // Ruby là tiền tệ duy nhất: người cũ giữ nguyên số dư, người mới nhận startBalance, khởi tạo lại không đổi gì.
        private void Wallet()
        {
            var fresh = new MemoryEconomyStore();
            var clock = new FixedEconomyClock { now = Day(4, 10) };
            Make(fresh, clock);
            Check("wallet.new", fresh.balance == data.startBalance, "balance=" + fresh.balance);
            Make(fresh, clock);
            Check("wallet.new.twice", fresh.balance == data.startBalance, "balance=" + fresh.balance);

            var old = new MemoryEconomyStore { balance = 7, legacy = true };
            Make(old, clock);
            Check("wallet.old.keeps1to1", old.balance == 7, "7 ruby -> " + old.balance);
            old.balance += 5;
            Make(old, clock);
            Check("wallet.old.twice", old.balance == 12, "second init keeps " + old.balance);
            Check("anchor", data.anchorUtcDate == "2026-01-01", data.anchorUtcDate);
        }

        // Hôm nay 2026-10-30 UTC: 302 ngày sau neo → ngày 303 của C1.
        private void Today()
        {
            var clock = new FixedEconomyClock { now = new DateTime(2026, 10, 30, 15, 0, 0, DateTimeKind.Utc) };
            var econ = Make(new MemoryEconomyStore(), clock);
            Check("today.303", econ.DayNumberAt(clock.now) == 302 && econ.DayIndex == 303 && econ.CycleId == "C1" && !econ.DayRow.bridge, econ.CycleId + " d" + econ.DayIndex + " week=" + econ.PeriodId("weekly"));
        }

        // Mở lại app (tạo service mới trên cùng store): login/quest/phiên quảng cáo đã nhận không cộng lần hai.
        private void Reopen()
        {
            var store = new MemoryEconomyStore();
            var clock = new FixedEconomyClock { now = Day(0, 9) };
            var econ = Make(store, clock);
            econ.ClaimLogin();
            econ.AddProgress(EconomyService.METRIC_WIN, 3);
            econ.ClaimQuest("daily", "dq_win_3");
            string session = econ.BeginRewarded(EconomyService.AD_SHOP);
            int got = econ.CompleteRewarded(session);
            int balance = store.balance;
            var reopened = Make(store, clock);
            bool login = reopened.ClaimLogin();
            bool quest = reopened.ClaimQuest("daily", "dq_win_3");
            int replay = reopened.CompleteRewarded(session);
            Check("reopen.noDouble", got > 0 && !login && !quest && replay == 0 && store.balance == balance && reopened.IsLoginClaimed, "login=" + login + " quest=" + quest + " replay=" + replay + " delta=" + (store.balance - balance));
        }

        private void Calendar()
        {
            var store = new MemoryEconomyStore();
            var clock = new FixedEconomyClock { now = Day(0, 0) };
            var econ = Make(store, clock);
            Check("day1", econ.DayIndex == 1 && econ.CycleId == "C1" && econ.PeriodId("daily") == "d001" && econ.PeriodId("weekly") == "w01", econ.CycleId + " d" + econ.DayIndex + " " + econ.PeriodId("weekly"));
            Check("day1.event", econ.PeriodId("event") == "ev_newyear", econ.PeriodId("event"));
            int before = store.balance;
            bool first = econ.ClaimLogin();
            bool dup = econ.ClaimLogin();
            Check("login.claim", first && !dup && store.balance == before + 10, "first=" + first + " dup=" + dup + " +" + (store.balance - before));

            clock.now = Day(6, 23);
            econ.Tick();
            Check("day7", econ.DayIndex == 7 && econ.PeriodId("weekly") == "w01" && econ.TodayLogin.id == "lr_week", "d" + econ.DayIndex + " " + econ.PeriodId("weekly") + " " + econ.TodayLogin.id);
            econ.AddProgress(EconomyService.METRIC_WIN, 4);
            var weekQuest = econ.Quest("wq_win_20");
            int weekProgress = econ.Progress("weekly", weekQuest);
            clock.now = Day(7, 0);
            econ.Tick();
            Check("day8.week2", econ.DayIndex == 8 && econ.PeriodId("weekly") == "w02", "d" + econ.DayIndex + " " + econ.PeriodId("weekly"));
            Check("day8.weekReset", weekProgress == 4 && econ.Progress("weekly", weekQuest) == 0, "w01=" + weekProgress + " w02=" + econ.Progress("weekly", weekQuest));

            clock.now = Day(363, 12);
            econ.Tick();
            Check("day364", econ.DayIndex == 364 && econ.PeriodId("weekly") == "w52" && econ.CycleId == "C1", econ.CycleId + " d" + econ.DayIndex + " " + econ.PeriodId("weekly"));
            clock.now = Day(364, 12);
            econ.Tick();
            Check("day365.bridge", econ.DayIndex == 365 && econ.DayRow.bridge && econ.PeriodId("weekly") == "bridge" && econ.QuestRefs("weekly").Length == 0 && econ.TodayLogin.id == "lr_cycle", "d" + econ.DayIndex + " week=" + econ.PeriodId("weekly") + " login=" + econ.TodayLogin.id);
            before = store.balance;
            econ.ClaimLogin();
            Check("day365.login100", store.balance - before == 100, "+" + (store.balance - before));
            clock.now = Day(365, 0);
            econ.Tick();
            Check("day366.C2", econ.DayIndex == 1 && econ.CycleId == "C2" && econ.PeriodId("event") == "ev_newyear" && !econ.IsLoginClaimed, econ.CycleId + " d" + econ.DayIndex + " loginClaimed=" + econ.IsLoginClaimed);
            Check("C2.claimKey", econ.ClaimKey("daily", "dq_win_3") == "claim:C2:daily:d001:dq_win_3", econ.ClaimKey("daily", "dq_win_3"));
        }

        private void ClockGuard()
        {
            var store = new MemoryEconomyStore();
            var clock = new FixedEconomyClock { now = Day(10, 8) };
            var econ = Make(store, clock);
            econ.ClaimLogin();
            int balance = store.balance;
            clock.now = Day(8, 8);
            econ.Tick();
            bool again = econ.ClaimLogin();
            Check("clock.back", econ.Today == 10 && !again && store.balance == balance, "today=" + econ.Today + " reclaim=" + again);
            var reloaded = Make(store, clock);
            Check("clock.back.reload", reloaded.Today == 10 && reloaded.IsLoginClaimed, "today=" + reloaded.Today + " claimed=" + reloaded.IsLoginClaimed);
            clock.now = Day(20, 8);
            reloaded.Tick();
            balance = store.balance;
            reloaded.ClaimLogin();
            Check("clock.forward.noBackfill", reloaded.Today == 20 && store.balance - balance == reloaded.TodayLogin.coin && reloaded.Streak == 1, "today=" + reloaded.Today + " +" + (store.balance - balance) + " streak=" + reloaded.Streak);
        }

        private void Streak()
        {
            var store = new MemoryEconomyStore();
            var clock = new FixedEconomyClock { now = Day(30, 1) };
            var econ = Make(store, clock);
            for (int d = 30; d < 33; d++)
            {
                clock.now = Day(d, 1);
                econ.Tick();
                econ.ClaimLogin();
            }
            Check("streak.3", econ.Streak == 3 && econ.GetStreakMilestoneState(data.streakMilestones[0]) == QuestState.Claimable, "streak=" + econ.Streak);
            int balance = store.balance;
            bool ms = econ.ClaimStreakMilestone("st_3");
            bool msDup = econ.ClaimStreakMilestone("st_3");
            Check("streak.milestone", ms && !msDup && store.balance - balance == 20, "+" + (store.balance - balance));
            clock.now = Day(34, 1);
            econ.Tick();
            Check("streak.broken", econ.Streak == 0 && econ.CanRepairStreak, "streak=" + econ.Streak + " repair=" + econ.CanRepairStreak);
            balance = store.balance;
            bool repaired = econ.RepairStreak();
            econ.ClaimLogin();
            Check("streak.repair", repaired && econ.Streak == 4 && balance - 50 + data.loginRewards[0].coin <= store.balance, "repaired=" + repaired + " streak=" + econ.Streak);
            clock.now = Day(37, 1);
            econ.Tick();
            Check("streak.noRepair2days", !econ.CanRepairStreak && econ.Streak == 0 && econ.BestStreak == 4, "repair=" + econ.CanRepairStreak + " best=" + econ.BestStreak);
        }

        private void Quests()
        {
            var store = new MemoryEconomyStore();
            var clock = new FixedEconomyClock { now = Day(0, 5) };
            var econ = Make(store, clock);
            string[] refs = econ.QuestRefs("daily");
            Check("quest.day1Refs", string.Join(",", refs) == "dq_win_3,dq_words_20,dq_daily", string.Join(",", refs));
            econ.AddProgress(EconomyService.METRIC_WIN, 2);
            Check("quest.inProgress", !econ.ClaimQuest("daily", "dq_win_3"), "2/3 not claimable");
            econ.AddProgress(EconomyService.METRIC_WIN, 1);
            int balance = store.balance;
            bool claimed = econ.ClaimQuest("daily", "dq_win_3");
            bool dup = econ.ClaimQuest("daily", "dq_win_3");
            Check("quest.claim", claimed && !dup && store.balance - balance == 15, "+" + (store.balance - balance) + " dup=" + dup);
            Check("quest.allClear.locked", econ.GetAllClearState("daily") == QuestState.InProgress && !econ.ClaimAllClear("daily"), econ.GetAllClearState("daily").ToString());
            for (int i = 0; i < 20; i++) econ.OnWordFound();
            econ.OnDailyChallengeDone();
            econ.ClaimQuest("daily", "dq_words_20");
            econ.ClaimQuest("daily", "dq_daily");
            balance = store.balance;
            bool all = econ.ClaimAllClear("daily");
            Check("quest.allClear", all && store.balance - balance == data.dailyAllClearCoin && !econ.ClaimAllClear("daily"), "+" + (store.balance - balance));

            clock.now = Day(1, 5);
            econ.Tick();
            Check("quest.botUnavailable", econ.GetQuestState("daily", econ.Quest("dq_bot_1")) == QuestState.Unavailable, string.Join(",", econ.QuestRefs("daily")));
            clock.now = Day(2, 5);
            econ.Tick();
            econ.OnExtraWord(true);
            econ.OnDailyChallengeDone();
            econ.ClaimQuest("daily", "dq_daily");
            econ.ClaimQuest("daily", "dq_extra_1");
            Check("quest.allClearSkipsBot", econ.GetAllClearState("daily") == QuestState.Claimable, string.Join(",", econ.QuestRefs("daily")) + " " + econ.GetAllClearState("daily"));
            Check("quest.levelMilestone", econ.GetLevelMilestoneState(data.levelMilestones[0], 10) == QuestState.Claimable && econ.ClaimLevelMilestone("lm_10", 10) && !econ.ClaimLevelMilestone("lm_10", 10), "lm_10 once");
            balance = store.balance;
            int win = econ.OnLevelWon(true, true);
            int win2 = econ.OnLevelWon(true, true);
            int replay = econ.OnLevelWon(false, false);
            Check("earn.win", win == 30 && win2 == 10 && replay == 0 && store.balance - balance == 40, "first=" + win + " second=" + win2 + " replay=" + replay);
            Check("earn.bot", econ.OnBotMatch("hard", 1) == 15 && econ.OnBotMatch("hard", 1) == 15 && econ.OnBotMatch("hard", 1) == 15 && econ.OnBotMatch("hard", 1) == 15 && econ.OnBotMatch("easy", 1) == 0, "cap 60/day");
        }

        private void Rewarded()
        {
            var store = new MemoryEconomyStore();
            var clock = new FixedEconomyClock { now = Day(5, 9) };
            var econ = Make(store, clock);
            int balance = store.balance;
            string session = econ.BeginRewarded(EconomyService.AD_SHOP);
            int got = econ.CompleteRewarded(session);
            int dup = econ.CompleteRewarded(session);
            Check("ad.complete", got == 20 && dup == 0 && store.balance - balance == 20, "got=" + got + " dup=" + dup);
            Check("ad.cooldown", econ.GetAdState(EconomyService.AD_SHOP) == EconomyAdState.Cooldown && econ.BeginRewarded(EconomyService.AD_SHOP) == null, econ.GetAdState(EconomyService.AD_SHOP) + " " + econ.CooldownRemain(EconomyService.AD_SHOP) + "s");
            string cancelled = econ.BeginRewarded(EconomyService.AD_LOW_COIN);
            econ.CancelRewarded();
            Check("ad.cancel", econ.CompleteRewarded(cancelled) == 0 && econ.RewardedLeft(EconomyService.AD_LOW_COIN) == 4, "left=" + econ.RewardedLeft(EconomyService.AD_LOW_COIN));
            for (int i = 0; i < 6; i++)
            {
                clock.now = clock.now.AddSeconds(121);
                string s = econ.BeginRewarded(i % 2 == 0 ? EconomyService.AD_LOW_COIN : EconomyService.AD_SHOP);
                econ.CompleteRewarded(s);
            }
            Check("ad.groupCap", econ.GetAdState(EconomyService.AD_SHOP) == EconomyAdState.CapReached && econ.GetAdState(EconomyService.AD_LOW_COIN) == EconomyAdState.CapReached, "rw_coin cap 5 shared");
            Check("ad.x2Separate", econ.GetAdState(EconomyService.AD_WIN_X2) == EconomyAdState.Ready, econ.GetAdState(EconomyService.AD_WIN_X2).ToString());
            string daily = econ.BeginRewarded(EconomyService.AD_DAILY_X2);
            econ.CompleteRewarded(daily);
            clock.now = clock.now.AddSeconds(121);
            Check("ad.dailyX2Cap1", econ.GetAdState(EconomyService.AD_DAILY_X2) == EconomyAdState.CapReached && econ.GetAdState(EconomyService.AD_WIN_X2) == EconomyAdState.Ready, econ.GetAdState(EconomyService.AD_DAILY_X2).ToString());
            clock.now = Day(6, 0);
            econ.Tick();
            Check("ad.resetNextDay", econ.GetAdState(EconomyService.AD_SHOP) == EconomyAdState.Ready && econ.RewardedLeft(EconomyService.AD_SHOP) == 5, "left=" + econ.RewardedLeft(EconomyService.AD_SHOP));
            clock.now = clock.now.AddHours(-30);
            Check("ad.clockBackCooldownBounded", econ.CooldownRemain(EconomyService.AD_SHOP) <= 120, econ.CooldownRemain(EconomyService.AD_SHOP) + "s");
        }

        private void Interstitial()
        {
            var store = new MemoryEconomyStore();
            var clock = new FixedEconomyClock { now = Day(5, 9) };
            var econ = Make(store, clock);
            Check("inter.grace", !econ.CanShowInterstitial(5, false) && econ.CanShowInterstitial(6, false), "level<=5 blocked");
            Check("inter.noads", !econ.CanShowInterstitial(50, true), "noads");
            econ.OnInterstitialShown();
            clock.now = clock.now.AddSeconds(60);
            bool gap = econ.CanShowInterstitial(50, false);
            clock.now = clock.now.AddSeconds(31);
            Check("inter.gap90", !gap && econ.CanShowInterstitial(50, false), "60s blocked, 91s ok");
            string s = econ.BeginRewarded(EconomyService.AD_WIN_X2);
            econ.CompleteRewarded(s);
            bool afterRewarded = econ.CanShowInterstitial(50, false);
            clock.now = clock.now.AddSeconds(31);
            Check("inter.afterRewarded30", !afterRewarded && econ.CanShowInterstitial(50, false), "30s after rewarded");
            for (int i = 0; i < 5; i++)
            {
                econ.OnInterstitialShown();
                clock.now = clock.now.AddSeconds(100);
            }
            Check("inter.cap6", !econ.CanShowInterstitial(50, false), "6/day");
        }
    }
}
