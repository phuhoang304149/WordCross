using UnityEngine;
using System;

// Google Mobile Ads SDK đã được gỡ. Các hàm dưới đây là điểm gắn để tích hợp lại SDK quảng cáo
// (ví dụ CatGamingAds của catgamingmodule); hiện chỉ ghi log và không hiển thị quảng cáo.
public class AdmobController : MonoBehaviour
{
    public static AdmobController instance;

    // Gọi khi người chơi xem xong rewarded video; SDK mới cần invoke sự kiện này.
    public event Action onRewardedVideoRewarded;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!CUtils.IsBuyItem() && !CUtils.IsAdsRemoved())
        {
            RequestBanner();
            RequestInterstitial();
        }

        RequestRewardBasedVideo();
    }

    // Điểm gắn: tải banner.
    public void RequestBanner()
    {
        print("AdmobController.RequestBanner: ads SDK not integrated.");
    }

    // Điểm gắn: tải interstitial.
    public void RequestInterstitial()
    {
        print("AdmobController.RequestInterstitial: ads SDK not integrated.");
    }

    // Điểm gắn: tải rewarded video.
    public void RequestRewardBasedVideo()
    {
        print("AdmobController.RequestRewardBasedVideo: ads SDK not integrated.");
    }

    public void ShowBanner()
    {
        if (CUtils.IsBuyItem()) return;
        // Điểm gắn: hiển thị banner.
    }

    public void HideBanner()
    {
        // Điểm gắn: ẩn banner.
    }

    // Điểm gắn: trả về true khi interstitial được hiển thị.
    public bool ShowInterstitial(bool video = false)
    {
        return false;
    }

    // Điểm gắn: trả về true khi rewarded video đã sẵn sàng.
    public bool IsRewardedVideoLoaded()
    {
        return false;
    }

    public void ShowRewardBasedVideo()
    {
        if (!IsRewardedVideoLoaded())
        {
            print("Reward based video ad is not ready yet");
        }
    }

    // SDK mới gọi hàm này trong callback nhận thưởng.
    public void HandleRewardBasedVideoRewarded()
    {
        if (onRewardedVideoRewarded != null) onRewardedVideoRewarded();
    }
}
