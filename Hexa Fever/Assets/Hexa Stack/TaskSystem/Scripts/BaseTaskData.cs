using Sirenix.OdinInspector;
using Tag.HexaStack;
using UnityEngine;

namespace Tag.TaskSystem
{
    public abstract class BaseTaskData : SerializedScriptableObject
    {
        #region PUBLIC_VARS
        public abstract string TaskId { get; }

        public string description;

        public BaseReward requiredCurrencyData;

        public BaseReward[] rewardDatas;

        public Sprite sprite;

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private BaseTaskData[] nextUnlockTasks;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public BaseTaskData[] GetNextTaskDatas()
        {
            return nextUnlockTasks;
        }

        #endregion
        [Button]
        public void SetData()
        {
            requiredCurrencyData = new CurrencyReward { currencyID = CurrencyConstant.META_CURRENCY, curruncyValue = 0 };
            rewardDatas = new BaseReward[1];
            rewardDatas[0] = new CurrencyReward { currencyID = CurrencyConstant.COINS, curruncyValue = 0 };
        }
    }

}
