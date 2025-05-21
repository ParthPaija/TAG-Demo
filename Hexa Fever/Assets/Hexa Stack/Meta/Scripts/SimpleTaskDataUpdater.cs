#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Tag.TaskSystem;
using Tag.MetaGame.TaskSystem;
using Tag.HexaStack;

namespace Tag.EditorScripts
{
    public class SimpleTaskDataUpdater : MonoBehaviour
    {
        #region PRIVATE_VARS

        [SerializeField] private TextAsset csvFile;
        [SerializeField] private List<TaskAreaData> taskAreaDataList;
        private List<Dictionary<string, object>> rewardsData;
        private CSVReader csvReader = new CSVReader();

        #endregion

        #region PUBLIC_FUNCTIONS

        [ContextMenu("Update Task Data")]
        public void UpdateTaskData()
        {
            if (!ValidateInput())
                return;

            rewardsData = csvReader.Read(csvFile);
            UpdateAllTaskData(rewardsData);
            Debug.Log("Task data updated successfully.");
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private bool ValidateInput()
        {
            if (csvFile == null)
            {
                Debug.LogError("CSV file is not assigned.");
                return false;
            }

            if (taskAreaDataList == null || taskAreaDataList.Count == 0)
            {
                Debug.LogError("TaskAreaData list is not assigned or empty.");
                return false;
            }

            return true;
        }

        private List<Dictionary<string, string>> ParseCSV(string csvText)
        {
            var taskDataList = new List<Dictionary<string, string>>();
            string[] lines = csvText.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            string[] headers = lines[0].Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');
                var data = new Dictionary<string, string>();

                for (int j = 0; j < headers.Length; j++)
                {
                    data[headers[j].Trim()] = values[j].Trim();
                }

                taskDataList.Add(data);
            }

            return taskDataList;
        }

        private void UpdateAllTaskData(List<Dictionary<string, object>> taskDataList)
        {
            int index = 0;
            foreach (var taskAreaData in taskAreaDataList)
            {
                foreach (var task in taskAreaData.baseTaskDatas)
                {
                    Dictionary<string, object> matchingData = taskDataList[index];
                    if (matchingData != null)
                    {
                        UpdateSimpleTaskData(task as SimpleTask, matchingData);
                    }
                    index++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void UpdateSimpleTaskData(SimpleTask simpleTask, Dictionary<string, object> data)
        {
            simpleTask.requiredCurrencyData = new CurrencyReward
            {
                currencyID = CurrencyConstant.META_CURRENCY,
                curruncyValue = int.Parse(data["Cost Star One"].ToString())
            };
            EditorUtility.SetDirty(simpleTask);
        }

        #endregion

    }
}


#endif