using System;
using UnityEngine;

namespace Tag.HexaStack
{
    public class RateUsView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        private Action actionToCallOnHide;

        public static bool IsRated { get; internal set; }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowWithHideAction(Action actionToCallOnHide)
        {
            this.actionToCallOnHide = actionToCallOnHide;
            Show();
        }

        public override void OnHideComplete()
        {
            base.OnHideComplete();
            actionToCallOnHide?.Invoke();
            actionToCallOnHide = null;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnSubmitButtonClick()
        {
            RateUsManager.IsRated = true;
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.hexasort.color.puzzle");
            Hide();
        }

        public void OnCloseButtonClick()
        {
            Hide();
        }

        #endregion
    }
}
