using System;
using UnityEngine;

namespace WordCross.Gameplay
{
    // Dữ liệu kinh tế coin + chu kỳ 365 ngày, nhập từ design/GD/economy-year-data/*.json bằng WordCross.Editor.EconomyImporter.
    // Nằm trong Addressables (Config/GameData/Economy/EconomyData.asset) để chỉnh số liệu chỉ cần OTA AA.
    public class EconomyData : ScriptableObject
    {
        [Serializable]
        public class Sink
        {
            public string id;
            public int coin;
            public string locKey;
            public int maxMissedDays;
        }

        [Serializable]
        public class Pack
        {
            public string id;
            public string productId;
            public int coin;
            public bool removesAds;
            public string locKey;
        }

        [Serializable]
        public class Rewarded
        {
            public string id;
            public string placement;
            public int coin;
            public int cooldownSec;
            public int dailyCap;
            public string sharedCapGroup;
        }

        [Serializable]
        public class Interstitial
        {
            public int graceLevels;
            public int minGapSec;
            public int dailyCap;
            public int blockedAfterRewardedSec;
            public string[] disabledBy;
        }

        [Serializable]
        public class Milestone
        {
            public string id;
            public int days;
            public int coin;
            public string locKey;
        }

        [Serializable]
        public class Quest
        {
            public string id;
            public string scope;
            public string metric;
            public int target;
            public int coin;
            public string locKey;
        }

        [Serializable]
        public class LoginReward
        {
            public string id;
            public int coin;
            public string locKey;
        }

        [Serializable]
        public class Day
        {
            public int day;
            public int month;
            public int week;
            public int dow;
            public string loginRef;
            public string[] dailyQuestRefs;
            public string eventRef;
            public bool bridge;
        }

        [Serializable]
        public class Period
        {
            public string id;
            public int startDay;
            public int endDay;
            public string[] questRefs;
            public int allClearCoin;
            public string locKey;
        }

        [Header("Currency")]
        public int startBalance = 100;
        // Người chơi cũ: số ruby × hệ số này, một lần (khoá ledger migrate:ruby_v1).
        public int rubyToCoin = 10;

        [Header("Earn")]
        public int firstWinCoin;
        public int firstEverWinBonusCoin;
        public int replayWinCoin;
        public int dailyChallengeCoin;
        public int botEasyCoin;
        public int botMediumCoin;
        public int botHardCoin;
        public int botDrawCoin;
        public int botDailyCap;
        public int extraWordCoin;
        public int extraWordCapPerLevel;

        public Sink[] sinks;
        public Pack[] packs;
        public Rewarded[] rewarded;
        public Interstitial interstitial;
        public int maxRewardedCoinPerDay;
        public Milestone[] streakMilestones;
        public Milestone[] levelMilestones;

        [Header("Quests")]
        public Quest[] quests;
        public int dailyAllClearCoin;

        [Header("Calendar")]
        // yyyy-MM-dd, 00:00 UTC của ngày 1 chu kỳ C1.
        public string anchorUtcDate;
        public int lengthDays = 365;
        public LoginReward[] loginRewards;
        public Day[] days;
        public Period[] weeks;
        public Period[] months;
        public Period[] events;
    }
}
