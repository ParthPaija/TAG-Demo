using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class GoalPointCellUnlocker : BaseCellUnlocker
    {
        #region PUBLIC_VARS
        public BaseLevelGoal LevelGoal { get => levelGoal; set => levelGoal = value; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] protected BaseLevelGoal levelGoal;
        [SerializeField] protected GameObject lockedObj;
        [SerializeField] protected Text goalText;
        protected MeshRenderer meshRenderer;

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            GameplayGoalHandler.Instance.AddListenerOnGoalUpdate(OnGoalChnage);
        }

        private void OnDisable()
        {
            GameplayGoalHandler.Instance.RemoveListenerOnGoalUpdate(OnGoalChnage);
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Init(BaseCell baseCell, LevelProgressData levelProgressData)
        {
            meshRenderer = baseCell.GetComponentInChildren<MeshRenderer>(true);
            base.Init(baseCell, levelProgressData);

            List<BaseLevelGoal> baseLevelGoals = GameplayManager.Instance.CurrentLevel.LevelGoals;
            levelGoal.InitGoal();

            for (int i = 0; i < baseLevelGoals.Count; i++)
            {
                if (baseLevelGoals[i].GetType() == levelGoal.GetType())
                {
                    levelGoal.CurrentCount = baseLevelGoals[i].CurrentCount;
                    break;
                }
            }

            goalText.text = levelGoal.GoalCount.ToString();
            SetObject();
            SetPosition();
            meshRenderer.material = ResourceManager.Instance.GetDefaultLockerMaterial();

            if (levelGoal.IsGoalFullFilled())
            {
                Unlock();
                GameRuleManager.Instance.AddOnRemoveItemCell(MyCell);
            }
        }

        public override bool IsLocked()
        {
            return !levelGoal.IsGoalFullFilled();
        }

        public override void Unlock()
        {
            if (isLocked)
            {
                base.Unlock();
                SoundHandler.Instance.PlaySound(SoundType.TileUnlock);
                meshRenderer.material = ResourceManager.Instance.GetDefaultCellmaterial();
                SetObject();
            }
        }

        public override bool IsDependendOnAdjacentCell()
        {
            return false;
        }

        public virtual void SetText()
        {
            goalText.text = levelGoal.GoalCount.ToString();
        }

        public void SetPosition()
        {
            if (MyCell.HasItem)
            {
                transform.localPosition = transform.position.With(0, MyCell.ItemStack.GetTopPosition().y, 0);
            }
        }

        public void SetTemp(Vector3 pos)
        {
            transform.localPosition = pos;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        internal virtual void OnGoalChnage(GoalType goalType, int itemId, int amount)
        {
            levelGoal.UpdateGoal(goalType, itemId, amount);
            if (levelGoal.IsGoalFullFilled() && isLocked)
            {
                Unlock();
                VFXManager.Instance.PlayUnlockCellAnimation(myCell);
                GameRuleManager.Instance.AddOnRemoveItemCell(MyCell);
            }
        }

        internal void SetObject()
        {
            lockedObj.SetActive(isLocked);
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
