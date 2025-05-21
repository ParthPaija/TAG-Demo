using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class DailyAdPassViewButton : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Text timerText;
        [SerializeField] private GameObject freeGO;
        [SerializeField] private Text rewardCountText;
        [SerializeField] private GameObject rewardCountGO;

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            if (!DailyAdPassManager.Instance.IsSystemActive())
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            SetView();
            DailyAdPassManager.onFreeRewardTimerRunTimerOver += SetView;
            DailyAdPassManager.onDailyAdPassRunTimerOver += SetView;
        }

        private void OnDisable()
        {
            DailyAdPassManager.onFreeRewardTimerRunTimerOver -= SetView;
            DailyAdPassManager.onDailyAdPassRunTimerOver -= SetView;
            UnregisterTimer();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView()
        {
            int rewardCount = DailyAdPassManager.Instance.GetCliamRewardCount();
            rewardCountGO.SetActive(rewardCount > 0);
            if (rewardCount <= 0)
            {
                if (!DailyAdPassManager.Instance.IsFreeRewardReady())
                {
                    RegisterTimer();
                }
                freeGO.SetActive(false);
                timerText.gameObject.SetActive(true);
            }
            else
            {
                timerText.gameObject.SetActive(false);
                freeGO.SetActive(true);
            }
            rewardCountText.text = DailyAdPassManager.Instance.GetCliamRewardCount().ToString();
        }

        private void RegisterTimer()
        {
            if (DailyAdPassManager.Instance.FreeRewardTimer != null)
                DailyAdPassManager.Instance.FreeRewardTimer.RegisterTimerTickEvent(UpdateTimer);
        }

        private void UpdateTimer()
        {
            timerText.text = DailyAdPassManager.Instance.FreeRewardTimer.GetRemainingTimeSpan().ParseTimeSpan(2);
        }

        private void UnregisterTimer()
        {
            if (DailyAdPassManager.Instance.FreeRewardTimer != null)
                DailyAdPassManager.Instance.FreeRewardTimer.UnregisterTimerTickEvent(UpdateTimer);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnDailyTaskButtonClick()
        {
            MainSceneUIManager.Instance.GetView<DailyAdPassView>().Show();
        }

        #endregion
    }
}
