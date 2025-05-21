using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class SettingView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private string privacyPolicyUrl;

        [SerializeField] private SettingToggle soundToggle;
        [SerializeField] private SettingToggle vibrationToggle;
        [SerializeField] private SettingToggle notificationToggle;

        [Space]
        [SerializeField] private Text buildVersionAndCodeText;
        private const string BuildVersionCodeFormat = "v {0} ({1})";
        private bool isNotifiactionOn = true;

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

        private void InitToggles()
        {
            soundToggle.InitView(GetSoundToggleValue, SetSoundToggleValue);
            vibrationToggle.InitView(GetVibrationToggleValue, SetVibrationToggleValue);
            notificationToggle.InitView(GetNotificationToggleValue, SetNotificationToggleValue);
        }

        private bool GetSoundToggleValue()
        {
            return SoundHandler.Instance.IsSFXOn;
        }

        private void SetSoundToggleValue(bool state)
        {
            SoundHandler.Instance.IsSFXOn = state;
            SoundHandler.Instance.IsMusicOn = state;
        }

        private bool GetNotificationToggleValue()
        {
            return isNotifiactionOn;
        }

        private void SetNotificationToggleValue(bool state)
        {
            isNotifiactionOn = state;
        }

        private bool GetVibrationToggleValue()
        {
            return Vibrator.IsVibrateOn;
        }

        private void SetVibrationToggleValue(bool state)
        {
            Vibrator.IsVibrateOn = state;
        }

        private void SetView()
        {
            InitToggles();
            buildVersionAndCodeText.text = Constant.BuildVersionCodeString;
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnSaveProgress()
        {
            GlobalUIManager.Instance.GetView<ToastMessageView>().ShowMessage("Coming Soon", transform.position.With(null, null, 0));
        }

        public void SendEmail()
        {
            //if (string.IsNullOrEmpty(rateUsFeedbackConfig.emailAddress))
            //    return;
            //string playfabid = "NONE";
            //if (PlayfabManager.Instance.IsLoggedIn)
            //    playfabid = PlayfabManager.Instance.PlayfabId;
            //string subject = Application.productName + " v" + Application.version + "(ID - " + playfabid + ")";
            Application.OpenURL("mailto:" + "official.gamingbrew@gmail.com" + "?subject=" + "Hexa Fever Support" + "&body=" + "");
        }

        public void OnButtonClick_Terms()
        {
            Application.OpenURL(privacyPolicyUrl);
        }

        public void OnButtonClick_PrivacyPolicy()
        {
            Application.OpenURL(privacyPolicyUrl);
        }

        public void OnClose()
        {
            Hide();
        }

        public void OnInAppReviewButton()
        {
            RateUsManager.Instance.ShowInAppReview();
        }

        public void OnLanguageButton()
        {
            MainSceneUIManager.Instance.GetView<LanguageSelectionView>().Show(null);
        }

        #endregion
    }
}
