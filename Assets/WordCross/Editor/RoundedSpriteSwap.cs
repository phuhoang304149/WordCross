using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace WordCross.Editor
{
    // T126: thay ui_round / ui_circle (128px kéo 9-slice, mềm cạnh) bằng bộ sprite Codex (design/codex/sprites, 4 px/css).
    // Bán kính mỗi Image suy từ multiplier cũ (đã hiệu chỉnh theo design): css = 64 / mul / 1.6914, snap về token theme.css rồi chọn sprite.
    // Các asset YAML kiểu cũ bị Unity serialize lại toàn bộ khi lưu, nên công cụ chạy chế độ khô (-swapDry): chỉ import sprite theo README rồi
    // ghi báo cáo từng Image (bán kính, sprite, multiplier); tools/t126-apply-rounded.py vá đúng các khối Image trong file gốc theo báo cáo đó.
    // Hàm static chỉ là điểm vào cho Unity (-executeMethod), phần việc nằm ở hàm instance.
    public class RoundedSpriteSwap
    {
        private const string Folder = "Assets/WordCross/UI/Rounded/";
        private const string OldRound = "Assets/WordCross/UI/Home/ui_round.png";
        private const string OldCircle = "Assets/WordCross/UI/Home/ui_circle.png";
        private const float CanvasPerCss = 1.6914f;
        private const float StdMultiplier = 2.3649048f;
        private const float SnapCss = 0.7f;

        private struct Piece
        {
            public string File;
            public float Css;
            public float Border;
        }

        // Bán kính css (theme.css) và border nguồn px tương ứng, xếp tăng dần.
        private static readonly Piece[] Pieces =
        {
            new Piece { File = "round-r10", Css = 10, Border = 40 },
            new Piece { File = "round-r12", Css = 12, Border = 48 },
            new Piece { File = "round-r14", Css = 14, Border = 56 },
            new Piece { File = "round-rm", Css = 16, Border = 64 },
            new Piece { File = "pill-44", Css = 22, Border = 88 },
            new Piece { File = "round-rl", Css = 24, Border = 96 },
            new Piece { File = "pill-52", Css = 26, Border = 104 },
        };

        // Ngoại lệ theo đường dẫn: design ghi rõ bán kính khác giá trị suy từ multiplier cũ.
        private static readonly KeyValuePair<string, float>[] Overrides =
        {
            new KeyValuePair<string, float>("TextPreview/Content/Background", 10f),
        };

        private readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        private readonly StringBuilder report = new StringBuilder();
        private Sprite oldRound;
        private Sprite oldCircle;
        private int changed;
        private bool dry;

        // -executeMethod WordCross.Editor.RoundedSpriteSwap.RunFromArgs -swapAssets "a.prefab;b.unity" -swapReport path -swapDry
        public static void RunFromArgs()
        {
            var args = Environment.GetCommandLineArgs();
            string assets = null;
            string reportPath = null;
            var dry = false;
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "-swapAssets") assets = args[i + 1];
                if (args[i] == "-swapReport") reportPath = args[i + 1];
            }
            foreach (var arg in args)
                if (arg == "-swapDry") dry = true;
            new RoundedSpriteSwap { dry = dry }.Run(assets.Split(';'), reportPath);
        }

        private void Run(string[] assets, string reportPath)
        {
            ImportSprites();
            oldRound = AssetDatabase.LoadAssetAtPath<Sprite>(OldRound);
            oldCircle = AssetDatabase.LoadAssetAtPath<Sprite>(OldCircle);
            foreach (var path in assets)
            {
                if (path.EndsWith(".unity")) SwapScene(path);
                else SwapPrefab(path);
            }
            AssetDatabase.SaveAssets();
            File.WriteAllText(reportPath, report.ToString());
            Debug.Log("[RoundedSpriteSwap] đổi " + changed + " Image");
        }

        private void ImportSprites()
        {
            foreach (var piece in Pieces) ImportOne(piece.File, piece.Border);
            ImportOne("circle", 0);
        }

        private void ImportOne(string name, float border)
        {
            var path = Folder + name + ".png";
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100;
            importer.spritePivot = new Vector2(0.5f, 0.5f);
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 512;
            importer.npotScale = TextureImporterNPOTScale.None;
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.spriteBorder = new Vector4(border, border, border, border);
            importer.SaveAndReimport();
            sprites[name] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private void SwapScene(string path)
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            var dirty = false;
            foreach (var root in scene.GetRootGameObjects()) dirty |= SwapTree(root, path);
            if (dirty && !dry)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        private void SwapPrefab(string path)
        {
            var root = PrefabUtility.LoadPrefabContents(path);
            if (SwapTree(root, path) && !dry) PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
        }

        private bool SwapTree(GameObject root, string asset)
        {
            var dirty = false;
            foreach (var image in root.GetComponentsInChildren<Image>(true))
            {
                if (asset.EndsWith(".unity") && PrefabUtility.IsPartOfPrefabInstance(image)) continue;
                if (image.sprite == oldRound) dirty |= SwapRound(image, asset);
                else if (image.sprite == oldCircle) dirty |= SwapCircle(image, asset);
            }
            return dirty;
        }

        private bool SwapCircle(Image image, string asset)
        {
            var rect = image.rectTransform.rect;
            if (!dry)
            {
                image.sprite = sprites["circle"];
                if (image.type == Image.Type.Simple && Mathf.Abs(rect.width - rect.height) < 0.5f) image.preserveAspect = true;
            }
            changed++;
            report.AppendLine(Line(asset, image) + "|circle|Simple|" + rect.width.ToString("0.#") + "x" + rect.height.ToString("0.#"));
            return true;
        }

        private bool SwapRound(Image image, string asset)
        {
            var rect = image.rectTransform.rect;
            var oldMul = image.type == Image.Type.Sliced ? image.pixelsPerUnitMultiplier : 4.3f;
            var css = 64f / oldMul / CanvasPerCss;
            var path = PathOf(image.transform);
            foreach (var pair in Overrides)
                if (path.EndsWith(pair.Key)) css = pair.Value;
            var minDim = Mathf.Min(rect.width, rect.height);
            var note = "";
            if (minDim > 2f && css * CanvasPerCss > minDim / 2f + 0.5f)
            {
                css = minDim / 2f / CanvasPerCss;
                note = " clamp";
            }
            Piece piece = Pieces[Pieces.Length - 1];
            var found = false;
            foreach (var p in Pieces)
            {
                if (Mathf.Abs(css - p.Css) <= SnapCss) { css = p.Css; piece = p; found = true; break; }
            }
            if (!found)
            {
                css = Mathf.Round(css * 2f) / 2f;
                foreach (var p in Pieces)
                    if (p.Css >= css) { piece = p; break; }
            }
            var mul = Mathf.Approximately(css, piece.Css) ? StdMultiplier : piece.Border / (css * CanvasPerCss);
            var oldType = image.type;
            if (!dry)
            {
                image.sprite = sprites[piece.File];
                image.type = Image.Type.Sliced;
                image.pixelsPerUnitMultiplier = mul;
            }
            changed++;
            report.AppendLine(Line(asset, image) + "|" + oldMul.ToString("0.###") + "|" + css.ToString("0.#") + "css|" + piece.File + "|" + mul.ToString("0.####") + "|" + oldType + "|" + rect.width.ToString("0.#") + "x" + rect.height.ToString("0.#") + note);
            return true;
        }

        private static string Line(string asset, Image image)
        {
            return Path.GetFileName(asset) + "|" + PathOf(image.transform);
        }

        private static string PathOf(Transform t)
        {
            var path = t.name;
            while (t.parent != null)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }
            return path;
        }
    }
}
