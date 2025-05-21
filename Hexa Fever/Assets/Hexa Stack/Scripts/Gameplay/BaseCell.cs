using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Collections.Generic;
using System;

namespace Tag.HexaStack
{
    public class BaseCell : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS

        public bool HasItem { get { return itemStack != null; } }

        public BaseItemStack ItemStack
        {
            get
            {
                return itemStack;
            }
            set
            {
                itemStack = value;
                if (itemStack != null)
                {
                    itemStack.SetParentCell(this);
                }
            }
        }

        public List<BaseCell> AdjacentCells { get => adjacentCells; }
        public CellDefaultDataSO CellDefaultDataSO { get => cellDefaultDataSO; set => cellDefaultDataSO = value; }
        public BaseCellUnlocker BaseCellUnlocker { get => baseCellUnlocker; set => baseCellUnlocker = value; }
        public int CellId { get => cellId; set => cellId = value; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private CellDefaultDataSO cellDefaultDataSO;
        [SerializeField] private BaseCellUnlocker baseCellUnlocker;
        [SerializeField] private List<BaseCell> adjacentCells;
        [SerializeField] private BaseItemStack itemStack;
        [SerializeField] private int cellId;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init()
        {
            GetComponentInChildren<MeshRenderer>().material = ResourceManager.Instance.GetDefaultCellmaterial();
            var levelProgressData = DataManager.LevelProgressData;
            List<int> savedItemData = levelProgressData.GetCellData(CellId);

            if (savedItemData != null)
            {
                SetItemData(savedItemData);
            }
            else
            {
                SetDefaultData();
            }
            if (baseCellUnlocker != null)
                baseCellUnlocker.Init(this, levelProgressData);
        }

        public void SetUndoBoosterData()
        {
            var levelProgressData = GameplayManager.Instance.UndoBoosterData.progressDataUndoBooster;
            List<int> savedItemData = levelProgressData.GetCellData(CellId);

            if (itemStack != null)
            {
                Destroy(itemStack.gameObject);
                itemStack = null;
            }

            if (savedItemData != null)
            {
                SetItemData(savedItemData);
            }
            else
            {
                SetDefaultData();
            }
            if (baseCellUnlocker != null)
                baseCellUnlocker.Init(this, levelProgressData);
        }

        public void ResetItemPosition()
        {
            if (itemStack == null)
            {
                return;
            }
            itemStack.transform.SetParent(transform);
            itemStack.transform.DOMove(transform.position, 0.1f).OnComplete(() =>
            {
                //itemStack.transform.localPosition = transform.position.With(0, 0.02f, 0);
                itemStack.transform.localScale = transform.localScale;
            });
        }

        public bool IsCellLocked()
        {
            if (baseCellUnlocker == null)
                return false;
            if (!baseCellUnlocker.IsBlocker())
                return false;
            return baseCellUnlocker.IsLocked();
        }

        public void OnClick()
        {
            if (baseCellUnlocker != null)
                baseCellUnlocker.OnClick();
        }

        public virtual bool IsCellLockerDependentOnAdjustCell()
        {
            return baseCellUnlocker != null && baseCellUnlocker.IsLocked() && baseCellUnlocker.IsBlocker() && baseCellUnlocker.IsDependendOnAdjacentCell();
        }

        public virtual bool IsCellReadyForUnlockOwnCell()
        {
            return baseCellUnlocker != null && baseCellUnlocker.IsLocked() && !baseCellUnlocker.IsBlocker() && baseCellUnlocker.IsDependendOnOwnCell();
        }

        public bool IsCellUnlockAdjacentAndOwnCellLocker()
        {
            return GetCellUnlockerCount() > 0;
        }

        public int GetCellUnlockerCount()
        {
            int count = 0;
            for (int i = 0; i < adjacentCells.Count; i++)
            {
                if (adjacentCells[i].IsCellLockerDependentOnAdjustCell())
                    count++;
            }
            if (IsCellReadyForUnlockOwnCell())
                count++;
            return count;
        }

        public int GetPriority()
        {
            if (baseCellUnlocker != null && baseCellUnlocker.IsLocked())
                return baseCellUnlocker.GetPriority();
            return 1000;
        }

        public int GetPriorityAdjacentCells()
        {
            int priority = 10000;
            for (int i = 0; i < adjacentCells.Count; i++)
            {
                if (adjacentCells[i].GetPriority() <= priority)
                    priority = adjacentCells[i].GetPriority();
            }
            if (GetPriority() <= priority)
                priority = GetPriority();
            return priority;
        }

        public void SavaData(LevelProgressData levelProgressData)
        {
            if (baseCellUnlocker != null)
                baseCellUnlocker.SaveData(levelProgressData);
            if (HasItem)
            {
                levelProgressData.UpdateGridCellData(cellId, itemStack.GetItemsId());
            }
            else
            {
                levelProgressData.UpdateGridCellData(cellId, new List<int>());
            }
        }

        public void OnBoosterUse(Action onComplate, bool isScoreCountOnItemRemove = false)
        {
            if (IsCellLocked() && baseCellUnlocker.CanUseBooster())
            {
                baseCellUnlocker.OnBoosterUse(onComplate);
                onComplate?.Invoke();
                return;
            }
            GameRuleManager.Instance.RemoveSatck(this, () => { onComplate?.Invoke(); }, isScoreCountOnItemRemove);
        }

        public bool CanUseBooster()
        {
            return (IsCellLocked() && baseCellUnlocker.CanUseBooster()) || (!IsCellLocked() && HasItem && itemStack.GetTopSameTypeItem().Count < 10);
        }

        public void OnItemAddInCell()
        {
            baseCellUnlocker?.OnItemAddInCell();
        }

        public void OnItemRemoveInCell()
        {
            baseCellUnlocker?.OnItemRemoveInCell();
        }

        public void OnRemoveAllItem()
        {
            baseCellUnlocker?.OnRemoveAllItem();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetDefaultData()
        {
            if (cellDefaultDataSO != null)
            {
                SetItemData(cellDefaultDataSO.itemTypes);
            }
        }

        private void SetItemData(List<int> items)
        {
            if (items.Count <= 0)
                return;

            itemStack = Instantiate(ResourceManager.Instance.ItemStack, transform);
            itemStack.transform.localRotation = Quaternion.identity;
            itemStack.transform.localPosition = new Vector3(0, 0.2f, 0);
            List<BaseItem> baseItems = new List<BaseItem>();
            for (int i = 0; i < items.Count; i++)
            {
                BaseItem temp = Instantiate(ResourceManager.Instance.GetItem(items[i]), itemStack.transform);
                baseItems.Add(temp);
            }
            itemStack.SetItem(baseItems);
            itemStack.transform.localPosition = Vector3.zero;
            itemStack.transform.localScale = transform.localScale;
            itemStack.ParentCell = this;
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        #endregion

        public void SetAdjacentCells(float cellRadius = 1)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, cellRadius * 1.5f);
            adjacentCells = new List<BaseCell>();

            foreach (var hitCollider in hitColliders)
            {
                BaseCell cell = hitCollider.GetComponent<BaseCell>();
                if (cell != null && cell != this)
                {
                    adjacentCells.Add(cell);
                }
            }
        }
    }
}
