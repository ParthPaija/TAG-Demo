using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Tag.HexaStack
{
    public class DailyDealsView : MonoBehaviour
    {
        #region PUBLIC_VARS
        [SerializeField] private List<DailyDealsItemSloatView> _slotViews = new List<DailyDealsItemSloatView>();
        [SerializeField] private Text timerText;
        #endregion

        #region PRIVATE_VARS
        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init()
        {
            if (DailyDealsManager.Instance.IsDailyDealsActive())
            {
                gameObject.SetActive(true);
                InitializeUI();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void UpdateDealsUI()
        {
            SetSlots();
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        private void InitializeUI()
        {
            SetSlots();
        }

        private void SetSlots()
        {
            List<DailyDealSlotConfig> deals = DailyDealsManager.Instance.GetCurrentDeals();

            for (int i = 0; i < deals.Count; i++)
            {
                _slotViews[i].Init(deals[i], i);
            }
        }

        public void UpdateTimerDisplay(TimeSpan timeSpan)
        {
            if (timerText != null)
            {
                timerText.text = timeSpan.ParseTimeSpan(2);
            }
        }

        private void OnDealClaimed(int dealIndex)
        {
        }

        private void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
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
