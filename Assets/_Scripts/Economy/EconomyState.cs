using System;
using System.Collections.Generic;

namespace WordCross.Gameplay
{
    // Ledger cục bộ của kinh tế coin (lưu JSON trong CPlayerPrefs). Chỉ giữ khoá claim/tiến độ của kỳ hiện tại + khoá trọn đời;
    // kỳ đã qua không nhận được nữa nên bị dọn khi sang ngày mới.
    [Serializable]
    public class EconomyState
    {
        public int version = 1;
        public bool initialized;
        // dayNumber lớn nhất từng thấy: đồng hồ lùi thì vẫn dùng ngày này.
        public int maxSeenDay = -1;

        public List<string> claims = new List<string>();
        public List<string> progressKeys = new List<string>();
        public List<int> progressValues = new List<int>();

        public int lastLoginDay = int.MinValue / 2;
        public int streak;
        public int bestStreak;

        // Bộ đếm theo ngày UTC (ads, interstitial, bot) reset khi counterDay khác hôm nay.
        public int counterDay = -1;
        public List<string> adCountKeys = new List<string>();
        public List<int> adCountValues = new List<int>();
        public int adCoinToday;
        public int interCount;
        public int botEarned;

        public List<string> cooldownKeys = new List<string>();
        public List<double> cooldownUntil = new List<double>();
        public double lastInterstitial;
        public double lastRewarded;

        public string pendingSession = "";
        public string pendingPlacement = "";
        public int sessionCounter;
    }

    public enum EconomyAdState
    {
        Ready,
        Cooldown,
        CapReached
    }

    public enum QuestState
    {
        InProgress,
        Claimable,
        Claimed,
        Unavailable
    }

    public interface IEconomyClock
    {
        DateTime UtcNow { get; }
    }

    // Số dư coin + nơi lưu ledger; bản chạy game dùng CurrencyController/CPlayerPrefs, test batch dùng bộ nhớ.
    public interface IEconomyStore
    {
        string LoadState();
        void SaveState(string json);
        bool HasLegacyBalance();
        int GetBalance();
        void SetBalance(int value);
        void Credit(int value);
        bool Debit(int value);
    }

    public class SystemEconomyClock : IEconomyClock
    {
#if GAME_DEBUG
        // Chỉ bản debug (preview/QA): dịch thời gian để thử chuyển ngày/tuần/chu kỳ. Bản production không có trường này.
        public double debugOffsetSeconds;
        public DateTime UtcNow { get { return DateTime.UtcNow.AddSeconds(debugOffsetSeconds); } }
#else
        public DateTime UtcNow { get { return DateTime.UtcNow; } }
#endif
    }

    public class PrefsEconomyStore : IEconomyStore
    {
        private const string STATE_KEY = "econ_state_v1";

        public string LoadState()
        {
            return CPlayerPrefs.GetString(STATE_KEY, "");
        }

        public void SaveState(string json)
        {
            CPlayerPrefs.SetString(STATE_KEY, json);
            CPlayerPrefs.Save();
        }

        public bool HasLegacyBalance()
        {
            return CPlayerPrefs.HasKey(PrefKeys.CURRENCY);
        }

        public int GetBalance()
        {
            return CurrencyController.GetBalance();
        }

        public void SetBalance(int value)
        {
            CurrencyController.SetBalance(value);
            if (CurrencyController.onBalanceChanged != null) CurrencyController.onBalanceChanged();
        }

        public void Credit(int value)
        {
            CurrencyController.CreditBalance(value);
        }

        public bool Debit(int value)
        {
            return CurrencyController.DebitBalance(value);
        }
    }

    public class MemoryEconomyStore : IEconomyStore
    {
        public string state = "";
        public int balance;
        public bool legacy;

        public string LoadState() { return state; }
        public void SaveState(string json) { state = json; }
        public bool HasLegacyBalance() { return legacy; }
        public int GetBalance() { return balance; }
        public void SetBalance(int value) { balance = value; legacy = true; }
        public void Credit(int value) { balance += value; legacy = true; }

        public bool Debit(int value)
        {
            if (balance < value) return false;
            balance -= value;
            return true;
        }
    }

    public class FixedEconomyClock : IEconomyClock
    {
        public DateTime now;
        public DateTime UtcNow { get { return now; } }
    }
}
