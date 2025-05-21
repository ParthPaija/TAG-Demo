using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Tag.HexaStack
{
    public abstract class BaseCellUnlocker : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS
        public BaseCell MyCell { get => myCell; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField, ObstacalId] protected int id;
        internal bool isLocked = true;
        [SerializeField] protected BaseCell myCell;
        protected Action onBoosterUse;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public virtual void Init(BaseCell baseCell, LevelProgressData levelProgressData)
        {
            myCell = baseCell;
            isLocked = true;
        }

        public virtual void OnClick()
        {
        }

        public virtual void Unlock()
        {
            isLocked = false;
        }

        public virtual bool IsLocked()
        {
            return isLocked;
        }

        public virtual bool IsDependendOnAdjacentCell()
        {
            return isLocked;
        }

        public virtual bool IsDependendOnOwnCell()
        {
            return false;
        }

        public virtual bool CanUseBooster()
        {
            return false;
        }

        public virtual void OnBoosterUse(Action action = null)
        {
            onBoosterUse = action;
        }

        public virtual int GetPriority()
        {
            return ResourceManager.Instance.GetPriority(id);
        }

        public virtual void SaveData(LevelProgressData levelProgressData)
        {
        }

        public virtual bool IsBlocker()
        {
            return true;
        }

        public virtual void OnItemAddInCell()
        {

        }

        public virtual void OnItemRemoveInCell()
        {
        }

        public virtual void OnRemoveAllItem()
        {

        }

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
