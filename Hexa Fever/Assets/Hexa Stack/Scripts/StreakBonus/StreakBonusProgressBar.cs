using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class StreakBonusProgressBar : MonoBehaviour
    {
        #region PUBLIC_VARS

        public Text rewardPropeller;
        public List<GameObject> progressGO;
        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init()
        {
            if (StreakBonusManager.Instance.IsSystemActive())
            {
                SetUi();
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetUi()
        {
            if (rewardPropeller != null)
            {
                rewardPropeller.text = StreakBonusManager.Instance.GetCurrentStreakBonusData().propellersCount.ToString();
            }
            int currentStreak = StreakBonusManager.Instance.CurrentStreak;
            for (int i = 0; i < progressGO.Count; i++)
            {
                progressGO[i].SetActive(i < currentStreak);
            }
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
