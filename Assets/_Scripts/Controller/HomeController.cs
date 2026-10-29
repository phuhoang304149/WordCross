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

    public Text resumeText;
    // Chấm đỏ trên nút DAILY (để trống thì tự tìm "Dot" dưới object "Daily" của scene).
    public GameObject dailyDot;

    protected override void Start()
    {
        base.Start();
        UpdateResume();
        UpdateDailyDot();
        Locator<LocalizationController>.Instance.onLanguageChanged += OnLanguageChanged;
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
    }

    private void OnLanguageChanged(string code)
    {
        UpdateResume();
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
                Toast.instance.ShowMessage(Locator<LocalizationController>.Instance.Get("home.toast.ai"));
                break;
            case LANGUAGE:
                DialogController.instance.ShowDialog(DialogType.Language);
                break;
        }
        Sound.instance.PlayButton();
    }

}
