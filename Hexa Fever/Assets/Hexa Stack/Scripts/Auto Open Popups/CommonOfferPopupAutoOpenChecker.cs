using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class CommonOfferPopupAutoOpenChecker : BaseAutoOpenPopupChecker
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private int offerId;
        [SerializeField] private int sessionCount;
        [SerializeField] private int playerLevel;
        private static bool isShowed = false;

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
            if (CanShowPopup() && CommonOfferManager.Instance.GetOfferData(offerId).IsActive())
            {
                isShowed = true;
                CommonOfferManager.Instance.ShowOfferWithHideAction(offerId, OnAutoOpenCheckComplete);
            }
            else
                OnAutoOpenCheckComplete();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private bool CanShowPopup()
        {
            if (DataManager.Instance.isFirstSession)
                return false;
            if (DataManager.PlayerData.playerGameplayLevel < playerLevel)
                return false;
            if (isShowed)
                return false;
            return true;
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        #endregion
    }
}
