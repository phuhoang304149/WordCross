using System;
using System.Collections.Generic;
using CatGaming.Clr;
using Script.Utils;
using UnityEngine;

namespace WordCross.Gameplay
{
    // Ngôn ngữ UI (tách khỏi ngôn ngữ chơi). Một instance sống xuyên scene, lấy qua Locator<LocalizationController>.Instance.
    [DefaultExecutionOrder(-1000)]
    public class LocalizationController : MonoBehaviour
    {
        private const string PrefKey = "ui_language";

        [Tooltip("Ngôn ngữ khi người chơi chưa chọn (design: vi).")]
        [SerializeField] private string defaultLanguage = "vi";
        [Tooltip("Ngôn ngữ dự phòng khi thiếu chuỗi.")]
        [SerializeField] private string fallbackLanguage = "en";
        [SerializeField] private string dataKey = "Localization/LocalizationData";

        public event Action<string> onLanguageChanged;

        private readonly Dictionary<string, string[]> table = new Dictionary<string, string[]>();
        private string[] languages = new string[0];
        private string language;

        public string Language => language;
        public string[] Languages => languages;

        private void Awake()
        {
            if (Locator<LocalizationController>.Instance != null && Locator<LocalizationController>.Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Locator<LocalizationController>.Set(this);

            var data = ClrContent.Instance.Load<LocalizationData>(dataKey);
            if (data == null)
            {
                Debug.LogError("[Localization] LocalizationData not found: " + dataKey, this);
            }
            else
            {
                languages = data.languages;
                foreach (var entry in data.entries) table[entry.key] = entry.values;
            }

            var saved = PlayerPrefs.GetString(PrefKey, "");
            language = IndexOf(saved) >= 0 ? saved : defaultLanguage;

            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Locator<LocalizationController>.Instance == this) Locator<LocalizationController>.Set(null);
        }

        public bool HasLanguage(string code)
        {
            return IndexOf(code) >= 0;
        }

        // Đổi ngôn ngữ, lưu PlayerPrefs, phát onLanguageChanged. Mã lạ bị bỏ qua.
        public void SetLanguage(string code)
        {
            if (IndexOf(code) < 0 || code == language) return;
            language = code;
            PlayerPrefs.SetString(PrefKey, code);
            onLanguageChanged?.Invoke(code);
        }

        // Get("win.first", "n", 2): tham số là cặp tên/giá trị thay cho {tên}. Thiếu ngôn ngữ -> fallback -> chính key.
        public string Get(string key, params object[] args)
        {
            if (!table.TryGetValue(key, out var values)) return key;
            var text = Pick(values, language);
            if (string.IsNullOrEmpty(text)) text = Pick(values, fallbackLanguage);
            if (string.IsNullOrEmpty(text)) return key;
            for (var i = 0; i + 1 < args.Length; i += 2)
                text = text.Replace("{" + args[i] + "}", Convert.ToString(args[i + 1], System.Globalization.CultureInfo.InvariantCulture));
            return text;
        }

        private string Pick(string[] values, string code)
        {
            var index = IndexOf(code);
            return index >= 0 && index < values.Length ? values[index] : null;
        }

        private int IndexOf(string code)
        {
            return string.IsNullOrEmpty(code) ? -1 : Array.IndexOf(languages, code);
        }
    }
}
