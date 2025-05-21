using System.Collections;
using System.Collections.Generic;
using Tag.HexaStack;
using TMPro;
using UnityEngine;

namespace Tag.HexaStack
{
    public class NotEnoughMetaCurrencyPopup : BaseView
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
        public void OnPlayClick()
        {
            Hide();
            //Start Game
        }
        public void OnCloseClick()
        {
            Hide();
        }
        #endregion
    }
}