using System;
using Tag.HexaStack;
using UnityEngine;

namespace Tag
{
    public class StreakBonusPopupAutoOpenChecker : BaseAutoOpenPopupChecker
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void InitializeChecker()
        {
        }

        public override void CheckForAutoOpenPopup(Action actionToCallOnAutoOpenComplete)
        {
            base.CheckForAutoOpenPopup(actionToCallOnAutoOpenComplete);
            if (StreakBonusManager.Instance.CanAnnounceShow())
            {
                StreakBonusManager.IsStreakBonusAnnounce = true;
                MainSceneUIManager.Instance.GetView<StreakBonusAnnounceView>().Show(OnAutoOpenCheckComplete);
            }
            else
                OnAutoOpenCheckComplete();
        }

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
