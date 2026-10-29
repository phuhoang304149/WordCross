using UnityEngine.UI;

public class HomeController : BaseController {
    private const int PLAY = 0;

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
        }
        Sound.instance.PlayButton();
    }

}
