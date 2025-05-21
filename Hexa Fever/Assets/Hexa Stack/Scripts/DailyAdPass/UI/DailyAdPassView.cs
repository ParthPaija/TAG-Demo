using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class DailyAdPassView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Text timerText;
        [SerializeField] private DailyAdPassFreeRewardView freeRewardView;
        [SerializeField] private List<DailyAdPassAdRewardView> adRewardViews;
        [SerializeField] private CurrencyTopbarComponents coinTopbar;
        private bool isCurrncyAnimationInProgress = false;

        #endregion

        #region UNITY_CALLBACKS

        private void OnDisable()
        {
            DailyAdPassManager.onDailyAdPassRunTimerOver -= SetView;
            UnregisterTimer();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Show(Action action = null, bool isForceShow = false)
        {
            isCurrncyAnimationInProgress = false;
            base.Show(action, isForceShow);
            SetView();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void SetNextAdRewardView(int index)
        {
            if (adRewardViews.Count > index + 1)
            {
                adRewardViews[index + 1].SetUI();
            }
        }

        public void PlayCurrencyCoinAnimation(Vector3 startPos, int amount)
        {
            isCurrncyAnimationInProgress = true;
            coinTopbar.CurrencyAnimation.UIStartAnimation(startPos, endPos: coinTopbar.EndPos, amount);
            CoroutineRunner.Instance.Wait(1.35f, () =>
            {
                isCurrncyAnimationInProgress = false;
            });
        }

        public override void OnBackButtonPressed()
        {
            OnClose();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetView()
        {
            DailyAdPassManager.onDailyAdPassRunTimerOver += SetView;
            RegisterTimer();
            SetFreeRewardView();
            SetAdRewardView();
        }

        private void SetFreeRewardView()
        {
            freeRewardView.SetView();
        }

        private void SetAdRewardView()
        {
            for (int i = 0; i < adRewardViews.Count; i++)
            {
                adRewardViews[i].SetView(DailyAdPassManager.Instance.DailyAdPassData.adReward[i]);
            }
        }

        private void RegisterTimer()
        {
            if (DailyAdPassManager.Instance.AdRewardTimer != null)
                DailyAdPassManager.Instance.AdRewardTimer.RegisterTimerTickEvent(UpdateTimer);
        }

        private void UpdateTimer()
        {
            timerText.text = DailyAdPassManager.Instance.AdRewardTimer.GetRemainingTimeSpan().ParseTimeSpan(2);
        }

        private void UnregisterTimer()
        {
            if (DailyAdPassManager.Instance.AdRewardTimer != null)
                DailyAdPassManager.Instance.AdRewardTimer.UnregisterTimerTickEvent(UpdateTimer);
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnClose()
        {
            if (isCurrncyAnimationInProgress)
                return;
            Hide();
        }

        #endregion
    }
}
