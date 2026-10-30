using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class CExtension
{
    public static void SetText(this GameObject obj, string value)
    {
        TMP_Text text = obj.GetComponent<TMP_Text>();
        if (text != null)
        {
            text.text = value;
        }
    }

    public static void SetText(this TMP_Text objText, string value)
    {
        objText.text = value;
    }

    public static void SetTimeText(this TMP_Text text, String preFix, int time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);
        text.text = preFix + string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
    }
}
