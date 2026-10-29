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

    protected override void Start()
    {
        base.Start();
        UpdateResume();
        Locator<LocalizationController>.Instance.onLanguageChanged += OnLanguageChanged;
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
                Toast.instance.ShowMessage(Locator<LocalizationController>.Instance.Get("home.toast.daily"));
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
