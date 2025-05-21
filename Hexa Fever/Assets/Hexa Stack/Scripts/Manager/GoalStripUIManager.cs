using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class GoalStripUIManager : SerializedManager<GoalStripUIManager>
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Dictionary<LevelType, GoalStripView> goalSripViews = new Dictionary<LevelType, GoalStripView>();
        [SerializeField] private StreakBonusGoalStripPopup streakBonusGoalStripPopup;
        #endregion

        #region UNITY_CALLBACKS

        public override void Awake()
        {
            base.Awake();
            foreach (var item in goalSripViews)
            {
                item.Value.Init();
            }
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        #endregion

        #region PRIVATE_FUNCTIONS

        public void ShowGoalStripView(LevelType levelType, Action onComplate)
        {
            streakBonusGoalStripPopup.Init();
            if (goalSripViews.ContainsKey(levelType))
            {
                goalSripViews[levelType].ShowView(onComplate);
                return;
            }
            goalSripViews[LevelType.Normal].ShowView(onComplate);
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
