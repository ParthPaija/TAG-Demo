using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "DailyDealsDataRemoteConfig", menuName = Constant.GAME_NAME + "/Remote Config Data/DailyDealsDataRemoteConfig")]
    public class DailyDealsDataSO : BaseConfig
    {
        #region PUBLIC_VARIABLES

        public DailyDealsRemoteConfig dailyDealsRemoteConfig;
        public Dictionary<int, List<DailyDealSlotConfig>> dailyDealSlots = new Dictionary<int, List<DailyDealSlotConfig>>();

        #endregion

        #region PRIVATE_VARIABLES

        #endregion

        #region PROPERTIES

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_METHODS

        public override void Init(string configString)
        {
            base.Init(configString);
            dailyDealsRemoteConfig = GetValue<DailyDealsRemoteConfig>();
        }

        public override string GetDefaultString()
        {
            return SerializeUtility.SerializeObject(dailyDealsRemoteConfig);
        }

        public List<DailyDealSlotConfig> GetRandomDealsFromEachSlot(DailyDealsPlayerData dailyDealsPlayerData)
        {
            List<DailyDealSlotConfig> randomDeals = new List<DailyDealSlotConfig>();
            // Keep track of already selected rewards to prevent them from appearing in other deals
            HashSet<int> selectedRewardIds = new HashSet<int>();

            if (dailyDealsPlayerData == null || dailyDealsPlayerData.slots == null || dailyDealsPlayerData.slots.Count == 0)
            {
                for (int i = 0; i < dailyDealSlots.Count; i++)
                {
                    List<DailyDealSlotConfig> slotConfigs = dailyDealSlots[i];
                    List<DailyDealSlotConfig> availableConfigs = new List<DailyDealSlotConfig>();

                    // Filter out configs with rewards that have already been selected in other deals
                    foreach (var config in slotConfigs)
                    {
                        if (!selectedRewardIds.Contains(config.dealPurchaseData.reward.GetCurrencyId()))
                        {
                            availableConfigs.Add(config);
                        }
                    }

                    // If no available configs, use all configs for this slot (fallback)
                    if (availableConfigs.Count == 0)
                    {
                        availableConfigs = slotConfigs;
                    }

                    DailyDealSlotConfig randomDeal = availableConfigs[UnityEngine.Random.Range(0, availableConfigs.Count)];
                    randomDeals.Add(randomDeal);

                    // Add the selected reward to the tracking set
                    selectedRewardIds.Add(randomDeal.dealPurchaseData.reward.GetCurrencyId());
                }
                return randomDeals;
            }

            for (int i = 0; i < dailyDealsPlayerData.slots.Count; i++)
            {
                DailyDealSlotPlayerData slotPlayerData = dailyDealsPlayerData.slots[i];
                if (dailyDealSlots.ContainsKey(i))
                {
                    List<DailyDealSlotConfig> slotConfigs = dailyDealSlots[i];
                    List<int> prefsId = new List<int>();
                    foreach (var item in slotConfigs)
                    {
                        if (item.prefsId != slotPlayerData.id)
                        {
                            // Only add this config if its reward hasn't been selected in previous deals
                            if (!selectedRewardIds.Contains(item.dealPurchaseData.reward.GetCurrencyId()))
                            {
                                prefsId.Add(item.prefsId);
                            }
                        }
                    }

                    // If no available configs, use configs that only avoid the previous day's deal
                    if (prefsId.Count == 0)
                    {
                        foreach (var item in slotConfigs)
                        {
                            if (item.prefsId != slotPlayerData.id)
                            {
                                prefsId.Add(item.prefsId);
                            }
                        }
                    }

                    int id = prefsId[UnityEngine.Random.Range(0, prefsId.Count)];
                    DailyDealSlotConfig selectedDeal = slotConfigs.Find(x => x.prefsId == id);
                    randomDeals.Add(selectedDeal);

                    // Add the selected reward to the tracking set
                    selectedRewardIds.Add(selectedDeal.dealPurchaseData.reward.GetCurrencyId());
                }
            }
            return randomDeals;
        }

        public List<DailyDealSlotConfig> GetDealsForSlot(DailyDealsPlayerData dailyDealsPlayerData)
        {
            List<DailyDealSlotConfig> randomDeals = new List<DailyDealSlotConfig>();
            for (int i = 0; i < dailyDealsPlayerData.slots.Count; i++)
            {
                DailyDealSlotPlayerData slotPlayerData = dailyDealsPlayerData.slots[i];
                if (dailyDealSlots.ContainsKey(i))
                {
                    for (int j = 0; j < dailyDealSlots[i].Count; j++)
                    {
                        DailyDealSlotConfig slotConfig = dailyDealSlots[i][j];
                        if (slotConfig.prefsId == slotPlayerData.id)
                        {
                            randomDeals.Add(slotConfig);
                            break;
                        }
                    }
                }
            }
            return randomDeals;
        }

        #endregion

        #region PRIVATE_METHODS

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region COROUTINES

        #endregion

        #region UI_CALLBACKS

        #endregion
    }

    public enum DealType
    {
        Free,
        Ad,
        Currency
    }

    public class DailyDealsRemoteConfig
    {
        public bool isActive;
        public float refreshTimeInSeconds = 86400f;
    }
    public class DailyDealSlotConfig
    {
        public int prefsId;
        public BaseDailyDealPurchaseData dealPurchaseData;
    }

    public abstract class BaseDailyDealPurchaseData
    {
        public virtual DealType dealType => DealType.Free;
        public BaseReward reward;

        protected Action onPurchase;
        public virtual bool CanPurchase() { return false; }

        public virtual void OnPurchase(Action action)
        {
            onPurchase = action;
        }
    }

    public class FreeDailyDealPurchaseData : BaseDailyDealPurchaseData
    {
        public override DealType dealType => DealType.Free;

        public override bool CanPurchase()
        {
            return true;
        }

        public override void OnPurchase(Action action)
        {
            base.OnPurchase(action);
            if (onPurchase != null)
            {
                onPurchase.Invoke();
            }
            if (reward.GetCurrencyId() == CurrencyConstant.COINS)
            {
                AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, AnalyticsConstants.CoinCurrency
                        , reward.GetAmount(), AnalyticsConstants.ItemType_Reward, AnalyticsConstants.ItemId_DailyDeals);
            }
            AnalyticsManager.Instance.LogGAEvent("DailyDealsFreeClaim : " + reward.GetName());
        }
    }
    public class AdDailyDealPurchaseData : BaseDailyDealPurchaseData
    {
        public override DealType dealType => DealType.Ad;

        public override bool CanPurchase()
        {
            return AdManager.Instance.IsRewardedAdLoad();
        }

        public override void OnPurchase(Action action)
        {
            base.OnPurchase(action);
            AdManager.Instance.ShowRewardedAd(() =>
            {
                onPurchase.Invoke();
                AnalyticsManager.Instance.LogGAEvent("DailyDealsAdClaim : " + reward.GetName());
                if (reward.GetCurrencyId() == CurrencyConstant.COINS)
                {
                    AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, AnalyticsConstants.CoinCurrency
                            , reward.GetAmount(), AnalyticsConstants.ItemType_AdReward, AnalyticsConstants.ItemId_DailyDeals);
                }
            }, RewardAdShowCallType.DailyDeals, "DailyDeals");
        }
    }

    public class CurrencyDailyDealPurchaseData : BaseDailyDealPurchaseData
    {
        public override DealType dealType => DealType.Currency;
        public BaseReward costReward;

        public override bool CanPurchase()
        {
            if (DataManager.Instance.GetCurrency(costReward.GetCurrencyId()).IsSufficentValue(costReward.GetAmount()))
            {
                return true;
            }
            GlobalUIManager.Instance.GetView<ToastMessageView>().ShowMessage("Not Enough Coins");
            return false;
        }

        public override void OnPurchase(Action action)
        {
            base.OnPurchase(action);
            costReward.RemoveReward();
            AnalyticsManager.Instance.LogGAEvent("DailyDealsCoinClaim : " + reward.GetName());
            if (reward.GetCurrencyId() == CurrencyConstant.COINS)
            {
                AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, AnalyticsConstants.CoinCurrency
                        , reward.GetAmount(), AnalyticsConstants.ItemType_Reward, AnalyticsConstants.ItemId_DailyDeals);
            }
            if (costReward.GetCurrencyId() == CurrencyConstant.COINS)
            {
                AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Sink, AnalyticsConstants.CoinCurrency
                        , costReward.GetAmount(), AnalyticsConstants.ItemType_Trade, AnalyticsConstants.ItemId_DailyDeals);
            }
            if (onPurchase != null)
            {
                onPurchase.Invoke();
            }
        }
    }
}