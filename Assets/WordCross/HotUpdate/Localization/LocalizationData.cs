using System;
using UnityEngine;

namespace WordCross.Gameplay
{
    // Bảng chuỗi UI: mỗi entry có values theo thứ tự languages (vi, en, fr, es, pt).
    [CreateAssetMenu(fileName = "LocalizationData", menuName = "WordCross/LocalizationData")]
    public class LocalizationData : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string key;
            public string[] values;
        }

        public string[] languages;
        public Entry[] entries;
    }
}
