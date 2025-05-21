using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "CommonOfferConfig", menuName = Constant.GAME_NAME + "/Remote Config Data/CommonOfferConfig")]
    public class CommonOfferConfigDataSO : BaseConfig
    {
        #region PUBLIC_VARS

        public List<CommonOfferRemoteConfigData> data;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override string GetDefaultString()
        {
            return SerializeUtility.SerializeObject(data);
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
