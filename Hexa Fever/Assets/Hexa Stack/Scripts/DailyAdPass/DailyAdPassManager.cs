using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Tag.HexaStack
{
    public class DailyAdPassManager : SerializedManager<DailyAdPassManager>
    {
        #region PUBLIC_VARS
        public DailyAdPassDataSO DailyAdPassData { get => dailyAdPassData; }
        public DailyAdPassPlayerPersistantData PlayerData { get => playerData; }
        public SystemTimer FreeRewardTimer { get => freeRewardTimer; }
        public SystemTimer AdRewardTimer { get => adRewardTimer; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private DailyAdPassDataSO dailyAdPassData;
        [ShowInInspector] private DailyAdPassPlayerPersistantData playerData;
        private SystemTimer freeRewardTimer;
        private SystemTimer adRewardTimer;
        private bool isInit = false;

        #endregion

        #region UNITY_CALLBACKS

        public override void Awake()
        {
            base.Awake();
            OnLoadingDone();
            Initialize();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Initialize()
        {
            if (isInit)
                return;

            if (!IsSystemActive())
                return;

            LoadPlayerData();
            CheckAndResetRewards();
            InitializedTimer();
            isInit = true;
        }

        public bool IsSystemActive()
        {
            if (!dailyAdPassData.isActive)
                return false;

            if (PlayerPersistantData.GetMainPlayerProgressData().playerGameplayLevel < dailyAdPassData.openAt)
                return false;

            return true;
        }

        public bool IsFreeRewardReady()
        {
            return TimeManager.Now >= playerData.dailyPassFreeRewardEndTime;
        }

        public bool IsAdRewardReady(int index)
        {
            return playerData.adRewardClaimIndex == index;
        }

        public void OnFreeRewardClaim()
        {
            if (dailyAdPassData.freeReward.baseReward.GetCurrencyId() == CurrencyConstant.COINS)
            {
                AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, AnalyticsConstants.CoinCurrency
                        , dailyAdPassData.freeReward.baseReward.GetAmount(), AnalyticsConstants.ItemType_Reward, AnalyticsConstants.ItemId_DailyAdPassFree);
            }
            AnalyticsManager.Instance.LogGAEvent("DailyRewardClaimed : Free ");
            dailyAdPassData.freeReward.baseReward.GiveReward();
            playerData.dailyPassFreeRewardEndTime = TimeManager.Now.AddSeconds(dailyAdPassData.refreshFreeRewardResetTimeInSecond);
            if (playerData.dailyPassFreeRewardEndTime >= playerData.dailyPassEndTime)
            {
                playerData.dailyPassFreeRewardEndTime = playerData.dailyPassEndTime;
            }
            if (playerData.dailyPassFreeRewardEndTime > TimeManager.Now)
            {
                freeRewardTimer.StartSystemTimer(playerData.dailyPassFreeRewardEndTime, OnFreeRewardTimeOver);
            }
            SavePlayerData();
        }

        public void OnAdRewardClaim(int index)
        {
            if (dailyAdPassData.adReward[index].baseReward.GetCurrencyId() == CurrencyConstant.COINS)
            {
                AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, AnalyticsConstants.CoinCurrency
                        , dailyAdPassData.adReward[index].baseReward.GetAmount(), AnalyticsConstants.ItemType_AdReward, AnalyticsConstants.ItemId_DailyAdPassAd);
            }
            AnalyticsManager.Instance.LogGAEvent("DailyRewardClaimed : AD : " + index);
            dailyAdPassData.adReward[index].baseReward.GiveReward();
            playerData.adRewardClaimIndex++;
            SavePlayerData();
        }

        public int GetCliamRewardCount()
        {
            int rewardCount = 0;

            if (IsFreeRewardReady())
                rewardCount = 1;

            rewardCount += (dailyAdPassData.adReward.Count - playerData.adRewardClaimIndex);

            return rewardCount;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void LoadPlayerData()
        {
            playerData = PlayerPersistantData.GetDailyAdPassData();

            if (playerData == null)
            {
                ResetPlayerData();
            }
        }

        private void ResetPlayerData()
        {
            playerData = new DailyAdPassPlayerPersistantData
            {
                dailyPassEndTime = GetDailyPassEndTime(),
                dailyPassFreeRewardEndTime = DateTime.MinValue,
                adRewardClaimIndex = 0,
            };

            SavePlayerData();
        }

        private void SavePlayerData()
        {
            PlayerPersistantData.SetDailyAdPassData(playerData);
        }

        private void CheckAndResetRewards()
        {
            if (TimeManager.Now >= playerData.dailyPassEndTime)
            {
                ResetPlayerData();
            }
        }

        private void InitializedTimer()
        {
            if (freeRewardTimer == null)
                freeRewardTimer = new SystemTimer();

            if (adRewardTimer == null)
                adRewardTimer = new SystemTimer();

            freeRewardTimer.ResetTimerObject();
            adRewardTimer.ResetTimerObject();

            adRewardTimer.StartSystemTimer(playerData.dailyPassEndTime, OnAdRewardTimeOver);
            if (playerData.dailyPassFreeRewardEndTime > TimeManager.Now)
            {
                freeRewardTimer.StartSystemTimer(playerData.dailyPassFreeRewardEndTime, OnFreeRewardTimeOver);
                RaiseOnFreeRewardRunTimerOver();
            }
        }

        private void OnFreeRewardTimeOver()
        {
            RaiseOnFreeRewardRunTimerOver();
            playerData.dailyPassFreeRewardEndTime = TimeManager.Now;
            SavePlayerData();
        }

        private void OnAdRewardTimeOver()
        {
            ResetPlayerData();
            RaiseOnDailyAdPassRunTimerOver();
            adRewardTimer.ResetTimerObject();
            CoroutineRunner.Instance.Wait(0.1f, () =>
            {
                adRewardTimer.StartSystemTimer(playerData.dailyPassEndTime, OnAdRewardTimeOver);
            });
        }

        private DateTime GetDailyPassEndTime()
        {
            DateTime currentTime = DateTime.Now;
            int slotDurationHours = dailyAdPassData.allRewardResetTimeInhours;

            // Calculate how many complete slots have passed today
            int currentHour = currentTime.Hour;
            int currentSlot = currentHour / slotDurationHours;

            // Calculate the next slot's end time
            int nextSlotEndHour = (currentSlot + 1) * slotDurationHours;

            // Create DateTime for today's date with the next slot's end time
            DateTime endTime = currentTime.Date.AddHours(nextSlotEndHour);

            // If we're already past the last slot of the day, move to first slot of next day
            if (currentTime >= endTime)
            {
                endTime = endTime.AddHours(slotDurationHours);
            }

            return endTime;
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        public delegate void DailyAdPassVoidEvent();

        public static event DailyAdPassVoidEvent onFreeRewardTimerRunTimerOver;

        public static void RaiseOnFreeRewardRunTimerOver()
        {
            onFreeRewardTimerRunTimerOver?.Invoke();
        }

        public static event DailyAdPassVoidEvent onDailyAdPassRunTimerOver;

        public static void RaiseOnDailyAdPassRunTimerOver()
        {
            onDailyAdPassRunTimerOver?.Invoke();
        }

        #endregion

        #region UI_CALLBACKS

        #endregion
    }

    public class DailyAdPassPlayerPersistantData
    {
        [JsonProperty("dpet")] public DateTime dailyPassEndTime;
        [JsonProperty("dpfret")] public DateTime dailyPassFreeRewardEndTime;
        [JsonProperty("arci")] public int adRewardClaimIndex = 0;
    }

    public class DailyAdPassRewardSloatData
    {
        [JsonProperty("ic")] public bool isClaim;

        public DailyAdPassRewardSloatData()
        {
            isClaim = false;
        }
    }
}
