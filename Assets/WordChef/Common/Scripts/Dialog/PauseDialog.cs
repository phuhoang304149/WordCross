using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseDialog : Dialog {

    protected override void Start()
    {
        base.Start();
    }

    public void OnContinueClick()
    {
        Sound.instance.PlayButton();
        Close();
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    public void OnMenuClick()
    {
        // Daily vào từ Home nên Menu quay về Home, không phải chọn level.
        CUtils.LoadScene(GameState.isDaily ? 0 : 2, true);
        Sound.instance.PlayButton();
        Close();
    }

    public void OnShareClick()
    {
        Sound.instance.PlayButton();
        Close();
    }

    public void OnHowToPlayClick()
    {
        Sound.instance.PlayButton();
        DialogController.instance.ShowDialog(DialogType.HowtoPlay);
    }

    public void OnLanguageClick()
    {
        Sound.instance.PlayButton();
        DialogController.instance.ShowDialog(DialogType.Language);
    }
}
