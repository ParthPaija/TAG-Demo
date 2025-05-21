using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class GameplayTopbarView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] GameObject winButton;
        [SerializeField] UIAnimationHandler uIAnimationHandler;
        [SerializeField] private List<CurrencyTopbarComponents> topbarComponents = new List<CurrencyTopbarComponents>();

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Show(Action action = null, bool isForceShow = false)
        {
            winButton.SetActive(!Constant.IsProductionBuid);
            base.Show(action, isForceShow);
            MainSceneUIManager.Instance.OnGameplayView();
        }

        public override void OnBackButtonPressed()
        {
            MainSceneUIManager.Instance.GetView<GameplaySettingView>().Show();
        }

        public void ShowAnimation()
        {
            uIAnimationHandler.ShowAnimation(() =>
            {

            });
        }

        public void HideAnimation()
        {
            uIAnimationHandler.HideAnimation(() =>
            {
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

        public void OnSetting()
        {
            MainSceneUIManager.Instance.GetView<GameplaySettingView>().Show();
        }

        #endregion
    }
}
