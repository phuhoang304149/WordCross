using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Util.Editor;

namespace WordCross.Editor
{
    // Tạo/cập nhật MỘT LocaleLevelPack cho mỗi vi/fr/es/pt từ design/GD/data/<mã>.json (534 level mỗi mã) và đưa vào AddressableConfig.
    // Hàm static chỉ là điểm vào cho Unity (-executeMethod), phần việc nằm ở hàm instance.
    public class LocaleLevelImporter
    {
        private const string DataFolder = "design/GD/data";
        private const string LevelRoot = "Assets/WordCross/Config/GameData/Levels";
        private const string ConfigPath = "Assets/WordCross/Config/Editor/AddressableConfig.asset";
        private static readonly string[] Locales = { "vi", "fr", "es", "pt" };

        [Serializable]
        private class LevelRow
        {
            public int world;
            public int subworld;
            public int level;
            public string word;
            public string[] answers;
            public string[] validWords;
        }

        [Serializable]
        private class LocaleFile
        {
            public string locale;
            public LevelRow[] levels;
        }

        public static void ImportAll()
        {
            new LocaleLevelImporter().Run();
        }

        private void Run()
        {
            var config = AssetDatabase.LoadAssetAtPath<AddressableConfig>(ConfigPath);
            if (config == null)
            {
                Debug.LogError("[LocaleLevelImporter] AddressableConfig not found: " + ConfigPath);
                EditorApplication.Exit(1);
                return;
            }

            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            var wanted = new List<string>();
            var differences = 0;
            var compared = 0;
            foreach (var locale in Locales)
            {
                var file = JsonUtility.FromJson<LocaleFile>(File.ReadAllText(Path.Combine(projectRoot, DataFolder, locale + ".json"), Encoding.UTF8));
                var path = LevelRoot + "/" + locale + "/Pack.asset";
                wanted.Add(path);
                var pack = AssetDatabase.LoadAssetAtPath<LocaleLevelPack>(path);
                var isNew = pack == null;
                if (isNew) pack = ScriptableObject.CreateInstance<LocaleLevelPack>();

                var entries = new LocaleLevelPack.Entry[file.levels.Length];
                for (var i = 0; i < entries.Length; i++)
                {
                    var row = file.levels[i];
                    // Cùng định dạng level en: answers kết thúc bằng '|', validWords gồm cả answers.
                    entries[i] = new LocaleLevelPack.Entry
                    {
                        world = row.world - 1,
                        subWorld = row.subworld - 1,
                        level = row.level - 1,
                        word = row.word,
                        answers = string.Join("|", row.answers) + "|",
                        validWords = string.Join("|", row.validWords)
                    };
                }
                pack.entries = entries;
                if (isNew)
                {
                    AssetDatabase.CreateAsset(pack, path);
                }
                else
                {
                    EditorUtility.SetDirty(pack);
                }

                // So với asset level riêng lẻ của T111 (nếu còn) trước khi xoá chúng: phải 0 khác biệt.
                foreach (var entry in entries)
                {
                    var oldPath = LevelRoot + "/" + locale + "/World_" + entry.world + "/SubWorld_" + entry.subWorld + "/Level_" + entry.level + ".asset";
                    var old = AssetDatabase.LoadAssetAtPath<GameLevel>(oldPath);
                    if (old == null) continue;
                    compared++;
                    if (old.word == entry.word && old.answers == entry.answers && old.validWords == entry.validWords) continue;
                    differences++;
                    Debug.LogError("[LocaleLevelImporter] DIFF " + oldPath);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var registered = RegisterInConfig(config, wanted);
            Debug.Log("[LocaleLevelImporter] packs=" + wanted.Count + " compared=" + compared + " differences=" + differences + " newConfigEntries=" + registered);
        }

        // Giữ entry của level en, thay toàn bộ entry level theo mã bằng đúng 4 pack (address = đường dẫn đầy đủ như level en).
        private int RegisterInConfig(AddressableConfig config, List<string> wanted)
        {
            var known = new HashSet<string>();
            config.scriptableObjects.RemoveAll(item => item != null && IsLocaleLevel(item.path) && !wanted.Contains(item.path));
            foreach (var item in config.scriptableObjects)
                if (item != null) known.Add(item.path);
            var added = 0;
            foreach (var path in wanted)
            {
                if (known.Contains(path)) continue;
                config.scriptableObjects.Add(new AddressableConfig.ScriptableObjectItem
                {
                    path = path,
                    asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path),
                    address = path,
                    label = ""
                });
                added++;
            }
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            return added;
        }

        private bool IsLocaleLevel(string path)
        {
            foreach (var locale in Locales)
                if (path != null && path.StartsWith(LevelRoot + "/" + locale + "/", StringComparison.Ordinal)) return true;
            return false;
        }
    }
}
