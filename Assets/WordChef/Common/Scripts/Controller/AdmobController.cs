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

    // Gọi khi người chơi đóng rewarded video sớm (không thưởng) hoặc video không phát được.
    public event Action onRewardedVideoClosed;
    public event Action onRewardedVideoFailed;

    private const string REWARDED_ACTION = "rewarded_video";

    // true trong lúc gọi CatGamingAds.ShowRewarded: callback(false) đến ngay trong lệnh gọi nghĩa là không phát được, không phải đóng sớm.
    private bool isShowingRewarded;

    // Chỉ true sau khi consent + MobileAds.Initialize xong; mọi request ads trước đó bị bỏ qua
    // để AdmobService không tự Initialize sớm (trước consent/GameDatabase) và đọc ad unit ID rỗng.
    private bool isAdsReady;

    private void Awake()
    {
        // GameMaster nằm trong mọi scene; bản trùng chỉ bị DonotDestroyOnLoad Destroy trễ (cuối frame),
        // nên không để nó ghi đè instance đang sống (isAdsReady đã true) rồi để lại tham chiếu đã hủy.
        if (instance == null) instance = this;
    }

    private IEnumerator Start()
    {
#if CATGAMING_ADS && CATGAMING_ADMOB
        // Purchaser nạp GameDatabase bất đồng bộ; chờ để AdmobService đọc được AdmobConfig thay vì ad unit ID rỗng.
        if (Locator<GameDatabase>.Instance == null)
        {
            Debug.Log("AdmobController: waiting for GameDatabase before initializing ads.");
            // Không dùng WaitUntil: APK 0.3 đã strip type này khỏi UnityEngine.CoreModule.
            while (Locator<GameDatabase>.Instance == null) yield return null;
        }
        CatGamingAds.Initialize(OnAdsInitialized);
#endif
        yield break;
    }

#if CATGAMING_ADS && CATGAMING_ADMOB
    private void OnAdsInitialized()
    {
        isAdsReady = true;
        if (!CUtils.IsBuyItem() && !CUtils.IsAdsRemoved())
        {
            RequestBanner();
        }
    }
#endif

    public void RequestBanner()
    {
#if CATGAMING_ADS && CATGAMING_ADMOB
        if (!isAdsReady) return;
        if (CUtils.IsBuyItem() || CUtils.IsAdsRemoved()) return;
        // Banner chỉ ở màn Main: init xong khi đang ở Home/World/Level thì không request.
        if (MainController.instance == null) return;
        CatGamingAds.ShowBanner();
#endif
    }

    public void RequestInterstitial()
    {
#if CATGAMING_ADS && CATGAMING_ADMOB
        if (isAdsReady && Locator<AdmobService>.Instance != null) Locator<AdmobService>.Instance.LoadInterstitial();
#endif
    }

    public void RequestRewardBasedVideo()
    {
#if CATGAMING_ADS && CATGAMING_ADMOB
        if (isAdsReady && Locator<AdmobService>.Instance != null) Locator<AdmobService>.Instance.LoadRewarded();
#endif
    }

    public void ShowBanner()
    {
        if (CUtils.IsBuyItem()) return;
#if CATGAMING_ADS && CATGAMING_ADMOB
        if (CUtils.IsAdsRemoved()) return;
        // Chưa init thì OnAdsInitialized sẽ tự request banner.
        if (!isAdsReady) return;
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
        if (!isAdsReady) return false;
        return CatGamingAds.ShowInterstitial();
#else
        return false;
#endif
    }

    // Trả về true khi rewarded video đã sẵn sàng.
    public bool IsRewardedVideoLoaded()
    {
#if CATGAMING_ADS && CATGAMING_ADMOB
        return isAdsReady && Locator<AdmobService>.Instance != null && Locator<AdmobService>.Instance.IsRewardedReady;
#else
        return false;
#endif
    }

    public bool IsOnline()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    // Số giây còn lại của cooldown giữa hai lần xem rewarded video (0 = xem được).
    public int GetRewardedCooldownRemain()
    {
        if (!CPlayerPrefs.HasKey(REWARDED_ACTION + "_time")) return 0;
        int remain = (int)(ConfigController.Config.rewardedVideoPeriod - CUtils.GetActionDeltaTime(REWARDED_ACTION));
        return Mathf.Max(0, remain);
    }

    public void ShowRewardBasedVideo()
    {
        if (!IsRewardedVideoLoaded())
        {
            print("Reward based video ad is not ready yet");
            RequestRewardBasedVideo();
            if (onRewardedVideoFailed != null) onRewardedVideoFailed();
            return;
        }

#if CATGAMING_ADS && CATGAMING_ADMOB
        isShowingRewarded = true;
        bool started = CatGamingAds.ShowRewarded(OnRewardedCompleted);
        isShowingRewarded = false;
        if (!started && onRewardedVideoFailed != null) onRewardedVideoFailed();
#endif
    }

    private void OnRewardedCompleted(bool rewarded)
    {
        if (rewarded) HandleRewardBasedVideoRewarded();
        else if (!isShowingRewarded && onRewardedVideoClosed != null) onRewardedVideoClosed();
    }

    public void HandleRewardBasedVideoRewarded()
    {
        if (onRewardedVideoRewarded != null) onRewardedVideoRewarded();
    }
}
