using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Tag.HexaStack
{
    public class StreakBonusManager : SerializedManager<StreakBonusManager>
    {
        #region PUBLIC_VARS

        public int CurrentStreak => playerData.currentStreak;
        public int OldStreak => playerData.oldStreak;
        public int CurrentPropellers => playerData.currentPropellers;
        public bool isPlayLoadedLevel = false;
        #endregion

        #region PRIVATE_VARS

        [SerializeField] private StreakBonusDataSO _streakBonusData;
        [SerializeField] private StreakBonusPlayerPersistantData playerData;
        private bool isInit = false;


        public static bool IsStreakBonusAnnounce
        {
            get => StreakBonusAnnounce;
            set => StreakBonusAnnounce = value;
        }
        private static bool StreakBonusAnnounce { get { return PlayerPrefs.GetInt(StreakBonusAnnounceKey, 0) == 1; } set { PlayerPrefs.SetInt(StreakBonusAnnounceKey, value ? 1 : 0); } }

        public StreakBonusDataSO StreakBonusData { get => _streakBonusData; }

        private const string StreakBonusAnnounceKey = "StreakBonusAnnouncePref";
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
            isInit = true;
        }

        public bool IsSystemActive()
        {
            if (!_streakBonusData.streakBonusDataConfig.isActive)
                return false;

            if (PlayerPersistantData.GetMainPlayerProgressData().playerGameplayLevel < _streakBonusData.streakBonusDataConfig.openAt)
                return false;

            return true;
        }
        public void OnWinLevel()
        {
            if (!IsSystemActive())
                return;

            IncrementStreak();
        }
        public void OnLossLevel()
        {
            if (!IsSystemActive())
                return;

            AnalyticsManager.Instance.LogGAEvent("StreakBrokenLevel : " + GameplayManager.Instance.CurrentHandler.GetCurrentLevelEventString());
            AnalyticsManager.Instance.LogGAEvent("StreakBrokenAtValue : " + playerData.currentStreak);
            ResetStreak();
        }
        public void OnLevelRetry()
        {
            OnLossLevel();
        }
        public void OnLevelStart(bool isPlayLoadedLevel)
        {
            this.isPlayLoadedLevel = isPlayLoadedLevel;
            if (!IsSystemActive() || isPlayLoadedLevel)
                return;

            AnalyticsManager.Instance.LogGAEvent("StreakCount : " + playerData.currentStreak);
            playerData.currentPropellers = GetCurrentStreakPropeller();
            SavePlayerData();
        }
        public void OnGoalStripShown()
        {
            if (!IsSystemActive())
                return;

            playerData.oldStreak = playerData.currentStreak;
            SavePlayerData();
        }
        public int GetCurrentStreakPropeller()
        {
            return _streakBonusData.GetCurrentStreakPropeller(playerData.currentStreak);
        }
        public int GetPropellerCount(int streak)
        {
            return _streakBonusData.GetCurrentStreakPropeller(streak);
        }
        public StreakBonusData GetCurrentStreakBonusData()
        {
            return _streakBonusData.GetCurrentStreakData(playerData.currentStreak);
        }

        public bool CanAnnounceShow()
        {
            return IsSystemActive() && !IsStreakBonusAnnounce;
        }

        public void UsePropeller()
        {
            if (playerData.currentPropellers == 0) return;

            AnalyticsManager.Instance.LogGAEvent("StreakPropellerUsed : " + playerData.currentStreak + " : " + GameplayManager.Instance.CurrentHandler.GetCurrentLevelEventString());
            playerData.currentPropellers--;
            SavePlayerData();
        }
        public bool IsMaxStreak()
        {
            return _streakBonusData.IsMaxStreak(playerData.currentStreak);
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        private void LoadPlayerData()
        {
            playerData = PlayerPersistantData.GetStreakBonusData();

            if (playerData == null)
            {
                ResetPlayerData();
            }
            SavePlayerData();
        }

        private void ResetPlayerData()
        {
            playerData = new StreakBonusPlayerPersistantData
            {
                currentStreak = 0,
            };
        }
        private void SavePlayerData()
        {
            PlayerPersistantData.SetStreakBonusData(playerData);
        }
        private void IncrementStreak()
        {
            int maxStreak = _streakBonusData.streakBonusDataConfig.streakBonusData.Last().streakCount;

            if (playerData.currentStreak >= maxStreak) return;

            playerData.currentStreak++;
            SavePlayerData();
        }
        private void ResetStreak()
        {
            if (playerData.currentStreak == 0) return;

            playerData.currentStreak = 0;
            SavePlayerData();
        }
        public BaseCell GetBeseCellForPropellerUse()
        {
            List<BaseCell> cells = LevelManager.Instance.LoadedLevel.BaseCells;
            BaseCell bestCell = null;

            for (int i = 0; i < cells.Count; i++)
            {
                if (!cells[i].CanUseBooster())
                    continue;

                if (bestCell == null)
                {
                    bestCell = cells[i];
                }
                else
                {
                    bestCell = GetBestCellBasedOnPriority(bestCell, cells[i]);
                }
            }
            return bestCell;
        }
        private BaseCell GetBestCellBasedOnPriority(BaseCell one, BaseCell two)
        {
            BaseCell bestCell = null;
            if (one != null && two != null)
            {
                if (one.GetPriority() < two.GetPriority())
                {
                    return one;
                }
                else if (one.GetPriority() > two.GetPriority())
                {
                    return two;
                }
                else
                {
                    int oneUniqueItem = one.HasItem ? one.ItemStack.GetUniqueItemCount() : 1000;
                    int twoUniqueItem = two.HasItem ? two.ItemStack.GetUniqueItemCount() : 1000;

                    if (oneUniqueItem > twoUniqueItem)
                    {
                        bestCell = one;
                    }
                    else if (twoUniqueItem > oneUniqueItem)
                    {
                        bestCell = two;
                    }
                    else
                    {
                        int oneTotoalItem = one.HasItem ? one.ItemStack.GetItems().Count : 1000;
                        int twoTotoalItem = two.HasItem ? two.ItemStack.GetItems().Count : 1000;

                        if (oneTotoalItem <= twoTotoalItem)
                        {
                            bestCell = one;
                        }
                        else
                        {
                            bestCell = two;
                        }
                    }

                    return bestCell;
                }
            }
            return bestCell;
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        #endregion
    }
}
public class StreakBonusPlayerPersistantData
{
    [JsonProperty("cs")] public int currentStreak = 0;
    [JsonProperty("os")] public int oldStreak = 0;
    [JsonProperty("cp")] public int currentPropellers = 0;
}
