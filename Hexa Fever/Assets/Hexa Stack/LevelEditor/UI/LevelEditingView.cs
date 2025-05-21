using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class LevelEditingView : BaseView
    {
        #region PUBLIC_VARS

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

        public void OnTileSet()
        {
            LevelEditorUIManager.Instance.GetView<TileSetSelectedView>().Show();
        }

        public void OnLevelGoal()
        {
            LevelEditorUIManager.Instance.GetView<LevelGoalsView>().Show();
        }

        public void OnSpawnConfig()
        {
            LevelEditorUIManager.Instance.GetView<LevelSpwanerConfigView>().Show();
        }

        public void OnSave()
        {
            LevelEditor.Instance.Save();
            Hide();
            LevelEditorUIManager.Instance.GetView<LevelLoadView>().Show();
        }

        #endregion
    }
}
