using System;

namespace Tag.HexaStack
{
    public class LeaderboardRewardClaimPopupAutoOpenChecker : BaseAutoOpenPopupChecker
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

            if (!VIPLeaderboardManager.Instance.IsLastEventResultReadyToShow())
            {
                OnAutoOpenCheckComplete();
                return;
            }

            bool isEventResult = VIPLeaderboardManager.Instance.IsEventResultRedayForShown();

            if (isEventResult)
            {
                MainSceneUIManager.Instance.GetView<VIPLeaderboardView>().ShowWithHideAction(OnAutoOpenCheckComplete);
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
