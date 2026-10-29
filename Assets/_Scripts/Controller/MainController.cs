using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Superpow;
using Script.Utils;
using WordCross.Gameplay;

public class MainController : BaseController {
    public Text levelNameText, levelSubText;

    private int world, subWorld, level;
    private bool isGameComplete;
    private GameLevel gameLevel;

    public static MainController instance;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        world = GameState.currentWorld;
        subWorld = GameState.currentSubWorld;
        level = GameState.currentLevel;

        gameLevel = Superpow.Utils.Load(world, subWorld, level);
        Pan.instance.Load(gameLevel);
        WordRegion.instance.Load(gameLevel);

        if (world == 0 && subWorld == 0 && level == 0)
        {
            Timer.Schedule(this, 0.5f, () =>
            {
                DialogController.instance.ShowDialog(DialogType.HowtoPlay);
            });
        }

        UpdateLevelTexts();
        Locator<LocalizationController>.Instance.onLanguageChanged += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        if (Locator<LocalizationController>.Instance != null)
            Locator<LocalizationController>.Instance.onLanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(string code)
    {
        UpdateLevelTexts();
    }

    private void UpdateLevelTexts()
    {
        levelNameText.text = Locator<LocalizationController>.Instance.Get("world.sub", "n", subWorld + 1) + " - " + (level + 1);
        levelSubText.text = Locator<LocalizationController>.Instance.Get("common.level", "n", level + 1);
    }

    public void OnComplete()
    {
        if (isGameComplete) return;
        isGameComplete = true;

        Timer.Schedule(this, 1f, () =>
        {
            DialogController.instance.ShowDialog(DialogType.Win);
            Sound.instance.Play(Sound.Others.Win);
        });
    }

    private string BuildLevelName()
    {
        return world + "-" + subWorld + "-" + level;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !DialogController.instance.IsDialogShowing())
        {
            DialogController.instance.ShowDialog(DialogType.Pause);
        }
    }
}
