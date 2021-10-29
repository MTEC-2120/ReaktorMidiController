using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;

namespace SgLib
{
    public static class Utilities
    {
        public static IEnumerator CRWaitForRealSeconds(float time)
        {
            float start = Time.realtimeSinceStartup;

            while (Time.realtimeSinceStartup < start + time)
            {
                yield return null;
            }
        }

        public static void ButtonClickSound()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        }

        // Opens a specific scene
        public static void GoToScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public static void RateApp()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    Application.OpenURL(AppInfo.Instance.APPSTORE_LINK);
                    break;
        			
                case RuntimePlatform.Android:
                    Application.OpenURL(AppInfo.Instance.PLAYSTORE_LINK);
                    break;
            }
        }

        public static void ShowMoreGames()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    Application.OpenURL(AppInfo.Instance.APPSTORE_HOMEPAGE);
                    break;
    			
                case RuntimePlatform.Android:
                    Application.OpenURL(AppInfo.Instance.PLAYSTORE_HOMEPAGE);
                    break;
            }
        }

        public static void OpenFacebookPage()
        {
            Application.OpenURL(AppInfo.Instance.FACEBOOK_LINK);
        }

        public static void OpenTwitterPage()
        {
            Application.OpenURL(AppInfo.Instance.TWITTER_LINK);
        }

        public static void ContactUs()
        {
            string email = AppInfo.Instance.SUPPORT_EMAIL;
            string subject = EscapeURL(AppInfo.Instance.APP_NAME + " [" + Application.version + "] Support");
            string body = EscapeURL("");
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }

        public static string EscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        public static int[] GenerateShuffleIndices(int length)
        {
            int[] array = new int[length];

            // Populate array
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = i;
            }

            // Shuffle
            for (int j = 0; j < array.Length; j++)
            {
                int tmp = array[j];
                int randomPos = UnityEngine.Random.Range(j, array.Length);
                array[j] = array[randomPos];
                array[randomPos] = tmp;
            }

            return array;
        }

        /// <summary>
        /// Stores a DateTime as string to PlayerPrefs.
        /// </summary>
        /// <param name="time">Time.</param>
        /// <param name="ppkey">Ppkey.</param>
        public static void StoreTime(string ppkey, DateTime time)
        {
            PlayerPrefs.SetString(ppkey, time.ToBinary().ToString());
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Gets the stored string in the PlayerPrefs and converts it to a DateTime.
        /// If no time stored previously, defaultTime is returned.
        /// </summary>
        /// <returns>The time.</returns>
        /// <param name="ppkey">Ppkey.</param>
        public static DateTime GetTime(string ppkey, DateTime defaultTime)
        {
            string storedTime = PlayerPrefs.GetString(ppkey, string.Empty);

            if (!string.IsNullOrEmpty(storedTime))
                return DateTime.FromBinary(Convert.ToInt64(storedTime));
            else
                return defaultTime;
        }
    }
}