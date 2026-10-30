using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class MonoUtils : MonoBehaviour {
    public TMP_Text letter;
    public Cell cell;
    public LineWord lineWord;
    public Transform textFlyTransform;
    public GameObject rubyFly;
    public GameObject levelButton;

    public static MonoUtils instance;

    private void Awake()
    {
        instance = this;
    }
}
