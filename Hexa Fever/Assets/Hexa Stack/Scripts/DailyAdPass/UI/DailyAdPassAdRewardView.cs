using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class DailyAdPassAdRewardView : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GameObject lockGO;
        [SerializeField] private GameObject freeGO;
        [SerializeField] private GameObject cliamGO;
        [SerializeField] private Image rewardImage;
        [SerializeField] private Text rewardAmountText;
        private DailyAdPassRewardData dailyAdPassRewardData;
        [SerializeField] private int index;

        [Header("Fillbar"), Space]
        [SerializeField] private GameObject cliamedGOFillbar;
        [SerializeField] private GameObject currentRewardGOFillbar;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetUI()
        {
            SetView(dailyAdPassRewardData);
        }

        public void SetView(DailyAdPassRewardData dailyAdPassRewardData)
        {
            HideAll();
            this.dailyAdPassRewardData = dailyAdPassRewardData;
            rewardImage.sprite = dailyAdPassRewardData.rewardSprite;
            rewardAmountText.text = dailyAdPassRewardData.baseReward.GetAmountStringForUI();

            if (DailyAdPassManager.Instance.IsAdRewardReady(index))
            {
                currentRewardGOFillbar.SetActive(true);
                freeGO.SetActive(true);
            }
            else if (DailyAdPassManager.Instance.PlayerData.adRewardClaimIndex > index)
            {
                cliamGO.SetActive(true);
                cliamedGOFillbar.SetActive(true);
            }
            else
            {
                lockGO.SetActive(true);
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void HideAll()
        {
            lockGO.SetActive(false);
            freeGO.SetActive(false);
            cliamGO.SetActive(false);
            cliamedGOFillbar.SetActive(false);
            currentRewardGOFillbar.SetActive(false);
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnClaimClick()
        {
            AdManager.Instance.ShowRewardedAd(() =>
            {
                DailyAdPassManager.Instance.OnAdRewardClaim(index);
                SetUI();
                MainSceneUIManager.Instance.GetView<DailyAdPassView>().SetNextAdRewardView(index);
                MainSceneUIManager.Instance.GetView<MainView>().DailyAdPassViewButton.SetView();

                if (dailyAdPassRewardData.baseReward.GetCurrencyId() == CurrencyConstant.COINS)
                    MainSceneUIManager.Instance.GetView<DailyAdPassView>().PlayCurrencyCoinAnimation(rewardImage.transform.position,dailyAdPassRewardData.baseReward.GetAmount());

            }, RewardAdShowCallType.DailyReward, "DailyReward");
        }

        public void OnLockButtonClick()
        {
            Debug.LogError("Cliam Previous Reward First");
        }

        #endregion
    }
}
