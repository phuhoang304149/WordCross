using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace WordCross.Editor
{
    // Dựng lại các SDF font (Baloo 2, CookieRun) với đủ ký tự Latin/Việt, đặt chuỗi fallback và TMP Settings.
    // Hàm static chỉ là điểm vào cho Unity (menu / -executeMethod), phần việc nằm ở hàm instance.
    public class TmpFontBuilder
    {
        private const string FontRoot = "Assets/WordCross/Fonts";
        private const string EssentialsPackage = "Packages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage";
        private const string SettingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";
        private const string LegacyFontPath = "Assets/_Sprites/RockoUltraFLF.ttf";
        private const string LegacyAssetPath = FontRoot + "/Legacy/RockoUltraFLF SDF.asset";
        private const int SamplingSize = 48;
        private const int Padding = 5;
        private const int AtlasSize = 1024;

        private static readonly string[][] Sources =
        {
            // { ttf, sdf asset, ttf dự phòng cùng độ đậm (rỗng nếu là font cuối chuỗi) }
            new[] { "Baloo2/static/Baloo2-Regular", "CookieRun/CookieRun Regular" },
            new[] { "Baloo2/static/Baloo2-Medium", "CookieRun/CookieRun Regular" },
            new[] { "Baloo2/static/Baloo2-SemiBold", "CookieRun/CookieRun Bold" },
            new[] { "Baloo2/static/Baloo2-Bold", "CookieRun/CookieRun Bold" },
            new[] { "Baloo2/static/Baloo2-ExtraBold", "CookieRun/CookieRun Black" },
            new[] { "CookieRun/CookieRun Regular", "" },
            new[] { "CookieRun/CookieRun Bold", "" },
            new[] { "CookieRun/CookieRun Black", "" },
        };

        [MenuItem("WordCross/Fonts/1 Import TMP Essentials")]
        public static void ImportEssentials()
        {
            new TmpFontBuilder().DoImportEssentials();
        }

        [MenuItem("WordCross/Fonts/2 Build SDF Assets")]
        public static void BuildFonts()
        {
            new TmpFontBuilder().DoBuildFonts();
        }

        [MenuItem("WordCross/Fonts/3 Configure TMP Settings")]
        public static void ConfigureSettings()
        {
            new TmpFontBuilder().DoConfigureSettings();
        }

        private void DoImportEssentials()
        {
            if (File.Exists(SettingsPath))
            {
                Debug.Log("[TMPFONT] TMP Essential Resources đã có");
                return;
            }
            AssetDatabase.ImportPackage(EssentialsPackage, false);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[TMPFONT] đã import TMP Essential Resources: " + File.Exists(SettingsPath));
        }

        // Ký tự cần có: ASCII, Latin-1, Latin Extended dùng cho Việt/Pháp, toàn bộ Latin Extended Additional tiếng Việt, dấu câu, ký hiệu UI.
        private string BuildCharacterSet()
        {
            var sb = new StringBuilder();
            AppendRange(sb, 0x20, 0x7E);
            AppendRange(sb, 0xA0, 0xFF);
            foreach (var c in "ĂăĐđĨĩŨũƠơƯưŒœŸ") sb.Append(c);
            AppendRange(sb, 0x1EA0, 0x1EF9);
            foreach (var c in "​–—‘’“”•…€←→★✓") sb.Append(c);
            return sb.ToString();
        }

        private void AppendRange(StringBuilder sb, int from, int to)
        {
            for (var i = from; i <= to; i++) sb.Append((char)i);
        }

        private void DoBuildFonts()
        {
            var characters = BuildCharacterSet();
            Debug.Log("[TMPFONT] số ký tự yêu cầu: " + characters.Length);

            // font cũ của game (RockoUltra) làm fallback cuối: Dynamic để không phải liệt kê ký tự
            if (!Directory.Exists(FontRoot + "/Legacy")) AssetDatabase.CreateFolder(FontRoot, "Legacy");
            var legacy = BuildAsset(LegacyFontPath, LegacyAssetPath, characters, AtlasPopulationMode.Dynamic);

            foreach (var source in Sources)
            {
                var sdfPath = FontRoot + "/" + source[0] + " SDF.asset";
                BuildAsset(FontRoot + "/" + source[0] + ".ttf", sdfPath, characters, AtlasPopulationMode.Static);
            }

            // chuỗi fallback: Baloo2 -> CookieRun cùng độ đậm -> font cũ; CookieRun thiếu glyph Việt nên rơi về fallback toàn cục (Baloo2 Bold)
            foreach (var source in Sources)
            {
                var asset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontRoot + "/" + source[0] + " SDF.asset");
                var next = source[1] != ""
                    ? AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontRoot + "/" + source[1] + " SDF.asset")
                    : legacy;
                asset.fallbackFontAssetTable = new List<TMP_FontAsset> { next };
                EditorUtility.SetDirty(asset);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // Dựng một SDF asset từ ttf, thay file cũ (giữ nguyên meta/GUID nếu file đã có).
        private TMP_FontAsset BuildAsset(string fontPath, string assetPath, string characters, AtlasPopulationMode finalMode)
        {
            var font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            var asset = TMP_FontAsset.CreateFontAsset(font, SamplingSize, Padding, GlyphRenderMode.SDFAA, AtlasSize, AtlasSize, AtlasPopulationMode.Dynamic, true);
            asset.name = Path.GetFileNameWithoutExtension(assetPath);
            string missing;
            asset.TryAddCharacters(characters, out missing, false);
            asset.atlasPopulationMode = finalMode;
            asset.material.name = asset.name + " Material";
            for (var i = 0; i < asset.atlasTextures.Length; i++)
                asset.atlasTextures[i].name = asset.name + " Atlas" + (i == 0 ? "" : " " + i);

            var oldGuid = AssetDatabase.AssetPathToGUID(assetPath);
            if (File.Exists(assetPath)) AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.AddObjectToAsset(asset.material, asset);
            foreach (var texture in asset.atlasTextures) AssetDatabase.AddObjectToAsset(texture, asset);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();

            Debug.Log("[TMPFONT] " + asset.name + " mode=" + finalMode + " glyphs=" + asset.characterTable.Count
                + " atlas=" + asset.atlasTextures.Length + " thiếu(" + missing.Length + ")=" + Escape(missing)
                + " guidCũ=" + oldGuid + " guidMới=" + AssetDatabase.AssetPathToGUID(assetPath));
            return asset;
        }

        private string Escape(string text)
        {
            var sb = new StringBuilder();
            foreach (var c in text) sb.Append(c).Append(' ');
            return sb.ToString();
        }

        private void DoConfigureSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(SettingsPath);
            var serialized = new SerializedObject(settings);
            serialized.FindProperty("m_defaultFontAsset").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontRoot + "/Baloo2/static/Baloo2-Bold SDF.asset");
            var fallbacks = serialized.FindProperty("m_fallbackFontAssets");
            fallbacks.ClearArray();
            fallbacks.InsertArrayElementAtIndex(0);
            fallbacks.GetArrayElementAtIndex(0).objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontRoot + "/Baloo2/static/Baloo2-Bold SDF.asset");
            serialized.FindProperty("m_warningsDisabled").boolValue = false;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            Debug.Log("[TMPFONT] TMP Settings: default=" + TMP_Settings.defaultFontAsset.name);
        }
    }
}
