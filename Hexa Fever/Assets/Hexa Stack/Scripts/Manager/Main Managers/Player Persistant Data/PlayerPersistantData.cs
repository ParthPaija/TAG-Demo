using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tag.HexaStack
{
    public static class PlayerPersistantData
    {
        #region PUBLIC_VARIABLES

        #endregion

        #region PRIVATE_VARIABLES

        private static PersistantVariable<MainPlayerProgressData> _mainPlayerProgressData = new PersistantVariable<MainPlayerProgressData>(PlayerPrefsKeys.Main_Player_Progress_Data_Key, null);
        private static PersistantVariable<TutorialsPlayerData> _tutorialsPlayerData = new PersistantVariable<TutorialsPlayerData>(PlayerPrefsKeys.Tutorial_Player_Data_Key, null);
        private static PersistantVariable<LevelProgressData> _levelProgressData = new PersistantVariable<LevelProgressData>(PlayerPrefsKeys.Level_Progress_Data_Key, null);
        private static PersistantVariable<VIPLeaderBoardPlayerPersistantData> _leaderboardPlayerData = new PersistantVariable<VIPLeaderBoardPlayerPersistantData>(PlayerPrefsKeys.VIP_Leaderboard_Player_Data_Key, null);
        private static PersistantVariable<GameStatsPlayerPersistantData> _gameStatsPlayerData = new PersistantVariable<GameStatsPlayerPersistantData>(PlayerPrefsKeys.GameStats_Player_Data_Key, null);
        private static PersistantVariable<PlayerProfileData> _playerProfileData = new PersistantVariable<PlayerProfileData>(PlayerPrefsKeys.Player_Profile_Data_Key, null);
        private static PersistantVariable<DailyAdPassPlayerPersistantData> _dailyAdPassData = new PersistantVariable<DailyAdPassPlayerPersistantData>(PlayerPrefsKeys.DailyAdPass_Data_Key, null);
        private static PersistantVariable<CommonOfferPlayerPersistantData> _commonOfferData = new PersistantVariable<CommonOfferPlayerPersistantData>(PlayerPrefsKeys.CommonOffer_Data_Key, null);
        private static Dictionary<int, Currency> _currencyDict = new Dictionary<int, Currency>();
        private static PersistantVariable<StreakBonusPlayerPersistantData> _streakBonusData = new PersistantVariable<StreakBonusPlayerPersistantData>(PlayerPrefsKeys.StreakBonus_Data_Key, null);

        #endregion

        #region PROPERTIES
        #endregion

        #region UNITY_CALLBACKS
        #endregion

        #region PUBLIC_METHODS
        public static MainPlayerProgressData GetMainPlayerProgressData()
        {
            return _mainPlayerProgressData.Value;
        }

        public static LevelProgressData GetLevelProgressData()
        {
            return _levelProgressData.Value;
        }

        public static void SetMainPlayerProgressData(MainPlayerProgressData mainPlayerProgressData)
        {
            _mainPlayerProgressData.Value = mainPlayerProgressData;
        }

        public static void SetLevelProgressData(LevelProgressData levelProgressData)
        {
            _levelProgressData.Value = levelProgressData;
        }

        public static VIPLeaderBoardPlayerPersistantData GetVIPLeaderboardPlayerData()
        {
            return _leaderboardPlayerData.Value;
        }

        public static void SetVIPLeaderboardPlayerData(VIPLeaderBoardPlayerPersistantData leaderboardPlayerData)
        {
            _leaderboardPlayerData.Value = leaderboardPlayerData;
        }

        public static PlayerProfileData GetPlayerProfileData()
        {
            return _playerProfileData.Value;
        }

        public static void SetPlayerProfileData(PlayerProfileData playerProfileData)
        {
            _playerProfileData.Value = playerProfileData;
        }

        public static DailyAdPassPlayerPersistantData GetDailyAdPassData()
        {
            return _dailyAdPassData.Value;
        }

        public static void SetDailyAdPassData(DailyAdPassPlayerPersistantData dailyAdPassData)
        {
            _dailyAdPassData.Value = dailyAdPassData;
        }

        public static CommonOfferPlayerPersistantData GetCommonOfferData()
        {
            return _commonOfferData.Value;
        }

        public static void SetCommonOfferData(CommonOfferPlayerPersistantData commonOfferData)
        {
            _commonOfferData.Value = commonOfferData;
        }

        public static GameStatsPlayerPersistantData GetGameStatsPlayerData()
        {
            return _gameStatsPlayerData.Value;
        }

        public static void SetGameStatsPlayerData(GameStatsPlayerPersistantData gameStatsPlayerData)
        {
            _gameStatsPlayerData.Value = gameStatsPlayerData;
        }

        public static TutorialsPlayerData GetTutorialsPlayerPersistantData()
        {
            return _tutorialsPlayerData.Value;
        }

        public static void SetTutorialsPlayerPersistantData(TutorialsPlayerData tutorialsPlayerData)
        {
            _tutorialsPlayerData.Value = tutorialsPlayerData;
        }

        public static Dictionary<int, Currency> GetCurrancyDictionary()
        {
            return _currencyDict;
        }

        public static void SetCurrancyDictionary(Dictionary<int, Currency> currencyDict)
        {
            _currencyDict = currencyDict;
        }

        public static StreakBonusPlayerPersistantData GetStreakBonusData()
        {
            return _streakBonusData.Value;
        }

        public static void SetStreakBonusData(StreakBonusPlayerPersistantData streakBonusData)
        {
            _streakBonusData.Value = streakBonusData;
        }


        // For Playfab Use Only >>>
        public static Dictionary<string, string> GetPlayerPersistantCurrancyData()
        {
            Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
            foreach (var pair in _currencyDict)
            {
                dataDictionary.Add(pair.Value.key, pair.Value.Value.ToString());
            }
            return dataDictionary;
        }

        // For Playfab Use Only >>>
        public static void SetPlayerPersistantCurrancyData(Dictionary<string, string> currancyData)
        {
            foreach (var pair in currancyData)
            {
                foreach (var values in _currencyDict.Values)
                {
                    if (values.key == pair.Key && int.TryParse(pair.Value, out int currancyVal))
                    {
                        values.SetValue(currancyVal);
                        break;
                    }
                }
            }
        }

        public static Dictionary<string, string> GetPlayerPrefsData()
        {
            Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
            dataDictionary.Add(PlayerPrefsKeys.Currancy_Data_Key, SerializeUtility.SerializeObject(GetPlayerPersistantCurrancyData()));
            dataDictionary.Add(_mainPlayerProgressData._key, _mainPlayerProgressData.RawValue);
            //dataDictionary.Add(_tutorialsPlayerData._key, _tutorialsPlayerData.RawValue);
            dataDictionary.Add(_levelProgressData._key, _levelProgressData.RawValue);
            return dataDictionary;
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

    #region MAIN_PLAYER_PROGRESS_DATA

    public class MainPlayerProgressData
    {
        [JsonProperty("pglev")] public int playerGameplayLevel;
        [JsonProperty("pws")] public int winStreak;
        [JsonProperty("pls")] public int loseStreak;
        [JsonProperty("naet")] public DateTime noAdsEndTime = DateTime.MinValue;

        public void AddNoAdsPurchase(int minutes)
        {
            if (noAdsEndTime >= DateTime.Now)
            {
                noAdsEndTime = noAdsEndTime.AddMinutes(minutes);
            }
            else
            {
                noAdsEndTime = DateTime.Now;
                noAdsEndTime = noAdsEndTime.AddMinutes(minutes);
            }
            CoroutineRunner.Instance.Wait(0.1f, () =>
            {
                AdManager.Instance.HideBannerAd();
                if (RemoveAdsHandler.Instance != null)
                    RemoveAdsHandler.Instance.OnRemoveAdsPurchase();
            });
        }

        public bool IsNoAdsActive()
        {
            return noAdsEndTime.Subtract(DateTime.Now).TotalSeconds > 0;
        }

        public void OnLevelWin(bool isVIP = false)
        {
            if (playerGameplayLevel > SpawnAlgoConstant.forceEasyModeLevel)
            {
                winStreak++;
                loseStreak = 0;
            }
            if (!isVIP)
                playerGameplayLevel++;
        }

        public void OnLevelFail()
        {
            if (playerGameplayLevel > SpawnAlgoConstant.forceEasyModeLevel)
            {
                loseStreak++;
                winStreak = 0;
            }
        }
    }

    public class CurrencyMappingData
    {
        [JsonProperty("cid"), CurrencyId] public int currencyID;
        [JsonProperty("cur")] public Currency currency;
    }

    public class LevelProgressData
    {
        [JsonProperty("ln")] public int levelNo = -1;
        //Cell Id and Cell CurrentItem
        [JsonProperty("gd")] public Dictionary<int, List<int>> gridData = new Dictionary<int, List<int>>();
        [JsonProperty("sd")] public Dictionary<int, List<int>> spawnerData = new Dictionary<int, List<int>>();
        [JsonProperty("od")] public Dictionary<int, string> obstacalData = new Dictionary<int, string>();
        [JsonProperty("gr")] public float gridRotation;
        [JsonProperty("cgv")] public List<int> currentGoalValue = new List<int>();
        [JsonProperty("si")] public int setIndex = 0;
        [JsonProperty("crc")] public int currentReviveCountCoin = 0;
        [JsonProperty("cra")] public int currentReviveCountAd = 0;
        [JsonProperty("crt")] public int currentRunningTime;
        [JsonProperty("atwc")] public int adTileWatchCount;
        //Booster Id and count
        [JsonProperty("bud")] public Dictionary<int, int> boosterUseData = new Dictionary<int, int>();

        public void OnLevelStart(int level)
        {
            if (levelNo != level)
            {
                ReSetData();
                levelNo = level;
            }
        }

        public void OnLevelEnd()
        {
            ReSetData();
        }

        public void ReSetData()
        {
            levelNo = -1;
            gridData.Clear();
            gridRotation = 0;
            currentGoalValue.Clear();
            spawnerData.Clear();
            setIndex = 0;
            obstacalData.Clear();
            currentReviveCountCoin = 0;
            currentRunningTime = 0;
            currentReviveCountAd = 0;
            adTileWatchCount = 0;
            boosterUseData.Clear();
        }

        public List<int> GetCellData(int cellId)
        {
            if (gridData.ContainsKey(cellId))
            {
                return gridData[cellId];
            }
            return null;
        }

        public string GetObstacalData(int cellId)
        {
            if (obstacalData.ContainsKey(cellId))
            {
                return obstacalData[cellId];
            }
            return "";
        }

        public void UpdateObstacalCellData(int cellId, string data)
        {
            if (obstacalData.ContainsKey(cellId))
            {
                obstacalData[cellId] = data;
                return;
            }
            obstacalData.Add(cellId, data);
        }

        public void UpdateGridCellData(int cellId, List<int> items)
        {
            if (gridData.ContainsKey(cellId))
            {
                gridData[cellId].Clear();
                gridData[cellId].AddRange(items);
                return;
            }
            gridData.Add(cellId, items);
        }

        public List<int> GetSpwanerData(int index)
        {
            if (spawnerData.ContainsKey(index))
            {
                return spawnerData[index];
            }
            return null;
        }

        public void UpdateSpwanerData(int cellId, List<int> items)
        {
            if (spawnerData.ContainsKey(cellId))
            {
                spawnerData[cellId] = new List<int>();
                spawnerData[cellId] = items;
                return;
            }
            spawnerData.Add(cellId, items);
        }

        public void UpdateGoalValue(List<int> goalValues)
        {
            currentGoalValue.Clear();
            currentGoalValue.AddRange(goalValues);
        }

        public void UpdateLevel(int level)
        {
            levelNo = level;
        }

        public void UpdateGridRoattion(int roattion)
        {
            gridRotation = roattion;
        }
    }

    #endregion

    public class PlayerProfileData
    {
        [JsonProperty("pn")] public string playerName;
        [JsonProperty("ai")] public int avtarId = 0;
        [JsonProperty("fi")] public int frameId = 0;
    }

    public class PlayerPrefsKeys
    {
        public const string Currancy_Data_Key = "CurrancyPlayerData";
        public const string Main_Player_Progress_Data_Key = "MainPlayerProgressData";
        public const string Tutorial_Player_Data_Key = "TutorialPlayerData";
        public const string Level_Progress_Data_Key = "LevelProgressData";
        public const string VIP_Leaderboard_Player_Data_Key = "VIPLeaderboardPlayerData";
        public const string GameStats_Player_Data_Key = "GameStatsPlayerData";
        public const string Player_Profile_Data_Key = "PlayerProfileData";
        public const string DailyAdPass_Data_Key = "DailyAdPassData";
        public const string CommonOffer_Data_Key = "CommonOfferData";
        public const string StreakBonus_Data_Key = "StreakBonusData";
    }
}