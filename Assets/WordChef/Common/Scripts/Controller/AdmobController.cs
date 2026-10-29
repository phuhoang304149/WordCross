using UnityEngine;
using System;
using System.Collections;
#if CATGAMING_ADS && CATGAMING_ADMOB
using catgamingmodule.Monetization;
using catgamingmodule.Tool.Database;
using Script.Utils;
#endif

// Nối quảng cáo với CatGamingAds/AdmobService của catgamingmodule (Google Mobile Ads SDK).
// App ID và ad unit ID lấy từ AdmobConfig đăng ký trong GameDatabase.
public class AdmobController : MonoBehaviour
{
    public static AdmobController instance;

    // Gọi khi người chơi xem xong rewarded video.
    public event Action onRewardedVideoRewarded;

    private void Awake()
    {
        instance = this;
    }

    private IEnumerator Start()
    {
#if CATGAMING_ADS && CATGAMING_ADMOB
        // Purchaser nạp GameDatabase bất đồng bộ; chờ để AdmobService đọc được AdmobConfig thay vì ad unit ID rỗng.
        if (Locator<GameDatabase>.Instance == null)
        {
            Debug.Log("AdmobController: waiting for GameDatabase before initializing ads.");
            yield return new WaitUntil(() => Locator<GameDatabase>.Instance != null);
        }
        CatGamingAds.Initialize(OnAdsInitialized);
#endif
        yield break;
    }

#if CATGAMING_ADS && CATGAMING_ADMOB
    private void OnAdsInitialized()
    {
        if (!CUtils.IsBuyItem() && !CUtils.IsAdsRemoved())
        {
            RequestBanner();
        }
    }
#endif

    public void RequestBanner()
    {
#if CATGAMING_ADS && CATGAMING_ADMOB
        if (CUtils.IsBuyItem() || CUtils.IsAdsRemoved()) return;
        CatGamingAds.ShowBanner();
#endif
    }

    public void RequestInterstitial()
    {
#if CATGAMING_ADS && CATGAMING_ADMOB
        if (Locator<AdmobService>.Instance != null) Locator<AdmobService>.Instance.LoadInterstitial();
#endif
    }

    public void RequestRewardBasedVideo()
    {
#if CATGAMING_ADS && CATGAMING_ADMOB
        if (Locator<AdmobService>.Instance != null) Locator<AdmobService>.Instance.LoadRewarded();
#endif
    }

    public void ShowBanner()
    {
        if (CUtils.IsBuyItem()) return;
#if CATGAMING_ADS && CATGAMING_ADMOB
        if (CUtils.IsAdsRemoved()) return;
        CatGamingAds.ShowBanner();
#endif
    }

    public void HideBanner()
    {
#if CATGAMING_ADS && CATGAMING_ADMOB
        CatGamingAds.HideBanner();
#endif
    }

    // Trả về true khi interstitial được hiển thị.
    public bool ShowInterstitial(bool video = false)
    {
        if (CUtils.IsBuyItem() || CUtils.IsAdsRemoved()) return false;
#if CATGAMING_ADS && CATGAMING_ADMOB
        return CatGamingAds.ShowInterstitial();
#else
        return false;
#endif
    }

    // Trả về true khi rewarded video đã sẵn sàng.
    public bool IsRewardedVideoLoaded()
    {
#if CATGAMING_ADS && CATGAMING_ADMOB
        return Locator<AdmobService>.Instance != null && Locator<AdmobService>.Instance.IsRewardedReady;
#else
        return false;
#endif
    }

    public void ShowRewardBasedVideo()
    {
        if (!IsRewardedVideoLoaded())
        {
            print("Reward based video ad is not ready yet");
            RequestRewardBasedVideo();
            return;
        }

#if CATGAMING_ADS && CATGAMING_ADMOB
        CatGamingAds.ShowRewarded(OnRewardedCompleted);
#endif
    }

    private void OnRewardedCompleted(bool rewarded)
    {
        if (rewarded) HandleRewardBasedVideoRewarded();
    }

    public void HandleRewardBasedVideoRewarded()
    {
        if (onRewardedVideoRewarded != null) onRewardedVideoRewarded();
    }
}
