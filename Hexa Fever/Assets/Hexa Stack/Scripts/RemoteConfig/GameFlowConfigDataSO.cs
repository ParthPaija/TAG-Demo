using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "GameFlowConfigData", menuName = Constant.GAME_NAME + "/Remote Config Data/GameFlowConfigData")]
    public class GameFlowConfigDataSO : BaseConfig
    {
        #region PUBLIC_VARS

        public GameFlowConfigData gameFlowConfigData;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override string GetDefaultString()
        {
            return SerializeUtility.SerializeObject(gameFlowConfigData);
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

    public class GameFlowConfigData
    {
        public bool isMetaVisiteEnableOnNext;
        public int unlockAt;
    }
}
