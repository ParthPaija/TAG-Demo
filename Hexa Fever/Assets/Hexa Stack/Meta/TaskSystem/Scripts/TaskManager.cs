using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Tag.CoreGame.Animation;
using Tag.HexaStack;
using Tag.RewardSystem;
using Tag.TaskSystem;
using UnityEngine;


namespace Tag.MetaGame.TaskSystem
{
    public class TaskManager : Manager<TaskManager>
    {
        #region PUBLIC_VARS

        public TaskAreaData CurrentTaskAreaData { get; private set; }
        public int LastRewardClaimedPercentage { get { return _gameplayerData.lastRewardClaimedPercentage; } }
        public int TotalTaskCount { get { return CurrentTaskAreaData.GetTotalTaskCount(); } }
        public int CompleteTaskCount { get { return _gameplayerData.completedTaskCount; } }
        #endregion

        #region PRIVATE_VARS
        [SerializeField] private string _defaultData;
        [ShowInInspector] private AreaPlayerData _gameplayerData;
        [SerializeField] private ChestDataSO _chestDataSO;

        private string AREA_PLAYER_DATA = "areaPlayerData";

        private BaseTaskData _currentTaskData;
        private TodoTaskPopup TodoTaskPopup { get { return MainSceneUIManager.Instance.GetView<TodoTaskPopup>(); } }
        private MainView MainView { get { return MainSceneUIManager.Instance.GetView<MainView>(); } }
        private BottombarView BottombarView { get { return MainSceneUIManager.Instance.GetView<BottombarView>(); } }
        private AreaEditMode AreaEditMode { get { return AreaEditMode.Instance; } }
        private AreaManager AreaManager { get { return AreaManager.Instance; } }
        private DataManager DataManager { get { return DataManager.Instance; } }
        private NewAreaComingSoonPopup NewAreaComingSoonPopup { get { return MainSceneUIManager.Instance.GetView<NewAreaComingSoonPopup>(); } }
        private AreaUnlockView AreaUnlockView { get { return MainSceneUIManager.Instance.GetView<AreaUnlockView>(); } }
        private GiftboxAnimationView GiftboxAnimationManager { get { return MainSceneUIManager.Instance.GetView<GiftboxAnimationView>(); } }
        private AreaCompletionPopup AreaCompletionPopup { get { return MainSceneUIManager.Instance.GetView<AreaCompletionPopup>(); } }
        private AreaAssetStateHandler AreaAssetStateHandler { get { return AreaAssetStateHandler.Instance; } }

        public AreaPlayerData GameplayerData { get => _gameplayerData; }

        #endregion

        #region UNITY_CALLBACKS
        public override void Awake()
        {
            base.Awake();
            Init();
        }
        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init()
        {
            LoadData();
        }

        public void InitData(TaskAreaData currentTaskAreaData)
        {
            CurrentTaskAreaData = currentTaskAreaData;
            TodoTaskPopup.InitView(GetTaskViewDatas(), CurrentTaskAreaData);
        }

        public void StartTask(BaseTaskData task)
        {
            _currentTaskData = task;
            MainView.Hide();
            BottombarView.Hide();
            SaveData();
            AreaEditMode.AreaEditModeOn();
            AreaManager.StartTask(task.TaskId);
        }
        public void UpdateCompleteTaskPlayerData()
        {
            _gameplayerData.completedTaskCount++;
            _currentTaskData.requiredCurrencyData.RemoveReward();
            ChangeTaskState(_currentTaskData.TaskId, TaskState.COMPLETED);
            TodoTaskPopup.CompleteTask(_currentTaskData);
            SetNextTaskPlayerData(_currentTaskData);
            SaveData();
        }
        public void UpdateCompleteTaskUI()
        {
            ShowViewDependOnTask();
            _currentTaskData = null;

            void ShowViewDependOnTask()
            {
                MainView.Show();
                BottombarView.Show();
                AreaEditMode.AreaEditModeOff();
                if (IsAllTaskComplete())
                {
                    TodoTaskPopup.Show();
                }
            }
        }

        public void SetNextTask(BaseTaskData baseTaskData)
        {
            BaseTaskData[] nextTaskData = CurrentTaskAreaData.GetNextBaseTaskData(baseTaskData.TaskId);
            if (nextTaskData != null)
            {
                for (int i = 0; i < nextTaskData.Length; i++)
                {
                    TodoTaskPopup.SetNewTask(nextTaskData[i]);
                }
            }
        }

        public void SaveData()
        {
            PlayerPrefs.SetString(AREA_PLAYER_DATA, SerializeUtility.SerializeObject(_gameplayerData));
        }
        public void LoadData()
        {
            _gameplayerData = SerializeUtility.DeserializeObject<AreaPlayerData>(PlayerPrefs.GetString(AREA_PLAYER_DATA, _defaultData));
        }
        public void DeleteTaskPlayerData(BaseTaskData task)
        {
            for (int i = 0; i < _gameplayerData.runningTasks.Count; i++)
            {
                if (task.TaskId == _gameplayerData.runningTasks[i].taskId)
                {
                    _gameplayerData.runningTasks.RemoveAt(i);
                    break;
                }
            }
            TodoTaskPopup.taskProgressBar.SetProgressBar(GetCompletedTaskPercentage());
            TodoTaskPopup.SetIntertactableTaskCompleteButtons(false);
            CheckForChestReward();
            SaveData();
        }

        public void UnlockNewArea(TaskAreaData taskAreaData)
        {
            StartNewArea(taskAreaData);
            MainView.Show();
            BottombarView.Show();
        }

        public bool IsAllTaskComplete()
        {
            return CurrentTaskAreaData.GetTotalTaskCount() <= _gameplayerData.completedTaskCount;
        }

        public int GetCompletedTaskPercentage()
        {
            return _gameplayerData.completedTaskCount * 100 / CurrentTaskAreaData.GetTotalTaskCount();
        }

        public string GetCompletedTaskProgressString()
        {
            return $"{_gameplayerData.completedTaskCount} / {CurrentTaskAreaData.GetTotalTaskCount()}";
        }

        public bool IsAllAreaCompleted()
        {
            if (IsAllTaskComplete() && IsItLastArea(CurrentTaskAreaData.areaId))
            {
                return true;
            }
            return false;
        }
        public bool IsEnoughLeafForTask()
        {
            int leafCount = DataManager.GetCurrency(CurrencyConstant.META_CURRENCY).Value;

            if (leafCount <= 0 || IsAllAreaCompleted())
                return false;

            List<string> notCompletedTaskId = _gameplayerData.GetNotCompletedTaskIdList();
            if (notCompletedTaskId.Count == 0)
            {
                return true;
            }
            int min = CurrentTaskAreaData.GetTaskCostByTaskId(notCompletedTaskId[0]);
            for (int i = 0; i < notCompletedTaskId.Count; i++)
            {
                min = Math.Min(min, CurrentTaskAreaData.GetTaskCostByTaskId(notCompletedTaskId[i]));
            }
            return min <= leafCount;
        }
        public bool CanOpenNewAreaView()
        {
            return IsAllTaskComplete() && _gameplayerData.lastRewardClaimedPercentage == 100;
        }
        public void OnOpenTodoTaskViewOpen()
        {
            if (!_gameplayerData.IsAnyCompletedTask())
            {
                if (IsAllTaskComplete() && _gameplayerData.lastRewardClaimedPercentage == 100)
                {
                    TodoTaskPopup.OnForceHideOnly();
                    if (IsAllAreaCompleted())
                    {
                        NewAreaComingSoonPopup.Show();
                    }
                    else
                    {
                        AreaUnlockView.Show();
                    }
                }
                else
                {
                    TodoTaskPopup.SetIntertactableTaskCompleteButtons(false);
                    CheckForChestReward();
                }
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS
        private bool IsItLastArea(string areaId)
        {
            return AreaAssetStateHandler.TotalAreaNo == AreaUtility.AreaIdToAreaNo(areaId);
        }
        private void SetNextTaskPlayerData(BaseTaskData baseTaskData)
        {
            BaseTaskData[] nextTaskData = CurrentTaskAreaData.GetNextBaseTaskData(baseTaskData.TaskId);
            if (nextTaskData != null)
            {
                for (int i = 0; i < nextTaskData.Length; i++)
                {
                    AddTaskPlayerData(nextTaskData[i].TaskId);
                }
            }
        }
        private void CheckForChestReward()
        {
            ChestData chestData = _chestDataSO.GetChestData(CurrentTaskAreaData.GetChestType
                (_gameplayerData.lastRewardClaimedPercentage, GetCompletedTaskPercentage()));
            if (chestData == null)
            {
                TodoTaskPopup.SetIntertactableTaskCompleteButtons(true);
                return;
            }
            RectTransform startTransformForGiftboxAnimation = TodoTaskPopup.GetGiftboxPosition(chestData.chestType);
            GiftboxAnimationManager.PlayGiftboxAnimation(CurrentTaskAreaData.GetChestType(_gameplayerData.lastRewardClaimedPercentage, GetCompletedTaskPercentage()), startTransformForGiftboxAnimation, chestData.rewards, OnRewardClaim, OnClaimAnimationComplete);

            void OnRewardClaim()
            {
                DataManager dataManager = DataManager;
                for (int i = 0; i < chestData.rewards.Length; i++)
                {
                    chestData.rewards[i].GiveReward();

                    if (chestData.rewards[i].GetCurrencyId() == CurrencyConstant.COINS)
                    {
                        AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, AnalyticsConstants.CoinCurrency
                                , chestData.rewards[i].GetAmount(), AnalyticsConstants.ItemType_Reward, AnalyticsConstants.ItemId_MetaChest);
                    }
                }
                _gameplayerData.lastRewardClaimedPercentage = GetCompletedTaskPercentage();
                SaveData();
                if (IsAllTaskComplete())
                {
                    AreaManager.CompleteCurrentArea();
                }
            }
            void OnClaimAnimationComplete()
            {
                if (IsAllTaskComplete())
                {
                    TodoTaskPopup.Hide();
                    MainView.Hide();
                    BottombarView.Hide();
                    AreaEditMode.AreaEditModeOn();
                    AreaCompletionPopup.Show();
                }
                TodoTaskPopup.taskProgressBar.SetGiftBoxSprite(CurrentTaskAreaData.areaRewardDatas);
                TodoTaskPopup.SetIntertactableTaskCompleteButtons(true);
            }
        }
        public void ChangeTaskState(string taskId, TaskState taskState)
        {
            for (int i = 0; i < _gameplayerData.runningTasks.Count; i++)
            {
                if (_gameplayerData.runningTasks[i].taskId == taskId)
                {
                    _gameplayerData.runningTasks[i].taskState = taskState;
                }
            }
        }
        private void StartNewArea(TaskAreaData taskAreaData)
        {
            _gameplayerData.completedTaskCount = 0;
            _gameplayerData.lastRewardClaimedPercentage = 0;
            SetAreaData(taskAreaData);
            TodoTaskPopup.taskProgressBar.ResetGift();
            TodoTaskPopup.InitView(GetTaskViewDatas(), CurrentTaskAreaData);
        }


        private void SetAreaData(TaskAreaData taskAreaData)
        {
            CurrentTaskAreaData = taskAreaData;
            _gameplayerData.areaid = taskAreaData.areaId;
            _gameplayerData.runningTasks.Clear();
            AddTaskPlayerData(CurrentTaskAreaData.baseTaskDatas[0].TaskId, TaskState.NOTCOMPLETED);
            SaveData();
        }
        private void AddTaskPlayerData(string taskId, TaskState taskState = TaskState.IN_QUEUE)
        {
            _gameplayerData.runningTasks.Add(new TaskPlayerData
            {
                taskId = taskId,
                taskState = taskState
            });
        }

        private List<TaskViewData> GetTaskViewDatas()
        {
            List<TaskViewData> TaskViewDatas = new List<TaskViewData>();
            for (int i = 0; i < _gameplayerData.runningTasks.Count; i++)
            {
                if (_gameplayerData.runningTasks[i].taskState != TaskState.IN_QUEUE)
                {
                    TaskViewDatas.Add(new TaskViewData
                    {
                        baseTaskData = CurrentTaskAreaData.GetBaseTaskData(_gameplayerData.runningTasks[i].taskId),
                        taskState = _gameplayerData.runningTasks[i].taskState
                    });
                }
            }
            return TaskViewDatas;
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion

        public void SetTaskPlayerData(string data)
        {
            PlayerPrefs.SetString(AREA_PLAYER_DATA, data);
        }

#if UNITY_EDITOR
        [ContextMenu("DebugPrefs")]
        public void DebugPrefs()
        {
            Debug.LogError(JsonConvert.SerializeObject(_gameplayerData));
        }
#endif

    }

    [Serializable]
    public class AreaPlayerData
    {
        public string areaid;
        public List<TaskPlayerData> runningTasks;
        public int completedTaskCount;
        public int lastRewardClaimedPercentage;

        public List<string> GetNotCompletedTaskIdList()
        {
            List<string> notCompletedtask = new List<string>();
            for (int i = 0; i < runningTasks.Count; i++)
            {
                if (runningTasks[i].taskState == TaskState.NOTCOMPLETED || runningTasks[i].taskState == TaskState.IN_QUEUE)
                {
                    notCompletedtask.Add(runningTasks[i].taskId);
                }
            }
            return notCompletedtask;
        }
        public bool IsAnyCompletedTask()
        {
            for (int i = 0; i < runningTasks.Count; i++)
            {
                if (runningTasks[i].taskState == TaskState.COMPLETED)
                    return true;
            }
            return false;
        }
    }

    [Serializable]
    public class TaskPlayerData
    {
        public string taskId;
        public TaskState taskState = TaskState.NOTCOMPLETED;
    }

    [Serializable]
    public class TaskViewData
    {
        public BaseTaskData baseTaskData;
        public TaskState taskState;
    }
    public enum TaskState
    {
        NOTCOMPLETED = 0,
        RUNNING = 1,
        COMPLETED = 2,
        IN_QUEUE = 3,
    }
}
