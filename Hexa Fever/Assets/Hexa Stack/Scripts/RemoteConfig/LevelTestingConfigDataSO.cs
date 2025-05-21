using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "LevelTestingConfigData", menuName = Constant.GAME_NAME + "/Remote Config Data/LevelTestingConfigData")]
    public class LevelTestingConfigDataSO : BaseConfig
    {
        #region PUBLIC_VARS

        public LevelTestingConfigData levelTestingConfigData;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override string GetDefaultString()
        {
            return SerializeUtility.SerializeObject(levelTestingConfigData);
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

    public class LevelTestingConfigData
    {
        public LevelTestingType levelTestingType;
    }
}
