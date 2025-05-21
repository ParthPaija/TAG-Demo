using Tag.RewardSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class IapShopCoinBundleView : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Image coinImage;
        [SerializeField] private CurrencyTopbarComponents coinTopbar;
        [SerializeField] private Text priceText;
        [SerializeField] private Text coinAmountText;
        private IapShopBundleData iapShopBundleData;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init(IapShopBundleData iapShopBundleData)
        {
            this.iapShopBundleData = iapShopBundleData;
            priceText.text = IAPManager.Instance.GetIAPPrice(iapShopBundleData.iapString);
            coinAmountText.text = iapShopBundleData.rewards[0].GetAmount().ToString();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

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
            iapShopBundleData.rewards[0].ShowRewardAnimation(coinTopbar.CurrencyAnimation, coinImage.transform.position, true, coinTopbar.EndPos);
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

        public void OnClick()
        {
            IAPManager.Instance.PurchaseProduct(iapShopBundleData.iapString, OnPackPurchaseSuccess, OnPackPurchaseFailed);
        }

        #endregion
    }
}
