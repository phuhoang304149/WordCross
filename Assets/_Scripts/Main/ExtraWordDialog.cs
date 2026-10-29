using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Script.Utils;
using WordCross.Gameplay;

public class ExtraWordDialog : Dialog
{
    public Transform claimTr;
    public ExtraProgress extraProgress;
    public GameObject claimButton;
    public Text progressText;
    public Text claimText;

    private int numWords, claimQuantity;
    private LocalizationController Loc { get { return Locator<LocalizationController>.Instance; } }

    protected override void Start()
    {
        base.Start();
        extraProgress.target = Prefs.extraTarget;
        extraProgress.current = Prefs.extraProgress;

        UpdateUI();
    }

    public void Claim()
    {
        claimQuantity = GetClaimQuantity();
        
        extraProgress.current -= (int)extraProgress.target;
        Prefs.extraProgress = (int)extraProgress.current;
        UpdateUI();

        StartCoroutine(ClaimEffect());
        ExtraWord.instance.OnClaimed();

        if (Prefs.extraTarget == 5 && Prefs.totalExtraAdded > 10)
        {
            Prefs.extraTarget = 10;
            extraProgress.target = 10;
            UpdateUI();
        }
    }

    private IEnumerator ClaimEffect()
    {
        Transform rubyBalance = GameObject.FindWithTag("RubyBalance").transform;
        // Pivot của RubyBalance nằm ở mép phải pill nên bay về icon ruby con (nếu có) thay vì pivot
        Transform rubyIcon = rubyBalance.Find("Ruby");
        Vector3 targetPosition = rubyIcon != null ? rubyIcon.position : rubyBalance.position;
        var middlePoint = CUtils.GetMiddlePoint(claimTr.position, targetPosition, -0.4f);
        Vector3[] waypoints = { claimTr.position, middlePoint, targetPosition };

        for (int i = 0; i < claimQuantity; i++)
        {
            GameObject gameObj = Instantiate(MonoUtils.instance.rubyFly);
            gameObj.transform.position = waypoints[0];
            gameObj.transform.localScale = 0.5f * Vector3.one;

            iTween.MoveTo(gameObj, iTween.Hash("path", waypoints, "speed", 30, "oncomplete", "OnMoveComplete"));
            iTween.ScaleTo(gameObj, iTween.Hash("scale", 0.7f * Vector3.one, "time", 0.3f));
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateUI()
    {
        // Theo design: nút luôn hiện, xám khi chưa đủ từ thưởng
        bool ready = extraProgress.current >= extraProgress.target;
        claimButton.GetComponent<Button>().interactable = ready;
        claimButton.GetComponent<Image>().color = ready ? new Color(0.0706f, 0.7098f, 0.651f) : new Color(0.8118f, 0.8627f, 0.8627f);
        claimButton.GetComponent<Shadow>().effectColor = ready ? new Color(0.0431f, 0.549f, 0.502f) : new Color(0.7098f, 0.7725f, 0.7725f);
        claimText.text = Loc.Get("extra.claim", "n", GetClaimQuantity());
        progressText.text = extraProgress.current + "/" + extraProgress.target;
    }

    private int GetClaimQuantity()
    {
        return (int)extraProgress.target / 5 * 2;
    }
}
