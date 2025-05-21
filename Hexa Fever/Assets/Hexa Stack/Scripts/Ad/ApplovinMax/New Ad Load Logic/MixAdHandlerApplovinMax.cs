using GameAnalyticsSDK;
using Hexa_Stack.Scripts.CoreGameSDK.Ads;
using System;
using Tag.HexaStack;
using UnityEngine;

namespace Tag.Ad
{
    public class MixAdHandlerApplovinMax : BaseRewardedAdHandler
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        internal bool isRewardAdWatched = false;
        internal Action actionWatched;
        internal Action actionShowed;
        private string adInfo;

        private bool isBannerAdLoaded;
        private RewardAdShowCallType rewardAdShowCallType;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Init()
        {
            this.adInfo = "None";
            InitializeRewardedAds();
            InitializeinterstitialAds();
        }

        public override void ShowAd(Action actionWatched, Action actionShowed = null, RewardAdShowCallType rewardAdShowCallType = RewardAdShowCallType.None)
        {
            this.rewardAdShowCallType = rewardAdShowCallType;
            this.adInfo = rewardAdShowCallType.ToString();
            this.actionShowed = actionShowed;
            this.actionWatched = () =>
            {
                AdManager.Instance.OnRewardedAdShowed();
                actionWatched?.Invoke();
            };

            AdsController.GetInstance().ShowVideoAd();
        }

        public override bool IsAdLoaded()
        {
            return IsRewardedVideoAvailable();// || IsInterstitialAvailable();
        }

        public override void LoadAd()
        {
        }

        //Logic For Simple Inter Ad.

        public void LoadSimpleInterstitialAd()
        {
            if (AdManager.Instance.IsNoAdsPurchased()) return;
        }

        public void ShowSimpleInterstitialAd()
        {
            if (AdManager.Instance.IsNoAdsPurchased()) return;

            AdsController.GetInstance().ShowInterstitialAd();
        }

        public bool IsSimpleInterstitialAdLoaded()
        {
            return AdsController.GetInstance().IsInterstitialAdAvailable();
        }

        public void InitSimpleInterstitialAd()
        {
            LoadSimpleInterstitialAd();
        }

        public void InitBannerAd()
        {
            InitializeBannerAds();
        }

        public void LoadBanner()
        {
            try
            {
                if (!isBannerAdLoaded)
                {
                    ForceStopBannerAds();
                    CreateBannerAd();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("LoadBanner Exception " + e);
            }
        }

        public void HideBanner()
        {
            AdsController.GetInstance().HideBannerAd();
        }

        public void StartBannerAdsAutoRefresh()
        {
        }
        public void ForceStopBannerAds()
        {
        }

        public Rect GetBannerAdRect()
        {
            return new Rect(0, 0, 0, 0);
        }

        public void ShowBanner()
        {
            AdsController.GetInstance().ShowBannerAd();
        }

        public bool IsBannerAdLoaded()
        {
            return isBannerAdLoaded;
        }

        public void OnBannerLoadSuccess()
        {
            isBannerAdLoaded = true;
        }

        public void OnBannerLoadFail()
        {
            LoadBanner();
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        private void InitializeRewardedAds()
        {
            AdsController.GetInstance()._adsMediationCallbacks.OnVideoAdLoaded += OnRewardedAdLoadedEvent;
            AdsController.GetInstance()._adsMediationCallbacks.OnVideoAdClicked += OnRewardedAdClickedEvent;
            AdsController.GetInstance()._adsMediationCallbacks.OnVideoAdDisplayed += OnRewardedAdDisplayedEvent;
            AdsController.GetInstance()._adsMediationCallbacks.OnVideoAdDisplayFailed += OnRewardedAdFailedToDisplayEvent;
            AdsController.GetInstance()._adsMediationCallbacks.OnVideoAdHidden += OnRewardedAdDismissedEvent;
            AdsController.GetInstance()._adsMediationCallbacks.OnVideoAdGrantReward += OnRewardedAdReceivedRewardEvent;

            AdsController.GetInstance()._adsMediationCallbacks.OnAdRevenueReceived += OnRewardedAdRevenuePaidEvent;
        }

        private void InitializeinterstitialAds()
        {
            // TODO : Return if no ads pack purchased

            AdsController.GetInstance()._adsMediationCallbacks.OnInterstitialAdLoaded += OnInterstitialAdLoadedEvent;
            AdsController.GetInstance()._adsMediationCallbacks.OnInterstitialAdClicked += OnInterstitialAdClickedEvent;
            AdsController.GetInstance()._adsMediationCallbacks.OnInterstitialAdDisplayed += OnInterstitialAdDisplayedEvent;
            AdsController.GetInstance()._adsMediationCallbacks.OnInterstitialAdDisplayFailed += OnInterstitialAdFailedToDisplayEvent;
            AdsController.GetInstance()._adsMediationCallbacks.OnInterstitialAdHidden += OnInterstitialAdDismissedEvent;
        }

        private void InitializeBannerAds()
        {
            if (AdManager.Instance.IsNoAdsPurchased()) return;

            // TODO : Return if no ads pack purchased
            CreateBannerAd();

            AdsController.GetInstance()._adsMediationCallbacks.OnBannerAdClicked += OnBannerAdClickedEvent;
            AdsController.GetInstance()._adsMediationCallbacks.OnBannerAdLoaded += OnBannerAdLoadedEvent;
        }

        private void CreateBannerAd()
        {
        }

        private bool IsRewardedVideoAvailable()
        {
            return AdsController.GetInstance().IsVideoAdAvailable();
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        private void OnRewardedAdLoadedEvent()//string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            AnalyticsManager.Instance.LogEvent_AdGAEvent(GAAdAction.Loaded, GAAdType.RewardedVideo, AdManager.Instance.AdNameType);
        }

        private void OnInterstitialAdLoadedEvent()//string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            AnalyticsManager.Instance.LogEvent_AdGAEvent(GAAdAction.Loaded, GAAdType.Interstitial, AdManager.Instance.AdNameType);
            Debug.Log("OnInterstitial AdLoadedEvent ");
        }

        private void OnRewardedAdFailedToDisplayEvent()//string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            this.adInfo = "None";
            AnalyticsManager.Instance.LogEvent_AdGAEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, AdManager.Instance.AdNameType);
        }

        private void OnInterstitialAdFailedToDisplayEvent()//string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("OnInterstitial AdFailedToDisplayEvent ");// + adUnitId);
            LoadSimpleInterstitialAd();
            AnalyticsManager.Instance.LogEvent_AdGAEvent(GAAdAction.FailedShow, GAAdType.Interstitial, AdManager.Instance.AdNameType);
        }

        private void OnRewardedAdDisplayedEvent()//string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad displayed");
            AnalyticsManager.Instance.LogEvent_AdGAEvent(GAAdAction.Show, GAAdType.RewardedVideo, AdManager.Instance.AdNameType);
        }

        private void OnInterstitialAdDisplayedEvent()//string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("OnInterstitial-Rewarded AdDisplayedEvent ");// + adUnitId);
            AnalyticsManager.Instance.LogEvent_AdGAEvent(GAAdAction.Show, GAAdType.Interstitial, AdManager.Instance.AdNameType);
        }

        private void OnRewardedAdClickedEvent()//string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad clicked");
            AnalyticsManager.Instance.LogEvent_AdGAEvent(GAAdAction.Clicked, GAAdType.RewardedVideo, AdManager.Instance.AdNameType);
        }

        private void OnInterstitialAdClickedEvent()//string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("OnInterstitial-Rewarded AdClickedEvent ");// + adUnitId);
            AnalyticsManager.Instance.LogEvent_AdGAEvent(GAAdAction.Clicked, GAAdType.Interstitial, AdManager.Instance.AdNameType);
        }

        private void OnRewardedAdDismissedEvent()//string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (actionWatched != null && (isRewardAdWatched))// || seconds > 15))
            {
                actionWatched();
            }
            else if (actionShowed != null)
            {
                actionShowed();
            }
            isRewardAdWatched = false;
        }

        private void OnInterstitialAdDismissedEvent()// string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("OnInterstitial AdDismissedEvent ");// + adUnitId);
        }

        private void OnRewardedAdReceivedRewardEvent()//string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad received reward");
            isRewardAdWatched = true;
            AnalyticsManager.Instance.LogEvent_AdGAEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, AdManager.Instance.AdNameType);
            AnalyticsManager.Instance.LogGAAdSuccessEvent(rewardAdShowCallType);
            rewardAdShowCallType = RewardAdShowCallType.None;
            this.adInfo = "None";
        }

        private void OnRewardedAdRevenuePaidEvent(string platform, string source, string format, string adUnitName, double value, string currency)//string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            AnalyticsManager.Instance.LogEvent_FirebaseAdRevenueAppLovin(platform, source, adUnitName, format, value, currency);

            double ecpmRewarded = value * (100000);
            SendFirebaseRevenueEvent("CPM_greaterthan_1000", 100000, ecpmRewarded);
            SendFirebaseRevenueEvent("CPM_greaterthan_500", 50000, ecpmRewarded);
            SendFirebaseRevenueEvent("CPM_greaterthan_100", 10000, ecpmRewarded);
            SendFirebaseRevenueEvent("CPM_greaterthan_10", 1000, ecpmRewarded);
        }

        private void OnBannerAdClickedEvent()//string arg1, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Banner Ad Clicked");
            AnalyticsManager.Instance.LogEvent_AdGAEvent(GAAdAction.Clicked, GAAdType.Banner, "Banner");
        }

        private void OnBannerAdLoadedEvent()//string arg1, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Banner Ad Loaded ");
            AnalyticsManager.Instance.LogEvent_AdGAEvent(GAAdAction.Loaded, GAAdType.Banner, "Banner");
            AnalyticsManager.Instance.LogEvent_AdGAEvent(GAAdAction.Show, GAAdType.Banner, "Banner");
            OnBannerLoadSuccess();
        }

        private void SendFirebaseRevenueEvent(string key, double threshold, double ecpmValue)
        {
            AnalyticsManager.Instance.LogEvent_FirebaseAdRevanueEvent(key, threshold, ecpmValue);
        }

        #endregion

        #region COROUTINES

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
