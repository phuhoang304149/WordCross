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
        private const string GamePrefKey = "game_language";

        [Tooltip("Ngôn ngữ khi người chơi chưa chọn (design: vi).")]
        [SerializeField] private string defaultLanguage = "vi";
        [Tooltip("Ngôn ngữ dự phòng khi thiếu chuỗi.")]
        [SerializeField] private string fallbackLanguage = "en";
        [SerializeField] private string dataKey = "Localization/LocalizationData";
        [Tooltip("Ngôn ngữ chơi mặc định: tiến độ giữ khoá cũ (không tiền tố progress.<lang>.).")]
        [SerializeField] private string defaultGameLanguage = "en";
        // Ngôn ngữ chơi đã có bộ level (Levels/<mã>/... trong Addressables; en ở Levels/). Không serialize để OTA CLR đổi được.
        private readonly string[] playableGameLanguages = { "en", "vi", "fr", "es", "pt" };

        public event Action<string> onLanguageChanged;
        public event Action<string> onGameLanguageChanged;

        private readonly Dictionary<string, string[]> table = new Dictionary<string, string[]>();
        private string[] languages = new string[0];
        private string language;
        private string gameLanguage;

        public string Language => language;
        public string GameLanguage => gameLanguage;
        public string DefaultGameLanguage => defaultGameLanguage;
        public string[] Languages => languages;

        // Thư mục gốc level của ngôn ngữ chơi: en giữ "Levels" như bản đang chạy, ngôn ngữ khác "Levels/<mã>".
        public string LevelFolder => gameLanguage == defaultGameLanguage ? "Levels" : "Levels/" + gameLanguage;

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

            var savedGame = PlayerPrefs.GetString(GamePrefKey, "");
            gameLanguage = IsGamePlayable(savedGame) ? savedGame : defaultGameLanguage;

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

        public bool IsGamePlayable(string code)
        {
            return !string.IsNullOrEmpty(code) && Array.IndexOf(playableGameLanguages, code) >= 0;
        }

        // Đổi ngôn ngữ chơi (tách khỏi ngôn ngữ UI). Chỉ nhận ngôn ngữ đã có level; tiến độ ngôn ngữ cũ giữ nguyên trong khoá riêng.
        public void SetGameLanguage(string code)
        {
            if (!IsGamePlayable(code) || code == gameLanguage) return;
            gameLanguage = code;
            PlayerPrefs.SetString(GamePrefKey, code);
            GameState.unlockedWorld = GameState.unlockedSubWord = GameState.unlockedLevel = -1;
            onGameLanguageChanged?.Invoke(code);
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
