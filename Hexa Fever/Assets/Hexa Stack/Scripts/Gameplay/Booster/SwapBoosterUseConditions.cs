using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class SwapBoosterUseConditions : BaseBoosterUseConditions
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override bool CanUseBooster()
        {
            if (!LevelManager.Instance.LoadedLevel.CanUseSwapBooster())
            {
                GlobalUIManager.Instance.GetView<ToastMessageView>().ShowMessage("Nothing to Swap!", transform.position.With(null, null, 0));
                return false;
            }
            return base.CanUseBooster();
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
