using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class VIPLeaderboardUIButton : MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        #endregion

        #region PRIVATE_VARIABLES

        [SerializeField] private GameObject buttonParent;
        [SerializeField] private GameObject notificationObject;
        [SerializeField] private Text leaderBoardTimerText;
        [SerializeField] private Text leaderBoardPlayerRank;

        #endregion

        #region PROPERTIES

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            CheckForLeaderboardButton();
            VIPLeaderboardManager.onLeaderboardEventStateChanged += LeaderboardManager_onLeaderboardEventStateChanged;
        }

        private void OnDisable()
        {
            VIPLeaderboardManager.onLeaderboardEventStateChanged -= LeaderboardManager_onLeaderboardEventStateChanged;
            UnregisterLeaderBoardTimer();
        }

        #endregion

        #region PUBLIC_METHODS

        #endregion

        #region PRIVATE_METHODS

        public void CheckForLeaderboardButton()
        {
            bool isLeaderboardUnlocked = VIPLeaderboardManager.Instance.IsLeaderboardUnlocked();
            UnregisterLeaderBoardTimer();

            if (isLeaderboardUnlocked)
                RegisterLeaderBoardTimer();

            buttonParent.gameObject.SetActive(VIPLeaderboardManager.Instance.IsEventResultRedayForShown() || VIPLeaderboardManager.Instance.IsCurrentLeaderboardEventActive());
            leaderBoardPlayerRank.text = "#" + VIPLeaderboardManager.Instance.GetPlayerRank();
            RefreshNotificationObject();
        }

        private void RegisterLeaderBoardTimer()
        {
            if (VIPLeaderboardManager.Instance.IsSystemInitialized && VIPLeaderboardManager.Instance.LeaderboardRunTimer != null)
                VIPLeaderboardManager.Instance.LeaderboardRunTimer.RegisterTimerTickEvent(UpdateLeaderBoardTimer);
        }

        private void UnregisterLeaderBoardTimer()
        {
            if (VIPLeaderboardManager.Instance.IsSystemInitialized && VIPLeaderboardManager.Instance.LeaderboardRunTimer != null)
                VIPLeaderboardManager.Instance.LeaderboardRunTimer.UnregisterTimerTickEvent(UpdateLeaderBoardTimer);
        }

        private void UpdateLeaderBoardTimer()
        {
            leaderBoardTimerText.text = VIPLeaderboardManager.Instance.LeaderboardRunTimer.GetRemainingTimeSpan().ParseTimeSpan(2);
        }

        #endregion

        #region EVENT_HANDLERS

        private void LeaderboardManager_onLeaderboardEventStateChanged()
        {
            CheckForLeaderboardButton();
            
        }

        private void RefreshNotificationObject()
        {
            notificationObject.gameObject.SetActive(false);
            if (!VIPLeaderboardManager.Instance.IsCurrentLeaderboardEventActive() && VIPLeaderboardManager.Instance.IsLastEventResultReadyToShow())
                notificationObject.gameObject.SetActive(true);
        }

        #endregion

        #region COROUTINES

        #endregion

        #region UI_CALLBACKS

        public void OnButtonClick_Leaderboard()
        {
            if (VIPLeaderboardManager.Instance.CanOpenLeaderboardUI())
                MainSceneUIManager.Instance.GetView<VIPLeaderboardView>().Show();
        }

        #endregion
    }
}