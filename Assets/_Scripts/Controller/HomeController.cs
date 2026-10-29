using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

public class HomeController : BaseController {
    private const int PLAY = 0;
    private const int DAILY = 1;
    private const int BOT = 2;
    private const int AI = 3;
    private const int LANGUAGE = 4;
    private const int DAILY_FONT = 29;
    private const int DAILY_MIN_FONT = 16;

    public Text resumeText;
    // Chấm đỏ trên nút DAILY (để trống thì tự tìm "Dot" dưới object "Daily" của scene).
    public GameObject dailyDot;

    protected override void Start()
    {
        base.Start();
        UpdateResume();
        UpdateDailyDot();
        Invoke("FitDailyLabel", 0f);
        Locator<LocalizationController>.Instance.onLanguageChanged += OnLanguageChanged;
        Locator<LocalizationController>.Instance.onGameLanguageChanged += OnGameLanguageChanged;
        if (GameState.openDailyOnHome)
        {
            GameState.openDailyOnHome = false;
            DialogController.instance.ShowDialog(DialogType.Daily);
        }
    }

    // Chấm đỏ chỉ hiện khi bàn Daily hôm nay chưa giải.
    public void UpdateDailyDot()
    {
        if (dailyDot == null)
        {
            var daily = GameObject.Find("Daily");
            if (daily != null)
            {
                foreach (var child in daily.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == "Dot") dailyDot = child.gameObject;
                }
            }
        }
        if (dailyDot == null) return;
        var challenge = new DailyChallenge();
        dailyDot.SetActive(!challenge.IsSolved(challenge.Today()));
    }

    private void OnDestroy()
    {
        if (Locator<LocalizationController>.Instance != null)
            Locator<LocalizationController>.Instance.onLanguageChanged -= OnLanguageChanged;
        if (Locator<LocalizationController>.Instance != null)
            Locator<LocalizationController>.Instance.onGameLanguageChanged -= OnGameLanguageChanged;
    }

    // Tiến độ và Daily theo ngôn ngữ chơi: làm mới nhãn "Tiếp tục" và chấm đỏ Daily ngay khi đổi.
    private void OnGameLanguageChanged(string code)
    {
        UpdateResume();
        UpdateDailyDot();
        Invoke("FitDailyLabel", 0f);
    }

    private void OnLanguageChanged(string code)
    {
        UpdateResume();
        Invoke("FitDailyLabel", 0f);
    }

    // Nhãn Daily luôn 1 dòng như design: giữ cỡ design khi đủ chỗ, thiếu thì giảm cỡ tới khi cả cụm chữ vừa phần còn lại sau chấm đỏ.
    private void FitDailyLabel()
    {
        var daily = GameObject.Find("Daily");
        if (daily == null) return;
        var label = daily.GetComponentInChildren<Text>(true);
        var group = label == null ? null : label.GetComponentInParent<HorizontalLayoutGroup>();
        if (label == null || group == null) return;
        Canvas.ForceUpdateCanvases();
        float available = group.GetComponent<RectTransform>().rect.width - group.padding.horizontal;
        if (dailyDot != null && dailyDot.activeSelf)
            available -= group.spacing + dailyDot.GetComponent<RectTransform>().rect.width;
        string text = Locator<LocalizationController>.Instance.Get("home.daily");
        int size = DAILY_FONT;
        var generator = label.cachedTextGeneratorForLayout;
        var settings = label.GetGenerationSettings(Vector2.zero);
        settings.resizeTextForBestFit = false;
        settings.horizontalOverflow = HorizontalWrapMode.Overflow;
        settings.fontSize = size;
        while (size > DAILY_MIN_FONT && generator.GetPreferredWidth(text, settings) / label.pixelsPerUnit > available)
        {
            size--;
            settings.fontSize = size;
        }
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.fontSize = size;
    }

    private void UpdateResume()
    {
        if (resumeText != null)
            resumeText.text = Locator<LocalizationController>.Instance.Get("home.resume", "s", Prefs.unlockedSubWorld + 1, "l", Prefs.unlockedLevel + 1);
    }

    public void OnClick(int index)
    {
        switch (index)
        {
            case PLAY:
                CUtils.LoadScene(1);
                break;
            case DAILY:
                DialogController.instance.ShowDialog(DialogType.Daily);
                break;
            case BOT:
                Toast.instance.ShowMessage(Locator<LocalizationController>.Instance.Get("home.toast.bot"));
                break;
            case AI:
                DialogController.instance.ShowDialog(DialogType.AiSoon);
                break;
            case LANGUAGE:
                DialogController.instance.ShowDialog(DialogType.Language);
                break;
        }
        Sound.instance.PlayButton();
    }

}
