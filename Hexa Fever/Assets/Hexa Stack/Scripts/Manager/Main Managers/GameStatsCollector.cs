using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class GameStatsCollector : SerializedManager<GameStatsCollector>
    {
        #region PUBLIC_VARIABLES
        [Header("TST")]
        public bool isUseTstTime;
        [ShowIf("isUseTstTime")] public string tstTime;

        public int CurrentPlayedLevelsInThisSession => currentPlayedLevelsInThisSession;
        #endregion

        #region PRIVATE_VARIABLES
        private const int Save_Data_Of_Past_X_Days = 7;
        [ShowInInspector, ReadOnly] private int currentPlayedLevelsInThisSession = 0;
        #endregion

        #region PROPERTIES
        #endregion

        #region UNITY_CALLBACKS
        public override void Awake()
        {
            base.Awake();
            UpdateLastDaysPlayedLevel();
            OnLoadingDone();
        }

        private void OnEnable()
        {
            //GameplayManager.onGameplayLevelOver += GameplayManager_onGameplayLevelOver;
        }

        private void OnDisable()
        {
            //GameplayManager.onGameplayLevelOver -= GameplayManager_onGameplayLevelOver;
        }
        #endregion

        #region PUBLIC_METHODS
        public int GetAveragePlayedLevelsInPastDays()
        {
            var statsData = PlayerPersistantData.GetGameStatsPlayerData();
            if (statsData == null || statsData.lastDaysPlayedLevels.Count == 0)
                return 0;

            float avgLevels = 0;
            foreach(var kvp in statsData.lastDaysPlayedLevels)
            {
                avgLevels += kvp.Value;
            }

            return Mathf.CeilToInt(avgLevels / statsData.lastDaysPlayedLevels.Count);
        }
        #endregion

        #region PRIVATE_METHODS
        private DateTime GetCurrentDayDate()
        {
            if (isUseTstTime)
            {
                CustomTime.TryParseDateTime(tstTime, out var dt);
                return dt.Date;
            }

            return CustomTime.GetCurrentTime().Date;
        }
        #endregion

        #region EVENT_HANDLERS
        public void GameplayManager_onGameplayLevelOver()
        {
            currentPlayedLevelsInThisSession++;

            var statsData = PlayerPersistantData.GetGameStatsPlayerData();
            if (statsData == null)
                statsData = new GameStatsPlayerPersistantData();

            DateTime currentTime = GetCurrentDayDate();
            string currentTimeSaveKey = currentTime.GetPlayerPrefsSaveString();

            if (!statsData.lastDaysPlayedLevels.ContainsKey(currentTimeSaveKey))
                statsData.lastDaysPlayedLevels.Add(currentTimeSaveKey, 0);
            statsData.lastDaysPlayedLevels[currentTimeSaveKey]++;

            PlayerPersistantData.SetGameStatsPlayerData(statsData);
        }

        private void UpdateLastDaysPlayedLevel()
        {
            DateTime currentTime = GetCurrentDayDate();

            var statsData = PlayerPersistantData.GetGameStatsPlayerData();
            if (statsData == null)
                statsData = new GameStatsPlayerPersistantData();

            List<string> keysToRemove = new List<string>();

            foreach (var kvp in statsData.lastDaysPlayedLevels)
            {
                bool parseResult = CustomTime.TryParseDateTime(kvp.Key, out DateTime savedTime);
                if (string.IsNullOrEmpty(kvp.Key) || !parseResult || (currentTime.Date - savedTime).TotalDays >= Save_Data_Of_Past_X_Days)
                    keysToRemove.Add(kvp.Key);
            }

            keysToRemove.ForEach(x => statsData.lastDaysPlayedLevels.Remove(x));
            PlayerPersistantData.SetGameStatsPlayerData(statsData);
        }
        #endregion

        #region COROUTINES
        #endregion

        #region UI_CALLBACKS
        [Button]
        public void Editor_OnLevelOver()
        {
            GameplayManager_onGameplayLevelOver();
        }

        [Button]
        public void Editor_PrintData()
        {
            var data = SerializeUtility.SerializeObject(PlayerPersistantData.GetGameStatsPlayerData());
            Debug.Log(data);
            GUIUtility.systemCopyBuffer = data;

            Debug.Log("AVG Levels : " + (GetAveragePlayedLevelsInPastDays()));
        }

        [Button]
        public void Editor_ClearData()
        {
            PlayerPersistantData.SetGameStatsPlayerData(null);
        }

        [Button]
        public void Editor_UpdateLastPlayedData()
        {
            UpdateLastDaysPlayedLevel();
        }
        #endregion
    }

    public class GameStatsPlayerPersistantData
    {
        public Dictionary<string, int> lastDaysPlayedLevels = new Dictionary<string, int>();
    }
}