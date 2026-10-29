using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace WordCross.Editor
{
    // Chuyển UnityEngine.UI.Text thành TextMeshProUGUI trên prefab / scene, giữ nguyên mọi tham chiếu tới component cũ.
    // Cách làm: đổi component bằng API Unity vào một file tạm, rồi ghép khối TMP vào đúng file gốc với CHÍNH fileID của Text cũ,
    // nên mọi tham chiếu (field script, Button.targetGraphic, UnityEvent persistent call, ...) giữ nguyên và diff git chỉ còn các khối Text.
    // Sau đó chỉ cần đổi kiểu field Text -> TMP_Text trong script.
    // Hàm static chỉ là điểm vào cho Unity (menu / -executeMethod), phần việc nằm ở hàm instance.
    public class TextToTmpConverter
    {
        private const string TextGuid = "f70555f144d8491a825f0804e09c671c";
        private const long TextFileId = 708705254;
        private const string TmpGuid = "f4688fdb7df04437aeb418b961361dc5";
        private const string DefaultFont = "Assets/WordCross/Fonts/Baloo2/static/Baloo2-Bold SDF.asset";
        private const string ShadowFileId = "1573420865";
        private const string OutlineFileId = "-900027084";
        private const string MaterialFolder = "Assets/WordCross/Fonts/Materials";

        private static readonly Regex BlockRegex = new Regex(@"--- !u!(\d+) &(-?\d+)[^\n]*\n.*?(?=\n--- !u!|\z)", RegexOptions.Singleline);
        private static readonly Regex GameObjectRegex = new Regex(@"m_GameObject: \{fileID: (-?\d+)\}");
        private static readonly Regex ScriptRegex = new Regex(@"m_Script: \{fileID: (-?\d+), guid: ([0-9a-f]+)");

        private TMP_FontAsset fontAsset;
        private int converted;
        private int skipped;

        // -executeMethod WordCross.Editor.TextToTmpConverter.ConvertFromArgs -tmpAssets "a.prefab;b.unity" [-tmpFont "path SDF.asset"]
        public static void ConvertFromArgs()
        {
            var args = Environment.GetCommandLineArgs();
            string assets = null;
            string font = DefaultFont;
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "-tmpAssets") assets = args[i + 1];
                if (args[i] == "-tmpFont") font = args[i + 1];
            }
            new TextToTmpConverter().ConvertAssets(assets.Split(';'), font);
        }

        [MenuItem("WordCross/Fonts/Convert Selected Text To TMP")]
        public static void ConvertSelected()
        {
            var paths = new List<string>();
            foreach (var guid in Selection.assetGUIDs) paths.Add(AssetDatabase.GUIDToAssetPath(guid));
            new TextToTmpConverter().ConvertAssets(paths.ToArray(), DefaultFont);
        }

        public void ConvertAssets(string[] paths, string fontPath)
        {
            fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
            foreach (var path in paths)
            {
                if (path.EndsWith(".prefab")) ConvertPrefab(path);
                else if (path.EndsWith(".unity")) ConvertScene(path);
            }
            Debug.Log("[TMPCONV] xong: chuyển " + converted + ", bỏ qua " + skipped);
        }

        private void ConvertPrefab(string path)
        {
            var original = File.ReadAllText(path);
            var root = PrefabUtility.LoadPrefabContents(path);
            ConvertTree(root, path);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
            SpliceIntoOriginal(path, original);
        }

        private void ConvertScene(string path)
        {
            var original = File.ReadAllText(path);
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects()) ConvertTree(root, path);
            EditorSceneManager.SaveScene(scene);
            SpliceIntoOriginal(path, original);
        }

        private void ConvertTree(GameObject root, string path)
        {
            foreach (var text in root.GetComponentsInChildren<Text>(true))
            {
                if (PrefabUtility.GetCorrespondingObjectFromSource(text) != null)
                {
                    Debug.LogWarning("[TMPCONV] bỏ qua (nằm trong prefab lồng, chuyển prefab nguồn trước): " + path + " :: " + text.name);
                    skipped++;
                    continue;
                }
                Convert(text);
                converted++;
            }
        }

        private void Convert(Text text)
        {
            var go = text.gameObject;
            var shadow = go.GetComponent<Shadow>();
            var outline = go.GetComponent<Outline>();
            var data = new TextData(text);

            UnityEngine.Object.DestroyImmediate(text);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.font = fontAsset;
            tmp.text = data.text;
            tmp.color = data.color;
            tmp.raycastTarget = data.raycastTarget;
            tmp.maskable = data.maskable;
            tmp.richText = data.richText;
            tmp.fontSize = data.fontSize;
            tmp.fontStyle = data.fontStyle;
            tmp.alignment = data.alignment;
            tmp.lineSpacing = (data.lineSpacing - 1f) * 100f;
            tmp.textWrappingMode = data.horizontalWrap ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
            tmp.overflowMode = data.verticalTruncate ? TextOverflowModes.Truncate : TextOverflowModes.Overflow;
            tmp.enableAutoSizing = data.bestFit;
            tmp.fontSizeMin = data.minSize;
            tmp.fontSizeMax = data.maxSize;
            tmp.enabled = data.enabled;

            var preset = ResolveMaterial(data.fontSize, shadow, outline);
            if (preset != null) tmp.fontSharedMaterial = preset;
            if (outline != null) UnityEngine.Object.DestroyImmediate(outline);
            if (shadow != null) UnityEngine.Object.DestroyImmediate(shadow);
        }

        // Shadow -> Underlay, Outline -> Outline của TMP, dùng chung một Material Preset (không tạo instance riêng cho từng text).
        // Hệ số quy đổi từ pixel sang đơn vị shader là ước lượng, cần chỉnh bằng mắt ở giai đoạn 2.
        private Material ResolveMaterial(float fontSize, Shadow shadow, Outline outline)
        {
            if (shadow == null && outline == null) return null;
            if (!Directory.Exists(MaterialFolder)) AssetDatabase.CreateFolder("Assets/WordCross/Fonts", "Materials");
            var material = new Material(fontAsset.material);
            var key = fontAsset.name;
            if (outline != null)
            {
                var width = Mathf.Clamp(Mathf.Abs(outline.effectDistance.x) / fontSize * 1.6f, 0.05f, 0.4f);
                material.EnableKeyword("OUTLINE_ON");
                material.SetColor("_OutlineColor", outline.effectColor);
                material.SetFloat("_OutlineWidth", width);
                key += "_Outline_" + ColorUtility.ToHtmlStringRGBA(outline.effectColor) + "_" + width.ToString("0.00");
            }
            if (shadow != null)
            {
                var offset = shadow.effectDistance / fontSize * 1.5f;
                material.EnableKeyword("UNDERLAY_ON");
                material.SetColor("_UnderlayColor", shadow.effectColor);
                material.SetFloat("_UnderlayOffsetX", offset.x);
                material.SetFloat("_UnderlayOffsetY", offset.y);
                material.SetFloat("_UnderlaySoftness", 0f);
                key += "_Shadow_" + ColorUtility.ToHtmlStringRGBA(shadow.effectColor) + "_" + offset.x.ToString("0.00") + "_" + offset.y.ToString("0.00");
            }
            var path = MaterialFolder + "/" + key + ".mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
            {
                UnityEngine.Object.DestroyImmediate(material);
                return existing;
            }
            material.name = key;
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        // Ghép khối TMP (do Unity vừa lưu) vào file gốc: TMP lấy lại fileID và phần header của khối Text cũ,
        // khối Shadow/Outline cùng GameObject bị gỡ. Phần còn lại của file gốc giữ nguyên từng byte.
        private void SpliceIntoOriginal(string path, string original)
        {
            var originalBlocks = ParseBlocks(original);
            var savedBlocks = ParseBlocks(File.ReadAllText(path));

            var textByGameObject = ReadComponents(originalBlocks, TextGuid, TextFileId);
            var tmpByGameObject = ReadComponents(savedBlocks, TmpGuid, 11500000);
            var savedById = new Dictionary<string, Block>();
            foreach (var block in savedBlocks) savedById[block.id] = block;

            var removedIds = new HashSet<string>();
            var gameObjectIds = new Dictionary<string, string>();
            var replaced = 0;
            foreach (var pair in textByGameObject)
            {
                string tmpId;
                if (!tmpByGameObject.TryGetValue(pair.Key, out tmpId)) continue;
                var oldBlock = originalBlocks.Find(b => b.id == pair.Value);
                var tmpText = savedById[tmpId].text;
                oldBlock.text = "--- !u!114 &" + oldBlock.id + "\nMonoBehaviour:\n" + HeaderOf(oldBlock.text) + BodyOf(tmpText);
                replaced++;
                gameObjectIds[pair.Key] = pair.Value;
            }

            // gỡ Shadow/Outline trên các GameObject vừa chuyển
            foreach (var block in originalBlocks)
            {
                if (block.cls != "114") continue;
                var script = ScriptRegex.Match(block.text);
                var owner = GameObjectRegex.Match(block.text);
                if (!script.Success || !owner.Success || !gameObjectIds.ContainsKey(owner.Groups[1].Value)) continue;
                if (script.Groups[2].Value != TextGuid) continue;
                if (script.Groups[1].Value == ShadowFileId || script.Groups[1].Value == OutlineFileId) removedIds.Add(block.id);
            }
            foreach (var block in originalBlocks)
            {
                if (block.cls != "1") continue;
                foreach (var id in removedIds) block.text = block.text.Replace("  - component: {fileID: " + id + "}\n", "");
            }
            originalBlocks.RemoveAll(b => removedIds.Contains(b.id));

            var builder = new System.Text.StringBuilder(original.Substring(0, original.IndexOf("--- !u!")));
            for (var i = 0; i < originalBlocks.Count; i++)
            {
                builder.Append(originalBlocks[i].text);
                if (i < originalBlocks.Count - 1) builder.Append('\n');
            }
            File.WriteAllText(path, builder.ToString());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Debug.Log("[TMPCONV] " + path + ": ghép " + replaced + " khối TMP, gỡ " + removedIds.Count + " Shadow/Outline");
        }

        // Phần từ sau dòng "MonoBehaviour:" đến trước "m_GameObject:" (hideFlags, thông tin prefab kiểu cũ/mới của file gốc).
        private string HeaderOf(string blockText)
        {
            var start = blockText.IndexOf("MonoBehaviour:\n") + "MonoBehaviour:\n".Length;
            var end = blockText.IndexOf("  m_GameObject:");
            return blockText.Substring(start, end - start);
        }

        // Từ "m_GameObject:" đến hết khối.
        private string BodyOf(string blockText)
        {
            return blockText.Substring(blockText.IndexOf("  m_GameObject:"));
        }

        private class Block
        {
            public string cls;
            public string id;
            public string text;
        }

        private List<Block> ParseBlocks(string yaml)
        {
            var blocks = new List<Block>();
            foreach (Match match in BlockRegex.Matches(yaml))
                blocks.Add(new Block { cls = match.Groups[1].Value, id = match.Groups[2].Value, text = match.Value });
            return blocks;
        }

        // Trả về: fileID GameObject -> fileID component (script có guid/fileID cho trước).
        private Dictionary<string, string> ReadComponents(List<Block> blocks, string guid, long fileId)
        {
            var map = new Dictionary<string, string>();
            foreach (var block in blocks)
            {
                if (block.cls != "114") continue;
                var script = ScriptRegex.Match(block.text);
                if (!script.Success || script.Groups[2].Value != guid || script.Groups[1].Value != fileId.ToString()) continue;
                var owner = GameObjectRegex.Match(block.text);
                if (owner.Success) map[owner.Groups[1].Value] = block.id;
            }
            return map;
        }

        private class TextData
        {
            public string text;
            public Color color;
            public bool raycastTarget;
            public bool maskable;
            public bool richText;
            public float fontSize;
            public FontStyles fontStyle;
            public TextAlignmentOptions alignment;
            public float lineSpacing;
            public bool horizontalWrap;
            public bool verticalTruncate;
            public bool bestFit;
            public float minSize;
            public float maxSize;
            public bool enabled;

            public TextData(Text source)
            {
                text = source.text;
                color = source.color;
                raycastTarget = source.raycastTarget;
                maskable = source.maskable;
                richText = source.supportRichText;
                fontSize = source.fontSize;
                lineSpacing = source.lineSpacing;
                horizontalWrap = source.horizontalOverflow == HorizontalWrapMode.Wrap;
                verticalTruncate = source.verticalOverflow == VerticalWrapMode.Truncate;
                bestFit = source.resizeTextForBestFit;
                minSize = source.resizeTextMinSize;
                maxSize = source.resizeTextMaxSize;
                enabled = source.enabled;
                fontStyle = source.fontStyle == FontStyle.Bold ? FontStyles.Bold
                    : source.fontStyle == FontStyle.Italic ? FontStyles.Italic
                    : source.fontStyle == FontStyle.BoldAndItalic ? FontStyles.Bold | FontStyles.Italic
                    : FontStyles.Normal;
                alignment = MapAlignment(source.alignment);
            }

            private TextAlignmentOptions MapAlignment(TextAnchor anchor)
            {
                switch (anchor)
                {
                    case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
                    case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
                    case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
                    case TextAnchor.MiddleLeft: return TextAlignmentOptions.Left;
                    case TextAnchor.MiddleCenter: return TextAlignmentOptions.Center;
                    case TextAnchor.MiddleRight: return TextAlignmentOptions.Right;
                    case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
                    case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
                    default: return TextAlignmentOptions.BottomRight;
                }
            }
        }
    }
}
