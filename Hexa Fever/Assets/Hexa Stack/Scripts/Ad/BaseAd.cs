using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Tag.HexaStack;

namespace Tag.Ad
{
    public class BaseAd : MonoBehaviour
    {
        #region PUBLIC_VARS

        public BaseRewardedAdHandler baseRewardedAdHandler;
        public BaseInterstitialAd baseInterstitialAd;
        public BaseBannerAd baseBannerAd;

        public const string PrefsKeyConsent = "PkConsent";

        #endregion

        #region PRIVATE_VARS

        private RewardAdShowCallType rewardAdShowCallType;
        private Action actionWatched;
        private Action actionShowed;
        private Action actionOnNoAds;

        private Action actionToCallOnInitSuccess;

        [SerializeField] private int playedLevelCount = 0;
        [SerializeField] private float lastAdShowTime = 0f;

        private bool isAdShownForFirstTimePref
        {
            get { return PlayerPrefs.GetInt("firstTimeAdShow", 0) == 1; }
            set { PlayerPrefs.SetInt("firstTimeAdShow", (value) ? 1 : 0); }
        }

        protected bool IsCMPDone
        {
            get { return PlayerPrefs.GetInt("isCmpDone", 0) == 1; }
            set { PlayerPrefs.SetInt("isCmpDone", (value) ? 1 : 0); }
        }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public virtual void Init(Action actionToCallOnInitSuccess = null)
        {
            this.actionToCallOnInitSuccess = actionToCallOnInitSuccess;

            playedLevelCount = 0;
            lastAdShowTime = Time.time;
        }

        public virtual void ShowInterstitial(InterstatialAdPlaceType interstatialAdPlaceType)
        {
            Debug.Log("In.Ad Try Show Interstitial " + interstatialAdPlaceType.ToString() + CanShowInterstitial(interstatialAdPlaceType) + "---" + baseInterstitialAd.IsAdLoaded());
            if (CanShowInterstitial(interstatialAdPlaceType) && baseInterstitialAd.IsAdLoaded())
            {
                Debug.Log("In.Ad Show Interstitial " + interstatialAdPlaceType.ToString());
                playedLevelCount = 0;
                lastAdShowTime = Time.time;
                isAdShownForFirstTimePref = true;
                baseInterstitialAd.ShowAd();
            }
            else
            {
                if (CanLoadInterstitial(interstatialAdPlaceType))
                {
                    Debug.Log("Load Interstitial " + interstatialAdPlaceType.ToString());
                    baseInterstitialAd.LoadAd();
                }
            }
        }

        public virtual void ShowInterstitialWithoutConditions(InterstatialAdPlaceType interstatialAdPlaceType)
        {
            Debug.Log("ShowInterstitialWithoutConditions " + interstatialAdPlaceType.ToString() + "---" + baseInterstitialAd.IsAdLoaded());
            if (baseInterstitialAd.IsAdLoaded())
            {
                Debug.Log("In.Ad Show Interstitial " + interstatialAdPlaceType.ToString());
                playedLevelCount = 0;
                lastAdShowTime = Time.time;
                isAdShownForFirstTimePref = true;
                baseInterstitialAd.ShowAd();
            }
            else
            {
                Debug.Log("Load Interstitial " + interstatialAdPlaceType.ToString());
                baseInterstitialAd.LoadAd();
            }
        }

        public virtual void ShowRewardedVideo(Action actionWatched, Action actionShowed = null, Action actionOnNoAds = null, RewardAdShowCallType rewardAdShowCallType = RewardAdShowCallType.None)
        {
            this.actionWatched = actionWatched;
            this.actionShowed = actionShowed;
            this.actionOnNoAds = actionOnNoAds;
            this.rewardAdShowCallType = rewardAdShowCallType;

            if (baseRewardedAdHandler.IsAdLoaded())
            {
                playedLevelCount = 0;
                lastAdShowTime = Time.time;
                Debug.Log("<MAX> Show RewardedVideo");
                baseRewardedAdHandler.ShowAd(actionWatched, actionShowed, rewardAdShowCallType);
            }
            else
            {
                baseRewardedAdHandler.LoadAd();
                StartCoroutine(WaitAndShowRewardAdCoroutine());
            }
        }

        public virtual void StartBannerAdAutoRefresh()
        {

        }

        public virtual void ShowBannerAd()
        {
            baseBannerAd.ShowBanner();
        }
        public virtual void HideBannerAd()
        {
            baseBannerAd.HideBanner();
        }
        public virtual bool IsBannerAdLoaded()
        {
            return baseBannerAd.IsBannerAdLoaded();
        }

        public virtual Rect GetBannerRect()
        {
            return baseBannerAd.GetBannerRect();
        }

        public virtual void OnInitSuccess()
        {
            if (actionToCallOnInitSuccess != null)
                actionToCallOnInitSuccess();
        }

        public bool IsAskedForConsent()
        {
            return PlayerPrefs.HasKey(PrefsKeyConsent);
        }

        public void OnLevelPlayed()
        {
            playedLevelCount++;
        }

        #endregion

        #region PRIVATE_FUNCTIONS
        private bool CanShowInterstatialAdsAccordoingToLevel(InterstatialAdPlaceType interstatialAdPlaceType)
        {
            return AdManager.Instance.AdConfigData.CanShowInterstitialAd(interstatialAdPlaceType);
        }

        private bool IsRemoveAdPurchased()
        {
            return false;
        }

        private bool CanShowInterstitial(InterstatialAdPlaceType interstatialAdPlaceType)
        {
            Debug.Log("In.Ad IsRemoveAdPurchased() " + IsRemoveAdPurchased());
            if (IsRemoveAdPurchased())
                return false;

            Debug.Log("In.Ad CanShowInterstatialAdsAccordoingToLevel(interstatialAdPlaceType) " + !CanShowInterstatialAdsAccordoingToLevel(interstatialAdPlaceType));
            if (!CanShowInterstatialAdsAccordoingToLevel(interstatialAdPlaceType))
                return false;

            int levelDifference = GetInterstitialAdIntervalInLevels(interstatialAdPlaceType);
            Debug.Log("In.Ad Level Played :- " + playedLevelCount + " Level Diff " + levelDifference);
            if (playedLevelCount < levelDifference)
                return false;

            float currentValueTime = Time.time;
            float timeDifference = GetInterstitialAdIntervalInTimes(interstatialAdPlaceType);
            Debug.Log("In.Ad Current Time :- " + currentValueTime + " Last Time  " + lastAdShowTime + "Time Diff" + (currentValueTime - lastAdShowTime));
            if ((currentValueTime - lastAdShowTime) < timeDifference)
                return false;
            return true;
        }

        private int GetInterstitialAdIntervalInLevels(InterstatialAdPlaceType interstatialAdPlaceType)
        {
            return AdManager.Instance.AdConfigData.GetShowInterstitialAdIntervalLevel(interstatialAdPlaceType);
        }

        private float GetInterstitialAdIntervalInTimes(InterstatialAdPlaceType interstatialAdPlaceType)
        {
            return AdManager.Instance.AdConfigData.GetShowInterstitialAdIntervalTime(interstatialAdPlaceType);
        }

        private bool CanLoadInterstitial(InterstatialAdPlaceType interstatialAdPlaceType)
        {
            if (!InternetManager.Instance.IsReachableToNetwork())
                return false;

            if (IsRemoveAdPurchased())
                return false;

            if (!CanShowInterstatialAdsAccordoingToLevel(interstatialAdPlaceType))
                return false;

            if (baseInterstitialAd.IsAdLoaded())
                return false;

            return true;
        }

        public void ResetActions()
        {
            actionShowed = actionWatched = actionOnNoAds = null;
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator WaitAndShowRewardAdCoroutine()
        {
            GlobalUIManager.Instance.GetView<BufferingView>().Show();
            yield return new WaitForSecondsRealtime(2f); // used Realtime because on AddMoreCustomer Ad Booster Feature Popup Make Timescale = 0 so we nee to run Coroutin
            GlobalUIManager.Instance.GetView<BufferingView>().Hide();

            if (baseRewardedAdHandler.IsAdLoaded())
            {
                playedLevelCount = 0;
                lastAdShowTime = Time.time;
                baseRewardedAdHandler.ShowAd(actionWatched, actionShowed, rewardAdShowCallType);
            }
            else
            {
                GlobalUIManager.Instance.GetView<UserMessagePromptView>().Show("No internet!",
                UserPromptMessageConstants.NoInternetConnection, InternetManager.Instance.noInterNetSprite, "Ok", "", null, null);
                Debug.Log("No Ad Available at this time");
                if (actionOnNoAds != null)
                    actionOnNoAds();
                ResetActions();
            }
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
