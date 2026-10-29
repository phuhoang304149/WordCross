using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardedButton : MonoBehaviour
{
    public GameObject content;
    public GameObject adAvailableTextHolder;
    public TimerText timerText;

    private const string ACTION_NAME = "rewarded_video";
    private bool isEventAttached;

    private void Start()
    {
        if (timerText != null) timerText.onCountDownComplete += OnCountDownComplete;

#if UNITY_ANDROID || UNITY_IOS
        Timer.Schedule(this, 0.1f, AddEvents);

        if (!IsAvailableToShow())
        {
            content.SetActive(false);
            if (IsAdAvailable() && !IsActionAvailable())
            {
                int remainTime = (int)(ConfigController.Config.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME));
                ShowTimerText(remainTime);
            }
        }

        InvokeRepeating("IUpdate", 1, 1);
#else
        content.SetActive(false);
#endif
    }

    private void OnEnable()
    {
        if (!isEventAttached) return;
        if (!IsAvailableToShow() && IsAdAvailable() && !IsActionAvailable())
        {
            content.SetActive(false);
            // Coroutine của TimerText đã chết khi Shop bị ẩn; Stop để Run() chạy lại.
            if (timerText != null) timerText.Stop();
            ShowTimerText((int)(ConfigController.Config.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME)));
        }
    }

    private void AddEvents()
    {
        AdmobController.instance.onRewardedVideoRewarded += HandleRewardBasedVideoRewarded;
        isEventAttached = true;
    }

    private void IUpdate()
    {
        content.SetActive(IsAvailableToShow());
    }

    // Mở popup xác nhận (design: rewarded-video ready); popup tự xử lý loading/cooldown/nofill/offline và phát video.
    public void OnClick()
    {
        Sound.instance.PlayButton();
        var dialog = (RewardedVideoDialog)DialogController.instance.GetDialog(DialogType.RewardedVideo);
        dialog.SetAmount(ConfigController.Config.rewardedVideoAmount);
        DialogController.instance.ShowDialog(dialog, DialogShow.STACK);
    }

    private void ShowTimerText(int time)
    {
        if (adAvailableTextHolder != null)
        {
            adAvailableTextHolder.SetActive(true);
            timerText.SetTime(time);
            timerText.Run();
        }
    }

    public void HandleRewardBasedVideoRewarded()
    {
        // Shop bị ẩn dưới popup (STACK): coroutine TimerText không chạy được trên object tắt; OnEnable dựng lại khi hiện.
        if (!isActiveAndEnabled) return;
        content.SetActive(false);
        ShowTimerText(ConfigController.Config.rewardedVideoPeriod);
    }

    private void OnCountDownComplete()
    {
        adAvailableTextHolder.SetActive(false);
        if (IsAdAvailable())
        {
            content.SetActive(true);
        }
    }

    public bool IsAvailableToShow()
    {
        return IsActionAvailable() && IsAdAvailable();
    }

    private bool IsActionAvailable()
    {
        return CUtils.IsActionAvailable(ACTION_NAME, ConfigController.Config.rewardedVideoPeriod);
    }

    private bool IsAdAvailable()
    {
        return AdmobController.instance.IsRewardedVideoLoaded();
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        AdmobController.instance.onRewardedVideoRewarded -= HandleRewardBasedVideoRewarded;
#endif
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            if (adAvailableTextHolder.activeSelf)
            {
                int remainTime = (int)(ConfigController.Config.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME));
                ShowTimerText(remainTime);
            }
        }
    }
}
