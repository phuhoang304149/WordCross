using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;
using TMPro;

// Popup chọn kiểu Hint khi bấm nút Hint (design chỉ có 4 nút): gợi ý 1 ô hoặc cả từ, giá lấy từ EconomyService (sink_hint_cell / sink_hint_word).
// Chọn xong đóng popup rồi gọi logic cũ của WordRegion (thiếu ruby thì WordRegion mở LowRubyDialog).
public class HintPickDialog : Dialog {
    public TMP_Text titleText;
    public Button cellButton;
    public TMP_Text cellText;
    public Button wordButton;
    public TMP_Text wordText;

    private WordRegion region;

    private LocalizationController Loc { get { return Locator<LocalizationController>.Instance; } }
    private EconomyService Econ { get { return Locator<EconomyService>.Instance; } }

    public void Setup(WordRegion wordRegion)
    {
        region = wordRegion;
    }

    protected override void Start()
    {
        base.Start();
        Refresh();
    }

    public void OnCellClick()
    {
        Close();
        region.HintClick();
    }

    public void OnWordClick()
    {
        Close();
        region.HintWordClick();
    }

    public void OnCloseClick()
    {
        Sound.instance.PlayButton();
        Close();
    }

    private int Cost(string sinkId, int fallback)
    {
        return Econ != null ? Econ.SinkCost(sinkId) : fallback;
    }

    private void Refresh()
    {
        titleText.text = Loc.Get("hint.pick.title");
        cellText.text = Loc.Get("hint.pick.cell", "n", Cost(EconomyService.SINK_HINT_CELL, Const.HINT_COST));
        wordText.text = Loc.Get("hint.pick.word", "n", Cost(EconomyService.SINK_HINT_WORD, Const.HINT_WORD_COST));
        // Hết ô trống thì không còn gì để gợi ý.
        bool canHint = region != null && region.HasHiddenLine();
        cellButton.interactable = canHint;
        wordButton.interactable = canHint;
    }
}
