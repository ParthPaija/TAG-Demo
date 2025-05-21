using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class LevelSpwanerConfigItemView : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] internal BaseIDMappingConfig itemIdConfig;
        [SerializeField] internal TMP_Dropdown itemIdDD;
        [SerializeField] internal TMP_InputField goalAmountIF;

        private LevelSpwanerConfig levelSpwanerConfig;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        #endregion

        #region PRIVATE_FUNCTIONS

        public void SetView(LevelSpwanerConfig levelSpwanerConfig)
        {
            this.levelSpwanerConfig = levelSpwanerConfig;
            itemIdDD.gameObject.SetActive(true);
            itemIdDD.ClearOptions();

            List<string> options = new List<string>();
            for (int i = 0; i < itemIdConfig.idMapping.Count; i++)
            {
                options.Add(itemIdConfig.idMapping[i]);
            }
            itemIdDD.AddOptions(options);
            itemIdDD.SetValueWithoutNotify(levelSpwanerConfig.itemId);
            goalAmountIF.text = levelSpwanerConfig.goalValue.ToString();
        }

        public void OnItemIdChanage()
        {
            levelSpwanerConfig.itemId = itemIdDD.value;
        }

        public void OnGoalAmountChanage()
        {
            if (!string.IsNullOrEmpty(goalAmountIF.text) && !string.IsNullOrWhiteSpace(goalAmountIF.text))
            {
                levelSpwanerConfig.goalValue = int.Parse(goalAmountIF.text);
            }
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnRemoveButton()
        {
            LevelEditor.CurrentLevelForEdit.RemoveLevelSpwanerConfig(levelSpwanerConfig);
            LevelEditorUIManager.Instance.GetView<LevelSpwanerConfigView>().SetView();
        }

        #endregion
    }
}
