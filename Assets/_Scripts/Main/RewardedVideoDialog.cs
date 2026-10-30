using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

// Trạng thái popup rewarded video (design: popups.rewarded / WC.rv).
public enum RewardedVideoState
{
    None,
    Ready,
    Loading,
    Cooldown,
    NoFill,
    Offline,
    Playing,
    Success,
    Cancel,
    Error
}

public class RewardedVideoDialog : Dialog {
    public TMP_Text amountText;
    public TMP_Text messageText;

    [Header("States (design: popups.rewarded)")]
    public RectTransform card;
    public RectTransform closeX;
    public TMP_Text titleText;
    public RectTransform amountHolder;
    public RectTransform spinner;
    public RectTransform spinnerArc;
    public RectTransform adBox;
    public TMP_Text adBoxText;
    public Button primaryButton;
    public TMP_Text primaryText;
    public Shadow primaryShadow;
    public Button secondaryButton;
    public TMP_Text secondaryText;

    // Thời gian chờ video tải trước khi coi là hết quảng cáo.
    public float loadTimeout = 8f;

    private LocalizationController Loc { get { return Locator<LocalizationController>.Instance; } }

    private static readonly Color TEAL = new Color(0.071f, 0.710f, 0.651f, 1f);
    private static readonly Color TEAL_DEEP = new Color(0.043f, 0.549f, 0.502f, 1f);
    private static readonly Color CORAL = new Color(1f, 0.420f, 0.369f, 1f);
    private static readonly Color CORAL_DEEP = new Color(0.851f, 0.294f, 0.251f, 1f);
    private static readonly Color WHITE = Color.white;
    private static readonly Color LINE = new Color(0.875f, 0.914f, 0.918f, 1f);

    private const float PLAYING_TIMEOUT = 180f;
    // Khoảng cách và cỡ theo design (css px x 1,6914 trên canvas 660x1397): padding trên 28, dưới/hai bên 22, gap 16, dòng title 32,5, dòng mô tả 22,4, nút 52.
    private const float PADDING_TOP = 47f;
    private const float PADDING_BOTTOM = 37f;
    private const float SPACING = 27f;
    private const float TITLE_LINE = 55f;
    private const float MESSAGE_LINE = 38f;
    private const float BUTTON_HEIGHT = 88f;
    private const float CLOSE_INSET = 10.3f;

    private RewardedVideoState state;
    private int amount;
    private float loadStart, playStart;

    public RewardedVideoState State { get { return state; } }

    protected override void Start()
    {
        base.Start();
        // Dialog do callback tạo sẵn trạng thái Success; dialog mở từ nút thì tự đánh giá trạng thái.
        if (state == RewardedVideoState.None)
        {
            loadStart = Time.unscaledTime;
            if (AdmobController.instance != null && !AdmobController.instance.IsRewardedVideoLoaded()) AdmobController.instance.RequestRewardBasedVideo();
            Evaluate();
        }
        InvokeRepeating("Evaluate", 1f, 1f);
    }

    // Update private này che Update của Dialog nên phải tự xử lý Escape (prefab bật enableEscape).
    private void Update()
    {
        if (enableEscape && Input.GetKeyDown(KeyCode.Escape)) Close();
        if (spinner.gameObject.activeSelf) spinnerArc.Rotate(0f, 0f, -360f * Time.unscaledDeltaTime);
    }

    public void SetAmount(int value)
    {
        amount = value;
        amountText.text = value.ToString();
        if (state != RewardedVideoState.None) Refresh();
    }

    // Trạng thái hiện tại theo môi trường: cooldown > offline > sẵn sàng > đang tải > hết quảng cáo.
    public RewardedVideoState GetAvailabilityState()
    {
        AdmobController admob = AdmobController.instance;
        if (admob == null) return RewardedVideoState.NoFill;
        if (admob.GetRewardedCooldownRemain() > 0) return RewardedVideoState.Cooldown;
        if (!admob.IsOnline()) return RewardedVideoState.Offline;
        if (admob.IsRewardedVideoLoaded()) return RewardedVideoState.Ready;
        if (Time.unscaledTime - loadStart < loadTimeout) return RewardedVideoState.Loading;
        return RewardedVideoState.NoFill;
    }

    // Chỉ cập nhật các trạng thái chờ; Playing/Success/Cancel/Error do sự kiện quảng cáo điều khiển.
    private void Evaluate()
    {
        // Lưới an toàn: Closed/Failed bị bỏ lỡ (dialog bị ẩn lúc đang phát) thì không kẹt Playing mãi.
        if (state == RewardedVideoState.Playing && Time.unscaledTime - playStart > PLAYING_TIMEOUT) ShowError();
        if (state == RewardedVideoState.Playing || state == RewardedVideoState.Success) return;
        if (state == RewardedVideoState.Cancel || state == RewardedVideoState.Error) return;
        SetState(GetAvailabilityState());
    }

    public void SetState(RewardedVideoState value)
    {
        if (state == value && value != RewardedVideoState.Cooldown) return;
        state = value;
        Refresh();
    }

    public void ShowSuccess()
    {
        SetState(RewardedVideoState.Success);
    }

    public void ShowCancel()
    {
        SetState(RewardedVideoState.Cancel);
    }

    public void ShowError()
    {
        SetState(RewardedVideoState.Error);
    }

    private void OnEnable()
    {
        AttachEvents();
        // Invoke bị huỷ khi dialog bị ẩn (STACK); dựng lại khi hiện lại (lần bật đầu do Start lo).
        if (state != RewardedVideoState.None) InvokeRepeating("Evaluate", 1f, 1f);
    }

    private void OnDisable()
    {
        DetachEvents();
    }

    private void AttachEvents()
    {
        AdmobController admob = AdmobController.instance;
        if (admob == null) return;
        admob.onRewardedVideoClosed -= HandleClosed;
        admob.onRewardedVideoFailed -= HandleFailed;
        admob.onRewardedVideoClosed += HandleClosed;
        admob.onRewardedVideoFailed += HandleFailed;
    }

    private void DetachEvents()
    {
        AdmobController admob = AdmobController.instance;
        if (admob == null) return;
        admob.onRewardedVideoClosed -= HandleClosed;
        admob.onRewardedVideoFailed -= HandleFailed;
    }

    // Chỉ phản ứng khi dialog đang chờ video của chính nó.
    private void HandleClosed()
    {
        if (state == RewardedVideoState.Playing) ShowCancel();
    }

    private void HandleFailed()
    {
        if (state == RewardedVideoState.Playing) ShowError();
    }

    // Nút chính: XEM VIDEO / THỬ LẠI bắt đầu phát; các trạng thái còn lại chỉ đóng.
    public void PrimaryClick()
    {
        if (state == RewardedVideoState.Ready || state == RewardedVideoState.Cancel || state == RewardedVideoState.Error)
        {
            Watch();
            return;
        }
        Claim();
    }

    public void Watch()
    {
        Sound.instance.PlayButton();
        // Thử lại sau Cancel/Error: service đang tải lại video, tính lại thời gian chờ thay vì báo NoFill ngay.
        if ((state == RewardedVideoState.Cancel || state == RewardedVideoState.Error) && AdmobController.instance != null && !AdmobController.instance.IsRewardedVideoLoaded())
        {
            loadStart = Time.unscaledTime;
            AdmobController.instance.RequestRewardBasedVideo();
        }
        RewardedVideoState available = GetAvailabilityState();
        if (available != RewardedVideoState.Ready)
        {
            SetState(available);
            return;
        }

        SetState(RewardedVideoState.Playing);
        playStart = Time.unscaledTime;
        AdmobController.instance.ShowRewardBasedVideo();
    }

    public void Claim()
    {
        Close();
        Sound.instance.PlayButton();
    }

    public override void Close ()
    {
        // Đang phát video thì không cho đóng dialog (quảng cáo toàn màn hình sẽ trả kết quả).
        if (state == RewardedVideoState.Playing) return;
        base.Close ();
    }

    private void Refresh()
    {
        string title = "", desc = "";
        bool showAmount = false, showSpinner = false, showAdBox = false;
        string primary = null, secondary = null;
        bool primaryCoral = false;

        switch (state)
        {
            case RewardedVideoState.Ready:
                title = Loc.Get("rv.title"); desc = Loc.Get("rv.value"); showAmount = true;
                primary = Loc.Get("rv.watch"); primaryCoral = true; secondary = Loc.Get("rv.no");
                break;
            case RewardedVideoState.Loading:
                title = Loc.Get("rv.s.loading"); desc = Loc.Get("rv.d.loading"); showSpinner = true;
                primary = Loc.Get("common.ok");
                break;
            case RewardedVideoState.Cooldown:
                int remain = AdmobController.instance != null ? AdmobController.instance.GetRewardedCooldownRemain() : 0;
                title = Loc.Get("rv.s.cooldown", "t", string.Format("{0:00}:{1:00}", remain / 60, remain % 60)); desc = Loc.Get("rv.d.cooldown");
                primary = Loc.Get("common.ok");
                break;
            case RewardedVideoState.NoFill:
                title = Loc.Get("rv.s.nofill"); desc = Loc.Get("rv.d.nofill");
                primary = Loc.Get("common.ok");
                break;
            case RewardedVideoState.Offline:
                title = Loc.Get("rv.s.offline"); desc = Loc.Get("rv.d.offline");
                primary = Loc.Get("common.ok");
                break;
            case RewardedVideoState.Playing:
                title = Loc.Get("rv.s.playingReal"); desc = Loc.Get("rv.d.playingReal", "n", amount); showAdBox = true;
                break;
            case RewardedVideoState.Success:
                title = Loc.Get("rv.s.success"); desc = Loc.Get("rv.d.success", "n", amount); showAmount = true;
                primary = Loc.Get("common.continue");
                break;
            case RewardedVideoState.Cancel:
                title = Loc.Get("rv.s.cancel"); desc = Loc.Get("rv.d.cancel");
                primary = Loc.Get("common.retry"); primaryCoral = true; secondary = Loc.Get("common.ok");
                break;
            case RewardedVideoState.Error:
                title = Loc.Get("rv.s.error"); desc = Loc.Get("rv.d.error");
                primary = Loc.Get("common.retry"); primaryCoral = true; secondary = Loc.Get("common.ok");
                break;
        }

        titleText.text = title;
        messageText.text = desc;
        amountHolder.gameObject.SetActive(showAmount);
        spinner.gameObject.SetActive(showSpinner);
        adBox.gameObject.SetActive(showAdBox);
        adBoxText.text = Loc.Get("rv.s.playingReal");

        primaryButton.gameObject.SetActive(primary != null);
        if (primary != null)
        {
            primaryText.text = primary;
            primaryButton.GetComponent<Image>().color = primaryCoral ? CORAL : TEAL;
            primaryShadow.effectColor = primaryCoral ? CORAL_DEEP : TEAL_DEEP;
        }
        secondaryButton.gameObject.SetActive(secondary != null);
        if (secondary != null) secondaryText.text = secondary;
        closeX.gameObject.SetActive(state != RewardedVideoState.Playing);

        Layout(showAmount || showSpinner || showAdBox, showAdBox ? adBox : (showSpinner ? spinner : amountHolder), primary != null, secondary != null);
    }

    // Xếp các phần tử đang bật theo chiều dọc rồi co giãn card cho vừa; title/mô tả cao theo số dòng.
    private void Layout(bool hasVisual, RectTransform visual, bool hasPrimary, bool hasSecondary)
    {
        float titleHeight = TITLE_LINE * LineCount(titleText);
        float messageHeight = MESSAGE_LINE * LineCount(messageText);
        float visualHeight = hasVisual ? visual.sizeDelta.y : 0f;
        float total = PADDING_TOP + PADDING_BOTTOM + titleHeight + SPACING + messageHeight;
        if (hasVisual) total += SPACING + visualHeight;
        if (hasPrimary) total += SPACING + BUTTON_HEIGHT;
        if (hasSecondary) total += SPACING + BUTTON_HEIGHT;

        card.sizeDelta = new Vector2(card.sizeDelta.x, total);
        float y = total / 2 - PADDING_TOP;
        y = Place(titleText.rectTransform, y, titleHeight);
        if (hasVisual) y = Place(visual, y - SPACING, visualHeight);
        y = Place(messageText.rectTransform, y - SPACING, messageHeight);
        if (hasPrimary) y = Place((RectTransform)primaryButton.transform, y - SPACING, BUTTON_HEIGHT);
        if (hasSecondary) y = Place((RectTransform)secondaryButton.transform, y - SPACING, BUTTON_HEIGHT);
        closeX.anchoredPosition = new Vector2(closeX.anchoredPosition.x, total / 2 - CLOSE_INSET);
    }

    // Số dòng chữ sau khi xuống dòng theo bề rộng khung hiện tại.
    private int LineCount(TMP_Text text)
    {
        text.ForceMeshUpdate();
        return Mathf.Max(1, text.textInfo.lineCount);
    }

    // Đặt phần tử có mép trên tại top, trả về mép dưới.
    private float Place(RectTransform rt, float top, float height)
    {
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, top - height / 2);
        return top - height;
    }
}
