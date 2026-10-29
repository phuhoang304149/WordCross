using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Text amountText;
    public Text messageText;

    [Header("States (design: popups.rewarded)")]
    public RectTransform card;
    public RectTransform closeX;
    public Text titleText;
    public RectTransform amountHolder;
    public RectTransform spinner;
    public RectTransform spinnerArc;
    public RectTransform adBox;
    public Text adBoxText;
    public Button primaryButton;
    public Text primaryText;
    public Shadow primaryShadow;
    public Button secondaryButton;
    public Text secondaryText;

    // Thời gian chờ video tải trước khi coi là hết quảng cáo.
    public float loadTimeout = 8f;

    private const string TEXT_READY_TITLE = "WATCH A VIDEO";
    private const string TEXT_READY_DESC = "Watch the whole video to get";
    private const string TEXT_LOADING = "Loading video...";
    private const string TEXT_LOADING_DESC = "You can keep playing while it loads.";
    private const string TEXT_COOLDOWN = "Next video in {0}";
    private const string TEXT_COOLDOWN_DESC = "Nothing lost. Come back soon.";
    private const string TEXT_NOFILL = "No video available right now";
    private const string TEXT_NOFILL_DESC = "No rubies charged, no reward. Try later.";
    private const string TEXT_OFFLINE = "You're offline";
    private const string TEXT_OFFLINE_DESC = "Video rubies need internet. Play continues normally.";
    private const string TEXT_PLAYING = "VIDEO PLAYING";
    private const string TEXT_PLAYING_DESC = "Watch to the end to get {0} rubies.";
    private const string TEXT_SUCCESS = "REWARD EARNED!";
    private const string TEXT_SUCCESS_DESC = "{0} rubies added.";
    private const string TEXT_CANCEL = "Closed early";
    private const string TEXT_CANCEL_DESC = "No reward and no rubies charged.";
    private const string TEXT_ERROR = "Video failed to play";
    private const string TEXT_BTN_WATCH = "WATCH VIDEO";
    private const string TEXT_BTN_NO = "NO, THANKS";
    private const string TEXT_BTN_RETRY = "TRY AGAIN";
    private const string TEXT_BTN_OK = "OK";
    private const string TEXT_BTN_CONTINUE = "CONTINUE";

    private static readonly Color TEAL = new Color(0.071f, 0.710f, 0.651f, 1f);
    private static readonly Color TEAL_DEEP = new Color(0.043f, 0.549f, 0.502f, 1f);
    private static readonly Color CORAL = new Color(1f, 0.420f, 0.369f, 1f);
    private static readonly Color CORAL_DEEP = new Color(0.851f, 0.294f, 0.251f, 1f);
    private static readonly Color WHITE = Color.white;
    private static readonly Color LINE = new Color(0.875f, 0.914f, 0.918f, 1f);

    private const float PADDING = 36f;
    private const float SPACING = 18f;
    private const float TITLE_HEIGHT = 60f;
    private const float VISUAL_HEIGHT = 90f;
    private const float MESSAGE_HEIGHT = 84f;
    private const float BUTTON_HEIGHT = 94f;

    private RewardedVideoState state;
    private int amount;
    private bool credited;
    private float loadStart;

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

    private void Update()
    {
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
        RewardedVideoState available = GetAvailabilityState();
        if (available != RewardedVideoState.Ready)
        {
            SetState(available);
            return;
        }

        SetState(RewardedVideoState.Playing);
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
        if (state == RewardedVideoState.Success && !credited)
        {
            credited = true;
            CurrencyController.CreditBalance(int.Parse(amountText.text));
        }
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
                title = TEXT_READY_TITLE; desc = TEXT_READY_DESC; showAmount = true;
                primary = TEXT_BTN_WATCH; primaryCoral = true; secondary = TEXT_BTN_NO;
                break;
            case RewardedVideoState.Loading:
                title = TEXT_LOADING; desc = TEXT_LOADING_DESC; showSpinner = true;
                primary = TEXT_BTN_OK;
                break;
            case RewardedVideoState.Cooldown:
                int remain = AdmobController.instance != null ? AdmobController.instance.GetRewardedCooldownRemain() : 0;
                title = string.Format(TEXT_COOLDOWN, string.Format("{0:00}:{1:00}", remain / 60, remain % 60)); desc = TEXT_COOLDOWN_DESC;
                primary = TEXT_BTN_OK;
                break;
            case RewardedVideoState.NoFill:
                title = TEXT_NOFILL; desc = TEXT_NOFILL_DESC;
                primary = TEXT_BTN_OK;
                break;
            case RewardedVideoState.Offline:
                title = TEXT_OFFLINE; desc = TEXT_OFFLINE_DESC;
                primary = TEXT_BTN_OK;
                break;
            case RewardedVideoState.Playing:
                title = TEXT_PLAYING; desc = string.Format(TEXT_PLAYING_DESC, amount); showAdBox = true;
                break;
            case RewardedVideoState.Success:
                title = TEXT_SUCCESS; desc = string.Format(TEXT_SUCCESS_DESC, amount); showAmount = true;
                primary = TEXT_BTN_CONTINUE;
                break;
            case RewardedVideoState.Cancel:
                title = TEXT_CANCEL; desc = TEXT_CANCEL_DESC;
                primary = TEXT_BTN_RETRY; primaryCoral = true; secondary = TEXT_BTN_OK;
                break;
            case RewardedVideoState.Error:
                title = TEXT_ERROR; desc = TEXT_CANCEL_DESC;
                primary = TEXT_BTN_RETRY; primaryCoral = true; secondary = TEXT_BTN_OK;
                break;
        }

        titleText.text = title;
        messageText.text = desc;
        amountHolder.gameObject.SetActive(showAmount);
        spinner.gameObject.SetActive(showSpinner);
        adBox.gameObject.SetActive(showAdBox);
        adBoxText.text = TEXT_PLAYING;

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

    // Xếp các phần tử đang bật theo chiều dọc rồi co giãn card cho vừa.
    private void Layout(bool hasVisual, RectTransform visual, bool hasPrimary, bool hasSecondary)
    {
        float visualHeight = hasVisual ? Mathf.Max(VISUAL_HEIGHT, visual.sizeDelta.y) : 0f;
        float total = PADDING * 2 + TITLE_HEIGHT + SPACING + MESSAGE_HEIGHT;
        if (hasVisual) total += SPACING + visualHeight;
        if (hasPrimary) total += SPACING + BUTTON_HEIGHT;
        if (hasSecondary) total += SPACING + BUTTON_HEIGHT;

        card.sizeDelta = new Vector2(card.sizeDelta.x, total);
        float y = total / 2 - PADDING;
        y = Place(titleText.rectTransform, y, TITLE_HEIGHT);
        if (hasVisual) y = Place(visual, y - SPACING, visualHeight);
        y = Place(messageText.rectTransform, y - SPACING, MESSAGE_HEIGHT);
        if (hasPrimary) y = Place((RectTransform)primaryButton.transform, y - SPACING, BUTTON_HEIGHT);
        if (hasSecondary) y = Place((RectTransform)secondaryButton.transform, y - SPACING, BUTTON_HEIGHT);
        closeX.anchoredPosition = new Vector2(closeX.anchoredPosition.x, total / 2 - 11f);
    }

    // Đặt phần tử có mép trên tại top, trả về mép dưới.
    private float Place(RectTransform rt, float top, float height)
    {
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, top - height / 2);
        return top - height;
    }
}
