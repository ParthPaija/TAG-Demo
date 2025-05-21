using UnityEngine;
using System.Collections.Generic;
using System;
using Tag.Ad;
using Sirenix.OdinInspector;

namespace Tag.HexaStack
{
    public class AdManager : SerializedManager<AdManager>
    {
        #region PUBLIC_VARS

        public AdManagerDataSO adManagerDataSO;
        public BaseAd baseAd;
        public RewardAdShowCallType rewardAdShowCallType;
        public static bool isNoAdLoadedIconDeactiveEnable = true;
        public bool isCMPOn = false;

        public AdConfigData AdConfigData => myAdConfigData;
        public string AdNameType => adNameType;

        public bool isApplovinTstMode = false;

        #endregion

        #region PRIVATE_VARS

        [SerializeField] public AdConfigData myAdConfigData = new AdConfigData();
        [SerializeField] private AdsDataRemoteConfig AdsDataRemoteConfig;
        private List<Action> onAdLoad = new List<Action>();
        private const string PrefsKeyConsent = "PkConsent";
        private string adNameType = "Init";

        private AdManagerPlayerData _adManagerPlayerData
        {
            get { return SerializeUtility.DeserializeObject<AdManagerPlayerData>(AdManagerDataString); }
            set { AdManagerDataString = SerializeUtility.SerializeObject(value); }
        }
        private string AdManagerDataString
        {
            get { return PlayerPrefs.GetString(AdManagerDataPrefsKey, SerializeUtility.SerializeObject(GetDefaultAdManagerPlayerData())); }
            set { PlayerPrefs.SetString(AdManagerDataPrefsKey, value); }
        }

        public bool IsBannerAdActive { get { return CanShowBannerAd(); } }

        public bool canShowBannerInEditor = false;

        private const string AdManagerDataPrefsKey = "AdManagerPlayerData";
        private bool isShowCallSuccess = false;
        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            GameAnalyticsManager.onRCValuesFetched += GameAnalyticsManager_onRCValuesFetched;
        }

        private void OnDisable()
        {
            GameAnalyticsManager.onRCValuesFetched -= GameAnalyticsManager_onRCValuesFetched;
        }

        public override void Awake()
        {
            base.Awake();
            OnInitializeAdManager();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void OnInitializeAdManager()
        {
            SetAdRCData();

            baseAd.gameObject.SetActive(true);
            baseAd.Init(OnLoadingDone);
        }

        public void ShowInterstitial(InterstatialAdPlaceType interstatialAdPlaceType, string adSourceName)
        {
            if (!Constant.IsAdOn || IsNoAdsPurchased())
                return;

            this.adNameType = adSourceName;
            baseAd.ShowInterstitial(interstatialAdPlaceType);
        }

        public void ShowInterstitialWithoutConditions(InterstatialAdPlaceType interstatialAdPlaceType, string adSourceName)
        {
            if (!Constant.IsAdOn || IsNoAdsPurchased())
                return;

            this.adNameType = adSourceName;
            baseAd.ShowInterstitialWithoutConditions(interstatialAdPlaceType);
        }

        public void ShowBannerAd()
        {
            if (!Constant.IsAdOn || !CanShowBannerAd() || IsNoAdsPurchased())
            {
                isShowCallSuccess = false;
                return;
            }

            if (isShowCallSuccess)
                return;

            Debug.LogError("Show BANNER");

            baseAd.ShowBannerAd();
            isShowCallSuccess = true;
        }

        public bool IsBannerAdLoaded()
        {
            return baseAd.IsBannerAdLoaded();
        }

        public void HideBannerAd()
        {
            baseAd.HideBannerAd();
        }

        public bool CanShowBannerAd()
        {
            if (DataManager.PlayerData.IsNoAdsActive())
                return false;
            return MainSceneUIManager.Instance != null &&
                PlayerPersistantData.GetMainPlayerProgressData().playerGameplayLevel >= AdConfigData.showBannerAdsAfterLevel;
        }

        public bool IsNoAdsPurchased()
        {
            return DataManager.PlayerData.IsNoAdsActive();
        }

        public void ShowRewardedAd(Action actionWatched, RewardAdShowCallType rewardAdShowCallType, string adSourceName, Action actionShowed = null, Action actionOnNoAds = null)
        {
            AnalyticsManager.Instance.LogGAAdClickEvent(rewardAdShowCallType);
            if (!Constant.IsAdOn)
            {
                actionWatched.Invoke();
                return;
            }

            if (!IsInternetAvailable())
            {
                GlobalUIManager.Instance.GetView<UserMessagePromptView>().Show("No internet!",
                UserPromptMessageConstants.NoInternetConnection, InternetManager.Instance.noInterNetSprite, "Ok", "", null, null);
                actionOnNoAds?.Invoke();
                return;
            }

#if UNITY_EDITOR
            actionWatched.Invoke();
            return;
#elif UNITY_ANDROID && !UNITY_EDITOR
            this.adNameType = adSourceName;
            this.rewardAdShowCallType = rewardAdShowCallType;
            baseAd.ShowRewardedVideo(actionWatched, actionShowed, actionOnNoAds, rewardAdShowCallType);
#endif
        }

        public void OnRewardedAdShowed()
        {
            var adManagerData = _adManagerPlayerData;
            adManagerData.currentShowedRewardedAdsCount++;
            _adManagerPlayerData = adManagerData;
        }

        public bool IsRewardedAdLoad()
        {
            return baseAd.baseRewardedAdHandler.IsAdLoaded();
        }

        public bool IsInternetAvailable()
        {
            return InternetManager.Instance.IsReachableToNetwork();
        }

        public void SetAdRCData()
        {
            myAdConfigData = AdsDataRemoteConfig.GetValue<AdConfigData>();
        }

        public void AddListenerMouseButtonDown(Action action)
        {
            if (!onAdLoad.Contains(action))
                onAdLoad.Add(action);
        }

        public void RemoveListenerMouseButtonDown(Action action)
        {
            if (onAdLoad.Contains(action))
                onAdLoad.Remove(action);
        }

        public void InvakeOnAdLoad()
        {
            foreach (var ev in onAdLoad)
            {
                ev?.Invoke();
            }
        }

        internal float GetBannerAdHeight()
        {
            if (!CanShowBannerUI())
                return 0;
#if UNITY_EDITOR
            return 180;
#else
            float bannerHeight = 150;
            float canvasHeight = 1920;
            float screenHight = SafeAreaUtility.GetResolution().y;
            return (bannerHeight * screenHight / canvasHeight);
#endif
        }

        public bool CanShowBannerUI()
        {
            if (DataManager.PlayerData.IsNoAdsActive())
                return false;
            //return MainSceneUIManager.Instance != null &&
            //    PlayerPersistantData.GetMainPlayerProgressData().playerGameplayLevel >= AdConfigData.showBannerAdsAfterLevel;
            return true;
        }

        public void OnLevelPlayed()
        {
            baseAd.OnLevelPlayed();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private AdManagerPlayerData GetDefaultAdManagerPlayerData()
        {
            AdManagerPlayerData adManagerPlayerData = new AdManagerPlayerData();
            adManagerPlayerData.currentShowedRewardedAdsCount = 0;
            return adManagerPlayerData;
        }

        private void Init()
        {
            if (!IsAskedForConsent())
            {
            }
        }

        private void SetConsent()
        {
            PlayerPrefs.SetInt(PrefsKeyConsent, 1);
        }

        private bool IsAskedForConsent()
        {
            return PlayerPrefs.HasKey(PrefsKeyConsent);
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        private void GameAnalyticsManager_onRCValuesFetched()
        {
            SetAdRCData();
        }

        #endregion

        #region UI_CALLBACKS

        #endregion

        #region EDITOR_FUNCTIONS
        [Button]
        public void Editor_DebugAdManagerPlayerData()
        {
            Debug.Log(SerializeUtility.SerializeObject(_adManagerPlayerData));
        }

        [Button]
        public void Editor_OnRewardAdShowed()
        {
            OnRewardedAdShowed();
        }


        #endregion
    }

    public enum RewardAdShowCallType
    {
        None = 0,
        HammerBooster = 1,
        SwapBooster = 2,
        RefreshBooster = 3,
        UndoBooster = 4,
        AdLocker = 5,
        OutOfSpace = 6,
        AdLife = 7,
        DoubleCoin = 8,
        DailyReward = 9,
        DailyDeals = 10
    }

    public class AdConfigData
    {
        public AdConfigData()
        {
            interstitialAdConfigDatas = new List<InterstitialAdConfigData>();
        }

        public List<InterstitialAdConfigData> interstitialAdConfigDatas = new List<InterstitialAdConfigData>();
        public int showBannerAdsAfterLevel = 0;
        public int showBannerAdsAfterDays = 0;

        public bool CanShowBannerAd()
        {
            DateTime firstSessionStartDT = DataManager.Instance.FirstSessionStartDateTime;
            var timeDuration = CustomTime.GetCurrentTime() - firstSessionStartDT;

            if (timeDuration.TotalDays < showBannerAdsAfterDays)
                return false;

            int currentPlayerLevel = PlayerPersistantData.GetMainPlayerProgressData().playerGameplayLevel;
            return currentPlayerLevel >= showBannerAdsAfterLevel;
        }

        public bool CanShowInterstitialAd(InterstatialAdPlaceType placeType)
        {
            int currentPlayerLevel = PlayerPersistantData.GetMainPlayerProgressData().playerGameplayLevel;
            InterstitialAdConfigData interstitialAdConfigData = interstitialAdConfigDatas.Find(x => x.interstatialAdPlaceType == placeType);
            if (interstitialAdConfigData != null)
                return interstitialAdConfigData.startLevel <= currentPlayerLevel;

            return true;
        }

        public int GetInterstitialAdStartLevel(InterstatialAdPlaceType placeType)
        {
            InterstitialAdConfigData interstitialAdConfigData = interstitialAdConfigDatas.Find(x => x.interstatialAdPlaceType == placeType);
            if (interstitialAdConfigData != null)
                return interstitialAdConfigData.startLevel;

            return 1;
        }

        public int GetShowInterstitialAdIntervalLevel(InterstatialAdPlaceType placeType)
        {
            DateTime firstSessionStartDT = DataManager.Instance.FirstSessionStartDateTime;
            var timeDuration = CustomTime.GetCurrentTime() - firstSessionStartDT;

            InterstitialAdConfigData interstitialAdConfigData = interstitialAdConfigDatas.Find(x => x.interstatialAdPlaceType == placeType);
            if (interstitialAdConfigData != null)
            {
                int levelInterval = interstitialAdConfigData.GetLevelInterval((int)timeDuration.TotalDays);
                Debug.Log($"<color=yellow>Interstitial Ad : Time difference : {timeDuration.TotalDays.ToString("F2")} config Level : {levelInterval}</color>");
                return levelInterval;
            }

            Debug.Log($"<color=yellow>Interstitial Ad : Time difference : {timeDuration.TotalHours.ToString("F2")} No Config found !");
            return 1;
        }

        public float GetShowInterstitialAdIntervalTime(InterstatialAdPlaceType placeType)
        {
            DateTime firstSessionStartDT = DataManager.Instance.FirstSessionStartDateTime;
            var timeDuration = CustomTime.GetCurrentTime() - firstSessionStartDT;

            InterstitialAdConfigData interstitialAdConfigData = interstitialAdConfigDatas.Find(x => x.interstatialAdPlaceType == placeType);
            if (interstitialAdConfigData != null)
            {
                float timeInterval = interstitialAdConfigData.GetTimeInterval(timeDuration.TotalDays);
                Debug.Log($"<color=yellow>Interstitial Ad : Time difference : {timeDuration.TotalDays.ToString("F2")} config Level : {timeInterval}</color>");
                return timeInterval;
            }

            Debug.Log($"<color=yellow>Interstitial Ad : Time difference : {timeDuration.TotalHours.ToString("F2")} No Config found !");
            return 100;
        }
    }

    public class InterstitialAdConfigData
    {
        public InterstatialAdPlaceType interstatialAdPlaceType;
        public int startLevel;
        public List<AdTimeIntervalLevelConfigData> adConfigDatas;

        public int GetLevelInterval(int currentNumberOfDays)
        {
            Debug.LogError("Days :- " + currentNumberOfDays);
            for (int i = 0; i < adConfigDatas.Count; i++)
            {
                if (adConfigDatas[i].numberOfDays >= currentNumberOfDays)
                {
                    Debug.LogError("Days If :-" + adConfigDatas[i].numberOfDays);
                    return adConfigDatas[i].levelInterval;
                }
                else
                {
                    Debug.LogError("Days Else :-" + adConfigDatas[i].numberOfDays);
                    currentNumberOfDays -= adConfigDatas[i].numberOfDays;
                }
            }

            return adConfigDatas.GetLastItemFromList().levelInterval;
        }

        public float GetTimeInterval(double currentNumberOfDays)
        {
            for (int i = 0; i < adConfigDatas.Count; i++)
            {
                if (adConfigDatas[i].numberOfDays >= currentNumberOfDays)
                    return adConfigDatas[i].timeInSecond;
                else
                    currentNumberOfDays -= adConfigDatas[i].numberOfDays;
            }

            return adConfigDatas.GetLastItemFromList().timeInSecond;
        }
    }

    public class AdTimeIntervalLevelConfigData
    {
        public int numberOfDays;
        public int levelInterval;
        public float timeInSecond;
    }

    public class AdManagerPlayerData
    {
        public string lastRewardsAdsCountResetTime;
        public int currentShowedRewardedAdsCount;
    }

    public enum InterstatialAdPlaceType
    {
        Game_Win_Screen = 0,
        Reload_Level = 1,
        Ad_Break = 2
    }
}