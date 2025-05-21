using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class IapShopBundleView : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Text priceText;
        private IapShopBundleData iapShopBundleData;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView(IapShopBundleData iapShopBundleData)
        {
            this.iapShopBundleData = iapShopBundleData;
            priceText.text = IAPManager.Instance.GetIAPPrice(iapShopBundleData.iapString);
        }

        private void OnPackPurchaseSuccess(string productId)
        {
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
                SetView(iapShopBundleData);
                MainSceneUIManager.Instance.GetView<ShopView>().Show();
            });
        }

        private void OnPackPurchaseFailed(string productId)
        {
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnClick()
        {
            IAPManager.Instance.PurchaseProduct(iapShopBundleData.iapString, OnPackPurchaseSuccess, OnPackPurchaseFailed);
        }

        #endregion
    }
}
