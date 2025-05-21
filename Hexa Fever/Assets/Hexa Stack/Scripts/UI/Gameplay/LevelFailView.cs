using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tag.HexaStack
{
    public class LevelFailView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GameObject infiniteGO;
        [SerializeField] private GameObject lifeGO;
        //[SerializeField] private TMP_Text levelNoText;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Show(Action action = null, bool isForceShow = false)
        {
            var life = (CurrencyTimeBase)DataManager.Instance.GetCurrency(CurrencyConstant.ENERGY);
            infiniteGO.SetActive(life.IsInfiniteEnergyActive);
            lifeGO.SetActive(!life.IsInfiniteEnergyActive);
            base.Show(action, isForceShow);
            //levelNoText.text = $"Level {DataManager.PlayerData.playerGameplayLevel}";
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
            var currency = DataManager.Instance.GetCurrency(CurrencyConstant.ENERGY);
            if (currency.Value < 1)
            {
                MainSceneUIManager.Instance.GetView<NotEnoughLifeView>().ShowView(() =>
                {
                    if (GameplayManager.Instance.CurrentHandler.gameplayHandlerType == GameplayHandlerType.VIPLeague && !VIPLeaderboardManager.Instance.IsCurrentLeaderboardEventActive())
                    {
                        OnClose();
                    }
                    else {
                        GameplayManager.Instance.OnLevelFailRetry();
                        GameplayManager.Instance.OnCurrentLevelRestartOrNext();
                    }
                }, () =>
                {
                    OnClose();
                });
                return;
            }
            else
            {
                if (GameplayManager.Instance.CurrentHandler.gameplayHandlerType == GameplayHandlerType.VIPLeague && !VIPLeaderboardManager.Instance.IsCurrentLeaderboardEventActive())
                {
                    OnClose();
                }
                else
                {
                    GameplayManager.Instance.OnLevelFailRetry();
                    GameplayManager.Instance.OnCurrentLevelRestartOrNext();
                }
            }
        }

        public void OnClose()
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
    }
}
