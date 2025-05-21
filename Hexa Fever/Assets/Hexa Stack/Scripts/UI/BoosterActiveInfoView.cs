using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

namespace Tag.HexaStack
{
    public class BoosterActiveInfoView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Image boosterImage;
        [SerializeField] private Localize boosterInfoText;
        [SerializeField] private Localize boosterNameText;
        private Action onClose;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowView(BoosterData boosterData, Action onClose)
        {
            this.onClose = onClose;
            boosterImage.sprite = boosterData.boosterSprite;
            boosterInfoText.SetTerm(boosterData.boosterDes);
            boosterNameText.SetTerm(boosterData.boosterName);
            base.Show();
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

        #endregion

        #region UI_CALLBACKS       

        public void OnClose()
        {
            Hide();
            onClose?.Invoke();
        }

        #endregion
    }
}
