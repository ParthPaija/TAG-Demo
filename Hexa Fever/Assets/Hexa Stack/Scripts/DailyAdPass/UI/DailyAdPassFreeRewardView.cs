using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class DailyAdPassFreeRewardView : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GameObject lockGO;
        [SerializeField] private GameObject freeGO;
        [SerializeField] private Image rewardImage;
        [SerializeField] private Text rewardAmountText;
        [SerializeField] private Text timerText;
        private DailyAdPassRewardData dailyAdPassRewardData;

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            DailyAdPassManager.onFreeRewardTimerRunTimerOver += SetView;
        }

        private void OnDisable()
        {
            DailyAdPassManager.onFreeRewardTimerRunTimerOver -= SetView;
            UnregisterTimer();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView()
        {
            dailyAdPassRewardData = DailyAdPassManager.Instance.DailyAdPassData.freeReward;
            rewardImage.sprite = dailyAdPassRewardData.rewardSprite;
            rewardAmountText.text = dailyAdPassRewardData.baseReward.GetAmountStringForUI();

            if (!DailyAdPassManager.Instance.IsFreeRewardReady())
            {
                RegisterTimer();
                lockGO.SetActive(true);
                freeGO.SetActive(false);
            }
            else
            {
                lockGO.SetActive(false);
                freeGO.SetActive(true);
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void RegisterTimer()
        {
            if (DailyAdPassManager.Instance.FreeRewardTimer != null)
                DailyAdPassManager.Instance.FreeRewardTimer.RegisterTimerTickEvent(UpdateTimer);
        }

        private void UpdateTimer()
        {
            timerText.text = DailyAdPassManager.Instance.FreeRewardTimer.GetRemainingTimeSpan().ParseTimeSpan(2);
        }

        private void UnregisterTimer()
        {
            if (DailyAdPassManager.Instance.FreeRewardTimer != null)
                DailyAdPassManager.Instance.FreeRewardTimer.UnregisterTimerTickEvent(UpdateTimer);
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnClaimClick()
        {
            if (dailyAdPassRewardData.baseReward.GetCurrencyId() == CurrencyConstant.COINS)
            {
                MainSceneUIManager.Instance.GetView<DailyAdPassView>().PlayCurrencyCoinAnimation(rewardImage.transform.position, dailyAdPassRewardData.baseReward.GetAmount());
            }
            DailyAdPassManager.Instance.OnFreeRewardClaim();
            SetView();
            MainSceneUIManager.Instance.GetView<MainView>().DailyAdPassViewButton.SetView();
        }

        #endregion
    }
}
