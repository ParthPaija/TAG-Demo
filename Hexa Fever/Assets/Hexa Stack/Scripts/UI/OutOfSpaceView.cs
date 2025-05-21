using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class OutOfSpaceView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private CommonOfferView commonOfferView;
        [SerializeField] private ReviveDataSO reviveDataSO;
        [SerializeField] private Text goalText;
        [SerializeField] private Image goalImage;
        [SerializeField] private Text coinText;
        [SerializeField] private Button adButton;
        [SerializeField] private BaseUiAnimation animationForHold;
        [SerializeField] private GameObject normalOutOfSpaceGO;
        [SerializeField] private GameObject streakBonusGO;
        [SerializeField] private StreakBonusProgressBar streakBonusProgressBar;


        private ReviveDataConfig reviveDataConfig;

        #endregion

        #region UNITY_CALLBACKS

        private BaseLevelGoal levelGoal;
        private int coinAmount;
        private bool isStreakBonusFailShow;
        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Show(Action action = null, bool isForceShow = false)
        {
            SoundHandler.Instance.PlaySound(SoundType.LevelFail);

            reviveDataConfig = reviveDataSO.GetValue<ReviveDataConfig>();

            coinAmount = reviveDataConfig.GetCointAmount(GameplayManager.Instance.ReviveCountCoin);

            for (int i = 0; i < GameplayManager.Instance.CurrentLevel.LevelGoals.Count; i++)
            {
                if (!GameplayManager.Instance.CurrentLevel.LevelGoals[i].IsGoalFullFilled())
                {
                    levelGoal = GameplayManager.Instance.CurrentLevel.LevelGoals[i];
                    break;
                }
            }

            goalText.text = (levelGoal.GoalCount - levelGoal.CurrentCount).ToString();
            goalImage.sprite = levelGoal.GetRender();
            coinText.text = (coinAmount).ToString();
            adButton.gameObject.SetActive(reviveDataConfig.reviveCountAd > GameplayManager.Instance.ReviveCountAd);
            normalOutOfSpaceGO.SetActive(true);
            streakBonusGO.SetActive(false);
            base.Show(action, isForceShow);

            commonOfferView.Init();

            CommonOfferData commonOfferData = CommonOfferManager.Instance.GetOfferData(commonOfferView.OfferId);
            if (commonOfferData != null && commonOfferData.IsActive())
            {
                if (reviveDataConfig.isLevelFailOfferActive)
                {
                    commonOfferView.ShowView(commonOfferData.iapShopBundleData);
                }
                else
                {
                    commonOfferView.gameObject.SetActive(false);
                    commonOfferView.Hide();
                }
            }
            else
            {
                commonOfferView.gameObject.SetActive(false);
                commonOfferView.Hide();
            }
            isStreakBonusFailShow = StreakBonusManager.Instance.IsSystemActive() && StreakBonusManager.Instance.CurrentStreak > 0;
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void OnBackButtonPressed()
        {
            OnClose();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        public void Event_OnPointerDown_Hold_Button()
        {
            animationForHold.HideAnimation(null);
        }

        public void Event_OnPointerup_Hold_Button()
        {
            animationForHold.ShowAnimation(null);
        }

        #endregion

        #region UI_CALLBACKS       

        public void OnClose()
        {
            if (isStreakBonusFailShow)
            {
                isStreakBonusFailShow = false;
                normalOutOfSpaceGO.SetActive(false);
                streakBonusProgressBar.Init();
                streakBonusGO.SetActive(true);
            }
            else
            {
                GameplayManager.Instance.OnLevelFail();
                Hide();
            }
        }

        public void OnAdContinue()
        {
            if (LevelEditorManager.Instance != null)
            {
                LevelManager.Instance.LoadedLevel.RemoveStackFromGrid(reviveDataConfig.removeStackCountCoin);
                Hide();
                return;
            }

            if (reviveDataSO != null)
            {
                AdManager.Instance.ShowRewardedAd(() =>
                {
                    string levelString = GameplayManager.Instance.CurrentHandler.GetCurrentLevelEventString();

                    GameplayManager.Instance.ReviveCountAd++;
                    AnalyticsManager.Instance.LogGAEvent("LevelContinue : " + levelString + " : " + "Ad : " + GameplayManager.Instance.ReviveCountAd);
                    GameplayManager.testPrintData.AddOutOfSpaceCoount();
                    LevelManager.Instance.LoadedLevel.RemoveStackFromGrid(reviveDataConfig.removeStackCountAd);
                    Hide();
                    InputManager.StopInteraction = false;

                    if (streakBonusGO.activeInHierarchy)
                    {
                        AnalyticsManager.Instance.LogGAEvent("StreakLevelContinue : " + StreakBonusManager.Instance.CurrentStreak + " : " + levelString);
                    }
                },
                    RewardAdShowCallType.OutOfSpace, "OutOfSpace");
            }
        }

        public void OnContinue()
        {
            if (LevelEditorManager.Instance != null)
            {
                LevelManager.Instance.LoadedLevel.RemoveStackFromGrid(reviveDataConfig.removeStackCountCoin);
                Hide();
                return;
            }

            if (reviveDataSO != null)
            {
                if (DataManager.Instance.GetCurrency(CurrencyConstant.COINS).Value >= coinAmount)
                {
                    AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Sink, AnalyticsConstants.CoinCurrency
                        , coinAmount, AnalyticsConstants.ItemType_Continue, AnalyticsConstants.ItemId_LevelContinue);

                    string levelString = GameplayManager.Instance.CurrentHandler.GetCurrentLevelEventString();

                    GameplayManager.testPrintData.AddOutOfSpaceCoount();
                    DataManager.Instance.GetCurrency(CurrencyConstant.COINS).Add(-coinAmount);
                    GameplayManager.Instance.ReviveCountCoin++;
                    AnalyticsManager.Instance.LogGAEvent("LevelContinue : " + levelString + " : " + "Coin : " + coinAmount);
                    LevelManager.Instance.LoadedLevel.RemoveStackFromGrid(reviveDataConfig.removeStackCountCoin);
                    Hide();
                    InputManager.StopInteraction = false;

                    if (streakBonusGO.activeInHierarchy)
                    {
                        AnalyticsManager.Instance.LogGAEvent("StreakLevelContinue : " + StreakBonusManager.Instance.CurrentStreak + " : " + levelString);
                    }
                }
                else
                {
                    MainSceneUIManager.Instance.GetView<ShopView>().Show();
                }
            }
        }

        #endregion
    }
}
