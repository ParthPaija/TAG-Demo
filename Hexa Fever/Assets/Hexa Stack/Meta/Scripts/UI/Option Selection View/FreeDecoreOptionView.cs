using System;
using Tag.CommonPurchaseSystem;
using UnityEngine;

namespace Tag.MetaGame
{
    public class FreeDecoreOptionView : BaseDecoreOptionView
    {
        #region PUBLIC_VARS

        public override PurchaseType PrefabType
        {
            get { return PurchaseType.Free; }
        }

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void BuyOption(Action onSuccessAction)
        {
            Debug.Log("Free Option Selected");
            onSuccessAction.Invoke();
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
