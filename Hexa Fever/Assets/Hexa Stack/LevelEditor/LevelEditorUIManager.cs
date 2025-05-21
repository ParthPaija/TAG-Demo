using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class LevelEditorUIManager : UIManager<LevelEditorUIManager>
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        public override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            GetView<LevelLoadView>().Show();
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
