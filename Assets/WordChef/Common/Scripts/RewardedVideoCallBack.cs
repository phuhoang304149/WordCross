using UnityEngine;

public class RewardedVideoCallBack : MonoBehaviour {

    private void Start()
    {
        Timer.Schedule(this, 0.1f, AddEvents);
    }

    private void AddEvents()
    {
#if UNITY_ANDROID || UNITY_IOS
        AdmobController.instance.onRewardedVideoRewarded += HandleRewardBasedVideoRewarded;
#endif
    }

    private const string ACTION_NAME = "rewarded_video";
    public void HandleRewardBasedVideoRewarded()
    {
        int amount = ConfigController.Config.rewardedVideoAmount;
        // Dialog xác nhận đang chờ video thì chuyển sang Success tại chỗ, không mở dialog thứ hai.
        var open = FindFirstObjectByType<RewardedVideoDialog>();
        if (open != null && open.State == RewardedVideoState.Playing)
        {
            open.SetAmount(amount);
            open.ShowSuccess();
        }
        else
        {
            var dialog = (RewardedVideoDialog)DialogController.instance.GetDialog(DialogType.RewardedVideo);
            dialog.SetAmount(amount);
            dialog.ShowSuccess();
            // STACK: dialog đang mở (vd WinDialog) chỉ bị ẩn rồi hiện lại khi đóng Success, không bị Destroy.
            DialogController.instance.ShowDialog(dialog, DialogShow.STACK);
        }

        // Cộng ruby cùng lúc bắt đầu cooldown, không phụ thuộc người chơi bấm Continue.
        CurrencyController.CreditBalance(amount);
        CUtils.SetActionTime(ACTION_NAME);
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        AdmobController.instance.onRewardedVideoRewarded -= HandleRewardBasedVideoRewarded;
#endif
    }
}
