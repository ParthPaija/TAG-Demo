using System.Collections;
using System.Collections.Generic;
using Tag.CoreGame;
using Tag.HexaStack;
using UnityEngine;

namespace Tag.MetaGame.TaskSystem
{
    public class NewAreaComingSoonPopup : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS
        public override void Hide()
        {
            MainSceneUIManager.Instance.GetView<MainView>().Show();
            MainSceneUIManager.Instance.GetView<BottombarView>().Show();
            base.Hide();
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS   
        public void OnCloseClick()
        {
            Hide();
        }
        public void OnContinueClick()
        {
            Hide();

        }
        #endregion
    }
}
