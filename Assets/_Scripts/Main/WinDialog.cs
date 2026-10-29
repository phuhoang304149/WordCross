using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private const float VIDEO_PENDING_TIMEOUT = 120f;

    // TODO l10n: chuỗi tiếng Anh theo design i18n.js (win.*, rv.*)
    private const string TEXT_FIRST = "First clear: +{0} rubies";
    private const string TEXT_REPLAY = "Replay is practice: no extra rubies";
    private const string TEXT_X2 = "WATCH VIDEO: +{0} more rubies";
    private const string TEXT_X2_DONE = "Bonus collected";
    private const string TEXT_X2_COOLDOWN = "Next video in {0}";
    private const string TEXT_X2_NOFILL = "No video available right now";
    private const string TEXT_GOAL = "Next: Level {0} - {1} more levels to unlock the next Subworld";
    private const string TEXT_GOAL_LAST = "Subworld complete! Next: Subworld {0}";
    private const string TEXT_EXTRA_READY = "Extra words {0}/{1}: ready to claim";
    private const string TEXT_EXTRA_NOT = "Extra words {0}/{1}";

    private int numLevels;
    private bool isLastLevel;
    private int world, subWorld, level;
    private int reward;
    private bool bonusClaimed, bonusPending;
    private float bonusPendingUntil;

    protected override void Start()
    {
        base.Start();
        Invoke("ShowTitle", 0.35f);
        CheckUnlock();
        ShowRewardInfo();

        if (AdmobController.instance != null) AdmobController.instance.onRewardedVideoRewarded += OnRewardedVideoRewarded;
        if (reward > 0) InvokeRepeating("RefreshDoubleReward", 0f, 1f);
    }

    private void OnDestroy()
    {
        if (AdmobController.instance != null) AdmobController.instance.onRewardedVideoRewarded -= OnRewardedVideoRewarded;
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
            CurrencyController.CreditBalance(reward);
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

            int score = Superpow.Utils.GetLeaderboardScore();
            GPGSController.instance.ReportScore(score);
        }
    }

    private void ShowRewardInfo()
    {
        bool first = reward > 0;
        rewardHolder.SetActive(first);
        rewardAmountText.text = "+" + reward;
        rewardText.gameObject.SetActive(true);
        rewardText.text = first ? string.Format(TEXT_FIRST, reward) : TEXT_REPLAY;
        doubleRewardButton.gameObject.SetActive(first);

        int left = numLevels - level - 1;
        if (left == 0)
        {
            goalText.text = string.Format(TEXT_GOAL_LAST, (subWorld + 1) % Const.NUM_SUBWORLD + 1);
        }
        else
        {
            goalText.text = string.Format(TEXT_GOAL, level + 2, left);
        }
        goalText.gameObject.SetActive(true);

        int extraProgress = Prefs.extraProgress, extraTarget = Prefs.extraTarget;
        extraWordsText.text = string.Format(extraProgress >= extraTarget ? TEXT_EXTRA_READY : TEXT_EXTRA_NOT, extraProgress, extraTarget);
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
            doubleRewardText.text = TEXT_X2_DONE;
            doubleRewardButton.interactable = false;
            CancelInvoke("RefreshDoubleReward");
            return;
        }

        if (bonusPending && Time.unscaledTime > bonusPendingUntil) bonusPending = false;

        if (IsVideoCooldown())
        {
            int remain = Mathf.Max(0, (int)(GetVideoPeriod() - CUtils.GetActionDeltaTime(VIDEO_ACTION)));
            doubleRewardText.text = string.Format(TEXT_X2_COOLDOWN, string.Format("{0}:{1:00}", remain / 60, remain % 60));
            doubleRewardButton.interactable = false;
        }
        else if (!IsVideoLoaded())
        {
            doubleRewardText.text = TEXT_X2_NOFILL;
            doubleRewardButton.interactable = false;
        }
        else
        {
            doubleRewardText.text = string.Format(TEXT_X2, reward);
            doubleRewardButton.interactable = !bonusPending;
        }
    }

    public void DoubleRewardClick()
    {
        if (reward <= 0 || bonusClaimed || bonusPending || IsVideoCooldown() || !IsVideoLoaded()) return;

        bonusPending = true;
        bonusPendingUntil = Time.unscaledTime + VIDEO_PENDING_TIMEOUT;
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

    public void NextClick()
    {
        Close();
        Sound.instance.PlayButton();

        CUtils.LoadScene(level == numLevels - 1 ? 1 : 3, true);
    }

    public void MenuClick()
    {
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
