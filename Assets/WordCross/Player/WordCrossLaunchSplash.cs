using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WordCross.Player
{
    // AOT, scene-owned launch splash: solid teal + game logo from the first Unity frame until the first scene after the
    // bootstrap scene is loaded (Home), so the player never sees a black frame while the CLR and Addressables load.
    [DisallowMultipleComponent]
    public sealed class WordCrossLaunchSplash : MonoBehaviour
    {
        [SerializeField] private Sprite logo;
        [SerializeField] private Color backgroundColor = new Color(0.07058824f, 0.70980394f, 0.65098041f, 1f);
        [SerializeField, Range(0.3f, 1f)] private float logoWidthFraction = 0.797f;
        [SerializeField] private float holdAfterSceneSeconds = 0.6f;
        [SerializeField] private float maxSeconds = 60f;

        private GameObject canvasRoot;
        private string bootstrapScene;
        private float startTime;
        private float sceneTime = -1f;

        private void Awake()
        {
            startTime = Time.unscaledTime;
            bootstrapScene = gameObject.scene.name;
            if (transform.parent == null) DontDestroyOnLoad(gameObject);
            BuildUi();
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("[LaunchSplash] start scene=" + bootstrapScene);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == bootstrapScene || sceneTime >= 0f) return;
            sceneTime = Time.unscaledTime;
            Debug.Log("[LaunchSplash] scene loaded=" + scene.name + " elapsed=" + (sceneTime - startTime).ToString("F2"));
        }

        private void Update()
        {
            var now = Time.unscaledTime;
            var sceneDone = sceneTime >= 0f && now - sceneTime >= holdAfterSceneSeconds;
            if (!sceneDone && now - startTime < maxSeconds) return;
            Debug.Log("[LaunchSplash] teardown elapsed=" + (now - startTime).ToString("F2") + " sceneDone=" + sceneDone);
            Destroy(gameObject);
        }

        private void BuildUi()
        {
            canvasRoot = new GameObject("Launch Splash Canvas", typeof(RectTransform));
            canvasRoot.transform.SetParent(transform, false);
            var canvas = canvasRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;

            var background = NewChild("Background", canvasRoot.transform).gameObject.AddComponent<Image>();
            background.color = backgroundColor;
            background.raycastTarget = false;
            var backgroundRect = background.rectTransform;
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;

            var logoImage = NewChild("Logo", canvasRoot.transform).gameObject.AddComponent<Image>();
            logoImage.sprite = logo;
            logoImage.raycastTarget = false;
            logoImage.preserveAspect = true;
            var side = Mathf.Min(Screen.width, Screen.height) * logoWidthFraction;
            var logoRect = logoImage.rectTransform;
            logoRect.anchorMin = new Vector2(0.5f, 0.5f);
            logoRect.anchorMax = new Vector2(0.5f, 0.5f);
            logoRect.pivot = new Vector2(0.5f, 0.5f);
            logoRect.sizeDelta = new Vector2(side, side);
        }

        private RectTransform NewChild(string objectName, Transform parent)
        {
            var child = new GameObject(objectName, typeof(RectTransform));
            child.transform.SetParent(parent, false);
            return (RectTransform)child.transform;
        }
    }
}
