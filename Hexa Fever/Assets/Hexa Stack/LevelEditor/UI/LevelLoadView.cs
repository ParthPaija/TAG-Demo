using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class LevelLoadView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private TMP_InputField levelIF;

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

        public void OnLoadButton()
        {
            if (!string.IsNullOrEmpty(levelIF.text) && !string.IsNullOrWhiteSpace(levelIF.text))
            {
                Hide();
                LevelEditor.Instance.OnLoadLevel(int.Parse(levelIF.text));
                LevelEditorUIManager.Instance.GetView<LevelEditingView>().Show();
            }
        }

        #endregion
    }
}
