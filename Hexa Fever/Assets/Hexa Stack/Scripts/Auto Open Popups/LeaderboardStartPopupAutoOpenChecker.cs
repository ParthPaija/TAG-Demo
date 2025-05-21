using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class LeaderboardStartPopupAutoOpenChecker : BaseAutoOpenPopupChecker
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
            if (VIPLeaderboardManager.isIntroViewShownRemianing)
            {
                VIPLeaderboardManager.isIntroViewShownRemianing = false;
                MainSceneUIManager.Instance.GetView<VipLeagueIntroView>().ShowWithHideAction(OnAutoOpenCheckComplete);
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
