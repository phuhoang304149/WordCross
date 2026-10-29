using System.IO;
using CatGaming.Clr;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace WordCross.Gameplay
{
    public sealed class WordCrossGameEntry : IClrEntry
    {
        private const string FirstScene = "Assets/_Scenes/Home.unity";

        public void Initialize(GameObject host)
        {
            LogBootstrapVersions();
#if GAME_DEBUG || DEVELOPMENT_BUILD
            new GameObject("DebugOpener").AddComponent<DebugOpener>();
#endif
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("[Scene] load " + FirstScene + " t=" + Time.realtimeSinceStartup.ToString("F2"));
            Addressables.LoadSceneAsync(FirstScene, LoadSceneMode.Single).Completed += handle =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded)
                    Debug.LogError("[WordCross] Home scene load failed from Addressables: " + handle.OperationException, host);
            };
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("[Scene] loaded " + scene.name + " t=" + Time.realtimeSinceStartup.ToString("F2"));
        }

        // Hash nội dung (không phải bí mật) để QA biết bản CLR / catalog đang chạy; ClrBootstrap nằm trong submodule nên đọc từ cache ở phía game.
        private void LogBootstrapVersions()
        {
            var clrRoot = Path.Combine(Application.persistentDataPath, "clr");
            var clrHash = "none (no verified cache, built-in)";
            if (Directory.Exists(clrRoot))
            {
                var newest = System.DateTime.MinValue;
                foreach (var marker in Directory.GetFiles(clrRoot, "last-verified.hash", SearchOption.AllDirectories))
                {
                    var written = File.GetLastWriteTimeUtc(marker);
                    if (written < newest) continue;
                    newest = written;
                    clrHash = File.ReadAllText(marker).Trim();
                }
            }
            Debug.Log("[Bootstrap] clr hash=" + clrHash + " appVersion=" + Application.version);

            var catalogHash = "none (no remote catalog cache)";
            var catalogRoot = Path.Combine(Application.persistentDataPath, "com.unity.addressables");
            if (Directory.Exists(catalogRoot))
            {
                var hashFiles = Directory.GetFiles(catalogRoot, "*.hash", SearchOption.TopDirectoryOnly);
                if (hashFiles.Length > 0)
                    catalogHash = Path.GetFileName(hashFiles[0]) + ":" + File.ReadAllText(hashFiles[0]).Trim();
            }
            Debug.Log("[Bootstrap] catalog hash=" + catalogHash);
            foreach (var locator in Addressables.ResourceLocators)
                Debug.Log("[Bootstrap] catalog locator=" + locator.LocatorId);
        }
    }
}
