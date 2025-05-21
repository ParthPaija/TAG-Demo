using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class GameplayGoalHandler : SerializedManager<GameplayGoalHandler>
    {
        #region PUBLIC_VARS
        public bool IsGameComplate { get => isGameComplate; set => isGameComplate = value; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private List<BaseLevelGoal> levelGoals = new List<BaseLevelGoal>();
        [SerializeField] bool isGameComplate = false;
        private List<Action<GoalType, int, int>> onGoalUpdate = new List<Action<GoalType, int, int>>();

        #endregion

        #region UNITY_CALLBACKS

        public override void OnDestroy()
        {
            base.OnDestroy();
            MainSceneUIManager.Instance.GetView<VFXView>().DeRagisterOnItemAnimationComplete(OnGoalAnimationComplate);
        }

        private void Start()
        {
            MainSceneUIManager.Instance.GetView<VFXView>().RagisterOnItemAnimationComplete(OnGoalAnimationComplate);
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void OnLevelStart(LevelDataSO levelDataSO)
        {
            InputManager.StopInteraction = false;
            GameRuleManager.Instance.IsGoalAnimationInProgress = false;
            isGameComplate = false;
            levelGoals = levelDataSO.LevelGoals;
            InitGoals();
            LoadGoalData();
            MainSceneUIManager.Instance.GetView<GameplayGoalView>().ShowView(levelGoals);
        }

        public void UpdateGoals(GoalType goalType, int itemId, int value)
        {
            for (int i = 0; i < levelGoals.Count; i++)
            {
                levelGoals[i].UpdateGoal(goalType, itemId, value);
            }
            InvakeOnGoalUpdate(goalType, itemId, value);

            if (IsGoalFullFilled())
            {
                InputManager.StopInteraction = true;
            }
        }

        public bool IsGoalFullFilled()
        {
            for (int i = 0; i < levelGoals.Count; i++)
            {
                if (!levelGoals[i].IsGoalFullFilled())
                    return false;
            }
            return true;
        }

        public void SaveGoalData(LevelProgressData levelProgressData)
        {
            List<int> goalValue = new List<int>();

            for (int i = 0; i < levelGoals.Count; i++)
            {
                goalValue.Add(levelGoals[i].CurrentCount);
            }

            levelProgressData.currentGoalValue = new List<int>();
            levelProgressData.currentGoalValue = goalValue;
        }

        public void SetUndoBoosterGoalData(LevelProgressData levelProgressData)
        {
            if (levelProgressData.currentGoalValue.Count == levelGoals.Count)
            {
                for (int i = 0; i < levelGoals.Count; i++)
                {
                    levelGoals[i].CurrentCount = levelProgressData.currentGoalValue[i];
                }
            }
            MainSceneUIManager.Instance.GetView<GameplayGoalView>().SetGoalItem();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void OnGoalAnimationComplate(int itemId, int amount, bool isLastObject)
        {
            if (isGameComplate)
                return;

            if (isLastObject)
            {
                GameRuleManager.Instance.IsGoalAnimationInProgress = false;
                if (IsGoalFullFilled())
                {
                    isGameComplate = true;
                    GameplayManager.Instance.OnLevelWin();
                }
            }
        }

        private void InitGoals()
        {
            for (int i = 0; i < levelGoals.Count; i++)
            {
                levelGoals[i].InitGoal();
            }
        }

        private void LoadGoalData()
        {
            LevelProgressData levelProgressData = DataManager.LevelProgressData;

            if (levelProgressData.levelNo == GameplayManager.Instance.CurrentLevel.Level && levelProgressData.currentGoalValue.Count == levelGoals.Count)
            {
                for (int i = 0; i < levelGoals.Count; i++)
                {
                    levelGoals[i].CurrentCount = levelProgressData.currentGoalValue[i];
                }
            }
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        public void InvakeOnGoalUpdate(GoalType goalType, int itemId, int amount)
        {
            foreach (var ev in onGoalUpdate)
            {
                ev?.Invoke(goalType, itemId, amount);
            }
        }

        public void AddListenerOnGoalUpdate(Action<GoalType, int, int> action)
        {
            if (!onGoalUpdate.Contains(action))
                onGoalUpdate.Add(action);
        }

        public void RemoveListenerOnGoalUpdate(Action<GoalType, int, int> action)
        {
            if (onGoalUpdate.Contains(action))
                onGoalUpdate.Remove(action);
        }

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
