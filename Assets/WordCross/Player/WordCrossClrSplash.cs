using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using CatGaming.Clr;

namespace WordCross.Player
{
    // AOT, scene-owned splash shown while the CLR bootstrap loads. Lives in the player assembly so it
    // can render before any hot-update code exists. It only reads ClrBootstrap state; it never drives it.
    [DisallowMultipleComponent]
    public sealed class WordCrossClrSplash : MonoBehaviour
    {
        [SerializeField] private ClrBootstrap bootstrap;
        [SerializeField] private VideoClip clip;
        [SerializeField] private Texture2D firstFrame;
        [SerializeField] private Color backgroundColor = new Color(0.14902f, 0.21961f, 0.28627f, 1f);
        [SerializeField] private float prepareTimeoutSeconds = 15f;
        [SerializeField] private string errorMessage = "Unable to load the game. Please restart and try again.";

        private GameObject canvasRoot;
        private RawImage posterImage;
        private RawImage videoImage;
        private RawImage backgroundImage;
        private Text errorText;
        private VideoPlayer player;
        private RenderTexture renderTexture;
        private float startTime;
        private bool prepared;
        private bool videoFailed;
        private bool frameShown;
        private bool playedOnce;
        private bool clipEnded;
        private float preparedTime;
        private float frameTime;
        private bool ownsRoot;
        private bool finished;
        private bool endLogged;
        private string teardownReason = "destroyed";

        private void Awake()
        {
            startTime = Time.unscaledTime;
            Debug.Log("[Splash] start t=" + startTime.ToString("F2") + " prepareTimeout=" + prepareTimeoutSeconds.ToString("F1"));
            // Survive gameplay LoadSceneMode.Single loads; only a parentless root can be made persistent, and only an owner destroys it.
            ownsRoot = transform.parent == null;
            if (ownsRoot) DontDestroyOnLoad(gameObject);
            if (bootstrap == null) bootstrap = FindFirstObjectByType<ClrBootstrap>();
            BuildUi();
            if (clip == null) { videoFailed = true; Debug.LogWarning("[Splash] fallback reason=no-clip"); return; }
            renderTexture = new RenderTexture((int)clip.width, (int)clip.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            videoImage.texture = renderTexture;
            player = canvasRoot.AddComponent<VideoPlayer>();
            player.playOnAwake = false;
            player.source = VideoSource.VideoClip;
            player.clip = clip;
            player.renderMode = VideoRenderMode.RenderTexture;
            player.targetTexture = renderTexture;
            player.aspectRatio = VideoAspectRatio.Stretch;
            player.audioOutputMode = VideoAudioOutputMode.None;
            // Play the clip exactly once and keep every frame so a slow boot cannot skip the clip to its end.
            // Drive playback from unscaled game time so it never depends on the audio DSP clock or Time.timeScale.
            player.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
            player.isLooping = false;
            player.skipOnDrop = false;
            player.waitForFirstFrame = true;
            player.sendFrameReadyEvents = true;
            player.prepareCompleted += OnPrepared;
            player.errorReceived += OnVideoError;
            player.loopPointReached += OnLoopPoint;
            player.frameReady += OnFrameReady;
            player.Prepare();
            Debug.Log("[Splash] prepare requested elapsed=" + (Time.unscaledTime - startTime).ToString("F2"));
        }

        private void OnDestroy()
        {
            Teardown();
        }

        private void Update()
        {
            if (finished) return;
            var now = Time.unscaledTime;
            if (!videoFailed)
            {
                if (!prepared)
                {
                    if (now - startTime > prepareTimeoutSeconds) FailVideo("prepare-timeout");
                }
                else if (!frameShown)
                {
                    if (now - preparedTime > prepareTimeoutSeconds) FailVideo("first-frame-timeout");
                }
                else if (!playedOnce)
                {
                    var clipSeconds = (float)clip.length;
                    var elapsed = now - frameTime;
                    // Loop callback can be missed; the last frame index is an equivalent end signal.
                    var reachedEnd = clipEnded || (player != null && player.frameCount > 0UL && player.frame >= (long)player.frameCount - 1L);
                    // Done only once the clip ended AND it has been visible for its full duration (wall clock).
                    if (reachedEnd && elapsed >= clipSeconds)
                    {
                        playedOnce = true;
                        LogClipEnd(now, elapsed, "end");
                    }
                    // Decoder stalled after the first frame: stop holding the overlay, never block bootstrap.
                    else if (elapsed > Mathf.Max(clipSeconds * 2f, 1f) + 2f)
                    {
                        playedOnce = true;
                        LogClipEnd(now, elapsed, "decoder-stall");
                    }
                }
            }
            // A destroyed bootstrap (scene replaced after load) counts as ready.
            var ready = bootstrap == null || bootstrap.IsLoaded;
            if (ready)
            {
                if (videoFailed || playedOnce)
                {
                    teardownReason = videoFailed ? "clr-ready-video-failed" : "clr-ready-clip-done";
                    Teardown();
                }
                return;
            }
            var failed = !string.IsNullOrEmpty(bootstrap.LastError);
            if (errorText != null) errorText.gameObject.SetActive(failed);
            // Never pause a valid video pass on CLR error: it still plays once in full, then holds the last frame beside the error.
        }

        private void OnPrepared(VideoPlayer source)
        {
            if (finished || videoFailed) return;
            prepared = true;
            preparedTime = Time.unscaledTime;
            Debug.Log("[Splash] prepared elapsed=" + (preparedTime - startTime).ToString("F2"));
            source.Play();
        }

        private void OnFrameReady(VideoPlayer source, long frameIdx)
        {
            if (finished || videoFailed || frameShown) return;
            frameShown = true;
            frameTime = Time.unscaledTime;
            Debug.Log("[Splash] first-frame elapsed=" + (frameTime - startTime).ToString("F2"));
            videoImage.enabled = true;
            posterImage.enabled = false;
            if (renderTexture != null) SetBackgroundTexture(renderTexture, renderTexture.width, renderTexture.height);
            source.sendFrameReadyEvents = false;
        }

        private void OnLoopPoint(VideoPlayer source)
        {
            // Non-looping: player holds the last frame in the RenderTexture until teardown.
            clipEnded = true;
        }

        private void OnVideoError(VideoPlayer source, string message)
        {
            if (finished) return;
            Debug.LogWarning("[Splash] video error: " + message);
            FailVideo("video-error");
        }

        private void LogClipEnd(float now, float visibleSeconds, string how)
        {
            if (endLogged) return;
            endLogged = true;
            Debug.Log("[Splash] clip-end(" + how + ") elapsed=" + (now - startTime).ToString("F2") + " duration-visible=" + visibleSeconds.ToString("F2"));
        }

        private void FailVideo(string reason)
        {
            if (videoFailed) return;
            videoFailed = true;
            Debug.LogWarning("[Splash] fallback reason=" + reason + " elapsed=" + (Time.unscaledTime - startTime).ToString("F2"));
            if (player != null && player.isPlaying) player.Stop();
            if (videoImage != null) videoImage.enabled = false;
            if (posterImage != null) posterImage.enabled = true;
            SetBackgroundPoster();
        }

        private void BuildUi()
        {
            canvasRoot = new GameObject("Brand Splash Canvas", typeof(RectTransform));
            canvasRoot.transform.SetParent(transform, false);
            var canvas = canvasRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;
            var scaler = canvasRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasRoot.AddComponent<GraphicRaycaster>();

            backgroundImage = NewChild("Background", canvasRoot.transform).gameObject.AddComponent<RawImage>();
            backgroundImage.raycastTarget = false;
            Stretch(backgroundImage.rectTransform);
            SetBackgroundPoster();

            posterImage = NewFitted("Poster", firstFrame);
            videoImage = NewFitted("Video", null);
            videoImage.enabled = false;

            var textRect = NewChild("Error", canvasRoot.transform);
            errorText = textRect.gameObject.AddComponent<Text>();
            errorText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            errorText.text = errorMessage;
            errorText.fontSize = 42;
            errorText.alignment = TextAnchor.MiddleCenter;
            errorText.color = Color.white;
            errorText.raycastTarget = false;
            textRect.anchorMin = new Vector2(0.08f, 0.06f);
            textRect.anchorMax = new Vector2(0.92f, 0.16f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            errorText.gameObject.SetActive(false);
        }

        // Background samples one solid corner texel of the texture the foreground shows (same shader/path), so borders match without color math.
        private void SetBackgroundTexture(Texture texture, int width, int height)
        {
            if (backgroundImage == null) return;
            backgroundImage.texture = texture;
            backgroundImage.uvRect = new Rect(0f, 0f, 1f / Mathf.Max(1, width), 1f / Mathf.Max(1, height));
            backgroundImage.color = texture != null ? Color.white : backgroundColor;
        }

        private void SetBackgroundPoster()
        {
            if (firstFrame != null) SetBackgroundTexture(firstFrame, firstFrame.width, firstFrame.height);
            else SetBackgroundTexture(null, 1, 1);
        }

        private RawImage NewFitted(string objectName, Texture texture)
        {
            var rect = NewChild(objectName, canvasRoot.transform);
            var image = rect.gameObject.AddComponent<RawImage>();
            image.texture = texture;
            image.raycastTarget = false;
            var fitter = rect.gameObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            fitter.aspectRatio = clip != null ? (float)clip.width / clip.height : 1080f / 1920f;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            return image;
        }

        private RectTransform NewChild(string objectName, Transform parent)
        {
            var child = new GameObject(objectName, typeof(RectTransform));
            child.transform.SetParent(parent, false);
            return (RectTransform)child.transform;
        }

        private void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void Teardown()
        {
            if (finished) return;
            finished = true;
            Debug.Log("[Splash] teardown reason=" + teardownReason + " elapsed=" + (Time.unscaledTime - startTime).ToString("F2") + " prepared=" + prepared + " frameShown=" + frameShown + " playedOnce=" + playedOnce);
            if (player != null)
            {
                player.prepareCompleted -= OnPrepared;
                player.errorReceived -= OnVideoError;
                player.loopPointReached -= OnLoopPoint;
                player.frameReady -= OnFrameReady;
                player.Stop();
                player.targetTexture = null;
                player = null;
            }
            if (backgroundImage != null) backgroundImage.texture = null;
            if (videoImage != null) videoImage.texture = null;
            if (renderTexture != null)
            {
                renderTexture.Release();
                Destroy(renderTexture);
                renderTexture = null;
            }
            if (canvasRoot != null)
            {
                Destroy(canvasRoot);
                canvasRoot = null;
            }
            enabled = false;
            if (ownsRoot)
            {
                ownsRoot = false;
                Destroy(gameObject);
            }
        }
    }
}
