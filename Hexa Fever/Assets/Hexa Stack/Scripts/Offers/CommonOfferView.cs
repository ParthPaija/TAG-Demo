using System;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class CommonOfferView : BaseView
    {
        #region PUBLIC_VARS
        public int OfferId { get => offerId; set => offerId = value; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private int offerId;
        [SerializeField] private Text priceText;
        [SerializeField] private IapShopBundleData iapShopBundleData;
        private Action actionToCallOnHide;
        [SerializeField] private bool isHideInPuchase = false;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowView(IapShopBundleData iapShopBundleData)
        {
            this.iapShopBundleData = iapShopBundleData;
            Debug.LogError("Iap Bundle Data :- " + (iapShopBundleData == null));
            priceText.text = IAPManager.Instance.GetIAPPrice(this.iapShopBundleData.iapString);
            base.Show();
        }

        public void ShowWithHideAction(Action actionToCallOnHide, IapShopBundleData iapShopBundleData)
        {
            this.actionToCallOnHide = actionToCallOnHide;
            ShowView(iapShopBundleData);
        }

        public override void OnHideComplete()
        {
            base.OnHideComplete();
            actionToCallOnHide?.Invoke();
            actionToCallOnHide = null;
        }

        #endregion

        #region PRIVATE_FUNCTIONS
        private void OnPackPurchaseSuccess(string productId)
        {
            CommonOfferManager.Instance.OnOfferPurchase(offerId);
            for (int i = 0; i < iapShopBundleData.rewards.Length; i++)
            {
                iapShopBundleData.rewards[i].GiveReward();

                if (iapShopBundleData.rewards[i].GetCurrencyId() == CurrencyConstant.COINS)
                {
                    AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, AnalyticsConstants.CoinCurrency
                            , iapShopBundleData.rewards[i].GetAmount(), AnalyticsConstants.ItemType_Purchase, AnalyticsConstants.ItemId_IAP);
                }
            }

            MainSceneUIManager.Instance.GetView<CommonRewardFeedbackView>().ShowView(iapShopBundleData.rewards.ToList(), () =>
            {
                if (MainSceneUIManager.Instance.GetView<GameplayBottomView>().IsActive)
                    MainSceneUIManager.Instance.GetView<GameplayBottomView>().SetView();
                if (isHideInPuchase)
                    Hide();
            });
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

        public void OnClose()
        {
            Hide();
        }

        public void OnClick()
        {
            IAPManager.Instance.PurchaseProduct(iapShopBundleData.iapString, OnPackPurchaseSuccess, OnPackPurchaseFailed);
        }

        #endregion
    }
}
