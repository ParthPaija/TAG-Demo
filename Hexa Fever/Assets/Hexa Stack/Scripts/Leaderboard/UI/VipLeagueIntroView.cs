using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class VipLeagueIntroView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        public static bool isReadyToShow = false;
        [SerializeField] private Text leaderBoardTimerText;
        private Action actionToCallOnHide;

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            VIPLeaderboardManager.onLeaderboardEventRunTimerOver += LeaderboardManager_onLeaderboardEventRunTimerOver;
        }

        private void OnDisable()
        {
            VIPLeaderboardManager.onLeaderboardEventRunTimerOver -= LeaderboardManager_onLeaderboardEventRunTimerOver;
            UnregisterLeaderBoardTimer();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void OnBackButtonPressed()
        {
            OnEnterButtonClick();
        }

        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);
            RefreshView();
        }

        public void RefreshView()
        {
            bool isEventResult = !VIPLeaderboardManager.Instance.IsCurrentLeaderboardEventActive() && VIPLeaderboardManager.Instance.IsLastEventResultReadyToShow();

            if (VIPLeaderboardManager.Instance.IsCurrentLeaderboardEventActive() && !VIPLeagueManager.Instance.IsVIPLeageUnlocked())
            {
                isEventResult = true;
            }

            if (!isEventResult)
            {
                RegisterLeaderBoardTimer();
                UpdateLeaderBoardTimer();
            }
        }

        public void ShowWithHideAction(Action actionToCallOnHide)
        {
            this.actionToCallOnHide = actionToCallOnHide;
            Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void OnHideComplete()
        {
            base.OnHideComplete();

            actionToCallOnHide?.Invoke();
            actionToCallOnHide = null;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void LeaderboardManager_onLeaderboardEventRunTimerOver()
        {
            RefreshView();
        }

        private void RegisterLeaderBoardTimer()
        {
            if (VIPLeaderboardManager.Instance.IsSystemInitialized && VIPLeaderboardManager.Instance.LeaderboardRunTimer != null)
                VIPLeaderboardManager.Instance.LeaderboardRunTimer.RegisterTimerTickEvent(UpdateLeaderBoardTimer);
        }

        private void UpdateLeaderBoardTimer()
        {
            leaderBoardTimerText.text = VIPLeaderboardManager.Instance.LeaderboardRunTimer.GetRemainingTimeSpan().ParseTimeSpan(2);
        }

        private void UnregisterLeaderBoardTimer()
        {
            if (VIPLeaderboardManager.Instance.IsSystemInitialized && VIPLeaderboardManager.Instance.LeaderboardRunTimer != null)
                VIPLeaderboardManager.Instance.LeaderboardRunTimer.UnregisterTimerTickEvent(UpdateLeaderBoardTimer);
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnEnterButtonClick()
        {
            MainSceneUIManager.Instance.GetView<VIPLeaderboardView>().Show();
            Hide();
        }

        #endregion
    }
}
