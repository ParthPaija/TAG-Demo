using System;
using System.Collections;
using System.Collections.Generic;
using Tag.HexaStack;

namespace Tag.AssetManagement
{
    public class AssetDownloadFailedPopup : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        private Action _redownloadBtnAction;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowPopup(Action redownloadBtnAction)
        {
            _redownloadBtnAction = redownloadBtnAction;
            base.ShowView();
        }

        
        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       
        public void RedownloadButtonClick()
        {
            Hide();
            _redownloadBtnAction?.Invoke();
        }
        #endregion
    }
}
