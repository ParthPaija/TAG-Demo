using System;
using UnityEngine;

namespace Tag.HexaStack
{
    public class GameplaySettingView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GameObject soundOffGO;
        [SerializeField] private GameObject vibrateOffGO;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);
            SetView();
        }

        public override void Hide()
        {
            base.Hide();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private bool GetVibrationToggleValue()
        {
            return Vibrator.IsVibrateOn;
        }

        private void SetVibrationToggleValue(bool state)
        {
            Vibrator.IsVibrateOn = state;
        }

        private bool GetSoundToggleValue()
        {
            return SoundHandler.Instance.IsSFXOn;
        }

        private void SetSoundToggleValue(bool state)
        {
            SoundHandler.Instance.IsSFXOn = state;
        }

        private void SetView()
        {
            soundOffGO.SetActive(!GetSoundToggleValue());
            vibrateOffGO.SetActive(!GetVibrationToggleValue());
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnSoundClick()
        {
            SetSoundToggleValue(!GetSoundToggleValue());
            SetView();
        }

        public void OnVibarteClick()
        {
            SetVibrationToggleValue(!GetVibrationToggleValue());
            SetView();
        }

        public void OnClose()
        {
            Hide();
        }

        public void OnRestart()
        {
            MainSceneUIManager.Instance.GetView<LevelRestartView>().Show();
            Hide();
        }

        public void OnExit()
        {
            if (PropellerCellUnlocker.IsPropellerAnimationRunning)
                return;

            if (GameRuleManager.Instance.IsMovementInProgress)
            {
                return;
            }

            Hide();
            GlobalUIManager.Instance.GetView<InGameLoadingView>().ShowView(0.8f, () =>
            {
            });
            GameplayManager.Instance.OnExit();
        }

        #endregion
    }
}
