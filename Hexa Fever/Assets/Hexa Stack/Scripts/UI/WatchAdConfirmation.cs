using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class WatchAdConfirmation : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        private Action onYes;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowView(Action onYes)
        {
            base.Show();
            this.onYes = onYes;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnYes()
        {
            onYes.Invoke();
            Hide();
        }

        public void OnClose()
        {
            Hide();
        }

        #endregion
    }
}
