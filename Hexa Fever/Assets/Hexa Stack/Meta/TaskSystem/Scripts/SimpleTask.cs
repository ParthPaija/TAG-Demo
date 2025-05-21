using Tag.TaskSystem;
using UnityEngine;

namespace Tag.MetaGame.TaskSystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/TaskSystem/Tasks/SimpleTask", fileName = "SimpleTask")]

    public class SimpleTask : BaseTaskData
    {
        #region PUBLIC_VARS
        public override string TaskId
        {
            get { return _taskId; }
        }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] public string _taskId;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion

    }
}
