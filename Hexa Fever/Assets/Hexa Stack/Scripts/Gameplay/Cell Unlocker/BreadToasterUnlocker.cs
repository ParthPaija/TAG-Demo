using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class BreadToasterUnlocker : BaseCellUnlocker
    {
        #region PUBLIC_VARS
        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Animator animator;
        [SerializeField] private string animationName;
        [SerializeField] private Transform startPoint;
        private Coroutine animationCO;

        private int noOfItemGenrated = 0;
        [ShowInInspector] private BaseLevelGoal levelGoal;

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            if (GameplayManager.Instance != null)
                GameplayManager.Instance.AddListenerOnStackRemove(OnRemoveSatck);
        }

        private void OnDisable()
        {
            if (GameplayManager.Instance != null)
                GameplayManager.Instance.RemoveListenerOnStackRemove(OnRemoveSatck);
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Init(BaseCell baseCell, LevelProgressData levelProgressData)
        {
            base.Init(baseCell, levelProgressData);
            myCell.GetComponentInChildren<MeshRenderer>(true).gameObject.SetActive(false);
            GetComponentInChildren<MeshRenderer>(true).material = ResourceManager.Instance.GetDefaultSmallLockerMaterial();
            List<BaseLevelGoal> baseLevelGoals = GameplayManager.Instance.CurrentLevel.LevelGoals;

            for (int i = 0; i < baseLevelGoals.Count; i++)
            {
                if (baseLevelGoals[i].GetType() == typeof(SpecificObstacleCollectGoal) && baseLevelGoals[i].GetGoalItemId() == id)
                {
                    levelGoal = baseLevelGoals[i];
                    break;
                }
            }
        }

        public override bool IsLocked()
        {
            return isLocked;
        }

        public void OnRemoveSatck(BaseCell baseCell, int itemId)
        {
            if (!isLocked)
                return;

            if (myCell.AdjacentCells.Contains(baseCell))
            {
                if (!levelGoal.IsGoalFullFilled() && LevelEditorManager.Instance == null)
                {
                    noOfItemGenrated++;
                    CollectBread();
                }
                else if (LevelEditorManager.Instance != null)
                {
                    noOfItemGenrated++;
                    CollectBread();
                }
            }
        }

        public void CollectBread()
        {
            if (animationCO == null)
                animationCO = StartCoroutine(DoBreadCollect());
        }

        public override void Unlock()
        {
            myCell.GetComponentInChildren<MeshRenderer>(true).gameObject.SetActive(true);
            base.Unlock();
        }

        public override bool IsDependendOnAdjacentCell()
        {
            if (!levelGoal.IsGoalFullFilled())
                return true;
            return false;
        }

        public override bool CanUseBooster()
        {
            return !levelGoal.IsGoalFullFilled();
        }

        public override void OnBoosterUse(Action action)
        {
            base.OnBoosterUse(action);
            if (CanUseBooster())
            {
                if (!levelGoal.IsGoalFullFilled())
                {
                    noOfItemGenrated++;
                    CollectBread();
                }
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        IEnumerator DoBreadCollect()
        {
            animator.Play(animationName);
            yield return new WaitForSeconds(animator.GetAnimationLength(animationName) - 0.015f);
            MainSceneUIManager.Instance.GetView<VFXView>().PlayBreadItemAnimation(GoalType.Obsatcal, startPoint.position, noOfItemGenrated, id);
            GameplayGoalHandler.Instance.UpdateGoals(GoalType.Obsatcal, id, noOfItemGenrated);
            onBoosterUse?.Invoke();
            onBoosterUse = null;
            animationCO = null;
            noOfItemGenrated = 0;
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
