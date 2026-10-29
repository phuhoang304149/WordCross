using UnityEngine.UI;

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
        if (resumeText != null)
            resumeText.text = string.Format("Resume: Subworld {0} - {1}", Prefs.unlockedSubWorld + 1, Prefs.unlockedLevel + 1);
    }

    public void OnClick(int index)
    {
        switch (index)
        {
            case PLAY:
                CUtils.LoadScene(1);
                break;
            case DAILY:
                Toast.instance.ShowMessage("Daily challenge is coming soon");
                break;
            case BOT:
                Toast.instance.ShowMessage("VS Bot is coming soon");
                break;
            case AI:
                Toast.instance.ShowMessage("VS AI is coming soon");
                break;
            case LANGUAGE:
                Toast.instance.ShowMessage("Language selection is coming soon");
                break;
        }
        Sound.instance.PlayButton();
    }

}
