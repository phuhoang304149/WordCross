using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        // Text có hai kiểu tham chiếu script trong YAML: kiểu cũ (guid UnityEngine.UI.dll + fileID class) và kiểu Unity 6 (guid file .cs + 11500000).
        private const string TextGuid = "f70555f144d8491a825f0804e09c671c";
        private const long TextFileId = 708705254;
        private const string TextGuidOld = "f5f67c52d1564df4a8936ccd202a3bd8";
        private const string TextGuid6 = "5f7201a12d95ffc409449d95f23cf332";
        private const string ShadowGuid6 = "cfabb0440166ab443bba8876756fdfa9";
        private const string OutlineGuid6 = "e19747de3f5aca642ab2be37e372fb86";
        private const string FontFolder = "Assets/WordCross/Fonts/Baloo2/static/";
        // Cỡ chữ TMP = cỡ UGUI x hệ số này (hiệu chỉnh theo ảnh đo trên máy).
        private const float SizeScale = 1f;
        private const string TmpGuid = "f4688fdb7df04437aeb418b961361dc5";
        private const string DefaultFont = "Assets/WordCross/Fonts/Baloo2/static/Baloo2-Bold SDF.asset";
        private const string ShadowFileId = "1573420865";
        private const string OutlineFileId = "-900027084";
        private const string MaterialFolder = "Assets/WordCross/Fonts/Materials";

        // letter-spacing của design (theme.css, đơn vị em/100): .btn 0.02em (mọi nhãn nút), .logo h1 0.02em, .tagx 0.06em, .adbox 0.08em.
        private static readonly KeyValuePair<Regex, float>[] SpacingRules =
        {
            new KeyValuePair<Regex, float>(new Regex(@"Home\.unity\|Canvas/Buttons/(Play|Daily|VsBot|VsAI|Shop|Language|Rate)/(Content/)?Label$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"Home\.unity\|Canvas/Tray/Image/Text$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"WinDialog\.prefab\|.*/(DoubleReward|Next|Menu)/Text$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"PauseDialog\.prefab\|.*/(Continue|HowtoPlayButton|Menu|Quit|LanguageButton)/Text$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"RateDialog\.prefab\|.*/(Later|Yes)/Text$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"RewardedVideoDialog\.prefab\|.*/(Primary|Secondary)/Text$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"RewardedVideoDialog\.prefab\|.*/AdBox/Text$"), 8f),
            new KeyValuePair<Regex, float>(new Regex(@"LowRubyDialog\.prefab\|.*/(Watch|Shop|Skip)/Text$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"AiSoonDialog\.prefab\|.*/(Soon|Bot)/Text$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"AiSoonDialog\.prefab\|.*/Pill/Text$"), 6f),
            new KeyValuePair<Regex, float>(new Regex(@"HowToPlayDialog\.prefab\|.*/GotItButton/Text$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"ExtraWordDialog\.prefab\|.*/Claim/Text$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"DailyDialog\.prefab\|.*/PlayButton/Text$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"ShopDialog\.prefab\|.*/WatchBtn/Text$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"ShopDialog\.prefab\|.*/ShopItem[^/]*/Price$"), 2f),
            new KeyValuePair<Regex, float>(new Regex(@"ShopItem\.prefab\|ShopItem/Price$"), 2f),
        };

        private static readonly Regex BlockRegex = new Regex(@"--- !u!(\d+) &(-?\d+)[^\n]*\n.*?(?=\n--- !u!|\z)", RegexOptions.Singleline);
        private static readonly Regex GameObjectRegex = new Regex(@"m_GameObject: \{fileID: (-?\d+)\}");
        private static readonly Regex ScriptRegex = new Regex(@"m_Script: \{fileID: (-?\d+), guid: ([0-9a-f]+)");

        private static readonly Regex ModificationRegex = new Regex(
            @"    - target: \{fileID: (-?\d+), guid: ([0-9a-f]+),\s+type: 3\}\n      propertyPath: ([^\n]+)\n      value: ((?:.|\n)*?)\n      objectReference: ([^\n]*)\n");

        private readonly Dictionary<string, HashSet<string>> convertedByPrefab = new Dictionary<string, HashSet<string>>();
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

        // -executeMethod WordCross.Editor.TextToTmpConverter.ListFromArgs -tmpAssets "a.prefab;b.unity" -tmpOut file.txt
        public static void ListFromArgs()
        {
            var args = Environment.GetCommandLineArgs();
            string assets = null;
            string output = null;
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "-tmpAssets") assets = args[i + 1];
                if (args[i] == "-tmpOut") output = args[i + 1];
            }
            new TextToTmpConverter().ListTexts(assets.Split(';'), output);
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
            foreach (var path in paths) RewriteOverrides(path);
            Debug.Log("[TMPCONV] xong: chuyển " + converted + ", bỏ qua " + skipped);
        }

        public void ListTexts(string[] paths, string output)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var path in paths)
            {
                var roots = new List<GameObject>();
                Scene scene = default(Scene);
                if (path.EndsWith(".prefab")) roots.Add(PrefabUtility.LoadPrefabContents(path));
                else
                {
                    scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                    roots.AddRange(scene.GetRootGameObjects());
                }
                foreach (var root in roots)
                    foreach (var text in root.GetComponentsInChildren<Text>(true))
                    {
                        var names = new List<string>();
                        for (var t = text.transform; t != null; t = t.parent) names.Insert(0, t.name);
                        var button = text.GetComponentInParent<Button>(true);
                        sb.Append(path).Append('|').Append(string.Join("/", names)).Append("|font=").Append(text.font != null ? text.font.name : "null")
                            .Append("|size=").Append(text.fontSize).Append("|style=").Append(text.fontStyle).Append("|best=").Append(text.resizeTextForBestFit ? text.resizeTextMinSize + "-" + text.resizeTextMaxSize : "no")
                            .Append("|hw=").Append(text.horizontalOverflow).Append("|vw=").Append(text.verticalOverflow).Append("|al=").Append(text.alignment).Append("|ls=").Append(text.lineSpacing)
                            .Append("|btn=").Append(button != null ? button.name : "-").Append("|nested=").Append(PrefabUtility.GetCorrespondingObjectFromSource(text) != null)
                            .Append("|fx=").Append(text.GetComponent<Shadow>() != null ? "S" : "").Append(text.GetComponent<Outline>() != null ? "O" : "")
                            .Append("|text=").Append(text.text.Replace("\n", "\\n")).Append('\n');
                    }
                if (path.EndsWith(".prefab")) PrefabUtility.UnloadPrefabContents(roots[0]);
            }
            File.WriteAllText(output, sb.ToString());
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
                Convert(text, System.IO.Path.GetFileName(path) + "|" + HierarchyPath(text.transform));
                converted++;
            }
        }

        private string HierarchyPath(Transform transform)
        {
            var names = new List<string>();
            for (var t = transform; t != null; t = t.parent) names.Insert(0, t.name);
            return string.Join("/", names);
        }

        private TMP_FontAsset FontFor(Font font)
        {
            var asset = font == null ? null : AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontFolder + font.name + " SDF.asset");
            return asset != null ? asset : fontAsset;
        }

        private void Convert(Text text, string key)
        {
            var go = text.gameObject;
            var shadow = go.GetComponent<Shadow>();
            var outline = go.GetComponent<Outline>();
            var data = new TextData(text);

            UnityEngine.Object.DestroyImmediate(text);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            var font = FontFor(text.font);
            tmp.font = font;
            tmp.text = data.text;
            tmp.color = data.color;
            tmp.raycastTarget = data.raycastTarget;
            tmp.maskable = data.maskable;
            tmp.richText = data.richText;
            tmp.fontSize = data.fontSize * SizeScale;
            tmp.fontStyle = data.fontStyle;
            tmp.alignment = data.alignment;
            // UGUI nhân chiều cao dòng của font; TMP cộng thêm theo % của cỡ chữ: quy đổi qua chiều cao dòng (em) của font.
            tmp.lineSpacing = (data.lineSpacing - 1f) * 100f * font.faceInfo.lineHeight / font.faceInfo.pointSize;
            foreach (var rule in SpacingRules)
                if (rule.Key.IsMatch(key)) tmp.characterSpacing = rule.Value;
            tmp.textWrappingMode = data.horizontalWrap ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
            tmp.overflowMode = data.verticalTruncate ? TextOverflowModes.Truncate : TextOverflowModes.Overflow;
            tmp.enableAutoSizing = data.bestFit;
            tmp.fontSizeMin = data.minSize * SizeScale;
            tmp.fontSizeMax = data.maxSize * SizeScale;
            tmp.enabled = data.enabled;

            var preset = ResolveMaterial(font, data.fontSize, shadow, outline);
            if (preset != null) tmp.fontSharedMaterial = preset;
            if (outline != null) UnityEngine.Object.DestroyImmediate(outline);
            if (shadow != null) UnityEngine.Object.DestroyImmediate(shadow);
        }

        // Shadow -> Underlay, Outline -> Outline của TMP, dùng chung một Material Preset (không tạo instance riêng cho từng text).
        // Hệ số quy đổi từ pixel sang đơn vị shader là ước lượng, cần chỉnh bằng mắt ở giai đoạn 2.
        private Material ResolveMaterial(TMP_FontAsset fontAsset, float fontSize, Shadow shadow, Outline outline)
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
            foreach (var pair in ReadComponents(originalBlocks, TextGuidOld, TextFileId)) textByGameObject[pair.Key] = pair.Value;
            foreach (var pair in ReadComponents(originalBlocks, TextGuid6, 11500000)) textByGameObject[pair.Key] = pair.Value;
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
                var prefabGuid = AssetDatabase.AssetPathToGUID(path);
                if (!convertedByPrefab.ContainsKey(prefabGuid)) convertedByPrefab[prefabGuid] = new HashSet<string>();
                convertedByPrefab[prefabGuid].Add(pair.Value);
            }

            // gỡ Shadow/Outline trên các GameObject vừa chuyển
            foreach (var block in originalBlocks)
            {
                if (block.cls != "114") continue;
                var script = ScriptRegex.Match(block.text);
                var owner = GameObjectRegex.Match(block.text);
                if (!script.Success || !owner.Success || !gameObjectIds.ContainsKey(owner.Groups[1].Value)) continue;
                var legacy = (script.Groups[2].Value == TextGuid || script.Groups[2].Value == TextGuidOld) && (script.Groups[1].Value == ShadowFileId || script.Groups[1].Value == OutlineFileId);
                var modern = script.Groups[2].Value == ShadowGuid6 || script.Groups[2].Value == OutlineGuid6;
                if (legacy || modern) removedIds.Add(block.id);
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

        // Prefab instance (trong scene hoặc prefab khác) ghi đè thuộc tính của Text bằng propertyPath của Text:
        // đổi sang propertyPath tương ứng của TMP, nếu không các ghi đè (chữ, cỡ, căn lề...) bị Unity bỏ qua.
        private void RewriteOverrides(string path)
        {
            var yaml = File.ReadAllText(path);
            var rewritten = 0;
            var result = ModificationRegex.Replace(yaml, match =>
            {
                HashSet<string> ids;
                if (!convertedByPrefab.TryGetValue(match.Groups[2].Value, out ids) || !ids.Contains(match.Groups[1].Value)) return match.Value;
                var head = "    - target: {fileID: " + match.Groups[1].Value + ", guid: " + match.Groups[2].Value + ", type: 3}\n      propertyPath: ";
                var value = match.Groups[4].Value;
                var reference = match.Groups[5].Value;
                var property = match.Groups[3].Value;
                string[] names;
                string[] values;
                string[] references;
                switch (property)
                {
                    case "m_Text": names = new[] { "m_text" }; values = new[] { value }; references = new[] { reference }; break;
                    case "m_FontData.m_FontSize":
                        names = new[] { "m_fontSize", "m_fontSizeBase" };
                        values = new[] { Scaled(value), Scaled(value) };
                        references = new[] { reference, reference };
                        break;
                    case "m_FontData.m_MinSize": names = new[] { "m_fontSizeMin" }; values = new[] { Scaled(value) }; references = new[] { reference }; break;
                    case "m_FontData.m_MaxSize": names = new[] { "m_fontSizeMax" }; values = new[] { Scaled(value) }; references = new[] { reference }; break;
                    case "m_FontData.m_BestFit": names = new[] { "m_enableAutoSizing" }; values = new[] { value }; references = new[] { reference }; break;
                    case "m_FontData.m_FontStyle": names = new[] { "m_fontStyle" }; values = new[] { value }; references = new[] { reference }; break;
                    case "m_FontData.m_Font":
                        names = new[] { "m_fontAsset" };
                        values = new[] { "" };
                        references = new[] { TmpFontReference(reference) };
                        break;
                    case "m_FontData.m_Alignment":
                        var anchor = int.Parse(value);
                        names = new[] { "m_HorizontalAlignment", "m_VerticalAlignment" };
                        values = new[] { new[] { "1", "2", "4" }[anchor % 3], new[] { "256", "512", "1024" }[anchor / 3] };
                        references = new[] { reference, reference };
                        break;
                    default: return match.Value;
                }
                var builder = new System.Text.StringBuilder();
                for (var i = 0; i < names.Length; i++)
                    builder.Append(head).Append(names[i]).Append("\n      value: ").Append(values[i]).Append("\n      objectReference: ").Append(references[i]).Append('\n');
                rewritten++;
                return builder.ToString();
            });
            if (rewritten == 0) return;
            File.WriteAllText(path, result);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Debug.Log("[TMPCONV] " + path + ": đổi " + rewritten + " ghi đè prefab instance sang thuộc tính TMP");
        }

        private string Scaled(string value)
        {
            return (float.Parse(value, System.Globalization.CultureInfo.InvariantCulture) * SizeScale).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        }

        // Tham chiếu Font cũ -> TMP_FontAsset cùng tên (Baloo2-ExtraBold -> Baloo2-ExtraBold SDF).
        private string TmpFontReference(string reference)
        {
            var guid = Regex.Match(reference, @"guid: ([0-9a-f]+)").Groups[1].Value;
            var fontPath = AssetDatabase.GUIDToAssetPath(guid);
            var assetPath = FontFolder + System.IO.Path.GetFileNameWithoutExtension(fontPath) + " SDF.asset";
            var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(assetGuid)) assetGuid = AssetDatabase.AssetPathToGUID(DefaultFont);
            return "{fileID: 11400000, guid: " + assetGuid + ", type: 2}";
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
