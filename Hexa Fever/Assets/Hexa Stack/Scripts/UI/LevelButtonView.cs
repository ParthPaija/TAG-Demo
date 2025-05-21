using I2.Loc;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class LevelButtonView : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private LocalizationParamsManager levelNumberTextLocalizeParam;
        [SerializeField] private StreakBonusProgressBar streakBonusProgressBar;

        private Action onClick;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView(Action onClick)
        {
            gameObject.SetActive(true);
            this.onClick = onClick;
            levelNumberTextLocalizeParam.SetParameterValue(levelNumberTextLocalizeParam._Params[0].Name, DataManager.PlayerData.playerGameplayLevel.ToString());
            streakBonusProgressBar.Init();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnClick()
        {
            onClick?.Invoke();
        }

        #endregion
    }
}
