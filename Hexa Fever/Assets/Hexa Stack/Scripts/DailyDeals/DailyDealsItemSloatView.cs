using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class DailyDealsItemSloatView : MonoBehaviour
    {
        #region PUBLIC_VARS

        [SerializeField] private Image rewardImage;
        [SerializeField] private Text rewardAmountText;
        [SerializeField] private GameObject claimedButton;
        [SerializeField] private GameObject unClaimButton;

        #endregion

        #region PRIVATE_VARS

        protected DailyDealSlotConfig _currentDeal;
        private int _slotIndex;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public virtual void Init(DailyDealSlotConfig deal, int index)
        {
            _currentDeal = deal;
            _slotIndex = index;

            if (deal == null)
                return;

            rewardImage.sprite = deal.dealPurchaseData.reward.GetRewardImageSprite();
            rewardAmountText.text = deal.dealPurchaseData.reward.GetAmountStringForUI();
            SetButton(DailyDealsManager.Instance.DailyDealsPlayerData.slots[_slotIndex].isClaimed);
        }

        public void OnPurchase()
        {
            SetButton(true);
            DailyDealsManager.Instance.OnPurchase(_slotIndex);
            _currentDeal.dealPurchaseData.reward.GiveReward();
            _currentDeal.dealPurchaseData.reward.ShowRewardToastAnimation(rewardImage.transform.position, null, Vector3.one * 2);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetButton(bool isClaimed)
        {
            claimedButton.SetActive(isClaimed);
            unClaimButton.SetActive(!isClaimed);
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void ClaimButtonClick()
        {
            if (_currentDeal.dealPurchaseData.CanPurchase())
            {
                _currentDeal.dealPurchaseData.OnPurchase(OnPurchase);
            }
        }

        #endregion
    }
}
