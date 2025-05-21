using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class LevelGoalsItemViewLE : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] internal BaseIDMappingConfig itemIdConfig;
        [SerializeField] internal BaseIDMappingConfig obstacalIdConfig;

        [SerializeField] internal TMP_Dropdown itemIdDD;
        [SerializeField] internal TMP_Dropdown obstacalIdDD;

        [SerializeField] internal TMP_Text goalDesText;
        [SerializeField] internal TMP_InputField goalAmountIF;
        [SerializeField] internal BaseLevelGoal BaseLevelGoal;
        [SerializeField] internal LevelGoalType goalType;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public virtual void SetView(BaseLevelGoal baseLevelGoal)
        {
            HideAll();
            if (baseLevelGoal.GetType() == typeof(AllItemCollectGoal))
            {
                BaseLevelGoal = (AllItemCollectGoal)baseLevelGoal;
                goalType = LevelGoalType.AllItem;
            }
            else if (baseLevelGoal.GetType() == typeof(AllObstacleCollectGoal))
            {
                BaseLevelGoal = (AllObstacleCollectGoal)baseLevelGoal;
                goalType = LevelGoalType.AllObstacal;
            }
            else if (baseLevelGoal.GetType() == typeof(SpecificItemCollectGoal))
            {
                BaseLevelGoal = (SpecificItemCollectGoal)baseLevelGoal;
                goalType = LevelGoalType.SingleItem;

                itemIdDD.gameObject.SetActive(true);
                itemIdDD.ClearOptions();

                List<string> options = new List<string>();
                for (int i = 0; i < itemIdConfig.idMapping.Count; i++)
                {
                    options.Add(itemIdConfig.idMapping[i]);
                }
                itemIdDD.AddOptions(options);
                itemIdDD.SetValueWithoutNotify(BaseLevelGoal.GetGoalItemId());
            }
            else if (baseLevelGoal.GetType() == typeof(SpecificObstacleCollectGoal))
            {
                BaseLevelGoal = (SpecificObstacleCollectGoal)baseLevelGoal;
                goalType = LevelGoalType.SingleObstacal;

                obstacalIdDD.gameObject.SetActive(true);
                obstacalIdDD.ClearOptions();

                List<string> options = new List<string>();
                for (int i = 0; i < obstacalIdConfig.idMapping.Count; i++)
                {
                    options.Add(obstacalIdConfig.idMapping[i]);
                }
                obstacalIdDD.AddOptions(options);
                obstacalIdDD.SetValueWithoutNotify(BaseLevelGoal.GetGoalItemId());
            }
            goalDesText.text = goalType.ToString();
            goalAmountIF.text = BaseLevelGoal.GoalCount.ToString();
        }

        public void OnGoalAmountChanage()
        {
            if (!string.IsNullOrEmpty(goalAmountIF.text) && !string.IsNullOrWhiteSpace(goalAmountIF.text))
            {
                BaseLevelGoal.GoalCount = int.Parse(goalAmountIF.text);
            }
        }

        public void OnItemIdChanage()
        {
            SpecificItemCollectGoal specificItemCollectGoal = (SpecificItemCollectGoal)BaseLevelGoal;
            specificItemCollectGoal.itemId = itemIdDD.value;
        }

        public void OnObstacalIdChanage()
        {
            SpecificObstacleCollectGoal specificItemCollectGoal = (SpecificObstacleCollectGoal)BaseLevelGoal;
            specificItemCollectGoal.obstacleId = obstacalIdDD.value;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void HideAll()
        {
            itemIdDD.gameObject.SetActive(false);
            obstacalIdDD.gameObject.SetActive(false);
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnRemoveButton()
        {
            LevelEditor.CurrentLevelForEdit.RemoveGoal(BaseLevelGoal);
            LevelEditorUIManager.Instance.GetView<LevelGoalsView>().SetView();
        }

        #endregion
    }

    public enum LevelGoalType
    {
        AllItem,
        AllObstacal,
        SingleItem,
        SingleObstacal
    }
}
