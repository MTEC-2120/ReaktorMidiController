using UnityEngine;
using System.Collections;

public class AppInfo : MonoBehaviour
{
    public static AppInfo Instance;

    // App-specific metadata
    public string APP_NAME = "[YOUR_APP_NAME]";

    public string APPSTORE_ID = "[YOUR_APPSTORE_ID";
    // App Store id

    public string BUNDLE_ID = "[YOUR_BUNDLE_ID]";
    // app bundle id

    [HideInInspector]
    public string APPSTORE_LINK = "itms-apps://itunes.apple.com/app/id";
    // App Store link

    [HideInInspector]
    public string PLAYSTORE_LINK = "market://details?id=";
    // Google Play store link

    [HideInInspector]
    public string APPSTORE_SHARE_LINK = "https://itunes.apple.com/app/id";
    // App Store link

    [HideInInspector]
    public string PLAYSTORE_SHARE_LINK = "https://play.google.com/store/apps/details?id=";
    // Google Play store link

    // Publisher links
    public string APPSTORE_HOMEPAGE = "[YOUR_APPSTORE_PUBLISHER_LINK]";
    // e.g itms-apps://itunes.apple.com/artist/[publisher-name]/[publisher-id]

    public string PLAYSTORE_HOMEPAGE = "[YOUR_GOOGLEPLAY_PUBLISHER_NAME]";
    // e.g https://play.google.com/store/apps/developer?id=[PUBLISHER_NAME]

    public string FACEBOOK_ID = "[YOUR_FACEBOOK_PAGE_ID]";

    public string TWITTER_NAME = "[YOUR_TWITTER_PAGE_NAME]";

    public string SUPPORT_EMAIL = "[YOUR_SUPPORT_EMAIL]";

    [HideInInspector]
    public string FACEBOOK_LINK = "https://facebook.com/";

    [HideInInspector]
    public string TWITTER_LINK = "https://twitter.com/";

    void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        APPSTORE_LINK += APPSTORE_ID;
        PLAYSTORE_LINK += BUNDLE_ID;
        APPSTORE_SHARE_LINK += APPSTORE_ID;
        PLAYSTORE_SHARE_LINK += BUNDLE_ID;
        FACEBOOK_LINK += FACEBOOK_ID;
        TWITTER_LINK += TWITTER_NAME;
    }
}
