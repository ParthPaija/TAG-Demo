using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    [CreateAssetMenu(fileName = "CellLockerSelcetorData", menuName = Constant.GAME_NAME + "/LevelEditor/CellLockerSelcetorData")]
    public class CellLockerSelcetorDataSO : SerializedScriptableObject
    {
        #region PUBLIC_VARS

        public Dictionary<Type, BaseCellUnlocker> cellLockerPrefab = new Dictionary<Type, BaseCellUnlocker>();
        public Dictionary<int, BaseItem> items = new Dictionary<int, BaseItem>();
        public List<GameObject> gridGenerators = new List<GameObject>();

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
}
