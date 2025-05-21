using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class DailyDealsCurrencyItemSloatView : DailyDealsItemSloatView
    {
        #region PUBLIC_VARS

        [SerializeField] private Image purchaseCurrencyImage;
        [SerializeField] private Text purchaseCurrencyAmountText;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Init(DailyDealSlotConfig deal, int index)
        {
            base.Init(deal, index);
            SetPurchaseCurrencyButton();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetPurchaseCurrencyButton()
        {
            CurrencyDailyDealPurchaseData dealPurchaseData = _currentDeal.dealPurchaseData as CurrencyDailyDealPurchaseData;
            purchaseCurrencyImage.sprite = dealPurchaseData.costReward.GetRewardImageSprite();
            purchaseCurrencyAmountText.text = dealPurchaseData.costReward.GetAmountStringForUI();
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
