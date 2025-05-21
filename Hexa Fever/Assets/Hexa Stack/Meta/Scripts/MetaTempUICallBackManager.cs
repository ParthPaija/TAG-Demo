using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Tag.MetaGame;
using Tag.MetaGame.TaskSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.CoreGame.Temp
{
    public class MetaTempUICallBackManager : MonoBehaviour
    {
        #region PUBLIC_VARS
        [SerializeField] private GameObject _selectionView;
        [SerializeField] private Dropdown _areaDropDown;
        #endregion

        #region PRIVATE_VARS
        [SerializeField] private int _totalArea;
        [SerializeField] private PrefsDataTest[] prefsDataTests;

        #endregion

        #region UNITY_CALLBACKS
        public void Start()
        {
            FillAreaData();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

#if UNITY_EDITOR
        public List<Area> areas;
        [Button]
        public void SetAreaDataString()
        {
            Dictionary<string, MetaGame.AreaPlayerData> _allAreaProgressDict = new Dictionary<string, MetaGame.AreaPlayerData>();
            prefsDataTests = new PrefsDataTest[areas.Count];
            _totalArea = areas.Count;
            for (int i = 0; i < areas.Count; i++)
            {
                foreach (var item in _allAreaProgressDict)
                {
                    item.Value.areaState = AreaState.Complete;
                }
                _allAreaProgressDict.Add(areas[i].id, new MetaGame.AreaPlayerData(areas[i].GetData()));
                PrefsDataTest prefsData = new PrefsDataTest();
                MetaGame.TaskSystem.AreaPlayerData taskPlayerData = new MetaGame.TaskSystem.AreaPlayerData();
                taskPlayerData.areaid = areas[i].id;
                taskPlayerData.runningTasks = new List<TaskPlayerData>
                {
                    new TaskPlayerData { taskId = "T1", taskState = TaskState.RUNNING }
                };

                prefsData.areaId = areas[i].id;
                prefsData.areaPrefsString = JsonConvert.SerializeObject(_allAreaProgressDict);
                prefsData.taskPrefsString = JsonConvert.SerializeObject(taskPlayerData);

                prefsDataTests[i] = prefsData;
            }
        }
#endif

        #endregion

        #region PRIVATE_FUNCTIONS

        private void FillAreaData()
        {
            List<string> option = new List<string>();
            for (int i = 0; i < _totalArea; i++)
            {
                option.Add((i + 1).ToString());
            }
            _areaDropDown.AddOptions(option);
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       
        public void OnViewOpenClick()
        {
            _selectionView.SetActive(true);
        }
        public void OnCloseClick()
        {
            _selectionView.SetActive(false);
        }
        public void OnSetButtonClick()
        {
            string areaId = AreaUtility.AreaNoToAreaId(_areaDropDown.value + 1);

            for (int i = 0; i < prefsDataTests.Length; i++)
            {
                if (areaId == prefsDataTests[i].areaId)
                {
                    TaskManager.Instance.SetTaskPlayerData(prefsDataTests[i].taskPrefsString);
                    AreaManager.Instance.SetAreaPlayerData(prefsDataTests[i].areaId, prefsDataTests[i].areaPrefsString);
                }
            }
            //RestartGame();
        }

        [Serializable]
        public class PrefsDataTest
        {
            public string areaId;
            public string taskPrefsString;
            public string areaPrefsString;
        }

        #endregion
    }
}