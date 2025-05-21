using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "UnlockerPriorityData", menuName = Constant.GAME_NAME + "/UnlockerPriorityData")]
    public class UnlockerPriorityDataSO : SerializedScriptableObject
    {
        #region PUBLIC_VARS

        public Dictionary<int, int> unlockerPrority = new Dictionary<int, int>();

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public int GetPriority(int obstacalId)
        {
            if (unlockerPrority.ContainsKey(obstacalId))
                return unlockerPrority[obstacalId];
            return 1000;
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
