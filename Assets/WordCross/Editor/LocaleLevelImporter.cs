using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Util.Editor;

namespace WordCross.Editor
{
    // Tạo/cập nhật asset GameLevel cho vi/fr/es/pt từ design/GD/data/<mã>.json (534 level mỗi mã) và đưa vào AddressableConfig.
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
            var created = 0;
            var updated = 0;
            var wanted = new List<string>();
            AssetDatabase.StartAssetEditing();
            foreach (var locale in Locales)
            {
                var file = JsonUtility.FromJson<LocaleFile>(File.ReadAllText(Path.Combine(projectRoot, DataFolder, locale + ".json"), Encoding.UTF8));
                foreach (var row in file.levels)
                {
                    var folder = LevelRoot + "/" + locale + "/World_" + (row.world - 1) + "/SubWorld_" + (row.subworld - 1);
                    var path = folder + "/Level_" + (row.level - 1) + ".asset";
                    wanted.Add(path);
                    var asset = AssetDatabase.LoadAssetAtPath<GameLevel>(path);
                    var isNew = asset == null;
                    if (isNew)
                    {
                        Directory.CreateDirectory(Path.Combine(projectRoot, folder));
                        asset = ScriptableObject.CreateInstance<GameLevel>();
                    }

                    // Cùng định dạng level en: answers kết thúc bằng '|', validWords gồm cả answers.
                    var answers = string.Join("|", row.answers) + "|";
                    var validWords = string.Join("|", row.validWords);
                    if (!isNew && asset.word == row.word && asset.answers == answers && asset.validWords == validWords) continue;
                    asset.word = row.word;
                    asset.answers = answers;
                    asset.validWords = validWords;
                    if (isNew)
                    {
                        AssetDatabase.CreateAsset(asset, path);
                        created++;
                    }
                    else
                    {
                        EditorUtility.SetDirty(asset);
                        updated++;
                    }
                }
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var registered = RegisterInConfig(config, wanted);
            Debug.Log("[LocaleLevelImporter] created=" + created + " updated=" + updated + " total=" + wanted.Count + " newConfigEntries=" + registered);
        }

        // Giữ entry của level en, thay toàn bộ entry level theo mã bằng danh sách mới (address = đường dẫn đầy đủ như level en).
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
