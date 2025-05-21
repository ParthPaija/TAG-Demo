using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class RemoveAdsView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private string packId;
        [SerializeField] private Text priceText;
        [SerializeField] private List<BaseReward> adsReward = new List<BaseReward>();

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        [Button]
        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);
            priceText.text = IAPManager.Instance.GetIAPPrice(packId);
        }

        [Button]
        public override void Hide()
        {
            base.Hide();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void OnPackPurchaseSuccess(string productId)
        {
            for (int i = 0; i < adsReward.Count; i++)
            {
                adsReward[i].GiveReward();
            }
            Hide();
        }

        private void OnPackPurchaseFailed(string productId)
        {
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnPurchaseClick()
        {
            IAPManager.Instance.PurchaseProduct(packId, OnPackPurchaseSuccess, OnPackPurchaseFailed);
        }

        #endregion
    }
}
