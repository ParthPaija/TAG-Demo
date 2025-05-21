using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class LevelEditorManager : Manager<LevelEditorManager>
    {
        #region PUBLIC_VARS

        public static int lastLevelEditNo { get { return Instance.reviveDataSO.lastEditLevel; } }

        public static LevelTestingType levelTestingType { get { return Instance.reviveDataSO.levelTestingType; } }

        public static bool IsEditorTest { get { return true; } }
        public ReviveDataSO reviveDataSO;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        private void Start()
        {
            GameplayManager.Instance.OnMainGameLevelStart_Editor();
        }

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
