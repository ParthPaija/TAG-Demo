using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "ReviveDataConfig", menuName = Constant.GAME_NAME + "/Remote Config Data/ReviveDataConfig")]
    public class ReviveDataSO : BaseConfig
    {
        #region PUBLIC_VARS

        public int lastEditLevel;
        public LevelTestingType levelTestingType;
        public ReviveDataConfig reviveDataConfig;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override string GetDefaultString()
        {
            return SerializeUtility.SerializeObject(reviveDataConfig);
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

    public class ReviveDataConfig
    {
        public int removeStackCountCoin;
        public int removeStackCountAd;
        public List<int> reviveCoinCount;
        public int reviveCountAd = 2;
        public bool isLevelFailOfferActive;

        public int GetCointAmount(int reviveCount)
        {
            reviveCount = Mathf.Clamp(reviveCount, 0, reviveCoinCount.Count);
            return reviveCoinCount[reviveCount];
        }
    }
}
