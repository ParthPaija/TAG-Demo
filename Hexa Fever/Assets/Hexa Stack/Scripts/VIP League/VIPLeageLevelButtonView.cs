using I2.Loc;
using System;
using UnityEngine;

namespace Tag.HexaStack
{
    public class VIPLeageLevelButtonView : MonoBehaviour
    {
        #region PUBLIC_VARS

        [SerializeField] private LocalizationParamsManager levelNumberTextLocalizeParam;
        [SerializeField] private StreakBonusProgressBar streakBonusProgressBar;
        private Action<LevelDataSO> onClick;
        private LevelDataSO levelData;

        #endregion

        #region PRIVATE_VARS

        #endregion+

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView(LevelDataSO levelDataSO, Action<LevelDataSO> onClick)
        {
            levelData = levelDataSO;
            gameObject.SetActive(true);
            this.onClick = onClick;
            //levelButtonText.text = $"Round {PlayerPersistantData.GetVIPLeaderboardPlayerData().currentLevel}";
            levelNumberTextLocalizeParam.SetParameterValue(levelNumberTextLocalizeParam._Params[0].Name, PlayerPersistantData.GetVIPLeaderboardPlayerData().currentLevel.ToString());
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
            if (!VIPLeaderboardManager.Instance.IsSystemInitialized)
                return;

            onClick?.Invoke(levelData);
        }

        #endregion
    }
}
