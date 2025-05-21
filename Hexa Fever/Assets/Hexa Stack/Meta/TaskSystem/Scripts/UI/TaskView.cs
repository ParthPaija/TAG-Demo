using I2.Loc;
using System;
using System.Collections;
using System.Linq;
using Tag.TaskSystem;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Tag.HexaStack;

namespace Tag.MetaGame.TaskSystem
{
    public class TaskView : MonoBehaviour
    {
        #region PUBLIC_VARS

        public TaskState taskState;
        public BaseTaskData Task { get; private set; }

        public Button taskButton;

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Localize taskDescriptionText;
        [SerializeField] private Image taskImage;
        [SerializeField] private Image currencyImage;
        [SerializeField] private Text currencyText;

        [SerializeField] private RectTransform _taskOutAnimationObject;
        [SerializeField] private float _outAnimationTime;
        [SerializeField] private CanvasGroup _cg;
        [SerializeField] private AnimationCurve alphaCurve;
        [SerializeField] private RewardViewData[] rewardViewDatas;

        private Vector3 startPos;
        private Vector3 endPos;
        [SerializeField] private AnimationCurve positionCurve;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform MovablePerent;
        [SerializeField] private AnimationCurve scaleAnimationCurve;
        [SerializeField] private AnimationCurve colorAnimationCurve;
        private TaskManager TaskManager { get { return TaskManager.Instance; } }
        private NotEnoughMetaCurrencyPopup NotEnoughMetaCurrencyPopup { get { return MainSceneUIManager.Instance.GetView<NotEnoughMetaCurrencyPopup>(); } }
        private DataManager DataManager { get { return DataManager.Instance; } }
        private TodoTaskPopup TodoTaskPopup { get { return MainSceneUIManager.Instance.GetView<TodoTaskPopup>(); } }
        private SoundHandler SoundHandler { get { return SoundHandler.Instance; } }

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            SetDataForTaskPopupOutAnimation();
            if (taskState == TaskState.COMPLETED)
            {
                TodoTaskPopup.SetIntertactableTaskCompleteButtons(false);
                taskButton.gameObject.SetActive(false);
                PlayTaskPopupOutAnimation();
            }
            else if (taskState == TaskState.IN_QUEUE)
            {
                TaskManager taskManager = TaskManager;
                taskManager.ChangeTaskState(Task.TaskId, TaskState.NOTCOMPLETED);
                taskManager.SaveData();
                PlayTaskPopUpInAnimation();
            }
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init(BaseTaskData baseTaskData, TaskState taskState)
        {
            this.taskState = taskState;
            Task = baseTaskData;
            taskDescriptionText.SetTerm(baseTaskData.description);
            taskImage.sprite = baseTaskData.sprite;
            currencyText.text = baseTaskData.requiredCurrencyData.GetAmount().ToString();
            gameObject.SetActive(true);
            SetRewardData();
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetRewardData()
        {
            for (int i = 0; i < Task.rewardDatas.Length; i++)
            {
                rewardViewDatas[i].SetView(Task.rewardDatas[i]);
            }
        }
        private void GiveReward(BaseReward[] rewardDatas)
        {
            DataManager dataManager = DataManager;
            for (int i = 0; i < rewardDatas.Length; i++)
            {
                rewardDatas[i].GiveReward();
            }
        }

        private void SetDataForTaskPopupOutAnimation()
        {
            startPos = _taskOutAnimationObject.position;
            endPos = startPos;
            endPos.x -= 500;
        }

        private void PlayTaskPopupOutAnimation()
        {
            StartCoroutine(DoTaskPopupOutAnimation());
            //StartCoroutine(WaitAndHideView());
        }

        private void PlayTaskPopUpInAnimation()
        {
            MovablePerent.localScale = Vector3.zero;
            MovablePerent.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        }
        #endregion

        #region CO-ROUTINES

        private IEnumerator DoTaskPopupOutAnimation()
        {
            TodoTaskPopup todoTaskPopup = TodoTaskPopup;
            yield return new WaitForSeconds(0.5f);
            //     todoTaskPopup.uiAnimation.BlockInput(true);
            _animator.gameObject.SetActive(true);
            _animator.Play("Tick_Mark_Animation");
            yield return new WaitForSeconds(0.4f);
            yield return StartCoroutine(TaskScaleAnimationRoutine(this.transform));
            TaskManager.SetNextTask(Task);
            TaskManager.DeleteTaskPlayerData(Task);
            GiveReward(Task.rewardDatas);
            //     todoTaskPopup.uiAnimation.BlockInput(false);
            todoTaskPopup.RemoveTaskView(this);
            TodoTaskPopup.SetIntertactableTaskCompleteButtons(true);
            Destroy(gameObject);
        }


        private IEnumerator TaskScaleAnimationRoutine(Transform tempSlot)
        {

            float i = 0;
            float rate = 1 / .25f;
            Vector3 startPos = MovablePerent.localPosition;
            Vector3 endPos = MovablePerent.localPosition + new Vector3(-1000, 0, 0);
            while (i < 1)
            {
                i += Time.deltaTime * rate;
                MovablePerent.localPosition = Vector3.LerpUnclamped(startPos, endPos, scaleAnimationCurve.Evaluate(i));
                _cg.alpha = 1f - colorAnimationCurve.Evaluate(i);
                yield return null;
            }
            i = 0;
            float rate1 = 1 / .2f;
            Vector3 fromScale = Vector3.one;
            Vector3 toScale = Vector3.zero;
            TodoTaskPopup.ArrangeTaskView();
            while (i < 1)
            {
                i += Time.deltaTime * rate1;
                tempSlot.transform.localScale = Vector3.LerpUnclamped(fromScale, toScale, scaleAnimationCurve.Evaluate(i));
                // LayoutRebuilder.ForceRebuildLayoutImmediate(TodoTaskPopup.ContentRect as RectTransform);
                yield return null;
            }
            TodoTaskPopup.ContentRect.enabled = true;
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       


        public void OnTaskStartClick()
        {
            TodoTaskPopup todoTaskPopup = TodoTaskPopup;
            SoundHandler.PlaySound(SoundType.StarCollectForTask);
            todoTaskPopup.SetIntertactableTaskCompleteButtons(false);
            if (DataManager.GetCurrency(Task.requiredCurrencyData.GetCurrencyId()).Value >= Task.requiredCurrencyData.GetAmount())
            {
                todoTaskPopup.IsTaskAnimationRunning = true;

                MainSceneUIManager.Instance.GetView<TodoTaskPopup>().PlayTokenAnimation(currencyImage.rectTransform.position);
                CoroutineRunner.Instance.Wait(1.4f, () =>
                {
                    //  FxItemFactory.PlayCurrencyReductionAnimation(Task.requiredCurrencyData, LeafView.startTransform.position, currencyImage.transform, delegate ()
                    {
                        todoTaskPopup.IsTaskAnimationRunning = false;
                        todoTaskPopup.Hide();
                        /*todoTaskPopup.RegisterAfterHideFXAction(delegate
                        {
                        });*/
                        TaskManager.StartTask(Task);
                        todoTaskPopup.SetIntertactableTaskCompleteButtons(true);
                    }
                });

                //, 720f);
            }
            else
            {
                todoTaskPopup.SetIntertactableTaskCompleteButtons(true);
                NotEnoughMetaCurrencyPopup.Show();
                //   todoTaskPopup.uiAnimation.BlockInput(false);
                //  NotEnoughLeafPopup.ShowView();
                //  MetaAnalyticsHandler.NotEnoughStarPopup_Open(Task.TaskId);
            }
        }


        #endregion
        [Serializable]
        private class RewardViewData
        {
            [SerializeField] private Image rewardImage;
            [SerializeField] private Text rewardText;
            public void SetView(BaseReward rewardData)
            {
                rewardImage.gameObject.SetActive(true);
                rewardImage.sprite = rewardData.GetRewardImageSprite();
                rewardText.text = "x" + rewardData.GetAmount();
            }
        }
    }
}