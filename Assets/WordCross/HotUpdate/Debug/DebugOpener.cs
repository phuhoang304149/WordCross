#if GAME_DEBUG || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WordCross.Gameplay
{
    // Công cụ dev (tools/ui-preview): mở thẳng một màn/dialog lúc khởi động để chụp ảnh, không cần tap.
    // Nguồn lệnh: intent extra "open" (adb shell am start --es open win) hoặc file <persistentDataPath>/debug_open.txt (đọc xong xoá).
    // Cú pháp: <màn>[:<tham số>][;ruby=N][;level=w-s-l]
    //   Hậu tố ;extra=N đặt tiến độ từ thưởng (lưu thật, như ruby).
    //   home | selectworld | selectlevel[:w-s] | main:w-s-l
    //   pause | win[:first|replay] | extra[:ready|notready] | howto   (mở trong Main, mặc định level đang mở khoá)
    //   shop | rate | language | rewarded:<ready|loading|cooldown|nofill|offline|playing|success|cancel|error> (mở trong Home)
    //   reset (trả số ruby về giá trị trước khi ghi đè)
    public sealed class DebugOpener : MonoBehaviour
    {
        private const string FileName = "debug_open.txt";
        private const string RubyBackupKey = "debug_ruby_backup";

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
            for (int i = 1; i < parts.Length; i++)
            {
                string[] pair = parts[i].Split(new[] { '=' }, 2);
                if (pair.Length < 2) continue;
                string key = pair[0].Trim().ToLowerInvariant();
                if (key == "ruby") ApplyRuby(int.Parse(pair[1]));
                else if (key == "level") level = ParseLevel(pair[1]);
                else if (key == "extra") Prefs.extraProgress = int.Parse(pair[1]);
            }

            switch (name)
            {
                case "reset":
                    RestoreRuby();
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
                    break;
                case "pause":
                case "howto":
                case "extra":
                case "win":
                    yield return OpenInMain(name, arg, level);
                    break;
                case "shop":
                    DialogController.instance.ShowDialog(DialogType.Shop);
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
                case "home":
                    break;
                default:
                    Debug.LogError("[DebugOpener] unknown command: " + command);
                    break;
            }
            Debug.Log("[DebugOpener] opened " + command);
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

        private void RestoreRuby()
        {
            if (!PlayerPrefs.HasKey(RubyBackupKey)) return;
            CurrencyController.SetBalance(PlayerPrefs.GetInt(RubyBackupKey));
            PlayerPrefs.DeleteKey(RubyBackupKey);
            if (CurrencyController.onBalanceChanged != null) CurrencyController.onBalanceChanged();
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
