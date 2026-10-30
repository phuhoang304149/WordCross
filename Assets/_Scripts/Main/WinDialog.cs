using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

public class WinDialog : Dialog {
    public Animator titleAnim;

    [Header("Reward (design: popups.win)")]
    public GameObject rewardHolder;
    public TMP_Text rewardAmountText;
    public TMP_Text rewardText;
    public Button doubleRewardButton;
    public TMP_Text doubleRewardText;
    public TMP_Text goalText;
    public TMP_Text extraWordsText;

    // Kinh tế thưởng thắng level (số đề xuất theo design ECON, chỉnh tại đây hoặc trên prefab).
    [Header("Economy")]
    public int firstClearReward = 2;
    public int firstClearTutorialReward = 3;
    // Số ruby luồng rewarded video chung (RewardedVideoCallBack) đã tự cộng; dùng khi ConfigController chưa có.
    public int videoReward = 2;
    public int videoCooldown = 120;

    private const string VIDEO_ACTION = "rewarded_video";
    private const int BUTTON_FONT = 32;
    private const int BUTTON_MIN_FONT = 16;
    private const float BUTTON_PADDING = 26f;

    private LocalizationController Loc { get { return Locator<LocalizationController>.Instance; } }

    private int numLevels;
    private bool isLastLevel;
    private int world, subWorld, level;
    private int reward;
    private bool bonusClaimed, bonusPending;
    private bool started, navigating;
    private bool isDaily;
    private DailyResult dailyResult;

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
        if (GameState.isDaily)
        {
            CheckDailyComplete();
            return;
        }
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

        // Level cuối của world cuối: hết nội dung, không tăng currentWorld quá giới hạn (Load sẽ trả null).
        bool isGameEnd = world == Superpow.Utils.NUM_WORLDS - 1 && subWorld == Const.NUM_SUBWORLD - 1 && level == numLevels - 1;
        if (!isGameEnd)
        {
            GameState.currentLevel = (level + 1) % numLevels;
            if (level == numLevels - 1)
            {
                GameState.currentSubWorld = (subWorld + 1) % Const.NUM_SUBWORLD;
                if (subWorld == Const.NUM_SUBWORLD - 1)
                {
                    GameState.currentWorld++;
                }
            }
        }

        if (isLastLevel)
        {
            if (isGameEnd)
            {
                // Đánh dấu đã hoàn thành tất cả (world = NUM_WORLDS); GameState.current* giữ ở level cuối.
                Prefs.unlockedWorld = Superpow.Utils.NUM_WORLDS;
                Prefs.unlockedSubWorld = 0;
                Prefs.unlockedLevel = 0;
            }
            else
            {
                Prefs.unlockedWorld = GameState.currentWorld;
                Prefs.unlockedSubWorld = GameState.currentSubWorld;
                Prefs.unlockedLevel = GameState.currentLevel;
            }
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

    // Thắng bàn Daily: không đổi tiến độ chính (unlocked*/current*), thưởng + chuỗi + mốc do DailyChallenge ghi.
    private void CheckDailyComplete()
    {
        isDaily = true;
        numLevels = Superpow.Utils.GetNumLevels(GameState.currentWorld, GameState.currentSubWorld);
        world = GameState.currentWorld;
        subWorld = GameState.currentSubWorld;
        level = GameState.currentLevel;
        dailyResult = new DailyChallenge().Complete(GameState.dailyDate);
        reward = dailyResult.reward;
    }

    private void ShowDailyInfo()
    {
        bool first = reward > 0;
        foreach (var label in GetComponentsInChildren<LocalizedText>(true))
        {
            if (label.key == "win.title") label.SetKey("win.dailyTitle");
            else if (label.key == "win.next") label.SetKey("win.toDaily");
        }

        rewardHolder.SetActive(first);
        rewardAmountText.text = "+" + reward;
        rewardText.gameObject.SetActive(true);
        rewardText.text = first ? Loc.Get("win.dailyReward", "n", reward) : Loc.Get("win.replay");
        doubleRewardButton.gameObject.SetActive(first);

        string streak = Loc.Get(dailyResult.streak == 1 ? "win.streak.one" : "win.streak", "n", dailyResult.streak);
        if (dailyResult.milestone > 0) streak += "\n" + Loc.Get("win.milestone", "d", dailyResult.milestoneDays, "n", dailyResult.milestone);
        goalText.text = streak;
        goalText.gameObject.SetActive(true);
        extraWordsText.text = Loc.Get("win.dailyNext");
        extraWordsText.gameObject.SetActive(true);
        ApplyDailyLayout();
        FitGoal(dailyResult.milestone > 0 ? 2 : 1);
        FitSingleLine(extraWordsText, extraWordsText.rectTransform, Mathf.RoundToInt(extraWordsText.fontSize));
    }

    // Design popups.win biến thể Daily: không có hàng sao và nút MENU, ruby cỡ lớn, dòng chuỗi nằm trước nút video, nút về Daily cao 88.
    private void ApplyDailyLayout()
    {
        Transform bg = transform.Find("Content/BG");
        bg.Find("Stars").gameObject.SetActive(false);
        bg.Find("Menu").gameObject.SetActive(false);

        RectTransform holder = rewardHolder.GetComponent<RectTransform>();
        holder.sizeDelta = new Vector2(504, 93);
        ((RectTransform)holder.Find("RubyIcon")).anchoredPosition = new Vector2(-49, 0);
        rewardAmountText.fontSize = 82;
        rewardAmountText.rectTransform.anchoredPosition = new Vector2(38, 0);
        rewardAmountText.rectTransform.sizeDelta = new Vector2(130, 100);

        if (dailyResult.milestone > 0) goalText.rectTransform.sizeDelta = new Vector2(504, 41);
        doubleRewardButton.transform.SetSiblingIndex(extraWordsText.transform.GetSiblingIndex());

        Transform next = bg.Find("Next");
        ((RectTransform)next).sizeDelta = new Vector2(504, 88);
        next.GetComponent<Image>().pixelsPerUnitMultiplier = 128f / 88f;
        next.GetComponent<Shadow>().effectDistance = new Vector2(0, -6.8f);
        next.GetComponentInChildren<TMP_Text>().fontSize = 32;
    }

    // Dòng mục tiêu có khung cao cố định, chữ tràn căn giữa: chữ dài (fr/es/pt) xuống thêm dòng thì nới khung và card để không đè dòng dưới.
    private void FitGoal(int baseLines)
    {
        goalText.ForceMeshUpdate();
        int lines = Mathf.Max(1, goalText.textInfo.lineCount);
        if (lines <= baseLines) return;

        float extra = goalText.preferredHeight / lines * (lines - baseLines);
        goalText.rectTransform.sizeDelta += new Vector2(0f, extra);
        RectTransform bg = (RectTransform)transform.Find("Content/BG");
        bg.sizeDelta += new Vector2(0f, extra);
        LayoutRebuilder.ForceRebuildLayoutImmediate(bg);
    }

    private void ShowRewardInfo()
    {
        if (isDaily)
        {
            ShowDailyInfo();
            return;
        }
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
            // Glyph '·' của font Rocko hiển thị sai (thành 'R'), đổi sang '•'
            goalText.text = Loc.Get("win.goal", "l", level + 2, "n", left).Replace('\u00b7', '\u2022');
        }
        goalText.gameObject.SetActive(true);

        int extraProgress = Prefs.extraProgress, extraTarget = Prefs.extraTarget;
        extraWordsText.text = Loc.Get(extraProgress >= extraTarget ? "win.extraReady" : "win.extraNot", "a", extraProgress, "b", extraTarget);
        extraWordsText.gameObject.SetActive(true);
        FitGoal(1);
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
            FitSingleLine(doubleRewardText, (RectTransform)doubleRewardButton.transform, BUTTON_FONT);
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
        FitSingleLine(doubleRewardText, (RectTransform)doubleRewardButton.transform, BUTTON_FONT);
    }

    // Nút một dòng: chuỗi dài (fr/es/pt) giảm cỡ chữ từ cỡ gốc xuống tối thiểu 14 cho vừa bề ngang nút (khung chữ trên prefab hẹp hơn nút nên đo theo nút).
    private void FitSingleLine(TMP_Text text, RectTransform area, int maxSize)
    {
        text.enableAutoSizing = false;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        int size = maxSize;
        text.fontSize = size;
        while (size > BUTTON_MIN_FONT && text.GetPreferredValues(text.text).x > area.rect.width - 2 * BUTTON_PADDING)
        {
            size--;
            text.fontSize = size;
        }
    }

    public void DoubleRewardClick()
    {
#if GAME_DEBUG || DEVELOPMENT_BUILD
        Debug.Log("[Win] x2 click reward=" + reward + " claimed=" + bonusClaimed + " pending=" + bonusPending + " cooldown=" + IsVideoCooldown() + " loaded=" + IsVideoLoaded());
#endif
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

        if (isDaily)
        {
            // Về Home và mở lại Daily (lịch/chuỗi đã cập nhật).
            GameState.openDailyOnHome = true;
            GameState.dailyDate = null;
            CUtils.LoadScene(0, true);
            return;
        }
        CUtils.LoadScene(level == numLevels - 1 ? 1 : 3, true);
    }

    public void MenuClick()
    {
        if (navigating) return;
        navigating = true;
        Close();
        Sound.instance.PlayButton();
        if (isDaily)
        {
            GameState.dailyDate = null;
            CUtils.LoadScene(0, true);
            return;
        }
        CUtils.LoadScene(level == numLevels - 1 ? 1 : 2, true);
    }

    public void LeaderboardClick()
    {
        Sound.instance.PlayButton();

        int score = Superpow.Utils.GetLeaderboardScore();
        GPGSController.instance.ReportScoreAndShowLeaderboard(score);
    }
}
