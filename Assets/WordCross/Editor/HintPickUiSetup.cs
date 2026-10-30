using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace WordCross.Editor
{
    // Dựng popup chọn Hint (HintPickDialog.prefab, clone khung từ LowRubyDialog: BG, Title, CloseButton, hai nút Watch/Shop đổi thành "1 ô"/"cả từ")
    // và đăng ký vào GameMaster.baseDialogs. Chạy lại nhiều lần vẫn an toàn (dựng lại từ khung gốc).
    // Hàm static chỉ là điểm vào cho Unity (-executeMethod), phần việc nằm ở hàm instance.
    public class HintPickUiSetup
    {
        private const string SourcePath = "Assets/_Prefabs/Dialogs/LowRubyDialog.prefab";
        private const string PrefabPath = "Assets/_Prefabs/Dialogs/HintPickDialog.prefab";
        private const string MainScenePath = "Assets/_Scenes/Main.unity";
        private const string GameMasterPath = "Assets/WordChef/Common/Prefabs/GameMaster.prefab";

        // Cùng số đo với LowRubyDialog.Layout: padding trên 47, dưới 37, gap 27, title 55, nút 88.
        private const float PaddingTop = 47f, PaddingBottom = 37f, Spacing = 27f, TitleLine = 55f, ButtonHeight = 88f, CloseInset = 10.3f;

        public static void Apply()
        {
            var setup = new HintPickUiSetup();
            setup.BuildPrefab();
            setup.RegisterDialog();
            setup.RewireMainScene();
        }

        private void BuildPrefab()
        {
            var root = PrefabUtility.LoadPrefabContents(SourcePath);
            var old = root.GetComponent<LowRubyDialog>();
            var anim = old.anim;
            var hiding = old.hidingAnimation;
            Object.DestroyImmediate(old);
            var dialog = root.AddComponent<HintPickDialog>();
            dialog.anim = anim;
            dialog.hidingAnimation = hiding;
            dialog.dialogType = DialogType.HintPick;
            dialog.enableAd = false;

            var content = (RectTransform)root.transform.Find("Content");
            foreach (var name in new[] { "Message", "Skip" }) Object.DestroyImmediate(content.Find(name).gameObject);

            float total = PaddingTop + PaddingBottom + TitleLine + 2 * (Spacing + ButtonHeight);
            var bg = (RectTransform)content.Find("BG");
            bg.sizeDelta = new Vector2(bg.sizeDelta.x, total);

            float y = total / 2 - PaddingTop;
            var title = (RectTransform)content.Find("Title");
            y = Place(title, y, TitleLine);
            var titleText = title.GetComponent<TMP_Text>();
            titleText.enableAutoSizing = true;
            titleText.fontSizeMax = titleText.fontSize;
            titleText.fontSizeMin = titleText.fontSize * 0.55f;

            var cell = (RectTransform)content.Find("Watch");
            cell.name = "Cell";
            y = Place(cell, y - Spacing, ButtonHeight);
            var word = (RectTransform)content.Find("Shop");
            word.name = "Word";
            Place(word, y - Spacing, ButtonHeight);

            var close = (RectTransform)content.Find("CloseButton");
            close.anchoredPosition = new Vector2(close.anchoredPosition.x, total / 2 - CloseInset);
            Rewire(close.GetComponent<Button>(), dialog.OnCloseClick);
            Rewire(cell.GetComponent<Button>(), dialog.OnCellClick);
            Rewire(word.GetComponent<Button>(), dialog.OnWordClick);

            dialog.titleText = titleText;
            dialog.cellButton = cell.GetComponent<Button>();
            dialog.cellText = Label(cell);
            dialog.wordButton = word.GetComponent<Button>();
            dialog.wordText = Label(word);

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[HintPickUiSetup] prefab saved " + PrefabPath);
        }

        private TMP_Text Label(RectTransform button)
        {
            var text = button.Find("Text").GetComponent<TMP_Text>();
            text.enableAutoSizing = true;
            text.fontSizeMax = text.fontSize;
            text.fontSizeMin = text.fontSize * 0.55f;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            return text;
        }

        private void Rewire(Button button, UnityEngine.Events.UnityAction action)
        {
            for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--) UnityEventTools.RemovePersistentListener(button.onClick, i);
            UnityEventTools.AddPersistentListener(button.onClick, action);
        }

        // Đặt phần tử có mép trên tại top, trả về mép dưới.
        private float Place(RectTransform rt, float top, float height)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, top - height / 2);
            return top - height;
        }

        // Scene Main: nút Hint mở popup chọn (WordRegion.OpenHintPick), gỡ nút HintWord riêng của T134 (code HintWordClick giữ nguyên).
        private void RewireMainScene()
        {
            var scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
            var region = Object.FindFirstObjectByType<WordRegion>(FindObjectsInactive.Include);
            foreach (var button in Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                int count = button.onClick.GetPersistentEventCount();
                for (int i = count - 1; i >= 0; i--)
                {
                    if (button.onClick.GetPersistentMethodName(i) != "HintClick") continue;
                    UnityEventTools.RemovePersistentListener(button.onClick, i);
                    UnityEventTools.AddPersistentListener(button.onClick, region.OpenHintPick);
                    Debug.Log("[HintPickUiSetup] Hint button '" + button.name + "' -> OpenHintPick");
                }
            }
            var word = GameObject.Find("HintWord");
            if (word != null) Object.DestroyImmediate(word);
            EditorUtility.SetDirty(region);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private void RegisterDialog()
        {
            var dialog = AssetDatabase.LoadAssetAtPath<HintPickDialog>(PrefabPath);
            var root = PrefabUtility.LoadPrefabContents(GameMasterPath);
            var controller = root.GetComponentInChildren<DialogController>(true);
            int index = (int)DialogType.HintPick;
            if (controller.baseDialogs.Length <= index) System.Array.Resize(ref controller.baseDialogs, index + 1);
            controller.baseDialogs[index] = dialog;
            PrefabUtility.SaveAsPrefabAsset(root, GameMasterPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[HintPickUiSetup] GameMaster.baseDialogs[" + index + "] = HintPickDialog");
        }
    }
}
