using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class VIPLeagueManager : SerializedManager<VIPLeagueManager>
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Dictionary<LevelType, List<LevelDataSO>> levels = new Dictionary<LevelType, List<LevelDataSO>>();

        [ShowInInspector]
        public string VIPLeageLevelPlayedDataString
        {
            get
            {
                return PlayerPrefbsHelper.GetString("VIPLeageLevelPlayedDataKey", SerializeUtility.SerializeObject(new VIPLeageLevelPlayedData()));
            }
            set
            {
                PlayerPrefbsHelper.SetString("VIPLeageLevelPlayedDataKey", value);
            }
        }

        [ShowInInspector]
        private VIPLeageLevelPlayedData vIPLeageLevelPlayedData = new VIPLeageLevelPlayedData();

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init()
        {
            if (!IsVIPLeageUnlocked())
            {
                return;
            }
        }

        public bool IsVIPLeageUnlocked()
        {
            return DataManager.PlayerData.playerGameplayLevel > ResourceManager.Instance.TotalLevelCount;
        }

        public LevelDataSO GetLevelData()
        {
            var leaderBoardPlayerData = PlayerPersistantData.GetVIPLeaderboardPlayerData();

            if (leaderBoardPlayerData == null)
                Debug.LogError("Leaderboard data is null");

            if (leaderBoardPlayerData.lastPlayedLevelIndex != -1)
            {
                for (int i = 0; i < levels[LevelType.Normal].Count; i++)
                {
                    if (levels[LevelType.Normal][i].Level == leaderBoardPlayerData.lastPlayedLevelIndex)
                        return levels[LevelType.Normal][i];
                }

                for (int i = 0; i < levels[LevelType.SuperHard].Count; i++)
                {
                    if (levels[LevelType.SuperHard][i].Level == leaderBoardPlayerData.lastPlayedLevelIndex)
                        return levels[LevelType.SuperHard][i];
                }
            }
            LevelDataSO levelDataSO;


            vIPLeageLevelPlayedData = SerializeUtility.DeserializeObject<VIPLeageLevelPlayedData>(VIPLeageLevelPlayedDataString);

            if (leaderBoardPlayerData.currentLevel % 5 == 0)
            {
                List<int> ints = vIPLeageLevelPlayedData.hardLevelPlayerd;
                do
                {
                    levelDataSO = levels[LevelType.SuperHard][Random.Range(0, levels[LevelType.SuperHard].Count)];

                } while (ints.Contains(levelDataSO.Level));


                ints.Add(levelDataSO.Level);

                if (ints.Count == levels[LevelType.SuperHard].Count)
                {
                    ints.Clear();
                }

                vIPLeageLevelPlayedData.hardLevelPlayerd = ints;
                VIPLeageLevelPlayedDataString = SerializeUtility.SerializeObject(vIPLeageLevelPlayedData);
            }
            else
            {
                Debug.LogError("Normal");

                List<int> ints = vIPLeageLevelPlayedData.noramlaLevelPlayerd;
                do
                {
                    levelDataSO = levels[LevelType.Normal][Random.Range(0, levels[LevelType.Normal].Count)];
                    Debug.LogError("Normal Level :- " + levelDataSO.Level);
                } while (ints.Contains(levelDataSO.Level));

                ints.Add(levelDataSO.Level);

                if (Mathf.Abs(ints.Count - levels[LevelType.Normal].Count) <= 10)
                {
                    ints.Clear();
                }

                vIPLeageLevelPlayedData.noramlaLevelPlayerd = ints;
                VIPLeageLevelPlayedDataString = SerializeUtility.SerializeObject(vIPLeageLevelPlayedData);
            }

            leaderBoardPlayerData.lastPlayedLevelIndex = levelDataSO.Level;
            PlayerPersistantData.SetVIPLeaderboardPlayerData(leaderBoardPlayerData);
            return levelDataSO;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        #endregion
    }

    public class VIPLeageLevelPlayedData
    {
        public List<int> noramlaLevelPlayerd = new List<int>();
        public List<int> hardLevelPlayerd = new List<int>();
    }
}
