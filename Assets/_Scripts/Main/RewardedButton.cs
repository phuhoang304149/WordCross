using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WordCross.Gameplay;

public class RewardedButton : MonoBehaviour
{
    public GameObject content;
    public GameObject adAvailableTextHolder;
    public TimerText timerText;
    // Dòng phụ "Video ngắn: +{n} ruby" (design shop.ready), n lấy từ cấu hình.
    public LocalizedText readyText;

    private const string ACTION_NAME = "rewarded_video";
    private bool isEventAttached;

    private void Start()
    {
        if (readyText != null) readyText.SetArg("n", Amount());
        if (timerText != null) timerText.onCountDownComplete += OnCountDownComplete;

#if UNITY_ANDROID || UNITY_IOS
        Timer.Schedule(this, 0.1f, AddEvents);

        // Hàng free luôn hiện (design shop.free): WATCH khi hết cooldown, đồng hồ khi đang cooldown; popup lo trạng thái không có quảng cáo.
        if (!IsActionAvailable())
        {
            content.SetActive(false);
            ShowTimerText(RemainTime());
        }

        InvokeRepeating("IUpdate", 1, 1);
#endif
    }

    private void OnEnable()
    {
        if (!isEventAttached) return;
        if (!IsActionAvailable())
        {
            content.SetActive(false);
            // Coroutine của TimerText đã chết khi Shop bị ẩn; Stop để Run() chạy lại.
            if (timerText != null) timerText.Stop();
            ShowTimerText(RemainTime());
        }
    }

    private void AddEvents()
    {
        AdmobController.instance.onRewardedVideoRewarded += HandleRewardBasedVideoRewarded;
        isEventAttached = true;
    }

    private void IUpdate()
    {
        content.SetActive(IsActionAvailable());
    }

    // Mở popup xác nhận (design: rewarded-video ready); popup tự xử lý loading/cooldown/nofill/offline và phát video.
    public void OnClick()
    {
        Sound.instance.PlayButton();
        var dialog = (RewardedVideoDialog)DialogController.instance.GetDialog(DialogType.RewardedVideo);
        dialog.SetPlacement(EconomyService.AD_SHOP);
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
        ShowTimerText(Cooldown());
    }

    private void OnCountDownComplete()
    {
        adAvailableTextHolder.SetActive(false);
        content.SetActive(true);
    }

    public bool IsAvailableToShow()
    {
        return IsActionAvailable() && IsAdAvailable();
    }

    private EconomyService Econ { get { return Script.Utils.Locator<EconomyService>.Instance; } }

    // Video miễn phí của Shop theo economy-config (ad_shop_free): ruby, hồi chiêu và thời gian còn lại; không có EconomyService thì dùng cấu hình cũ.
    private int Amount()
    {
        var ad = Econ == null ? null : Econ.RewardedFor(EconomyService.AD_SHOP);
        return ad == null ? ConfigController.Config.rewardedVideoAmount : ad.coin;
    }

    private int Cooldown()
    {
        var ad = Econ == null ? null : Econ.RewardedFor(EconomyService.AD_SHOP);
        return ad == null ? ConfigController.Config.rewardedVideoPeriod : ad.cooldownSec;
    }

    private int RemainTime()
    {
        if (Econ != null) return Econ.CooldownRemain(EconomyService.AD_SHOP);
        return (int)(ConfigController.Config.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME));
    }

    private bool IsActionAvailable()
    {
        if (Econ != null) return Econ.CooldownRemain(EconomyService.AD_SHOP) <= 0;
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
