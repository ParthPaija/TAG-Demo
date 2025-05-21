// TODO: Please mark the status of each TODO as done once addressed
//
// ADS INTEGRATION
//
// 1. TODO: Call `adsNativeBridge.OnResume` and `adsNativeBridge.OnPause` to send app's lifecycle status accurately to the SDK
// 2. TODO: Send Unix timestamp of install in seconds in `adsNativeBridge.InitialiseMediationSDK` call to the SDK
// 3. Implement as many callbacks as needed from the provided ones to listen to any ad event for any adType
// 4. TODO: Implement `adsMediationCallbacks.OnVideoAdGrantReward` action for granting rewards from w2e - do not add any custom logic other than just giving reward
// 5. TODO: Use `adsMediationCallbacks.OnInterstitialAdHidden`, `adsMediationCallbacks.OnInterstitialAdDisplayFailed` to understand that control is given back to the game, when interstitial show call was made. Can continue the game if stopped forcefully
// 6. TODO: Use `adsMediationCallbacks.OnVideoAdHidden`, `adsMediationCallbacks.OnVideoAdDisplayFailed` to understand that control is given back to the game, when video show call was made. Can continue the game if stopped forcefully
// 7. Best way to make show call for any full screen ad would be to first check if the ad is available (mark a flag when ad loads or make a call to SDK to check the status) and then make the show call
// 8. Let me know if any other callbacks are needed

// ADJUST INTEGRATION
// Make few calls to the SDK as mentioned below for tracking events
//
// TODO: IapController.cs
//       1. Send IAP call to the SDK - Call `IapController.GetInstance().sendPurchaseInfo`. Send dollar($) value of the purchase and currency(as "USD") as arguments

// TODO: PuzzleLevelController.cs
//       1. Send a call to the SDK when level starts for a user for the first time - Call `PuzzleController.GetInstance().onLevelStart`. Send levelNumber as argument 
//       2. Send a call to the SDK when level completes (user clears the level successfully) - Call `PuzzleController.GetInstance().onLevelComplete`. Send levelNumber and time to clear the level (only consider the time user has spent on the game screen in seconds, reset the time to 0 when user clears/fails the level)

using System;
using Mediation.Runtime.Scripts.Android;
using UnityEngine;

namespace Hexa_Stack.Scripts.CoreGameSDK.Ads
{
    public class AdsController
    {
        private static AdsController instance;
        private bool initialized = false;

        // Class to listen to the ad events
        public readonly AdsMediationCallbacks _adsMediationCallbacks;

        // Bridge class to show ads in the game
        private readonly AdsNativeBridge adsNativeBridge;

        private AdsController()
        {
            _adsMediationCallbacks = new AdsMediationCallbacks();
            adsNativeBridge = new AdsNativeBridge();
        }

        public static AdsController GetInstance()
        {
            return instance ??= new AdsController();
        }

        public void OnPauseGame()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            adsNativeBridge.OnPause();
#endif
        }

        public void OnResumeGame()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            adsNativeBridge.OnResume();
#endif
        }

        public void ShowGdprDialogue()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            adsNativeBridge.showGDPRConsentDialog();
#endif
        }

        public void Initialize(long installTimeStamp, bool testMode, Action actionToCallOnInitialization = null)
        {
            GameAnalyticsTracker.TrackIlrdInGameAnalytics(true);
            
            _adsMediationCallbacks.OnMediationSdkInitialised += () =>
            {
                initialized = true;
                actionToCallOnInitialization?.Invoke();
            };

            //_adsMediationCallbacks.OnAdRevenueReceived += (string platform, string source, string format, string adUnitName, double value, string currency) =>
            //{
            //    // FIREBASE EVENT
            //    // TODO: Use these params to log event in Firebase
            //};

            // TODO: Subscribe to adType callbacks - look for actions defined in `AdsMediationCallbacks` class

#if UNITY_EDITOR
            initialized = true;
            actionToCallOnInitialization?.Invoke();
#elif UNITY_ANDROID
            adsNativeBridge.InitialiseMediationSDK(_adsMediationCallbacks, installTimeStamp, testMode);
#endif
        }

        public void ShowBannerAd()
        {
            if (!initialized)
            {
                return;
            }

            Debug.Log("Mediation: Game: Show Banner Ad");
#if UNITY_ANDROID && !UNITY_EDITOR
            adsNativeBridge.ShowBannerAd();
#endif
        }

        public void HideBannerAd()
        {
            if (!initialized)
            {
                return;
            }

            Debug.Log("Mediation: Game: Hide Banner Ad");
#if UNITY_ANDROID && !UNITY_EDITOR
            adsNativeBridge.HideBannerAd();
#endif
        }

        public bool IsInterstitialAdAvailable()
        {
            if (!initialized)
            {
                return false;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            return adsNativeBridge.IsInterstitialAdAvailable();
#else
            return false;
#endif
        }

        public void ShowInterstitialAd()
        {
            if (!initialized)
            {
                return;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            adsNativeBridge.ShowInterstitialAd();
#endif
        }

        public bool IsVideoAdAvailable()
        {
            if (!initialized)
            {
                return false;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            return adsNativeBridge.IsRewardedVideoAdAvailable();
#else
            return false;
#endif
        }

        public void ShowVideoAd()
        {
            if (!initialized)
            {
                return;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            adsNativeBridge.ShowRewardedVideoAd();
#endif
        }
    }
}