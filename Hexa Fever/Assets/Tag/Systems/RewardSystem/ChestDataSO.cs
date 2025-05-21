using Sirenix.OdinInspector;
using UnityEngine;

namespace Tag.RewardSystem
{
    [CreateAssetMenu(fileName = "ChestDataSO", menuName = "Scriptable Objects/RewardSystem/ChestDataSO")]
    public class ChestDataSO : SerializedScriptableObject
    {
        #region PUBLIC_VARS

        public ChestData[] chestDatas;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public ChestData GetChestData(GiftBoxType chestType)
        {
            for (int i = 0; i < chestDatas.Length; i++)
            {
                if (chestDatas[i].chestType == chestType)
                    return chestDatas[i];
            }
            return null;
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
