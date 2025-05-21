using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class AdBreakView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Image rewardImage;
        [SerializeField] private Text rewardAmountText;
        [SerializeField] private CurrencyTopbarComponents coinTopbar;
        private bool isCurrencyAnimationInProgress = false;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowView(AdBreakRemoteData adBreakRemoteData)
        {
            base.Show();
            rewardImage.sprite = adBreakRemoteData.rewards.GetRewardImageSprite();
            rewardAmountText.text = adBreakRemoteData.rewards.GetAmountStringForUI();
        }

        public override bool CanPressBackButton()
        {
            return false;
        }

        public void PlayCurrencyCoinAnimation(int amount)
        {
            isCurrencyAnimationInProgress = true;
            coinTopbar.CurrencyAnimation.UIStartAnimation(rewardImage.transform.position, endPos: coinTopbar.EndPos, amount);
            CoroutineRunner.Instance.Wait(1.35f, () =>
            {
                isCurrencyAnimationInProgress = false;
            });
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
