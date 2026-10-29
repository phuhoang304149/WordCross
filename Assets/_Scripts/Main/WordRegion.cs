using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using Superpow;
using Script.Utils;
using WordCross.Gameplay;

// Bàn ô chữ theo design (.board): ô, khoảng cách, cột và vị trí dọc lấy theo css px của canvas 390 rộng.
public class WordRegion : MonoBehaviour {
    public TextPreview textPreview;
    public Compliment compliment;
    public RectTransform toolsTop;

    private const float DesignWidth = 390f;
    private const float RegionTop = 90f, RegionBottomPad = 6f;
    private const float MaxCell = 40f, MinCell = 14f, CellGapCss = 4f, RowGapCss = 6f, ColGapCss = 14f;
    private const float BlockGap = 12f, PreviewHeight = 46f, MessageHeight = 18f;
    private const float CoachPadX = 14f, CoachPadY = 8f, CoachFont = 12.85f, CoachLine = 17.55f, CoachMargin = 18f, CoachWrapSlack = 0.94f;

    private List<LineWord> lines = new List<LineWord>();
    private List<string> validWords = new List<string>();

    private GameLevel gameLevel;
    private int numWords, numCol, numRow;
    private float sizeCss;
    private float[] colWidthCss;
    private RectTransform coach;
    private float coachHeightCss, lastToolsY = float.NaN;

    private RectTransform rt;
    public static WordRegion instance;

    private void Awake()
    {
        instance = this;
        rt = GetComponent<RectTransform>();
    }

    public void Load(GameLevel gameLevel)
    {
        this.gameLevel = gameLevel;

        var wordList = CUtils.BuildListFromString<string>(this.gameLevel.answers);
        validWords = CUtils.BuildListFromString<string>(this.gameLevel.validWords);
        numWords = wordList.Count;

        numCol = numWords <= 4 ? 1 :
                     numWords <= 10 ? 2 : 3;

        numRow = (int)Mathf.Ceil(numWords / (float)numCol);

        colWidthCss = new float[numCol];
        int maxCells = 0;
        for (int c = 0; c < numCol; c++)
        {
            int widest = 0;
            for (int r = 0; r < numRow && c * numRow + r < numWords; r++)
            {
                widest = Mathf.Max(widest, wordList[c * numRow + r].Length);
            }
            maxCells += widest;
        }
        sizeCss = Mathf.Clamp(Mathf.Floor((350f - 14f * (numCol - 1)) / (maxCells * 1.1f)), MinCell, MaxCell);
        for (int c = 0; c < numCol; c++)
        {
            int widest = 0;
            for (int r = 0; r < numRow && c * numRow + r < numWords; r++)
            {
                widest = Mathf.Max(widest, wordList[c * numRow + r].Length);
            }
            colWidthCss[c] = widest * sizeCss + (widest - 1) * CellGapCss;
        }

        float cellSize = sizeCss * LocalPerCss();
        float cellGap = CellGapCss * LocalPerCss();

        string[] levelProgress = GetLevelProgress();
        bool useProgress = false;

        if (levelProgress.Length != 0)
        {
            useProgress = CheckLevelProgress(levelProgress, wordList);
            if (!useProgress) ClearLevelProgress(); 
        }

        int lineIndex = 0;
        foreach (var word in wordList)
        {
            LineWord line = Instantiate(MonoUtils.instance.lineWord);
            line.answer = word.ToUpper();
            line.cellSize = cellSize;
            line.cellGap = cellGap;
            line.SetLineWidth();
            line.Build();

            if (useProgress)
            {
                line.SetProgress(levelProgress[lineIndex]);
            }

            line.transform.SetParent(transform);
            line.transform.localScale = Vector3.one;
            line.transform.localPosition = Vector3.zero;

            lines.Add(line);
            lineIndex++;
        }

        BuildCoach();
        Layout();
    }

    private void LateUpdate()
    {
        if (lines.Count == 0 || toolsTop == null) return;
        float y = toolsTop.position.y;
        if (!Mathf.Approximately(y, lastToolsY)) Layout();
    }

    private RectTransform CanvasRect()
    {
        return (RectTransform)rt.GetComponentInParent<Canvas>().rootCanvas.transform;
    }

    // Số đơn vị canvas trên một css px của design (canvas rộng 390 css).
    private float CanvasPerCss()
    {
        return CanvasRect().rect.width / DesignWidth;
    }

    // Số đơn vị cục bộ của vùng chữ trên một css px.
    private float LocalPerCss()
    {
        return CanvasPerCss() * CanvasRect().lossyScale.x / rt.lossyScale.x;
    }

    private bool ShowsCoach()
    {
        if (GameState.isDaily) return true;
        return GameState.currentWorld == 0 && GameState.currentSubWorld == 0 && GameState.currentLevel == 0
            && Prefs.unlockedWorld == 0 && Prefs.unlockedSubWorld == 0 && Prefs.unlockedLevel == 0;
    }

    // Khung nhắc người chơi (design .coach): nền vàng nhạt, chữ nâu 13 css, bo 16.
    private void BuildCoach()
    {
        if (!ShowsCoach()) return;
        var loc = Locator<LocalizationController>.Instance;
        string message = GameState.isDaily
            ? loc.Get("game.coachDaily", "n", numWords)
            : loc.Get("game.coach1") + "\n" + loc.Get("game.coach2");
        int numLines = GameState.isDaily ? 1 : 2;
        coachHeightCss = CoachPadY * 2 + CoachLine * numLines;

        float perCss = LocalPerCss();
        var box = new GameObject("Coach", typeof(RectTransform), typeof(Image));
        coach = (RectTransform)box.transform;
        coach.SetParent(transform, false);
        coach.localScale = Vector3.one;
        var image = box.GetComponent<Image>();
        image.sprite = textPreview.backgroundRT.GetComponent<Image>().sprite;
        image.type = Image.Type.Sliced;
        image.pixelsPerUnitMultiplier = 1f;
        image.color = new Color32(0xff, 0xf1, 0xc4, 255);
        image.raycastTarget = false;

        var label = new GameObject("Text", typeof(RectTransform), typeof(Text));
        var labelRect = (RectTransform)label.transform;
        labelRect.SetParent(coach, false);
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = labelRect.offsetMax = Vector2.zero;
        var text = label.GetComponent<Text>();
        text.font = textPreview.text.font;
        text.fontSize = Mathf.RoundToInt(CoachFont * perCss);
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.color = new Color32(0x6b, 0x4d, 0x00, 255);
        text.raycastTarget = false;
        text.text = message;

        float width = text.preferredWidth + CoachPadX * 2 * perCss;
        float maxWidth = (DesignWidth - 2 * CoachMargin) * perCss;
        if (width > maxWidth)
        {
            // Chữ dài hơn bề ngang màn (fr/es/pt): xuống dòng trong khung và tăng chiều cao theo số dòng.
            width = maxWidth;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            labelRect.offsetMin = new Vector2(CoachPadX * perCss, 0f);
            labelRect.offsetMax = new Vector2(-CoachPadX * perCss, 0f);
            text.verticalOverflow = VerticalWrapMode.Overflow;
            // Đo từng câu theo bề rộng khi không xuống dòng (cùng đơn vị với preferredWidth ở trên) rồi chia cho bề rộng khung.
            float inner = (width - 2 * CoachPadX * perCss) * CoachWrapSlack;
            int lines = 0;
            foreach (string sentence in message.Split('\n'))
            {
                text.text = sentence;
                lines += Mathf.Max(1, Mathf.CeilToInt(text.preferredWidth / inner));
            }
            text.text = message;
            coachHeightCss = CoachPadY * 2 + CoachLine * lines;
        }
        coach.sizeDelta = new Vector2(width, coachHeightCss * perCss);
    }

    // Vùng bàn chơi = giữa thanh trên và hàng nút công cụ; ô, khung nhắc, preview, thông báo xếp dọc rồi căn giữa (design .board).
    private void Layout()
    {
        RectTransform canvasRt = CanvasRect();
        float perCss = CanvasPerCss();
        float top = canvasRt.rect.yMax - RegionTop * perCss;
        float bottom = canvasRt.rect.yMin;
        if (toolsTop != null)
        {
            var corners = new Vector3[4];
            toolsTop.GetWorldCorners(corners);
            bottom = canvasRt.InverseTransformPoint(corners[1]).y + RegionBottomPad * perCss;
            lastToolsY = toolsTop.position.y;
        }

        float gridWidth = ColGapCss * (numCol - 1);
        foreach (var w in colWidthCss) gridWidth += w;
        float gridHeight = numRow * sizeCss + (numRow - 1) * RowGapCss;
        float groupHeight = gridHeight + BlockGap + PreviewHeight + BlockGap + MessageHeight;
        if (coach != null) groupHeight += coachHeightCss + BlockGap;

        float gridTop = top - Mathf.Max(0f, (top - bottom) / perCss - groupHeight) / 2f * perCss;

        float colX = -gridWidth / 2f;
        for (int c = 0; c < numCol; c++)
        {
            for (int r = 0; r < numRow && c * numRow + r < lines.Count; r++)
            {
                float lineTop = gridTop / perCss - r * (sizeCss + RowGapCss);
                Vector3 origin = rt.InverseTransformPoint(canvasRt.TransformPoint(new Vector3(colX * perCss, (lineTop - sizeCss) * perCss, 0)));
                lines[c * numRow + r].transform.localPosition = new Vector3(origin.x, origin.y, 0);
            }
            colX += colWidthCss[c] + ColGapCss;
        }

        float cursor = gridTop / perCss - gridHeight - BlockGap;
        if (coach != null)
        {
            Vector3 center = rt.InverseTransformPoint(canvasRt.TransformPoint(new Vector3(0, (cursor - coachHeightCss / 2f) * perCss, 0)));
            coach.localPosition = new Vector3(center.x, center.y, 0);
            cursor -= coachHeightCss + BlockGap;
        }

        Vector3 preview = canvasRt.TransformPoint(new Vector3(0, (cursor - PreviewHeight / 2f) * perCss, 0));
        textPreview.transform.position = new Vector3(textPreview.transform.position.x, preview.y, textPreview.transform.position.z);
    }

    public void CheckAnswer(string checkWord)
    {
        LineWord line = lines.Find(x => x.answer == checkWord);
        if (line != null)
        {
            if (!line.isShown)
            {
                textPreview.SetAnswerColor();
                line.ShowAnswer();
                CheckGameComplete();

                if (lines.Last() == line)
                {
                    compliment.ShowRandom();
                }

                Sound.instance.Play(Sound.Others.Match);
            }
            else
            {
                textPreview.SetExistColor();
            }
        }
        else if (validWords.Contains(checkWord.ToLower()))
        {
            ExtraWord.instance.ProcessWorld(checkWord);
        }
        else
        {
            textPreview.SetWrongColor();
        }

        textPreview.FadeOut();
    }

    private void CheckGameComplete()
    {
        SaveLevelProgress();
        var isNotShown = lines.Find(x => !x.isShown);
        if (isNotShown == null)
        {
            ClearLevelProgress();
            MainController.instance.OnComplete();

            if (lines.Count >= 6)
            {
                compliment.ShowRandom();
            }
        }
    }

    public void HintClick()
    {
        int ballance = CurrencyController.GetBalance();
        if (ballance >= Const.HINT_COST)
        {
            var line = lines.Find(x => !x.isShown);

            if (line != null)
            {
                line.ShowHint();
                CurrencyController.DebitBalance(Const.HINT_COST);
                CheckGameComplete();

                Prefs.AddToNumHint(GameState.currentWorld, GameState.currentSubWorld, GameState.currentLevel);
            }
        }
        else
        {
            DialogController.instance.ShowDialog(DialogType.LowRuby);
        }
        Sound.instance.PlayButton();
    }

    public void SaveLevelProgress()
    {
        if (!Prefs.IsLastLevel()) return;

        List<string> results = new List<string>();
        foreach(var line in lines)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var cell in line.cells)
            {
                sb.Append(cell.isShown ? "1" : "0");
            }
            results.Add(sb.ToString());
        }

        Prefs.levelProgress = results.ToArray();
    }

    public string[] GetLevelProgress()
    {
        if (!Prefs.IsLastLevel()) return new string[0];
        return Prefs.levelProgress;
    }

    public void ClearLevelProgress()
    {
        if (!Prefs.IsLastLevel()) return;
        CPlayerPrefs.DeleteKey(Prefs.progressKeys.Of("level_progress"));
    }

    public bool CheckLevelProgress(string[] levelProgress, List<string> wordList)
    {
        if (levelProgress.Length != wordList.Count) return false;

        for(int i = 0; i < wordList.Count; i++)
        {
            if (levelProgress[i].Length != wordList[i].Length) return false;
        }
        return true;
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            Timer.Schedule(this, 0.5f, () =>
            {
                UpdateBoard();
            });
        }
    }

    private void UpdateBoard()
    {
        string[] progress = GetLevelProgress();
        if (progress.Length == 0) return;

        int i = 0;
        foreach(var line in lines)
        {
            line.SetProgress(progress[i]);
            i++;
        }
    }
}
