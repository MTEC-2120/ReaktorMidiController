using UnityEngine;
using System.Collections;

#if EASY_MOBILE
using EasyMobile;
#endif

namespace SgLib
{
    public class InAppPurchaser : MonoBehaviour
    {
        [System.Serializable]
        public struct CoinPack
        {
            public string productName;
            public string priceString;
            public int coinValue;
        }

        [Header("Name of Remove Ads products")]
        public string removeAds = "Remove_Ads";

        [Header("Name of coin pack products")]
        public CoinPack[] coinPacks;

        #if EASY_MOBILE
        public static InAppPurchaser Instance { get; private set; }

        void OnEnable()
        {
            InAppPurchasing.PurchaseCompleted += OnPurchaseCompleted;
            InAppPurchasing.RestoreCompleted += OnRestoreCompleted;
        }

        void OnDisable()
        {
            InAppPurchasing.PurchaseCompleted -= OnPurchaseCompleted;
            InAppPurchasing.RestoreCompleted -= OnRestoreCompleted;
        }

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
        }

        // Buy an IAP product using its name
        public void Purchase(string productName)
        {
            if (InAppPurchasing.IsInitialized())
            {
                InAppPurchasing.Purchase(productName);
            }
            else
            {
                NativeUI.Alert("Service Unavailable", "Please check your internet connection.");
            }
        }

        // Restore purchase
        public void RestorePurchase()
        {
            if (InAppPurchasing.IsInitialized())
            {
                InAppPurchasing.RestorePurchases();
            }
            else
            {
                NativeUI.Alert("Service Unavailable", "Please check your internet connection.");
            }
        }

        // Successful purchase handler
        void OnPurchaseCompleted(IAPProduct product)
        {
            string name = product.Name;

            if (name.Equals(removeAds))
            {
                // Purchase of Remove Ads
                Advertising.RemoveAds();
            }
            else
            {
                // Purchase of coin packs
                foreach (CoinPack pack in coinPacks)
                {
                    if (pack.productName.Equals(name))
                    {
                        // Grant the user with their purchased coins
                        CoinManager.Instance.AddCoins(pack.coinValue);
                        break;
                    }
                }
            }
        }

        // Successful purchase restoration handler
        void OnRestoreCompleted()
        {
            NativeUI.Alert("Restore Completed", "Your in-app purchases were restored successfully.");
        }
        #endif
    }
}

