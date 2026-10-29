using System;
using Script.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WordCross.Gameplay
{
    // Gắn lên Text / TMP_Text: hiển thị chuỗi theo key, tự cập nhật khi đổi ngôn ngữ.
    [DisallowMultipleComponent]
    public class LocalizedText : MonoBehaviour
    {
        [Serializable]
        public class Arg
        {
            public string name;
            public string value;
        }

        public string key;
        public Arg[] args = new Arg[0];

        private Text text;
        private TMP_Text tmp;

        private void Awake()
        {
            text = GetComponent<Text>();
            tmp = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            if (Locator<LocalizationController>.Instance == null)
                new GameObject("LocalizationController").AddComponent<LocalizationController>();
            Locator<LocalizationController>.Instance.onLanguageChanged += OnLanguageChanged;
            Refresh();
        }

        private void OnDisable()
        {
            if (Locator<LocalizationController>.Instance != null)
                Locator<LocalizationController>.Instance.onLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(string code)
        {
            Refresh();
        }

        public void SetKey(string value)
        {
            key = value;
            Refresh();
        }

        // Đặt (hoặc thêm) tham số {name} rồi vẽ lại.
        public void SetArg(string name, object value)
        {
            var str = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
            var found = Array.Find(args, arg => arg.name == name);
            if (found == null)
            {
                Array.Resize(ref args, args.Length + 1);
                args[args.Length - 1] = found = new Arg { name = name };
            }
            found.value = str;
            Refresh();
        }

        public void Refresh()
        {
            if (string.IsNullOrEmpty(key) || Locator<LocalizationController>.Instance == null) return;
            var pairs = new object[args.Length * 2];
            for (var i = 0; i < args.Length; i++)
            {
                pairs[i * 2] = args[i].name;
                pairs[i * 2 + 1] = args[i].value;
            }
            var value = Locator<LocalizationController>.Instance.Get(key, pairs);
            if (text != null) text.text = value;
            if (tmp != null) tmp.text = value;
        }
    }
}
