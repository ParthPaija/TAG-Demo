using I2.Loc;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class BoosterUnlockView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Localize boosterNameText;
        [SerializeField] private Text boosterCountText;
        [SerializeField] private Localize boosterDesText;
        [SerializeField] private Image boosterImage;
        [SerializeField] private Button cliamButton;
        [SerializeField] private CurrencyAnimation currencyAnimation;
        private Action onHide;
        private BoosterData boosterData;
        [SerializeField] private float animationTime = 1.2f;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowView(BoosterData boosterData, Action onHide = null)
        {
            cliamButton.interactable = true;
            this.boosterData = boosterData;
            this.onHide = onHide;
            boosterNameText.SetTerm(boosterData.boosterName);
            boosterCountText.text = "x" + boosterData.defaultBoosterCount.ToString();
            boosterDesText.SetTerm(boosterData.boosterDes);
            boosterImage.sprite = boosterData.boosterSprite;
            base.Show();
        }

        public override void OnHideComplete()
        {
            base.OnHideComplete();
            onHide?.Invoke();
        }

        public override void OnBackButtonPressed()
        {
            OnCliam();
        }

        public override bool CanPressBackButton()
        {
            if (!cliamButton.interactable)
                return false;
            return base.CanPressBackButton();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        [Button]
        public void OnCliam()
        {
            cliamButton.interactable = false;
            currencyAnimation.UIStartAnimation(MainSceneUIManager.Instance.GetView<GameplayBottomView>().GetBoosterPos(boosterData.boosterID), 1, isReverseAnimation: true, sprite: boosterData.boosterSprite);
            boosterData.SetAsShow();
            DataManager.Instance.GetCurrency(boosterData.boosterID).Add(boosterData.defaultBoosterCount);
            CoroutineRunner.Instance.Wait(animationTime, () =>
            {
                MainSceneUIManager.Instance.GetView<GameplayBottomView>().SetView();
                Hide();
            });
        }

        #endregion
    }
}
