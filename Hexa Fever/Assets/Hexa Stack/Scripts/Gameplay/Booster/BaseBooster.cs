using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Tag.HexaStack
{
    public abstract class BaseBooster : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS
        public bool IsActive { get => isActive; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField, CurrencyId] internal int boosteID;
        internal Action onUse;
        internal bool isActive;

        [Space(10)]
        [SerializeField] private bool isConfirmationActive;
        [ShowIf("isConfirmationActive"),SerializeField] private float cameraTweenDuration = 0.2f;
        [ShowIf("isConfirmationActive"),SerializeField] internal Camera cameraMain;
        [ShowIf("isConfirmationActive"),SerializeField] private Transform cameraDefautTransform;
        [ShowIf("isConfirmationActive"), SerializeField] private Transform cameraTransformForBooster;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public virtual void OnActive(Action onUse)
        {
            this.onUse = onUse;
            isActive = true;
        }

        public virtual void OnUse()
        {
            GameplayManager.Instance.CurrentHandler.OnBoosterUse(boosteID);
            GameplayManager.testPrintData.OnBoosterUse(boosteID);
            onUse.Invoke();
            isActive = false;
            BoosterManager.Instance.DeActvieBooster();
        }

        public virtual void OnUnUse()
        {
            isActive = false;
            BoosterManager.Instance.DeActvieBooster();
        }

        public virtual void ActiveConfirmationView()
        {
            cameraMain.transform.DORotate(cameraTransformForBooster.eulerAngles, cameraTweenDuration);
            cameraMain.transform.DOMove(cameraTransformForBooster.position, cameraTweenDuration);
            HideAllGameplayViews();
            MainSceneUIManager.Instance.GetView<BoosterActiveInfoView>().ShowView(ResourceManager.Instance.BoosterData.GetBoosterData(boosteID), OnUnUse);
            ItemStackSpawnerManager.Instance.StakOutAnimation();
        }

        public virtual void DeActiveConfirmationView()
        {
            MainSceneUIManager.Instance.GetView<BoosterActiveInfoView>().Hide();
            ShowAllGameplayViews();
            cameraMain.transform.DORotate(cameraDefautTransform.eulerAngles, cameraTweenDuration);
            cameraMain.transform.DOMove(cameraDefautTransform.position, cameraTweenDuration);
            ItemStackSpawnerManager.Instance.StakInAnimation();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void HideAllGameplayViews()
        {
            MainSceneUIManager.Instance.GetView<GameplayTopbarView>().HideAnimation();
            MainSceneUIManager.Instance.GetView<GameplayGoalView>().HideAnimation();
            MainSceneUIManager.Instance.GetView<GameplayBottomView>().HideAnimation();
        }

        private void ShowAllGameplayViews()
        {
            MainSceneUIManager.Instance.GetView<GameplayTopbarView>().ShowAnimation();
            MainSceneUIManager.Instance.GetView<GameplayGoalView>().ShowAnimation();
            MainSceneUIManager.Instance.GetView<GameplayBottomView>().ShowAnimation();
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
