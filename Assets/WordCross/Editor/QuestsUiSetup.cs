using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace WordCross.Editor
{
    // T134 bước 6: dựng popup Nhiệm vụ & Thưởng (QuestsDialog.prefab, clone khung từ LowRubyDialog), đăng ký vào GameMaster.baseDialogs
    // và thêm nút lối vào + chấm đỏ vào scene Home. Chạy lại nhiều lần vẫn an toàn (dựng lại prefab từ khung gốc, nút Home đã có thì bỏ qua).
    // Hàm static chỉ là điểm vào cho Unity (-executeMethod), phần việc nằm ở hàm instance.
    public class QuestsUiSetup
    {
        private const string SourcePath = "Assets/_Prefabs/Dialogs/LowRubyDialog.prefab";
        private const string PrefabPath = "Assets/_Prefabs/Dialogs/QuestsDialog.prefab";
        private const string GameMasterPath = "Assets/WordChef/Common/Prefabs/GameMaster.prefab";
        private const string HomeScenePath = "Assets/_Scenes/Home.unity";
        private const string CirclePath = "Assets/WordCross/UI/Rounded/circle.png";
        private const string RubyIconPath = "Assets/WordCross/UI/Header/icon_ruby_lg.png";
        private const string HomeButtonName = "Quests";
        // Thanh tiến độ cao 12: bo góc nhỏ để đầu thanh tròn, không nhọn.
        private const float BarMultiplier = 40f;
        // Ô ngày 81x68: bo góc vừa (design r-s 10px).
        private const float CellMultiplier = 5f;

        private static readonly Color Teal = new Color(0.071f, 0.710f, 0.651f, 1f);
        private static readonly Color Coral = new Color(1f, 0.420f, 0.369f, 1f);
        private static readonly Color CoralDeep = new Color(0.851f, 0.294f, 0.251f, 1f);
        private static readonly Color Yellow = new Color(1f, 0.788f, 0.235f, 1f);
        private static readonly Color YellowDeep = new Color(0.878f, 0.643f, 0.075f, 1f);
        private static readonly Color Ink = new Color(0.122f, 0.176f, 0.239f, 1f);
        private static readonly Color InkSoft = new Color(0.365f, 0.431f, 0.502f, 1f);
        private static readonly Color Line = new Color(0.875f, 0.914f, 0.918f, 1f);

        private TMP_Text titleSource;
        private TMP_Text bodySource;
        private Sprite roundSprite;
        private Sprite pillSprite;
        private float roundMultiplier;
        private float pillMultiplier;

        public static void Apply()
        {
            var setup = new QuestsUiSetup();
            setup.BuildPrefab();
            setup.RegisterDialog();
            setup.AddHomeButton();
        }

        // Chỉ dựng lại popup + đăng ký GameMaster (không đụng scene Home).
        public static void ApplyPrefab()
        {
            var setup = new QuestsUiSetup();
            setup.BuildPrefab();
            setup.RegisterDialog();
        }

        // ---------- prefab ----------

        private void BuildPrefab()
        {
            var root = PrefabUtility.LoadPrefabContents(SourcePath);
            var old = root.GetComponent<LowRubyDialog>();
            var anim = old.anim;
            var hiding = old.hidingAnimation;
            Object.DestroyImmediate(old);
            var dialog = root.AddComponent<QuestsDialog>();
            dialog.anim = anim;
            dialog.hidingAnimation = hiding;
            dialog.dialogType = DialogType.Quests;
            dialog.enableAd = false;

            var content = (RectTransform)root.transform.Find("Content");
            titleSource = content.Find("Title").GetComponent<TMP_Text>();
            bodySource = content.Find("Message").GetComponent<TMP_Text>();
            var watch = content.Find("Watch").GetComponent<Image>();
            roundSprite = watch.sprite;
            roundMultiplier = watch.pixelsPerUnitMultiplier;
            var skipFace = content.Find("Skip/Face").GetComponent<Image>();
            pillSprite = skipFace.sprite;
            pillMultiplier = skipFace.pixelsPerUnitMultiplier;
            foreach (var name in new[] { "Message", "Watch", "Shop", "Skip" }) Object.DestroyImmediate(content.Find(name).gameObject);

            // Khung: card 660x1180 trên canvas 720x1280.
            var bg = (RectTransform)content.Find("BG");
            bg.sizeDelta = new Vector2(660f, 1180f);
            var title = (RectTransform)content.Find("Title");
            title.sizeDelta = new Vector2(330f, 55f);
            title.anchoredPosition = new Vector2(20f, 520f);
            var titleText = title.GetComponent<TMP_Text>();
            titleText.enableAutoSizing = true;
            titleText.fontSizeMax = titleText.fontSize;
            titleText.fontSizeMin = titleText.fontSize * 0.55f;
            var close = (RectTransform)content.Find("CloseButton");
            close.anchoredPosition = new Vector2(307f, 574f);
            var closeButton = close.GetComponent<Button>();
            for (int i = closeButton.onClick.GetPersistentEventCount() - 1; i >= 0; i--) UnityEventTools.RemovePersistentListener(closeButton.onClick, i);
            UnityEventTools.AddPersistentListener(closeButton.onClick, dialog.OnCloseClick);

            dialog.titleText = titleText;
            dialog.balanceText = BuildBalance(content);
            BuildTabs(content, dialog);
            BuildScroll(content, dialog);
            BuildTemplates(content, dialog);

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[QuestsUiSetup] prefab saved " + PrefabPath);
        }

        private TMP_Text BuildBalance(RectTransform content)
        {
            var pill = NewRect("Balance", content, new Vector2(-240f, 520f), new Vector2(150f, 52f));
            AddImage(pill.gameObject, roundSprite, roundMultiplier, new Color(1f, 0.890f, 0.875f, 1f), false);
            var icon = NewRect("Icon", pill, new Vector2(-42f, 0f), new Vector2(34f, 34f));
            var image = icon.gameObject.AddComponent<Image>();
            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(RubyIconPath);
            image.preserveAspect = true;
            image.raycastTarget = false;
            var text = NewText("Text", pill, titleSource, 30f, CoralDeep, TextAlignmentOptions.Left);
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = new Vector2(50f, 0f);
            text.rectTransform.offsetMax = new Vector2(-12f, 0f);
            text.enableAutoSizing = true;
            text.fontSizeMax = 30f;
            text.fontSizeMin = 18f;
            return text;
        }

        private void BuildTabs(RectTransform content, QuestsDialog dialog)
        {
            var tabs = NewRect("Tabs", content, new Vector2(0f, 452f), new Vector2(604f, 64f));
            var layout = tabs.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            dialog.tabButtons = new Button[5];
            dialog.tabTexts = new TMP_Text[5];
            dialog.tabImages = new Image[5];
            for (int i = 0; i < 5; i++)
            {
                var tab = NewRect("Tab" + i, tabs, Vector2.zero, new Vector2(100f, 64f));
                var image = AddImage(tab.gameObject, roundSprite, roundMultiplier, Line, true);
                var button = tab.gameObject.AddComponent<Button>();
                button.targetGraphic = image;
                button.transition = Selectable.Transition.None;
                var text = NewText("Text", tab, titleSource, 24f, Ink, TextAlignmentOptions.Center);
                Stretch(text.rectTransform, 4f, 0f);
                text.enableAutoSizing = true;
                text.fontSizeMax = 24f;
                text.fontSizeMin = 14f;
                text.textWrappingMode = TextWrappingModes.NoWrap;
                UnityEventTools.AddIntPersistentListener(button.onClick, dialog.OnTabClick, i);
                dialog.tabButtons[i] = button;
                dialog.tabTexts[i] = text;
                dialog.tabImages[i] = image;
            }
        }

        private void BuildScroll(RectTransform content, QuestsDialog dialog)
        {
            var scroll = NewRect("Scroll", content, new Vector2(0f, -75f), new Vector2(604f, 940f));
            var viewport = NewRect("Viewport", scroll, Vector2.zero, Vector2.zero);
            Stretch(viewport, 0f, 0f);
            viewport.gameObject.AddComponent<RectMask2D>();
            var list = NewRect("List", viewport, Vector2.zero, new Vector2(0f, 100f));
            list.anchorMin = new Vector2(0f, 1f);
            list.anchorMax = new Vector2(1f, 1f);
            list.pivot = new Vector2(0.5f, 1f);
            list.anchoredPosition = Vector2.zero;
            var layout = list.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 12f;
            layout.padding = new RectOffset(0, 0, 0, 24);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            var fitter = list.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var rect = scroll.gameObject.AddComponent<ScrollRect>();
            rect.horizontal = false;
            rect.vertical = true;
            rect.movementType = ScrollRect.MovementType.Elastic;
            rect.scrollSensitivity = 40f;
            rect.viewport = viewport;
            rect.content = list;
            var hit = scroll.gameObject.AddComponent<Image>();
            hit.color = Color.clear;
            dialog.scroll = rect;
            dialog.content = list;
        }

        // Mẫu nằm trong "Templates" (inactive), QuestsDialog nhân bản vào danh sách.
        private void BuildTemplates(RectTransform content, QuestsDialog dialog)
        {
            var templates = NewRect("Templates", content, Vector2.zero, Vector2.zero);
            templates.gameObject.SetActive(false);

            dialog.cardTemplate = BuildCard(templates);
            dialog.sectionTemplate = BuildSection(templates);
            dialog.navTemplate = BuildNav(templates);

            var grid = NewRect("Grid", templates, Vector2.zero, new Vector2(604f, 400f));
            var gridLayout = grid.gameObject.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(81f, 68f);
            gridLayout.spacing = new Vector2(6f, 6f);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 7;
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.childAlignment = TextAnchor.UpperLeft;
            grid.gameObject.AddComponent<LayoutElement>().preferredHeight = 400f;
            dialog.gridTemplate = grid;

            var cell = NewRect("Cell", templates, Vector2.zero, new Vector2(81f, 68f));
            var outer = AddImage(cell.gameObject, roundSprite, CellMultiplier, Line, true);
            var button = cell.gameObject.AddComponent<Button>();
            button.targetGraphic = outer;
            button.transition = Selectable.Transition.None;
            var face = NewRect("Face", cell, Vector2.zero, Vector2.zero);
            Stretch(face, 3f, 3f);
            AddImage(face.gameObject, roundSprite, CellMultiplier, Color.white, false);
            var label = NewText("Text", cell, titleSource, 26f, Ink, TextAlignmentOptions.Center);
            Stretch(label.rectTransform, 0f, 0f);
            dialog.cellTemplate = cell;
        }

        private RectTransform BuildCard(RectTransform parent)
        {
            var card = NewRect("Card", parent, Vector2.zero, new Vector2(604f, 132f));
            AddImage(card.gameObject, roundSprite, roundMultiplier, Line, false);
            card.gameObject.AddComponent<LayoutElement>().preferredHeight = 132f;
            var face = NewRect("Face", card, Vector2.zero, Vector2.zero);
            Stretch(face, 3f, 3f);
            AddImage(face.gameObject, roundSprite, roundMultiplier, Color.white, false);

            var title = NewText("Title", card, titleSource, 28f, Ink, TextAlignmentOptions.TopLeft);
            Place(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), 22f, -190f, -52f, -14f);
            title.enableAutoSizing = true;
            title.fontSizeMax = 28f;
            title.fontSizeMin = 18f;
            title.textWrappingMode = TextWrappingModes.NoWrap;
            var sub = NewText("Sub", card, bodySource, 22f, InkSoft, TextAlignmentOptions.TopLeft);
            Place(sub.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), 22f, -190f, 40f, -54f);
            sub.enableAutoSizing = true;
            sub.fontSizeMax = 22f;
            sub.fontSizeMin = 15f;
            sub.textWrappingMode = TextWrappingModes.Normal;
            sub.overflowMode = TextOverflowModes.Truncate;

            var bar = NewRect("Bar", card, Vector2.zero, Vector2.zero);
            Place(bar, new Vector2(0f, 0f), new Vector2(1f, 0f), 22f, -190f, 16f, 28f);
            AddImage(bar.gameObject, pillSprite, BarMultiplier, Line, false);
            var fill = NewRect("Fill", bar, Vector2.zero, Vector2.zero);
            Stretch(fill, 0f, 0f);
            AddImage(fill.gameObject, pillSprite, BarMultiplier, Teal, false);

            var button = NewRect("Button", card, new Vector2(-18f, 0f), new Vector2(150f, 64f));
            button.anchorMin = button.anchorMax = new Vector2(1f, 0.5f);
            button.pivot = new Vector2(1f, 0.5f);
            button.anchoredPosition = new Vector2(-18f, 0f);
            var image = AddImage(button.gameObject, roundSprite, roundMultiplier, Yellow, true);
            var control = button.gameObject.AddComponent<Button>();
            control.targetGraphic = image;
            var shadow = button.gameObject.AddComponent<Shadow>();
            shadow.effectColor = YellowDeep;
            shadow.effectDistance = new Vector2(0f, -5f);
            var text = NewText("Text", button, titleSource, 26f, Ink, TextAlignmentOptions.Center);
            Stretch(text.rectTransform, 6f, 0f);
            text.enableAutoSizing = true;
            text.fontSizeMax = 26f;
            text.fontSizeMin = 14f;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            return card;
        }

        private RectTransform BuildSection(RectTransform parent)
        {
            var text = NewText("Section", parent, titleSource, 26f, InkSoft, TextAlignmentOptions.BottomLeft);
            text.margin = new Vector4(6f, 0f, 6f, 6f);
            text.enableAutoSizing = true;
            text.fontSizeMax = 26f;
            text.fontSizeMin = 16f;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.gameObject.AddComponent<LayoutElement>().preferredHeight = 48f;
            return text.rectTransform;
        }

        private RectTransform BuildNav(RectTransform parent)
        {
            var nav = NewRect("Nav", parent, Vector2.zero, new Vector2(604f, 64f));
            nav.gameObject.AddComponent<LayoutElement>().preferredHeight = 64f;
            var label = NewText("Label", nav, titleSource, 30f, Ink, TextAlignmentOptions.Center);
            Stretch(label.rectTransform, 80f, 0f);
            foreach (var side in new[] { "Prev", "Next" })
            {
                bool left = side == "Prev";
                var rect = NewRect(side, nav, Vector2.zero, new Vector2(64f, 64f));
                rect.anchorMin = rect.anchorMax = new Vector2(left ? 0f : 1f, 0.5f);
                rect.pivot = new Vector2(left ? 0f : 1f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                var image = AddImage(rect.gameObject, roundSprite, roundMultiplier, Line, true);
                var button = rect.gameObject.AddComponent<Button>();
                button.targetGraphic = image;
                var arrow = NewText("Text", rect, titleSource, 34f, Ink, TextAlignmentOptions.Center);
                Stretch(arrow.rectTransform, 0f, 0f);
                arrow.text = left ? "<" : ">";
            }
            return nav;
        }

        // ---------- helpers dựng UI ----------

        private RectTransform NewRect(string name, Transform parent, Vector2 position, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        private Image AddImage(GameObject go, Sprite sprite, float multiplier, Color color, bool raycast)
        {
            var image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = multiplier;
            image.color = color;
            image.raycastTarget = raycast;
            return image;
        }

        private TMP_Text NewText(string name, Transform parent, TMP_Text source, float size, Color color, TextAlignmentOptions alignment)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<TextMeshProUGUI>();
            text.font = source.font;
            text.fontSharedMaterial = source.fontSharedMaterial;
            text.fontSize = size;
            text.lineSpacing = source.lineSpacing * size / source.fontSize;
            text.characterSpacing = source.characterSpacing;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            text.textWrappingMode = TextWrappingModes.Normal;
            return text;
        }

        private void Stretch(RectTransform rect, float horizontal, float vertical)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(horizontal, vertical);
            rect.offsetMax = new Vector2(-horizontal, -vertical);
        }

        // Neo theo anchor, các lề là offsetMin/offsetMax so với anchor.
        private void Place(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, float left, float right, float bottom, float top)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(right, top);
        }

        // ---------- GameMaster ----------

        private void RegisterDialog()
        {
            var dialog = AssetDatabase.LoadAssetAtPath<QuestsDialog>(PrefabPath);
            var root = PrefabUtility.LoadPrefabContents(GameMasterPath);
            var controller = root.GetComponentInChildren<DialogController>(true);
            int index = (int)DialogType.Quests;
            if (controller.baseDialogs.Length <= index) System.Array.Resize(ref controller.baseDialogs, index + 1);
            controller.baseDialogs[index] = dialog;
            PrefabUtility.SaveAsPrefabAsset(root, GameMasterPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("[QuestsUiSetup] GameMaster.baseDialogs[" + index + "] = QuestsDialog");
        }

        // ---------- Home ----------

        private void AddHomeButton()
        {
            var scene = EditorSceneManager.OpenScene(HomeScenePath, OpenSceneMode.Single);
            var home = Object.FindFirstObjectByType<HomeController>();
            var buttons = GameObject.Find("Buttons").transform;
            var canvasRoot = buttons.parent;
            var existing = canvasRoot.Find(HomeButtonName);
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            var rect = NewRect(HomeButtonName, canvasRoot, Vector2.zero, new Vector2(96f, 96f));
            rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-30f, -70f);
            var circle = AssetDatabase.LoadAssetAtPath<Sprite>(CirclePath);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = circle;
            image.color = Yellow;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var shadow = rect.gameObject.AddComponent<Shadow>();
            shadow.effectColor = YellowDeep;
            shadow.effectDistance = new Vector2(0f, -6.8f);
            var icon = NewRect("Icon", rect, Vector2.zero, new Vector2(52f, 52f));
            var iconImage = icon.gameObject.AddComponent<Image>();
            iconImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(RubyIconPath);
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;

            // Chấm đỏ lấy kiểu từ chấm của nút Daily.
            Transform dailyDot = null;
            foreach (var child in GameObject.Find("Daily").GetComponentsInChildren<Transform>(true))
                if (child.name == "Dot") dailyDot = child;
            var dot = Object.Instantiate(dailyDot.gameObject, rect);
            dot.name = "Dot";
            var dotRect = (RectTransform)dot.transform;
            dotRect.anchorMin = dotRect.anchorMax = new Vector2(1f, 1f);
            dotRect.pivot = new Vector2(0.5f, 0.5f);
            dotRect.sizeDelta = new Vector2(30f, 30f);
            dotRect.anchoredPosition = new Vector2(-8f, -8f);
            dot.SetActive(false);

            UnityEventTools.AddIntPersistentListener(button.onClick, home.OnClick, 5);
            home.questsDot = dot;
            EditorUtility.SetDirty(home);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[QuestsUiSetup] Home button added");
        }
    }
}
