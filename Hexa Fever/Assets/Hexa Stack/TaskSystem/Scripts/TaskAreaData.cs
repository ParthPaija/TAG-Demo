using Sirenix.OdinInspector;
using System;
using Tag.MetaGame;
using Tag.MetaGame.TaskSystem;
using Tag.RewardSystem;
using UnityEditor;
using UnityEngine;

namespace Tag.TaskSystem
{
    [CreateAssetMenu(fileName = "TaskAreaData", menuName = "Scriptable Objects/TaskSystem/TaskAreaData")]
    public class TaskAreaData : ScriptableObject
    {
        #region PUBLIC_VARS

        public string areaId;
        public BaseTaskData[] baseTaskDatas;
        public AreaRewardData[] areaRewardDatas;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS
        public BaseTaskData GetBaseTaskData(string taskId)
        {
            for (int i = 0; i < baseTaskDatas.Length; i++)
            {
                if (baseTaskDatas[i].TaskId == taskId)
                    return baseTaskDatas[i];
            }
            return null;
        }
        public BaseTaskData[] GetNextBaseTaskData(string taskId)
        {
            for (int i = 0; i < baseTaskDatas.Length; i++)
            {
                if (baseTaskDatas[i].TaskId == taskId)
                    return baseTaskDatas[i].GetNextTaskDatas();
            }
            return null;
        }

        public int GetTotalTaskCount()
        {
            return baseTaskDatas.Length;
        }

        public GiftBoxType GetChestType(int lastRewardClaimedPercentage, int areaCompletePersantage)
        {
            for (int i = 0; i < areaRewardDatas.Length; i++)
            {
                if (areaRewardDatas[i].rewardClaimPercentage <= areaCompletePersantage &&
                    areaRewardDatas[i].rewardClaimPercentage > lastRewardClaimedPercentage)
                {
                    return areaRewardDatas[i].chestType;
                }
            }
            return GiftBoxType.None;
        }

        public int GetTaskCostByTaskId(string taskId)
        {
            for (int i = 0; i < baseTaskDatas.Length; i++)
            {
                if (taskId == baseTaskDatas[i].TaskId)
                {
                    return baseTaskDatas[i].requiredCurrencyData.GetAmount();
                }
            }
            throw new Exception("No TaskId Found");
        }

        #endregion

#if UNITY_EDITOR

        [Button]
        public void SetTaskID()
        {
            for (int i = 0; i < baseTaskDatas.Length; i++)
            {
                ((SimpleTask)baseTaskDatas[i])._taskId = "T" + (i + 1);
                UnityEditor.EditorUtility.SetDirty(baseTaskDatas[i]);
            }
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
