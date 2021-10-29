using UnityEngine;
using System.Collections;

#if EASY_MOBILE
using EasyMobile;
#endif

namespace SgLib
{
    public class ScreenshotSharer : MonoBehaviour
    {
        public enum SharedImageType
        {
            PNG,
            GIF,
            Both
        }

        [Header("Check to disable sharing")]
        public bool disableSharing = false;

        [Header("Sharing Config")]
#if !EASY_MOBILE
        [HideInInspector]
#endif
        public SharedImageType sharedImageFormat = SharedImageType.Both;
        [Tooltip("Any instances of [score] will be replaced by the actual score achieved in the last game, [AppName] will be replaced by the app name declared in AppInfo")]
        [TextArea(3, 3)]
        public string shareMessage = "Awesome! I've just scored [score] in [AppName]! [#AppName]";
#if EASY_MOBILE
        public string gifFilename = "animated_screenshot";
#endif
        public string pngFilename = "screenshot";

#if EASY_MOBILE
        [Header("GIF Settings")]
        [Tooltip("Enable this to automatically set GIF height based on the specified width and the screen aspect ratio")]
        public bool gifAutoHeight = true;
        public int gifWidth = 320;
        public int gifHeight = 480;
        [Range(1, 30), Tooltip("Frame per second")]
        public int gifFps = 15;
        [Range(0.1f, 30f), Tooltip("GIF length in seconds")]
        public float gifLength = 3f;
        [Tooltip("0: loop forver; -1: loop disabled; >0: loop a set number of times")]
        public int gifLoop = 0;
        [Range(1, 100)]
        public int gifQuality = 80;
        [Tooltip("Priority of the GIF generating thread")]
        public System.Threading.ThreadPriority gifThreadPriority = System.Threading.ThreadPriority.BelowNormal;

        [Header("Giphy Credentials - leave both empty to use Giphy Beta key")]
        public string giphyUsername;
        public string giphyApiKey;
        [Tooltip("Comma-delimited tags to use when uploading GIF to Giphy")]
        public string giphyUploadTags = "unity, mobile, game";
#endif

        public static ScreenshotSharer Instance { get; private set; }

        public Texture2D CapturedScreenshot { get; private set; }

#if EASY_MOBILE
        Recorder recorder;

        public AnimatedClip RecordedClip { get; private set; }
#endif

        void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
#if !EASY_MOBILE
            sharedImageFormat = SharedImageType.PNG;
#endif
        }

        void OnEnable()
        {
            GameManager.GameStateChanged += GameManager_GameStateChanged;
            PlayerController.PlayerDied += PlayerController_PlayerDied;
        }

        void OnDisable()
        {
            GameManager.GameStateChanged -= GameManager_GameStateChanged;
            PlayerController.PlayerDied -= PlayerController_PlayerDied;
        }

        void OnDestroy()
        {
#if EASY_MOBILE
            if (RecordedClip != null)
            {
                RecordedClip.Dispose();
                RecordedClip = null;
            }
#endif
        }

        void GameManager_GameStateChanged(GameState newState, GameState oldState)
        {
#if EASY_MOBILE
            if (newState == GameState.Playing)
            {
                if (PremiumFeaturesManager.Instance.enablePremiumFeatures && !disableSharing && (sharedImageFormat == SharedImageType.GIF || sharedImageFormat == SharedImageType.Both))
                {
                    if (RecordedClip != null)
                    {
                        RecordedClip.Dispose();
                    }

                    recorder = Camera.main.GetComponent<Recorder>();

                    if (recorder == null)
                    {
                        recorder = Camera.main.gameObject.AddComponent<Recorder>();
                        recorder.Setup(gifAutoHeight, gifWidth, gifHeight, gifFps, Mathf.RoundToInt(gifLength));
                    }

                    recorder.Record();
                }
            }
            else if (newState == GameState.GameOver)
            {
                StartCoroutine(CRStopRecording(1f));
            }
#endif
        }

#if EASY_MOBILE
        IEnumerator CRStopRecording(float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            if (recorder != null)
            {
                RecordedClip = recorder.Stop();
            }
        }
#endif

        void PlayerController_PlayerDied()
        {
            if (PremiumFeaturesManager.Instance.enablePremiumFeatures && !disableSharing && (sharedImageFormat == SharedImageType.PNG || sharedImageFormat == SharedImageType.Both))
            {
                StartCoroutine(CRCaptureScreenshot());
            }
        }

        IEnumerator CRCaptureScreenshot()
        {
            // Wait for right timing to take screenshot
            yield return new WaitForEndOfFrame();

            // Temporarily render the camera content to our screenshotRenderTexture.
            // Later we'll share the screenshot from this rendertexture.
            RenderTexture tempRT = RenderTexture.GetTemporary(Screen.width, Screen.height);
            Camera.main.targetTexture = tempRT;
            Camera.main.Render();
            yield return null;
            Camera.main.targetTexture = null;
            yield return null;

            // Read the rendertexture contents
            RenderTexture.active = tempRT;

            if (CapturedScreenshot == null)
                CapturedScreenshot = new Texture2D(tempRT.width, tempRT.height, TextureFormat.RGB24, false);

            CapturedScreenshot.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
            CapturedScreenshot.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(tempRT);
        }
    }
}
