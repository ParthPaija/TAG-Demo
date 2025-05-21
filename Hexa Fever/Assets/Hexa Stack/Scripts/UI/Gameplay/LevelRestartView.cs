using System;
using UnityEngine;

namespace Tag.HexaStack
{
    public class LevelRestartView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GameObject infiniteGO;
        [SerializeField] private GameObject lifeGO;
        [SerializeField] private GameObject streakBonusGO;
        [SerializeField] private StreakBonusProgressBar streakBonusProgressBar;
        private bool isStreakBonusFailShow;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Show(Action action = null, bool isForceShow = false)
        {
            var life = (CurrencyTimeBase)DataManager.Instance.GetCurrency(CurrencyConstant.ENERGY);
            infiniteGO.SetActive(life.IsInfiniteEnergyActive);
            lifeGO.SetActive(!life.IsInfiniteEnergyActive);
            streakBonusGO.SetActive(false);
            base.Show(action, isForceShow);
            isStreakBonusFailShow = StreakBonusManager.Instance.IsSystemActive() && StreakBonusManager.Instance.CurrentStreak > 0;
        }

        public override void OnBackButtonPressed()
        {
            OnClose();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void ForcefullyCloseAction()
        {
            GlobalUIManager.Instance.GetView<InGameLoadingView>().ShowView(1f, () =>
            {
            });

            Hide();
            MainSceneUIManager.Instance.GetView<GameplayTopbarView>().Hide();
            MainSceneUIManager.Instance.GetView<GameplayBottomView>().Hide();
            MainSceneUIManager.Instance.GetView<GameplayGoalView>().Hide();
            LevelManager.Instance.UnloadLevel();
            MainSceneUIManager.Instance.GetView<MainView>().Show();
            MainSceneUIManager.Instance.GetView<BottombarView>().Show();
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnRetry()
        {
            if (isStreakBonusFailShow)
            {
                isStreakBonusFailShow = false;
                streakBonusGO.SetActive(true);
                streakBonusProgressBar.Init();
            }
            else
            {
                GameplayManager.Instance.OnRetry();

                var currency = DataManager.Instance.GetCurrency(CurrencyConstant.ENERGY);

                if (currency.Value < 1)
                {
                    MainSceneUIManager.Instance.GetView<NotEnoughLifeView>().ShowView(() =>
                    {
                        if (GameplayManager.Instance.CurrentHandler.gameplayHandlerType == GameplayHandlerType.VIPLeague && !VIPLeaderboardManager.Instance.IsCurrentLeaderboardEventActive())
                        {
                            ForcefullyCloseAction();
                        }
                        else
                        {
                            StreakBonusManager.Instance.OnLevelRetry();
                            GameplayManager.Instance.OnCurrentLevelRestartOrNext();
                        }

                    }, () =>
                    {
                        ForcefullyCloseAction();
                    });
                    return;
                }

                if (GameplayManager.Instance.CurrentHandler.gameplayHandlerType == GameplayHandlerType.VIPLeague && !VIPLeaderboardManager.Instance.IsCurrentLeaderboardEventActive())
                {
                    ForcefullyCloseAction();
                }
                else
                {
                    StreakBonusManager.Instance.OnLevelRetry();
                    GameplayManager.Instance.OnCurrentLevelRestartOrNext();
                }
            }
        }

        public void OnClose()
        {
            Hide();
        }

        #endregion
    }
}
