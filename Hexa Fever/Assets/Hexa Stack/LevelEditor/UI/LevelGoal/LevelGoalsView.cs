using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class LevelGoalsView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] Transform parent;
        [SerializeField] private LevelGoalsItemViewLE levelGoalPrefab;
        [SerializeField] private List<LevelGoalsItemViewLE> levelGoalsItemViews;
        private LevelDataSO levelDataSO;

        [SerializeField] private TMP_Dropdown goalTypeDD;
        [SerializeField] private TMP_InputField goalAmountIF;

        [SerializeField] internal TMP_Dropdown itemIdDD;
        [SerializeField] internal TMP_Dropdown obstacalIdDD;

        [SerializeField] internal BaseIDMappingConfig itemIdConfig;
        [SerializeField] internal BaseIDMappingConfig obstacalIdConfig;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        [Button]
        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);
            SetView();
        }

        public void SetView()
        {
            levelDataSO = LevelEditor.CurrentLevelForEdit;

            List<BaseLevelGoal> baseLevelGoals = levelDataSO.LevelGoals;

            for (int i = 0; i < levelGoalsItemViews.Count; i++)
            {
                Destroy(levelGoalsItemViews[i].gameObject);
            }
            levelGoalsItemViews.Clear();
            levelGoalsItemViews = new List<LevelGoalsItemViewLE>();

            for (int i = 0; i < baseLevelGoals.Count; i++)
            {
                LevelGoalsItemViewLE temp = Instantiate(levelGoalPrefab, parent);
                temp.gameObject.SetActive(true);
                temp.SetView(baseLevelGoals[i]);
                levelGoalsItemViews.Add(temp);
            }

            goalTypeDD.ClearOptions();
            goalTypeDD.AddOptions(Enum.GetNames(typeof(LevelGoalType)).ToList());
            goalTypeDD.SetValueWithoutNotify((int)LevelGoalType.AllItem);

            itemIdDD.gameObject.SetActive(true);
            itemIdDD.ClearOptions();

            List<string> options = new List<string>();
            for (int i = 0; i < itemIdConfig.idMapping.Count; i++)
            {
                options.Add(itemIdConfig.idMapping[i]);
            }
            itemIdDD.AddOptions(options);

            obstacalIdDD.gameObject.SetActive(true);
            obstacalIdDD.ClearOptions();

            List<string> optionsNew = new List<string>();
            for (int i = 0; i < obstacalIdConfig.idMapping.Count; i++)
            {
                optionsNew.Add(obstacalIdConfig.idMapping[i]);
            }
            obstacalIdDD.AddOptions(optionsNew);
        }

        [Button]
        public override void Hide()
        {
            base.Hide();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnAddGoal()
        {
            if (string.IsNullOrEmpty(goalAmountIF.text) && string.IsNullOrWhiteSpace(goalAmountIF.text))
            {
                return;
            }

            LevelGoalType goalType = (LevelGoalType)goalTypeDD.value;
            switch (goalType)
            {
                case LevelGoalType.AllItem:
                    AllItemCollectGoal allItemCollectGoal = new AllItemCollectGoal();
                    allItemCollectGoal.GoalCount = int.Parse(goalAmountIF.text);
                    levelDataSO.AddGoal(allItemCollectGoal);
                    SetView();
                    break;
                case LevelGoalType.AllObstacal:
                    AllObstacleCollectGoal allObstacleCollectGoal = new AllObstacleCollectGoal();
                    allObstacleCollectGoal.GoalCount = int.Parse(goalAmountIF.text);
                    levelDataSO.AddGoal(allObstacleCollectGoal);
                    SetView();
                    break;
                case LevelGoalType.SingleItem:
                    SpecificItemCollectGoal specificItemCollectGoal = new SpecificItemCollectGoal();
                    specificItemCollectGoal.GoalCount = int.Parse(goalAmountIF.text);
                    specificItemCollectGoal.itemId = itemIdDD.value;
                    levelDataSO.AddGoal(specificItemCollectGoal);
                    SetView();
                    break;
                case LevelGoalType.SingleObstacal:
                    SpecificObstacleCollectGoal specificObstacleCollectGoal = new SpecificObstacleCollectGoal();
                    specificObstacleCollectGoal.GoalCount = int.Parse(goalAmountIF.text);
                    specificObstacleCollectGoal.obstacleId = obstacalIdDD.value;
                    levelDataSO.AddGoal(specificObstacleCollectGoal);
                    SetView();
                    break;
                default:
                    Debug.LogError("Default " + goalType);
                    break;
            }
        }

        #endregion
    }
}
