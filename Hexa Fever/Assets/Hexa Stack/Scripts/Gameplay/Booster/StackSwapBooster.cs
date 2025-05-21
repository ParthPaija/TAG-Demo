using System;

namespace Tag.HexaStack
{
    public class StackSwapBooster : BaseBooster
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void OnActive(Action onUse)
        {
            ActiveConfirmationView();
            base.OnActive(onUse);
        }

        public override void OnUse()
        {
            DeActiveConfirmationView();
            base.OnUse();
        }

        public override void OnUnUse()
        {
            DeActiveConfirmationView();
            base.OnUnUse();
        }

        public void OnSatckPlacedSucess()
        {
            OnUse();
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
