using I2.Loc;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class LanguageSelectionButton : MonoBehaviour
    {
        #region PRIVATE_VARS

        [SerializeField] private GameObject selectedGO;
        [SerializeField] private SetLanguage setLanguage;
        private bool isEnable;
        private Action languageSelectAction;

        public SetLanguage SetLanguage { get => setLanguage; set => setLanguage = value; }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void OnButtonClick()
        {
            if (!isEnable)
            {
                gameObject.SetActive(false);
                return;
            }

            if (LocalizationManager.CurrentLanguage == setLanguage._Language)
            {
                if (languageSelectAction != null)
                    languageSelectAction?.Invoke();
                GlobalUIManager.Instance.HideView<LanguageSelectionView>();
                return;
            }
            setLanguage.ApplyLanguage();

            MainSceneUIManager.Instance.GetView<LanguageSelectionView>().SetButtons();
            MainSceneUIManager.Instance.HideView<LanguageSelectionView>();

            if (languageSelectAction != null)
                languageSelectAction?.Invoke();
        }

        public void SetButtonImage(bool isSelcted, bool isEnable, Action languageSelectAction)
        {
            this.isEnable = isEnable;
            gameObject.SetActive(isEnable);
            selectedGO.SetActive(isSelcted); ;
            this.languageSelectAction = languageSelectAction;
        }

        [Button]
        public void SetButton()
        {
            SetLanguage = gameObject.GetComponent<SetLanguage>();
        }

        #endregion

        #region PRIVATE_FUNCTIONS
        #endregion

        #region CO-ROUTINES
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}
