using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;
using SgLib;

#if EASY_MOBILE
using EasyMobile;
#endif

public class UIManager : MonoBehaviour
{
    [Header("Object References")]
    public GameObject header;
    public GameObject title;
    public Text score;
    public Text bestScore;
    public Text coinText;
    public GameObject newBestScore;
    public GameObject tapToStart;
    public GameObject characterSelectBtn;
    public GameObject menuButtons;
    public GameObject dailyRewardBtn;
    public Text dailyRewardBtnText;
    public GameObject rewardUI;
    public GameObject settingsUI;
    public GameObject soundOnBtn;
    public GameObject soundOffBtn;
    public GameObject musicOnBtn;
    public GameObject musicOffBtn;

    [Header("Premium Features Buttons")]
    public GameObject watchRewardedAdBtn;
    public GameObject leaderboardBtn;
    public GameObject achievementBtn;
    public GameObject iapPurchaseBtn;
    public GameObject removeAdsBtn;
    public GameObject restorePurchaseBtn;

    [Header("In-App Purchase Store")]
    public GameObject storeUI;

    [Header("Sharing-Specific")]
    public GameObject shareUI;
    public ShareUIController shareUIController;

    Animator scoreAnimator;
    Animator dailyRewardAnimator;
    bool isWatchAdsForCoinBtnActive;

    void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
        ScoreManager.ScoreUpdated += OnScoreUpdated;
    }

    void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
        ScoreManager.ScoreUpdated -= OnScoreUpdated;
    }

    // Use this for initialization
    void Start()
    {
        scoreAnimator = score.GetComponent<Animator>();
        dailyRewardAnimator = dailyRewardBtn.GetComponent<Animator>();

        Reset();
        ShowStartUI();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = ScoreManager.Instance.Score.ToString();
        bestScore.text = ScoreManager.Instance.HighScore.ToString();
        coinText.text = CoinManager.Instance.Coins.ToString();

        if (!DailyRewardController.Instance.disable && dailyRewardBtn.gameObject.activeSelf)
        {
            if (DailyRewardController.Instance.CanRewardNow())
            {
                dailyRewardBtnText.text = "GRAB YOUR REWARD!";
                dailyRewardAnimator.SetTrigger("activate");
            }
            else
            {
                TimeSpan timeToReward = DailyRewardController.Instance.TimeUntilReward;
                dailyRewardBtnText.text = string.Format("REWARD IN {0:00}:{1:00}:{2:00}", timeToReward.Hours, timeToReward.Minutes, timeToReward.Seconds);
                dailyRewardAnimator.SetTrigger("deactivate");
            }
        }

        if (settingsUI.activeSelf)
        {
            UpdateMuteButtons();
            UpdateMusicButtons();
        }
    }

    void GameManager_GameStateChanged(GameState newState, GameState oldState)
    {
        if (newState == GameState.Playing)
        {              
            ShowGameUI();
        }
        else if (newState == GameState.PreGameOver)
        {
            // Before game over, i.e. game potentially will be recovered
        }
        else if (newState == GameState.GameOver)
        {
            Invoke("ShowGameOverUI", 3f);
        }
    }

    void OnScoreUpdated(int newScore)
    {
        scoreAnimator.Play("NewScore");
    }

    void Reset()
    {
        header.SetActive(false);
        title.SetActive(false);
        score.gameObject.SetActive(false);
        newBestScore.SetActive(false);
        tapToStart.SetActive(false);
        characterSelectBtn.SetActive(false);
        menuButtons.SetActive(false);
        dailyRewardBtn.SetActive(false);

        // Enable or disable premium stuff
        bool enablePremium = IsPremiumFeaturesEnabled();
        leaderboardBtn.SetActive(enablePremium);
        achievementBtn.SetActive(enablePremium);
        iapPurchaseBtn.SetActive(enablePremium);
        removeAdsBtn.SetActive(enablePremium);
        restorePurchaseBtn.SetActive(enablePremium);

        // Hidden by default
        storeUI.SetActive(false);
        settingsUI.SetActive(false);
        shareUI.SetActive(false);

        // These premium feature buttons are hidden by default
        // and shown when certain criteria are met (e.g. rewarded ad is loaded)
        watchRewardedAdBtn.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void EndGame()
    {
        GameManager.Instance.GameOver();
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame(0.2f);
    }

    public void ShowStartUI()
    {
        settingsUI.SetActive(false);

        header.SetActive(true);
        title.SetActive(true);
        tapToStart.SetActive(true);
        characterSelectBtn.SetActive(true);  

        // If first launch: show "WatchForCoins" and "DailyReward" buttons if the conditions are met
        if (GameManager.GameCount == 0)
        {
            ShowWatchForCoinsBtn();
            ShowDailyRewardBtn();
        }
    }

    public void ShowGameUI()
    {
        header.SetActive(true);
        title.SetActive(false);
        score.gameObject.SetActive(true);
        tapToStart.SetActive(false);
        characterSelectBtn.SetActive(false);
        dailyRewardBtn.SetActive(false);
        watchRewardedAdBtn.SetActive(false);
    }

    public void ShowGameOverUI()
    {
        header.SetActive(true);
        title.SetActive(false);
        score.gameObject.SetActive(true);
        tapToStart.SetActive(false);
        menuButtons.SetActive(true);
        newBestScore.SetActive(ScoreManager.Instance.HasNewHighScore);
        watchRewardedAdBtn.gameObject.SetActive(false);
        settingsUI.SetActive(false);

        ShowDailyRewardBtn();

        // Show these if premium features are enabled (and relevant conditions are met)
        if (IsPremiumFeaturesEnabled())
        {
            ShowShareUI();
            ShowWatchForCoinsBtn();
        }
    }

    void ShowWatchForCoinsBtn()
    {
        // Only show "watch for coins button" if a rewarded ad is loaded and premium features are enabled
        #if EASY_MOBILE
        if (IsPremiumFeaturesEnabled() && AdDisplayer.Instance.CanShowRewardedAd() && AdDisplayer.Instance.watchAdToEarnCoins)
        {
            watchRewardedAdBtn.SetActive(true);
            watchRewardedAdBtn.GetComponent<Animator>().SetTrigger("activate");
        }
        else
        {
            watchRewardedAdBtn.SetActive(false);
        }
        #endif
    }

    void ShowDailyRewardBtn()
    {
        // Not showing the daily reward button if the feature is disabled
        if (!DailyRewardController.Instance.disable)
        {
            dailyRewardBtn.SetActive(true);
        }
    }

    public void ShowSettingsUI()
    {
        settingsUI.SetActive(true);
    }

    public void HideSettingsUI()
    {
        settingsUI.SetActive(false);
    }

    public void ShowStoreUI()
    {
        storeUI.SetActive(true);
    }

    public void HideStoreUI()
    {
        storeUI.SetActive(false);
    }

    public void WatchRewardedAd()
    {
        #if EASY_MOBILE
        // Hide the button
        watchRewardedAdBtn.SetActive(false);

        AdDisplayer.CompleteRewardedAdToEarnCoins += OnCompleteRewardedAdToEarnCoins;
        AdDisplayer.Instance.ShowRewardedAdToEarnCoins();
        #endif
    }

    void OnCompleteRewardedAdToEarnCoins()
    {
        #if EASY_MOBILE
        // Unsubscribe
        AdDisplayer.CompleteRewardedAdToEarnCoins -= OnCompleteRewardedAdToEarnCoins;

        // Give the coins!
        ShowRewardUI(AdDisplayer.Instance.rewardedCoins);
        #endif
    }

    public void GrabDailyReward()
    {
        if (DailyRewardController.Instance.CanRewardNow())
        {
            int reward = DailyRewardController.Instance.GetRandomReward();

            // Round the number and make it mutiplies of 5 only.
            int roundedReward = (reward / 5) * 5;

            // Show the reward UI
            ShowRewardUI(roundedReward);

            // Update next time for the reward
            DailyRewardController.Instance.ResetNextRewardTime();
        }
    }

    public void ShowRewardUI(int reward)
    {
        rewardUI.SetActive(true);
        rewardUI.GetComponent<RewardUIController>().Reward(reward);
    }

    public void HideRewardUI()
    {
        rewardUI.GetComponent<RewardUIController>().Close();
    }

    public void ShowLeaderboardUI()
    {
        #if EASY_MOBILE
        if (GameServices.IsInitialized())
        {
            GameServices.ShowLeaderboardUI();
        }
        else
        {
            #if UNITY_IOS
            NativeUI.Alert("Service Unavailable", "The user is not logged in to Game Center.");
            #elif UNITY_ANDROID
            GameServices.Init();
            #endif
        }
        #endif
    }

    public void ShowAchievementsUI()
    {
        #if EASY_MOBILE
        if (GameServices.IsInitialized())
        {
            GameServices.ShowAchievementsUI();
        }
        else
        {
            #if UNITY_IOS
            NativeUI.Alert("Service Unavailable", "The user is not logged in to Game Center.");
            #elif UNITY_ANDROID
            GameServices.Init();
            #endif
        }
        #endif
    }

    public void PurchaseRemoveAds()
    {
        #if EASY_MOBILE
        InAppPurchaser.Instance.Purchase(InAppPurchaser.Instance.removeAds);
        #endif
    }

    public void RestorePurchase()
    {
        #if EASY_MOBILE
        InAppPurchaser.Instance.RestorePurchase();
        #endif
    }

    public void ShowShareUI()
    {
        if (!ScreenshotSharer.Instance.disableSharing)
        {
            Texture2D texture = ScreenshotSharer.Instance.CapturedScreenshot;
            shareUIController.ImgTex = texture;

            #if EASY_MOBILE
            AnimatedClip clip = ScreenshotSharer.Instance.RecordedClip;
            shareUIController.AnimClip = clip;
            #endif

            shareUI.SetActive(true);
        }
    }

    public void HideShareUI()
    {
        shareUI.SetActive(false);
    }

    public void ShowCharacterSelectionScene()
    {
        SceneManager.LoadScene("CharacterSelection");
    }

    public void ToggleSound()
    {
        SoundManager.Instance.ToggleMute();
    }

    public void ToggleMusic()
    {
        SoundManager.Instance.ToggleMusic();
    }

    public void RateApp()
    {
        Utilities.RateApp();
    }

    public void OpenTwitterPage()
    {
        Utilities.OpenTwitterPage();
    }

    public void OpenFacebookPage()
    {
        Utilities.OpenFacebookPage();
    }

    public void ButtonClickSound()
    {
        Utilities.ButtonClickSound();
    }

    void UpdateMuteButtons()
    {
        if (SoundManager.Instance.IsMuted())
        {
            soundOnBtn.gameObject.SetActive(false);
            soundOffBtn.gameObject.SetActive(true);
        }
        else
        {
            soundOnBtn.gameObject.SetActive(true);
            soundOffBtn.gameObject.SetActive(false);
        }
    }

    void UpdateMusicButtons()
    {
        if (SoundManager.Instance.IsMusicOff())
        {
            musicOffBtn.gameObject.SetActive(true);
            musicOnBtn.gameObject.SetActive(false);
        }
        else
        {
            musicOffBtn.gameObject.SetActive(false);
            musicOnBtn.gameObject.SetActive(true);
        }
    }

    bool IsPremiumFeaturesEnabled()
    {
        return PremiumFeaturesManager.Instance != null && PremiumFeaturesManager.Instance.enablePremiumFeatures;
    }
}
