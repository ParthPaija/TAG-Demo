using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class LevelSpwanerConfigView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] Transform parent;
        [SerializeField] private LevelSpwanerConfigItemView prefab;
        [SerializeField] private List<LevelSpwanerConfigItemView> itemViews;
        private LevelDataSO levelDataSO;

        [SerializeField] private TMP_InputField goalAmountIF;

        [SerializeField] internal TMP_Dropdown itemIdDD;

        [SerializeField] internal BaseIDMappingConfig itemIdConfig;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);
            SetView();
        }

        public void SetView()
        {
            levelDataSO = LevelEditor.CurrentLevelForEdit;

            List<LevelSpwanerConfig> levelSpwanerConfigs = levelDataSO.LevelSpwanerConfigs;

            for (int i = 0; i < itemViews.Count; i++)
            {
                Destroy(itemViews[i].gameObject);
            }
            itemViews.Clear();
            itemViews = new List<LevelSpwanerConfigItemView>();

            for (int i = 0; i < levelSpwanerConfigs.Count; i++)
            {
                LevelSpwanerConfigItemView temp = Instantiate(prefab, parent);
                temp.gameObject.SetActive(true);
                temp.SetView(levelSpwanerConfigs[i]);
                itemViews.Add(temp);
            }

            itemIdDD.ClearOptions();

            List<string> options = new List<string>();
            for (int i = 0; i < itemIdConfig.idMapping.Count; i++)
            {
                options.Add(itemIdConfig.idMapping[i]);
            }
            itemIdDD.AddOptions(options);
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

            LevelSpwanerConfig levelSpwanerConfig = new LevelSpwanerConfig();
            levelSpwanerConfig.goalValue = int.Parse(goalAmountIF.text);
            levelSpwanerConfig.itemId = itemIdDD.value;
            levelDataSO.AddLevelSpwanerConfig(levelSpwanerConfig);
            SetView();
        }

        #endregion
    }
}
