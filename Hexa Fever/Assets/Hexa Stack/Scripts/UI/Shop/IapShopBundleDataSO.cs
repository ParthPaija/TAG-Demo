using Sirenix.OdinInspector;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "IapShopBundleData", menuName = Constant.GAME_NAME + "/Shop/IapShopBundleData")]
    public class IapShopBundleDataSO : SerializedScriptableObject
    {
        #region PUBLIC_VARS

        public IapShopBundleData[] iapShopBundleDatas;
        public IapShopBundleData noAdsMiniBundleData;
        public IapShopBundleData noAdsMainBundleData;
        public IapShopBundleData specialBundleData;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

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
    public class IapShopBundleData
    {
        public string iapString;
        public BaseReward[] rewards;
    }
}
