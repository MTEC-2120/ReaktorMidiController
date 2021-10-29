using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SgLib;

#if EASY_MOBILE
using EasyMobile;
#endif

public class ShareUIController : MonoBehaviour
{
    public enum ImageFormat
    {
        GIF,
        PNG
    }

    public enum ScaleMode
    {
        AutoHeight,
        AutoWidth
    }

    public ImageFormat ImageType
    { 
        get { return _imageType; } 
        private set { _imageType = value; }
    }

    public Texture2D ImgTex { get; set; }

    [Header("Object References")]
    public GameObject container;
    public GameObject modal;
    public GameObject mask;
    public Image staticImage;
    public GameObject noImageMsg;
    public GameObject clipPlayer;
    public GameObject noClipMsg;
    public GameObject toolbar;
    public GameObject statusbar;
    public Text statusText;
    public Image progressBar;
    public Button gifButton;
    public Button pngButton;
    public GameObject giphyLogo;

    [Header("Apperance Config")]
    public ScaleMode scaleMode = ScaleMode.AutoHeight;
    public Color buttonEnableColor = Color.white;
    public Color buttonDisableColor = Color.gray;
#if EASY_MOBILE
    public string gifExportingText = "GENERATING GIF";
    public string gifUploadingText = "UPLOADING TO GIPHY";
    public float clipStartDelay = 0.5f;
#else
    [HideInInspector]
    public float clipStartDelay = 0.5f;
#endif

    RectTransform containerRT;
    Image gifButtonImage;
    Image pngButtonImage;

#if EASY_MOBILE
    public AnimatedClip AnimClip { get; set; }

    ImageFormat _imageType = ImageFormat.GIF;
    ClipPlayerUI clipPlayerComp;
    string gifPath;
    string giphyUrl;
    bool isExportingGif;
    bool isUploadingGif;
    

#else
    ImageFormat _imageType = ImageFormat.PNG;
#endif

    void Awake()
    {
        gifButtonImage = gifButton.GetComponent<Image>();
        pngButtonImage = pngButton.GetComponent<Image>();
        containerRT = container.GetComponent<RectTransform>();
        staticImage.GetComponent<RectTransform>().sizeDelta = containerRT.sizeDelta;
        clipPlayer.GetComponent<RectTransform>().sizeDelta = containerRT.sizeDelta;

        modal.SetActive(false);
        noImageMsg.SetActive(false);
        noClipMsg.SetActive(false);
        toolbar.SetActive(true);
        statusbar.SetActive(false);
        giphyLogo.SetActive(false);

#if EASY_MOBILE
        clipPlayerComp = clipPlayer.AddComponent<ClipPlayerUI>();
        clipPlayerComp.ScaleMode = scaleMode == ScaleMode.AutoHeight ? ClipPlayerScaleMode.AutoHeight : ClipPlayerScaleMode.AutoWidth;
        gifButton.gameObject.SetActive(true);
        pngButton.gameObject.SetActive(true);
#endif
    }

    void OnEnable()
    {
        if (ImageType == ImageFormat.GIF)
        {
            LoadAnimatedClip(clipStartDelay);
        }
        else
        {
            LoadStaticImage();
        }
    }

    void Update()
    {
#if EASY_MOBILE
        // Display the modal screen while working
        modal.SetActive(isExportingGif || isUploadingGif);

        // Display Giphy attribution mark.
        giphyLogo.SetActive(Giphy.IsUsingAPI); 
#endif

        gifButtonImage.color = ImageType == ImageFormat.GIF ? buttonEnableColor : buttonDisableColor;
        pngButtonImage.color = ImageType == ImageFormat.PNG ? buttonEnableColor : buttonDisableColor;
    }

    public void SwitchFormat()
    {
        ImageType = (ImageType == ImageFormat.GIF) ? ImageFormat.PNG : ImageFormat.GIF;

        if (ImageType == ImageFormat.GIF)
            LoadAnimatedClip(clipStartDelay);
        else
            LoadStaticImage();
    }

    public void Share()
    {
        if (ImageType == ImageFormat.PNG)
            SharePNG();
        else
            ShareGIF();
    }

    void SharePNG()
    {
        if (ImgTex == null)
        {
            Debug.Log("SharePNG failed: no captured screenshot.");
            return;
        } 

#if EASY_MOBILE
        Sharing.ShareTexture2D(ImgTex, ScreenshotSharer.Instance.pngFilename, ConstructShareMessage());
#else
        Debug.Log("Sharing feature requires Easy Mobile plugin.");
#endif
    }

    void ShareGIF()
    {
#if EASY_MOBILE
        if (!string.IsNullOrEmpty(giphyUrl))
        {
            ShareURL(giphyUrl);
        }
        else if (!string.IsNullOrEmpty(gifPath))
        {
            UploadGIFToGiphy(gifPath);
        }
        else if (AnimClip != null)
        {
            ExportGIF();
        }
        else
        {
            Debug.LogWarning("ShareGIF failed: nothing was recorded.");
        }
#else
        Debug.Log("Sharing feature requires Easy Mobile Pro plugin.");
#endif
    }

    void ShowGifExportingProgress(float progress)
    {
#if EASY_MOBILE
        if (toolbar.activeSelf)
            toolbar.SetActive(false);

        if (!statusbar.activeSelf)
            statusbar.SetActive(true);
        
        statusText.text = gifExportingText;
        progressBar.fillAmount = progress;
#endif
    }

    void ShowGifUploadingProgress(float progress)
    {
#if EASY_MOBILE
        if (toolbar.activeSelf)
            toolbar.SetActive(false);

        if (!statusbar.activeSelf)
            statusbar.SetActive(true);
        
        statusText.text = gifUploadingText;
        progressBar.fillAmount = progress;
#endif
    }

    void ShowToolbar()
    {
        toolbar.SetActive(true);
        statusbar.SetActive(false);
    }

    string ConstructShareMessage()
    {
        string msg = ScreenshotSharer.Instance.shareMessage;
        msg = msg.Replace("[score]", ScoreManager.Instance.Score.ToString());
        msg = msg.Replace("[AppName]", AppInfo.Instance.APP_NAME);
        msg = msg.Replace("[#AppName]", "#" + AppInfo.Instance.APP_NAME.Replace(" ", ""));

        return msg;
    }

    void LoadStaticImage()
    {
#if EASY_MOBILE
        clipPlayerComp.Stop();
#endif

        if (ImgTex != null)
        {
            noImageMsg.SetActive(false);
            Sprite sprite = Sprite.Create(ImgTex, new Rect(0.0f, 0.0f, ImgTex.width, ImgTex.height), new Vector2(0.5f, 0.5f));
            Transform imgTf = staticImage.gameObject.transform;
            RectTransform imgRtf = staticImage.GetComponent<RectTransform>();
            float scaleFactor = 1;

            if (scaleMode == ScaleMode.AutoHeight)
                scaleFactor = imgRtf.rect.width / sprite.rect.width;
            else
                scaleFactor = imgRtf.rect.height / sprite.rect.height;
            
            staticImage.sprite = sprite;
            staticImage.SetNativeSize();
            imgTf.localScale = imgTf.localScale * scaleFactor;

            ScaleContainer(sprite.rect.width / sprite.rect.height);
        }
        else
        {
            noImageMsg.SetActive(true);
        }

        clipPlayer.SetActive(false);
        staticImage.gameObject.SetActive(true);
    }

    void LoadAnimatedClip(float playStartDelay = 0f)
    {
        StartCoroutine(CRLoadAnimatedClip(playStartDelay));
    }

    IEnumerator CRLoadAnimatedClip(float playStartDelay)
    {
        staticImage.gameObject.SetActive(false);
        clipPlayer.SetActive(true);
        yield return null;

#if EASY_MOBILE
        if (AnimClip != null)
        {
            noClipMsg.SetActive(false);
            ScaleContainer((float)AnimClip.Width / AnimClip.Height);
            clipPlayerComp.Play(AnimClip, playStartDelay);
        }
        else
        {
            noClipMsg.SetActive(true);
        }
#else
        noClipMsg.SetActive(true);
#endif
    }

    void ScaleContainer(float aspect)
    {
        if (scaleMode == ScaleMode.AutoHeight)
        {
            float y = containerRT.sizeDelta.x / aspect;
            containerRT.sizeDelta = new Vector2(containerRT.sizeDelta.x, y);
        }
        else
        {
            float x = containerRT.sizeDelta.y * aspect;
            containerRT.sizeDelta = new Vector2(x, containerRT.sizeDelta.y);
        }
    }

#if EASY_MOBILE
    void ExportGIF()
    {
        if (AnimClip != null)
        {
            Gif.ExportGif(AnimClip, 
                ScreenshotSharer.Instance.gifFilename, 
                ScreenshotSharer.Instance.gifLoop, 
                ScreenshotSharer.Instance.gifQuality, 
                ScreenshotSharer.Instance.gifThreadPriority, 
                OnGifExportProgress, 
                OnGifExportCompleted);

            isExportingGif = true;
            ShowGifExportingProgress(0);
        }
    }

    void UploadGIFToGiphy(string path)
    {
        var content = new GiphyUploadParams();
        content.localImagePath = path;
        content.tags = ScreenshotSharer.Instance.giphyUploadTags;

        if (!string.IsNullOrEmpty(ScreenshotSharer.Instance.giphyUsername) && !string.IsNullOrEmpty(ScreenshotSharer.Instance.giphyApiKey))
            Giphy.Upload(ScreenshotSharer.Instance.giphyUsername, ScreenshotSharer.Instance.giphyApiKey,
                content, OnGiphyUploadProgress, OnGiphyUploadCompleted, OnGiphyUploadFailed);
        else
            Giphy.Upload(content, OnGiphyUploadProgress, OnGiphyUploadCompleted, OnGiphyUploadFailed);

        isUploadingGif = true;
    }

    void ShareURL(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("ShareURL failed: the given URL is invalid.");
            return;
        }

        Sharing.ShareURL(url);
    }

    // This callback is called repeatedly during the GIF exporting process.
    // It receives a progress value ranging from 0 to 1.
    void OnGifExportProgress(AnimatedClip clip, float progress)
    {
        ShowGifExportingProgress(progress);
    }

    // This callback is called once the GIF exporting has completed.
    // It receives the filepath of the generated image.
    void OnGifExportCompleted(AnimatedClip clip, string path)
    {
        gifPath = path;
        isExportingGif = false;
        UploadGIFToGiphy(gifPath);
        ShowGifUploadingProgress(0);
    }

    // This callback is called repeatedly during the uploading process.
    // It receives a progress value ranging from 0 to 1.
    void OnGiphyUploadProgress(float progress)
    {
        ShowGifUploadingProgress(progress);
    }

    // This callback is called once the uploading has completed.
    // It receives the URL of the uploaded image.
    void OnGiphyUploadCompleted(string url)
    {
        Debug.Log("Giphy URL: " + url);
        isUploadingGif = false;
        giphyUrl = url;
        ShowToolbar();
        ShareURL(url);
    }

    // This callback is called if the upload has failed.
    // It receives the error message.
    void OnGiphyUploadFailed(string error)
    {
        isUploadingGif = false;
        ShowToolbar();
        NativeUI.Alert("Upload Failed", "Uploading to Giphy has failed with error " + error);
    }
#endif
}
