using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

public class WinDialog : Dialog {
    public Animator titleAnim;

    [Header("Reward (design: popups.win)")]
    public GameObject rewardHolder;
    public Text rewardAmountText;
    public Text rewardText;
    public Button doubleRewardButton;
    public Text doubleRewardText;
    public Text goalText;
    public Text extraWordsText;

    // Kinh tế thưởng thắng level (số đề xuất theo design ECON, chỉnh tại đây hoặc trên prefab).
    [Header("Economy")]
    public int firstClearReward = 2;
    public int firstClearTutorialReward = 3;
    // Số ruby luồng rewarded video chung (RewardedVideoCallBack) đã tự cộng; dùng khi ConfigController chưa có.
    public int videoReward = 2;
    public int videoCooldown = 120;

    private const string VIDEO_ACTION = "rewarded_video";

    private LocalizationController Loc { get { return Locator<LocalizationController>.Instance; } }

    private int numLevels;
    private bool isLastLevel;
    private int world, subWorld, level;
    private int reward;
    private bool bonusClaimed, bonusPending;
    private bool started, navigating;

    protected override void Start()
    {
        base.Start();
        Invoke("ShowTitle", 0.35f);
        CheckUnlock();
        ShowRewardInfo();

        if (AdmobController.instance != null)
        {
            AdmobController.instance.onRewardedVideoRewarded += OnRewardedVideoRewarded;
            AdmobController.instance.onRewardedVideoClosed += OnRewardedVideoEnded;
            AdmobController.instance.onRewardedVideoFailed += OnRewardedVideoEnded;
        }
        if (reward > 0) InvokeRepeating("RefreshDoubleReward", 0f, 1f);
        started = true;
    }

    // Dialog bị STACK (Success của video) rồi hiện lại: Invoke bị huỷ khi tắt và animator về trạng thái đầu.
    private void OnEnable()
    {
        if (!started) return;
        ShowTitle();
        if (reward > 0) InvokeRepeating("RefreshDoubleReward", 0f, 1f);
    }

    // Hiện lại từ STACK không bật lại interstitial (người chơi vừa xem xong quảng cáo thưởng).
    public override void Show()
    {
        bool ad = enableAd;
        if (started) enableAd = false;
        base.Show();
        enableAd = ad;
    }

    private void OnDestroy()
    {
        if (AdmobController.instance != null)
        {
            AdmobController.instance.onRewardedVideoRewarded -= OnRewardedVideoRewarded;
            AdmobController.instance.onRewardedVideoClosed -= OnRewardedVideoEnded;
            AdmobController.instance.onRewardedVideoFailed -= OnRewardedVideoEnded;
        }
    }

    private void ShowTitle()
    {
        titleAnim.SetTrigger("show");
    }

    private void CheckUnlock()
    {
        numLevels = Superpow.Utils.GetNumLevels(GameState.currentWorld, GameState.currentSubWorld);
        world = GameState.currentWorld;
        subWorld = GameState.currentSubWorld;
        level = GameState.currentLevel;

        isLastLevel = Prefs.IsLastLevel();

        // Lần đầu thắng = level đang là level mở khoá mới nhất; sau khi thắng tiến độ nhảy sang level kế nên chỉ thưởng 1 lần.
        if (isLastLevel)
        {
            bool isTutorial = world == 0 && subWorld == 0 && level == 0;
            reward = isTutorial ? firstClearTutorialReward : firstClearReward;
        }

        GameState.currentLevel = (level + 1) % numLevels;
        if (level == numLevels - 1)
        {
            GameState.currentSubWorld = (subWorld + 1) % Const.NUM_SUBWORLD;
            if (subWorld == Const.NUM_SUBWORLD - 1)
            {
                GameState.currentWorld++;
            }
        }

        if (isLastLevel)
        {
            Prefs.unlockedWorld = GameState.currentWorld;
            Prefs.unlockedSubWorld = GameState.currentSubWorld;
            Prefs.unlockedLevel = GameState.currentLevel;
            // Lưu tiến độ trước khi cộng thưởng để thưởng không bị nhận lại nếu app chết giữa chừng.
            CPlayerPrefs.Save();
            CurrencyController.CreditBalance(reward);

            if (GPGSController.instance != null)
            {
                int score = Superpow.Utils.GetLeaderboardScore();
                GPGSController.instance.ReportScore(score);
            }
        }
    }

    private void ShowRewardInfo()
    {
        bool first = reward > 0;
        rewardHolder.SetActive(first);
        rewardAmountText.text = "+" + reward;
        rewardText.gameObject.SetActive(true);
        rewardText.text = first ? Loc.Get("win.first", "n", reward) : Loc.Get("win.replay");
        doubleRewardButton.gameObject.SetActive(first);

        int left = numLevels - level - 1;
        if (left == 0)
        {
            goalText.text = Loc.Get("win.goalLast", "s", (subWorld + 1) % Const.NUM_SUBWORLD + 1);
        }
        else
        {
            goalText.text = Loc.Get("win.goal", "l", level + 2, "n", left);
        }
        goalText.gameObject.SetActive(true);

        int extraProgress = Prefs.extraProgress, extraTarget = Prefs.extraTarget;
        extraWordsText.text = Loc.Get(extraProgress >= extraTarget ? "win.extraReady" : "win.extraNot", "a", extraProgress, "b", extraTarget);
        extraWordsText.gameObject.SetActive(true);
    }

    private int GetVideoPeriod()
    {
        return ConfigController.instance != null ? ConfigController.Config.rewardedVideoPeriod : videoCooldown;
    }

    private int GetVideoPaidAmount()
    {
        return ConfigController.instance != null ? ConfigController.Config.rewardedVideoAmount : videoReward;
    }

    private bool IsVideoLoaded()
    {
        return AdmobController.instance != null && AdmobController.instance.IsRewardedVideoLoaded();
    }

    private bool IsVideoCooldown()
    {
        return !CUtils.IsActionAvailable(VIDEO_ACTION, GetVideoPeriod());
    }

    // Nút x2 theo trạng thái: đã nhận / sẵn sàng / cooldown / không có video.
    private void RefreshDoubleReward()
    {
        if (bonusClaimed)
        {
            doubleRewardText.text = Loc.Get("win.x2done");
            doubleRewardButton.interactable = false;
            CancelInvoke("RefreshDoubleReward");
            return;
        }

        if (IsVideoCooldown())
        {
            int remain = Mathf.Max(0, (int)(GetVideoPeriod() - CUtils.GetActionDeltaTime(VIDEO_ACTION)));
            doubleRewardText.text = Loc.Get("rv.s.cooldown", "t", string.Format("{0}:{1:00}", remain / 60, remain % 60));
            doubleRewardButton.interactable = false;
        }
        else if (!IsVideoLoaded())
        {
            doubleRewardText.text = Loc.Get("rv.s.nofill");
            doubleRewardButton.interactable = false;
        }
        else
        {
            doubleRewardText.text = Loc.Get("win.x2", "n", reward);
            doubleRewardButton.interactable = !bonusPending;
        }
    }

    public void DoubleRewardClick()
    {
        if (reward <= 0 || bonusClaimed || bonusPending || IsVideoCooldown() || !IsVideoLoaded()) return;

        bonusPending = true;
        doubleRewardButton.interactable = false;
        Sound.instance.PlayButton();
        AdmobController.instance.ShowRewardBasedVideo();
    }

    // Luồng rewarded video chung đã cộng GetVideoPaidAmount() ruby và hiện RewardedVideoDialog; ở đây bù phần còn thiếu để tổng = reward.
    private void OnRewardedVideoRewarded()
    {
        if (!bonusPending || bonusClaimed) return;

        bonusClaimed = true;
        bonusPending = false;
        int topUp = reward - GetVideoPaidAmount();
        if (topUp > 0) CurrencyController.CreditBalance(topUp);
        RefreshDoubleReward();
    }

    // Video đóng sớm hoặc lỗi (không có thưởng): mở khoá lại nút x2.
    private void OnRewardedVideoEnded()
    {
        if (!bonusPending) return;
        bonusPending = false;
        RefreshDoubleReward();
    }

    public void NextClick()
    {
        if (navigating) return;
        navigating = true;
        Close();
        Sound.instance.PlayButton();

        CUtils.LoadScene(level == numLevels - 1 ? 1 : 3, true);
    }

    public void MenuClick()
    {
        if (navigating) return;
        navigating = true;
        Close();
        Sound.instance.PlayButton();
        CUtils.LoadScene(level == numLevels - 1 ? 1 : 2, true);
    }

    public void LeaderboardClick()
    {
        Sound.instance.PlayButton();

        int score = Superpow.Utils.GetLeaderboardScore();
        GPGSController.instance.ReportScoreAndShowLeaderboard(score);
    }
}
