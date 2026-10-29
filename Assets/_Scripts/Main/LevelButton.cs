using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {
    public Text levelText;
    public int world, subWorld, level;
    public Transform centerPoint;
    public Image background;

    public Sprite solvedSprite, currentSprite, lockedSprite;

    // index 0: solved, 1: current, 2: locked
    public Image shadow;
    public Color[] backColors, shadowColors, textColors;

    private GameLevel gameLevel;

    public void Start()
    {
        world = GameState.currentWorld;
        subWorld = GameState.currentSubWorld;
        level = transform.GetSiblingIndex();
        levelText.text = (level + 1).ToString();
        GetComponent<Button>().onClick.AddListener(OnButtonClick);

        gameLevel = Superpow.Utils.Load(world, subWorld, level);

        if (gameLevel != null)
        {
            Load();
        }

        int unlockedWorld = Prefs.unlockedWorld;
        int unlockedSubWorld = Prefs.unlockedSubWorld;
        int unlockedLevel = Prefs.unlockedLevel;

        if  (world < unlockedWorld || 
            (world == unlockedWorld && subWorld < unlockedSubWorld) || 
            (world == unlockedWorld && subWorld <= unlockedSubWorld && level < unlockedLevel))
        {
            background.sprite = solvedSprite;
            ApplyColors(0);
        }
        else if (world == unlockedWorld && subWorld == unlockedSubWorld && level == unlockedLevel)
        {
            background.sprite = currentSprite;
            ApplyColors(1);
        }
        else
        {
            background.sprite = lockedSprite;
            GetComponent<Button>().interactable = false;
            ApplyColors(2);
        }
    }

    private void ApplyColors(int state)
    {
        if (state < backColors.Length) background.color = backColors[state];
        if (shadow != null && state < shadowColors.Length) shadow.color = shadowColors[state];
        if (state < textColors.Length) levelText.color = textColors[state];
    }

    public void Load()
    {
        // The pan disk preview is not part of the design: only keep the saved letter order data
        int numLetters = gameLevel.word.Trim().Length;
        if (numLetters == 0) return;

        List<int> indexes = Prefs.GetPanWordIndexes(world, subWorld, level).ToList();
        if (indexes.Count != numLetters)
        {
            indexes = Enumerable.Range(0, numLetters).ToList();
            indexes.Shuffle(level);
            Prefs.SetPanWordIndexes(world, subWorld, level, indexes.ToArray());
        }
    }

    public void OnButtonClick()
    {
        GameState.currentLevel = level;

        CUtils.LoadScene(3, true);
        Sound.instance.PlayButton();

        // Set the music
        Music.instance.Play(CUtils.GetRandom(Music.Type.Main_0, Music.Type.Main_1, Music.Type.Main_2));
    }
}
