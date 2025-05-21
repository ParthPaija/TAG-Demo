using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using Tag.HexaStack;
using Tag.RewardSystem;
using Tag.TaskSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.MetaGame.TaskSystem
{
    public class TodoTaskPopup : BaseView
    {
        #region PUBLIC_VARS

        public TaskProgressBar taskProgressBar;
        public bool IsTaskAnimationRunning { set; get; }
        public VerticalLayoutGroup ContentRect { get { return contentTransform; } }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Localize _areaText;
        [SerializeField] private VerticalLayoutGroup contentTransform;
        [SerializeField] private TaskView taskViewPrefab;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private ScrollRect _scrollRect;
        private List<TaskView> _taskViewList = new List<TaskView>();
        private TaskManager TaskManager { get { return TaskManager.Instance; } }
        private AreaSpriteHandler AreaSpriteHandler { get { return AreaSpriteHandler.Instance; } }
        private MainView MainView { get { return MainSceneUIManager.Instance.GetView<MainView>(); } }
        private BottombarView BottombarView { get { return MainSceneUIManager.Instance.GetView<BottombarView>(); } }

        [SerializeField] private CurrencyTopbarComponents tokenTopbar;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void InitView(List<TaskViewData> baseTasks, TaskAreaData taskAreaData)
        {
            _areaText.SetTerm(AreaSpriteHandler.GetAreaName(taskAreaData.areaId));
            taskProgressBar.SetProgressBar(TaskManager.GetCompletedTaskPercentage());
            taskProgressBar.PrepareGifts(taskAreaData.areaRewardDatas);
            for (int i = 0; i < baseTasks.Count; i++)
            {
                SetView(GetEmptyTaskView(), baseTasks[i].baseTaskData, baseTasks[i].taskState);
            }
        }

        public void SetNewTask(BaseTaskData baseTaskData)
        {
            _scrollRect.verticalNormalizedPosition = 1f;
            TaskView taskView = GetEmptyTaskView();
            SetView(taskView, baseTaskData, TaskState.IN_QUEUE);
        }

        public override void Show(Action action = null, bool isForceShow = false)
        {
            _scrollRect.verticalNormalizedPosition = 1f;
            base.Show();
            TaskManager.OnOpenTodoTaskViewOpen();
        }

        public override void Hide()
        {
            if (IsTaskAnimationRunning)
            {
                return;
            }
            base.Hide();
            MainView.Show();
            BottombarView.Show();
        }


        public RectTransform GetGiftboxPosition(GiftBoxType chestType)
        {
            return taskProgressBar.GetGiftboxByType(chestType).rectTransform;
        }

        public void CompleteTask(BaseTaskData completeTaskData)
        {
            for (int i = 0; i < _taskViewList.Count; i++)
            {
                if (_taskViewList[i].Task == completeTaskData)
                {
                    _taskViewList[i].taskState = TaskState.COMPLETED;
                }
            }
        }

        public void SetIntertactableTaskCompleteButtons(bool value)
        {
            for (int i = 0; i < _taskViewList.Count; i++)
            {
                _taskViewList[i].taskButton.interactable = value;
            }
        }

        public void RemoveTaskView(TaskView taskView)
        {
            _taskViewList.Remove(taskView);
        }

        public void PlayTokenAnimation(Vector3 start)
        {
            tokenTopbar.CurrencyAnimation.UIStartAnimation(start, 20, isReverseAnimation: true);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetView(TaskView taskView, BaseTaskData baseTaskData, TaskState taskState)
        {
            taskView.Init(baseTaskData, taskState);
            _taskViewList.Add(taskView);
        }

        private TaskView GetEmptyTaskView()
        {
            return Instantiate(taskViewPrefab, contentTransform.transform);
        }

        public void ArrangeTaskView()
        {
            contentTransform.enabled = false;
            for (int i = 0; i < _taskViewList.Count - 1; i++)
            {
                //disable layout group to prevent layout group from repositioning task view
                StartCoroutine(TaskPositionAnimationRoutine(_taskViewList[i + 1]));
            }
        }

        #endregion

        #region CO-ROUTINES
        private IEnumerator TaskPositionAnimationRoutine(TaskView tempSlot)
        {

            float i = 0;
            float rate = 1 / .2f;
            // RectTransform rectTransform = tempSlot.GetComponent<RectTransform>();
            float height = 240f;
            Vector3 startPos = tempSlot.transform.localPosition;
            Vector3 endPos = tempSlot.transform.localPosition + new Vector3(0, height, 0);

            while (i < 1)
            {
                i += Time.deltaTime * rate;
                tempSlot.transform.localPosition = Vector3.LerpUnclamped(startPos, endPos, i);
                yield return null;
            }

        }
        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       
        public void OnCloseClick()
        {
            Hide();
        }

        #endregion
    }
}
