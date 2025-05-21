using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class GameplayGoalView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] UIAnimationHandler uIAnimationHandler;
        [SerializeField] private GoalItemView allItemGoalView;
        [SerializeField] private GoalItemView goalUIItemPrefab;
        [SerializeField] private Transform parent;
        [SerializeField] private Image goalBGImage;

        private List<BaseLevelGoal> baseLevelGoals;
        private List<GoalItemView> goalUIItemsObj = new List<GoalItemView>();

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            TimeManager.Instance.RegisterTimerTickEvent(TimeManager_onTimerTick);
        }

        private void OnDisable()
        {
            TimeManager.Instance.DeRegisterTimerTickEvent(TimeManager_onTimerTick);
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowView(List<BaseLevelGoal> baseLevelGoals)
        {
            this.baseLevelGoals = baseLevelGoals;
            SetView();
            base.Show();
            SetGoalItem();
        }

        public void SetGoalItem()
        {
            for (int i = 0; i < goalUIItemsObj.Count; i++)
            {
                goalUIItemsObj[i].SetGoalItem();
            }
        }

        public List<Transform> GetItemEndPosition(GoalType goalType, int itemId)
        {
            List<Transform> tempTransform = new List<Transform>();
            for (int i = 0; i < goalUIItemsObj.Count; i++)
            {
                Transform temp = goalUIItemsObj[i].GetItemEndPosition(goalType, itemId);
                if (temp != null)
                    tempTransform.Add(temp);
            }
            return tempTransform;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetView()
        {
            parent.gameObject.SetActive(false);
            goalBGImage.enabled = false;
            allItemGoalView.gameObject.SetActive(false);
            GoalItemView goalUIItem = null;
            ClearViewGoal();
            for (int i = 0; i < baseLevelGoals.Count; i++)
            {
                if ((baseLevelGoals[i].GetType() == typeof(AllItemCollectGoal) || baseLevelGoals[i].GetType() == typeof(CoinItemCollectGoal)) && i == 0)
                {
                    goalUIItem = allItemGoalView;
                    goalUIItem.gameObject.SetActive(true);
                }
                else
                {
                    goalBGImage.enabled = true;
                    parent.gameObject.SetActive(true);
                    goalUIItem = Instantiate(goalUIItemPrefab, parent);
                }
                goalUIItem.gameObject.SetActive(true);
                goalUIItem.SetView(baseLevelGoals[i]);
                goalUIItemsObj.Add(goalUIItem);
            }
        }

        private void ClearViewGoal()
        {
            for (int i = 0; i < goalUIItemsObj.Count; i++)
            {
                if (goalUIItemsObj[i] != allItemGoalView)
                    Destroy(goalUIItemsObj[i].gameObject);
            }
            goalUIItemsObj.Clear();
        }

        public void ShowAnimation()
        {
            uIAnimationHandler.ShowAnimation(() => { });
        }

        public void HideAnimation()
        {
            uIAnimationHandler.HideAnimation(() => { });
        }

        private void TimeManager_onTimerTick(DateTime currentDateTime)
        {
            GameplayManager.Instance.IncreaseLevelRunTime();
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
