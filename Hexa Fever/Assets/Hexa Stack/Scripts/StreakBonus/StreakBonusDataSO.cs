using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "StreakBonusDataConfig", menuName = Constant.GAME_NAME + "/Remote Config Data/StreakBonusDataConfig")]
    public class StreakBonusDataSO : BaseConfig
    {
        #region PUBLIC_VARS

        public StreakBonusDataConfig streakBonusDataConfig;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Init(string configString)
        {
            base.Init(configString);
            streakBonusDataConfig = GetValue<StreakBonusDataConfig>();
        }

        public override string GetDefaultString()
        {
            return SerializeUtility.SerializeObject(streakBonusDataConfig);
        }

        public int GetCurrentStreakPropeller(int streakCount)
        {
            for (int i = 0; i < streakBonusDataConfig.streakBonusData.Count; i++)
            {
                if (streakCount == streakBonusDataConfig.streakBonusData[i].streakCount)
                {
                    return streakBonusDataConfig.streakBonusData[i].propellersCount;
                }
            }
            return 0;
        }

        public StreakBonusData GetCurrentStreakData(int streakCount)
        {
            return streakBonusDataConfig.streakBonusData.Find(x => x.streakCount == streakCount);
        }
        public StreakBonusData GetNonZeroPropellerData()
        {
            for (int i = 0; i < streakBonusDataConfig.streakBonusData.Count; i++)
            {
                if (streakBonusDataConfig.streakBonusData[i].propellersCount > 0)
                {
                    return streakBonusDataConfig.streakBonusData[i];
                }
            }
            return null;
        }

        public bool IsMaxStreak(int streak)
        {
            return streakBonusDataConfig.streakBonusData[streakBonusDataConfig.streakBonusData.Count - 1].streakCount == streak;
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
}
public class StreakBonusDataConfig
{
    public bool isActive;
    public int openAt;
    public List<StreakBonusData> streakBonusData = new List<StreakBonusData>();
}
public class StreakBonusData
{
    public int streakCount;
    public int propellersCount;
}
