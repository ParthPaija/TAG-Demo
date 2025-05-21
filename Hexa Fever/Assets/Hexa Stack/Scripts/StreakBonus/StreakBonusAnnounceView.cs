using System;
using UnityEngine;

namespace Tag.HexaStack
{
    public class StreakBonusAnnounceView : BaseView
    {
        #region PUBLIC_VARS

        

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void OnBackButtonPressed()
        {
            OnClose();
        }

        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnClose()
        {
            Hide();
        }
        #endregion
    }
}
