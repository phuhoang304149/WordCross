using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CTextNumber : MonoBehaviour
{
    public int defaultValue;
    private int number;
    public int Number
    {
        get { return number; }
        set
        {
            number = value;
            UpdateUI();
        }
    }

    private void Awake()
    {
        Number = defaultValue;
    }

    private void UpdateUI()
    {
        TMP_Text text = GetComponent<TMP_Text>();
        text.text = Number.ToString();
    }
}