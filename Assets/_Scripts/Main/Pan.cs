using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Text;

public class Pan : MonoBehaviour
{
    private int numLetters;
    private string word, panWord;
    private GameLevel gameLevel;
    private const float DesignWidth = 390f, RadiusCss = 84f, RadiusManyCss = 90f;
    // Design đặt chữ theo hệ toạ độ của lòng vòng (lệch viền 3 css) nên tâm chữ nằm (+3, +3) so với tâm vòng; đo trên máy: (+2,9, +1,2 sau bù Pan).
    private const float OffsetRightCss = 2.9f, OffsetDownCss = 1.2f;
    private List<Vector3> letterPositions = new List<Vector3>();
    private List<Vector3> letterLocalPositions = new List<Vector3>();
    private List<Text> letterTexts = new List<Text>();
    private List<LetterText> letterViews = new List<LetterText>();
    private List<int> indexes = new List<int>();

    private int world, subWorld, level;

    public Transform centerPoint;
    public TextPreview textPreview;

    public static Pan instance;

    private void Awake()
    {
        instance = this;
    }

    // Cụm vòng chữ có thể bị dời (banner bật/tắt) sau Load: giữ toạ độ chạm của LineDrawer khớp vị trí chữ.
    private void LateUpdate()
    {
        for (int i = 0; i < letterPositions.Count; i++)
        {
            letterPositions[i] = centerPoint.TransformPoint(letterLocalPositions[i]);
        }
    }

    public void Load(GameLevel gameLevel)
    {
        // Gán tại đây thay vì Start để không phụ thuộc thứ tự Start với MainController.
        world = GameState.currentWorld;
        subWorld = GameState.currentSubWorld;
        level = GameState.currentLevel;

        this.gameLevel = gameLevel;
        numLetters = gameLevel.word.Trim().Length;

        // Bán kính vòng chữ theo design (.wheel: 84 css, trên 6 chữ 90 css), đổi ra đơn vị cục bộ của centerPoint.
        RectTransform canvasRt = (RectTransform)centerPoint.GetComponentInParent<Canvas>().rootCanvas.transform;
        float localPerCss = canvasRt.rect.width / DesignWidth * canvasRt.lossyScale.x / centerPoint.lossyScale.x;
        float radius = (numLetters > 6 ? RadiusManyCss : RadiusCss) * localPerCss;

        float delta = 360f / numLetters;

        float angle = 90;
        for (int i = 0; i < numLetters; i++)
        {
            float angleRadian = angle * Mathf.PI / 180f;
            float x = Mathf.Cos(angleRadian);
            float y = Mathf.Sin(angleRadian);
            Vector3 position = radius * new Vector3(x, y, 0) + new Vector3(OffsetRightCss, -OffsetDownCss, 0) * localPerCss;

            letterLocalPositions.Add(position);
            letterPositions.Add(centerPoint.TransformPoint(position));

            angle += delta;
        }

        LineDrawer.instance.letterPositions = letterPositions;

        for(int i = 0; i < numLetters; i++)
        {
            Text letter = Instantiate(MonoUtils.instance.letter);
            letter.transform.SetParent(centerPoint);
            letter.transform.localScale = Vector3.one;
            letter.transform.localRotation = Quaternion.identity;
            letter.text = gameLevel.word[i].ToString().ToUpper();
            letterTexts.Add(letter);
            letterViews.Add(letter.GetComponent<LetterText>());
        }

        indexes = Prefs.GetPanWordIndexes(world, subWorld, level).ToList();
        if (indexes.Count != numLetters)
        {
            indexes = Enumerable.Range(0, numLetters).ToList();
            indexes.Shuffle(level);
            Prefs.SetPanWordIndexes(world, subWorld, level, indexes.ToArray());
        }

        GetPanWord();

        Timer.Schedule(this, 0, () => 
        {
            for (int i = 0; i < numLetters; i++)
            {
                letterTexts[i].transform.localPosition = letterLocalPositions[indexes.IndexOf(i)];
            }
        });
    }

    private void GetShuffeWord()
    {
        List<int> origin = new List<int>();
        origin.AddRange(indexes);
        while (true)
        {
            indexes.Shuffle();
            if (!origin.SequenceEqual(indexes)) break;
        }
        GetPanWord();
    }

    private void GetPanWord()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < numLetters; i++)
        {
            sb.Append(gameLevel.word[indexes[i]]);
        }
        panWord = sb.ToString();
        textPreview.word = panWord.ToUpper();
    }

    public void SetSelectedPositions(List<int> positions)
    {
        for (int i = 0; i < numLetters; i++)
        {
            letterViews[i].SetSelected(positions.Contains(indexes.IndexOf(i)));
        }
    }

    public void Shuffle()
    {
        GetShuffeWord();
        Prefs.SetPanWordIndexes(world, subWorld, level, indexes.ToArray());

        int i = 0;
        foreach(var text in letterTexts)
        {
            iTween.MoveTo(text.gameObject, iTween.Hash("position", letterLocalPositions[indexes.IndexOf(i)], "time", 0.15f, "isLocal", true));
            text.transform.localRotation = Quaternion.identity;
            i++;
        }
        Sound.instance.PlayButton();
    }
}
