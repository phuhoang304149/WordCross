#if GAME_DEBUG || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Script.Utils;

namespace WordCross.Gameplay
{
    // Công cụ dev (tools/ui-preview): mở thẳng một màn/dialog lúc khởi động để chụp ảnh, không cần tap.
    // Nguồn lệnh: intent extra "open" (adb shell am start --es open win) hoặc file <persistentDataPath>/debug_open.txt (đọc xong xoá).
    // Cú pháp: <màn>[:<tham số>][;ruby=N][;level=w-s-l]
    //   Hậu tố ;extra=N đặt tiến độ từ thưởng (lưu thật, như ruby).
    //   Hậu tố ;lang=vi|en|fr|es|pt đổi ngôn ngữ giao diện (lưu thật; trả lại bằng ;lang=<mã cũ>).
    //   Hậu tố ;unlock=w-s-l đặt level đang mở khoá (lưu thật; `reset` trả lại giá trị trước khi ghi đè).
    //   home | selectworld | selectlevel[:w-s] | main:w-s-l
    //   pause | win[:first|replay] | extra[:ready|notready] | howto   (mở trong Main, mặc định level đang mở khoá)
    //   shop | rate | language | rewarded:<ready|loading|cooldown|nofill|offline|playing|success|cancel|error> (mở trong Home)
    //   daily[:fresh|risk|safe|lost]  dialog Daily ở Home với dữ liệu chuỗi giả (lưu thật, trả lại bằng reset)
    //   dailymain[:fresh|risk|safe|lost]  Main với bàn Daily hôm nay
    //   dailywin[:fresh|two|risk|safe|lost]   Win biến thể Daily (thắng bàn hôm nay; ruby/dữ liệu Daily trả lại sau 1 giây)
    //   Hậu tố ;glang=vi|en|fr|es|pt đổi ngôn ngữ chơi (lưu thật; trả lại bằng ;glang=en).
    //   Hậu tố ;solve=K (cho main) tự nhập mọi đáp án trừ K từ cuối, in toạ độ màn hình của các chữ trong vòng ("letters") để adb swipe nốt.
    //   Hậu tố ;noads=N giả lập mua gói tắt quảng cáo thành công sau N giây (gọi Purchaser.GrantNoAds, cùng đường OnNoAdsRestored; không qua Google Play).
    //   noads | noads:off   giả lập mua ngay / xoá cờ buy_item + remove_ads để chụp lại trạng thái có banner.
    //   Hậu tố ;iap=<productId> (hoặc lệnh iap:<productId> ở Home) giả lập đơn IAP về (gọi Purchaser.ProcessPurchase như tầng sau store, không qua Google Play; cộng ruby thật, trả lại bằng ;ruby/reset).
    //   reset (trả số ruby và dữ liệu Daily về giá trị trước khi ghi đè)
    public sealed class DebugOpener : MonoBehaviour
    {
        private const string FileName = "debug_open.txt";
        private const string RubyBackupKey = "debug_ruby_backup";
        private const string DailyBackupKey = "debug_daily_backup";
        private const string UnlockBackupKey = "debug_unlock_backup";
        private static readonly string[] DailyKeys = { "daily_solved", "daily_milestones", "daily_best", "daily_last_seen" };

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            StartCoroutine(Run());
        }

        private string ReadCommand()
        {
            string command = ReadIntentExtra();
            string path = Path.Combine(Application.persistentDataPath, FileName);
            if (File.Exists(path))
            {
                if (string.IsNullOrEmpty(command)) command = File.ReadAllText(path);
                File.Delete(path);
            }
            return command == null ? "" : command.Trim();
        }

        private string ReadIntentExtra()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            if (activity == null) return null;
            var intent = activity.Call<AndroidJavaObject>("getIntent");
            if (intent == null) return null;
            return intent.Call<string>("getStringExtra", "open");
#else
            return null;
#endif
        }

        private IEnumerator Run()
        {
            string command = ReadCommand();
            if (command.Length == 0) yield break;
            Debug.Log("[DebugOpener] command=" + command);

            yield return WaitScene("Home");

            string[] parts = command.Split(';');
            string[] head = parts[0].Split(new[] { ':' }, 2);
            string name = head[0].Trim().ToLowerInvariant();
            string arg = head.Length > 1 ? head[1].Trim().ToLowerInvariant() : "";
            int[] level = null;
            int solveKeep = -1;
            float noAdsDelay = -1f;
            string iapProduct = null;
            for (int i = 1; i < parts.Length; i++)
            {
                string[] pair = parts[i].Split(new[] { '=' }, 2);
                if (pair.Length < 2) continue;
                string key = pair[0].Trim().ToLowerInvariant();
                if (key == "ruby") ApplyRuby(int.Parse(pair[1]));
                else if (key == "level") level = ParseLevel(pair[1]);
                else if (key == "extra") Prefs.extraProgress = int.Parse(pair[1]);
                else if (key == "unlock") ApplyUnlock(ParseLevel(pair[1]));
                else if (key == "lang") Locator<LocalizationController>.Instance.SetLanguage(pair[1]);
                else if (key == "glang") Locator<LocalizationController>.Instance.SetGameLanguage(pair[1].Trim().ToLowerInvariant());
                else if (key == "solve") solveKeep = int.Parse(pair[1]);
                else if (key == "noads") noAdsDelay = float.Parse(pair[1]);
                else if (key == "iap") iapProduct = pair[1].Trim();
            }
            if (noAdsDelay >= 0) StartCoroutine(GrantNoAdsAfter(noAdsDelay));

            switch (name)
            {
                case "reset":
                    RestoreRuby();
                    RestoreDaily();
                    RestoreUnlock();
                    break;
                case "daily":
                    ApplyDaily(arg);
                    DialogController.instance.ShowDialog(DialogType.Daily);
                    break;
                case "dailymain":
                case "dailywin":
                    yield return OpenDailyInMain(name, arg);
                    break;
                case "selectworld":
                    CUtils.LoadScene(1);
                    break;
                case "selectlevel":
                    int[] sub = arg.Length > 0 ? ParseLevel(arg) : null;
                    GameState.currentWorld = sub != null ? sub[0] : Prefs.unlockedWorld;
                    GameState.currentSubWorld = sub != null && sub.Length > 1 ? sub[1] : Prefs.unlockedSubWorld;
                    CUtils.LoadScene(2);
                    break;
                case "main":
                    SetCurrentLevel(ParseLevel(arg));
                    CUtils.LoadScene(3);
                    yield return WaitScene("Main");
                    if (solveKeep >= 0) yield return SolveLevel(solveKeep);
                    break;
                case "pause":
                case "howto":
                case "extra":
                case "win":
                case "lowruby":
                    yield return OpenInMain(name, arg, level);
                    break;
                case "shop":
                    DialogController.instance.ShowDialog(DialogType.Shop);
                    break;
                case "aisoon":
                    DialogController.instance.ShowDialog(DialogType.AiSoon);
                    break;
                case "rate":
                    DialogController.instance.ShowDialog(DialogType.RateGame);
                    break;
                case "language":
                    DialogController.instance.ShowDialog(DialogType.Language);
                    break;
                case "rewarded":
                    yield return OpenRewarded(arg);
                    break;
                case "noads":
                    if (arg == "off") ClearNoAds();
                    else Purchaser.instance.GrantNoAds();
                    break;
                case "iap":
                    iapProduct = head[1].Trim();
                    break;
                case "home":
                    break;
                default:
                    Debug.LogError("[DebugOpener] unknown command: " + command);
                    break;
            }
            if (iapProduct != null)
            {
                // Chờ dialog mở xong rồi mới giả lập đơn về, để thấy cả toast của Shop.
                yield return new WaitForSecondsRealtime(1.0f);
                int rubyBefore = CurrencyController.GetBalance();
                Purchaser.instance.ProcessPurchase(iapProduct);
                Debug.Log("[DebugOpener] iap " + iapProduct + " ruby " + rubyBefore + "->" + CurrencyController.GetBalance() + " buy_item=" + CUtils.IsBuyItem());
            }
            Debug.Log("[DebugOpener] opened " + command + " glang=" + Locator<LocalizationController>.Instance.GameLanguage +
                " unlocked=" + Prefs.unlockedWorld + "-" + Prefs.unlockedSubWorld + "-" + Prefs.unlockedLevel + " ruby=" + CurrencyController.GetBalance());
        }

        // Nhập đáp án qua WordRegion.CheckAnswer (chuỗi chữ hoa như vuốt vòng chữ), chừa lại `keep` từ cuối; in toạ độ chữ để vuốt tay.
        private IEnumerator SolveLevel(int keep)
        {
            yield return new WaitForSecondsRealtime(1.0f);
            GameLevel gameLevel = Superpow.Utils.Load(GameState.currentWorld, GameState.currentSubWorld, GameState.currentLevel);
            var words = CUtils.BuildListFromString<string>(gameLevel.answers);
            for (int i = 0; i < words.Count - keep; i++)
            {
                WordRegion.instance.CheckAnswer(words[i].ToUpper());
                yield return new WaitForSecondsRealtime(0.5f);
            }
            var report = new System.Text.StringBuilder("[DebugOpener] letters:");
            Canvas canvas = Pan.instance.centerPoint.GetComponentInParent<Canvas>().rootCanvas;
            Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            foreach (UnityEngine.UI.Text text in Pan.instance.centerPoint.GetComponentsInChildren<UnityEngine.UI.Text>())
            {
                Vector2 point = RectTransformUtility.WorldToScreenPoint(camera, text.transform.position);
                report.Append(' ').Append(text.text).Append('@').Append(Mathf.RoundToInt(point.x)).Append(',').Append(Mathf.RoundToInt(Screen.height - point.y));
            }
            Debug.Log(report.ToString());
            Debug.Log("[DebugOpener] remaining: " + string.Join("|", words.GetRange(words.Count - keep, keep)) + " word=" + gameLevel.word + " screen=" + Screen.width + "x" + Screen.height);
        }

        private IEnumerator GrantNoAdsAfter(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            Purchaser.instance.GrantNoAds();
            Debug.Log("[DebugOpener] noads granted buy_item=" + CUtils.IsBuyItem() + " remove_ads=" + CUtils.IsAdsRemoved());
        }

        private void ClearNoAds()
        {
            CPlayerPrefs.SetBool("buy_item", false);
            CUtils.SetRemoveAds(false);
            CPlayerPrefs.Save();
            Debug.Log("[DebugOpener] noads cleared");
        }

        private IEnumerator WaitScene(string sceneName)
        {
            while (SceneManager.GetActiveScene().name != sceneName || DialogController.instance == null) yield return null;
            // Để Start của controller trong scene chạy xong trước khi mở thứ khác.
            yield return new WaitForSecondsRealtime(0.6f);
        }

        private int[] ParseLevel(string text)
        {
            string[] pieces = text.Trim().Split('-');
            int[] result = new int[pieces.Length];
            for (int i = 0; i < pieces.Length; i++) result[i] = int.Parse(pieces[i]);
            return result;
        }

        private void SetCurrentLevel(int[] level)
        {
            GameState.currentWorld = level[0];
            GameState.currentSubWorld = level.Length > 1 ? level[1] : 0;
            GameState.currentLevel = level.Length > 2 ? level[2] : 0;
        }

        private void ApplyRuby(int value)
        {
            if (!PlayerPrefs.HasKey(RubyBackupKey)) PlayerPrefs.SetInt(RubyBackupKey, CurrencyController.GetBalance());
            CurrencyController.SetBalance(value);
            if (CurrencyController.onBalanceChanged != null) CurrencyController.onBalanceChanged();
        }

        private void ApplyUnlock(int[] unlock)
        {
            if (!PlayerPrefs.HasKey(UnlockBackupKey))
                PlayerPrefs.SetString(UnlockBackupKey, Prefs.unlockedWorld + "-" + Prefs.unlockedSubWorld + "-" + Prefs.unlockedLevel);
            Prefs.unlockedWorld = unlock[0];
            Prefs.unlockedSubWorld = unlock.Length > 1 ? unlock[1] : 0;
            Prefs.unlockedLevel = unlock.Length > 2 ? unlock[2] : 0;
        }

        private void RestoreUnlock()
        {
            if (!PlayerPrefs.HasKey(UnlockBackupKey)) return;
            ApplyUnlock(ParseLevel(PlayerPrefs.GetString(UnlockBackupKey)));
            PlayerPrefs.DeleteKey(UnlockBackupKey);
        }

        private void RestoreRuby()
        {
            if (!PlayerPrefs.HasKey(RubyBackupKey)) return;
            CurrencyController.SetBalance(PlayerPrefs.GetInt(RubyBackupKey));
            PlayerPrefs.DeleteKey(RubyBackupKey);
            if (CurrencyController.onBalanceChanged != null) CurrencyController.onBalanceChanged();
        }

        private void ApplyDaily(string state)
        {
            if (!PlayerPrefs.HasKey(DailyBackupKey))
            {
                var saved = new string[DailyKeys.Length];
                for (int i = 0; i < DailyKeys.Length; i++) saved[i] = CPlayerPrefs.GetString(DailyKeys[i], "");
                PlayerPrefs.SetString(DailyBackupKey, string.Join("|", saved));
            }
            var daily = new DailyChallenge();
            string today = daily.Today();
            int[] offsets = new int[0];
            int best = 0;
            if (state == "risk") { offsets = new[] { -1, -2, -3 }; best = 3; }
            else if (state == "safe") { offsets = new[] { 0, -1, -2, -3 }; best = 6; }
            else if (state == "two") { offsets = new[] { -1 }; best = 1; }
            else if (state == "lost") { offsets = new[] { -5, -6 }; best = 6; }
            var days = new string[offsets.Length];
            for (int i = 0; i < offsets.Length; i++) days[i] = daily.AddDays(today, offsets[i]);
            CPlayerPrefs.SetString("daily_solved", string.Join(",", days));
            CPlayerPrefs.SetString("daily_milestones", "");
            CPlayerPrefs.SetInt("daily_best", best);
            CPlayerPrefs.Save();
        }

        private void RestoreDaily()
        {
            if (!PlayerPrefs.HasKey(DailyBackupKey)) return;
            string[] saved = PlayerPrefs.GetString(DailyBackupKey).Split('|');
            CPlayerPrefs.SetString("daily_solved", saved[0]);
            CPlayerPrefs.SetString("daily_milestones", saved[1]);
            CPlayerPrefs.SetInt("daily_best", saved[2].Length == 0 ? 0 : int.Parse(saved[2]));
            CPlayerPrefs.SetString("daily_last_seen", saved[3]);
            PlayerPrefs.DeleteKey(DailyBackupKey);
            CPlayerPrefs.Save();
        }

        private IEnumerator OpenDailyInMain(string name, string arg)
        {
            ApplyDaily(arg);
            int savedRuby = CurrencyController.GetBalance();
            var daily = new DailyChallenge();
            int world, subWorld, level;
            GameState.dailyDate = daily.Today();
            daily.PickLevel(GameState.dailyDate, out world, out subWorld, out level);
            SetCurrentLevel(new[] { world, subWorld, level });
            CUtils.LoadScene(3);
            yield return WaitScene("Main");
            if (name == "dailywin")
            {
                DialogController.instance.ShowDialog(DialogType.Win);
                yield return new WaitForSecondsRealtime(1f);
                CurrencyController.SetBalance(savedRuby);
                RestoreDaily();
            }
        }

        // Dialog trong Main: vào level đang mở khoá (hoặc level=w-s-l) rồi mở dialog tại đó.
        private IEnumerator OpenInMain(string name, string arg, int[] level)
        {
            int savedWorld = Prefs.unlockedWorld, savedSub = Prefs.unlockedSubWorld, savedLevel = Prefs.unlockedLevel;
            int savedExtraProgress = Prefs.extraProgress, savedExtraTarget = Prefs.extraTarget;
            int savedRuby = CurrencyController.GetBalance();

            SetCurrentLevel(level != null ? level : new[] { savedWorld, savedSub, savedLevel });
            // Win lần đầu: level vừa thắng chính là level mở khoá mới nhất. Win replay: level đã qua từ trước.
            if (name == "win" && arg == "replay")
            {
                GameState.unlockedWorld = GameState.currentWorld;
                GameState.unlockedSubWord = GameState.currentSubWorld;
                GameState.unlockedLevel = GameState.currentLevel + 1;
            }
            else if (name == "win")
            {
                GameState.unlockedWorld = GameState.currentWorld;
                GameState.unlockedSubWord = GameState.currentSubWorld;
                GameState.unlockedLevel = GameState.currentLevel;
            }
            if (name == "extra")
            {
                Prefs.extraTarget = 5;
                Prefs.extraProgress = arg == "notready" ? 2 : 5;
            }

            CUtils.LoadScene(3);
            yield return WaitScene("Main");
            // Level đầu tiên tự mở How to play sau 0.5s; chờ nó rồi mới thay thế.
            yield return new WaitForSecondsRealtime(0.6f);

            DialogType type = DialogType.Pause;
            if (name == "howto") type = DialogType.HowtoPlay;
            else if (name == "extra") type = DialogType.ExtraWord;
            else if (name == "win") type = DialogType.Win;
            else if (name == "lowruby") type = DialogType.LowRuby;
            DialogController.instance.ShowDialog(type);

            // Win/Extra ghi tiến độ vào prefs khi mở; trả lại sau khi dialog đã đọc xong dữ liệu.
            yield return new WaitForSecondsRealtime(1f);
            Prefs.unlockedWorld = savedWorld;
            Prefs.unlockedSubWorld = savedSub;
            Prefs.unlockedLevel = savedLevel;
            Prefs.extraProgress = savedExtraProgress;
            Prefs.extraTarget = savedExtraTarget;
            if (name == "win") CurrencyController.SetBalance(savedRuby);
            CPlayerPrefs.Save();
        }

        private IEnumerator OpenRewarded(string arg)
        {
            RewardedVideoState state = (RewardedVideoState)Enum.Parse(typeof(RewardedVideoState), arg, true);
            var dialog = (RewardedVideoDialog)DialogController.instance.GetDialog(DialogType.RewardedVideo);
            dialog.SetAmount(2);
            DialogController.instance.ShowDialog(dialog);
            yield return new WaitForSecondsRealtime(0.5f);
            // Dialog tự đánh giá lại mỗi giây theo AdMob; dừng để giữ nguyên trạng thái cần xem.
            dialog.CancelInvoke("Evaluate");
            dialog.SetState(state);
        }
    }
}
#endif
