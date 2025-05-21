using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class PropellerCellUnlocker : BaseCellUnlocker
    {
        #region PUBLIC_VARS

        [ShowInInspector]
        public static bool IsPropellerAnimationRunning
        {
            get
            {
                Debug.LogError("Already Cell Count :- " + alredyUseCell.Count);
                return alredyUseCell.Count > 0;
            }
        }

        public List<Propeller> Propellers { get => propellers; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GameObject parent;
        [SerializeField] List<Propeller> propellers = new List<Propeller>();
        [SerializeField] internal int removeItemCount;
        protected CountBaseObstacalData obstacalData;
        [ShowInInspector] public static List<BaseCell> alredyUseCell = new List<BaseCell>();

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            alredyUseCell.Clear();
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
            removeItemCount = propellers.Count;
            myCell.GetComponentInChildren<MeshRenderer>(true).gameObject.SetActive(false);
            GetComponentInChildren<MeshRenderer>(true).material = ResourceManager.Instance.GetDefaultSmallLockerMaterial();
            SetLockerData(levelProgressData);
        }

        public void OnRemoveSatck(BaseCell baseCell, int itemId)
        {
            if (!isLocked)
                return;

            if (myCell.AdjacentCells.Contains(baseCell) && removeItemCount > 0)
            {
                BaseCell targetCell = GetBeseCellForPropellerUse();

                if (targetCell != null)
                {
                    Propeller propeller = propellers[removeItemCount - 1];
                    removeItemCount--;
                    if (removeItemCount < 0)
                        removeItemCount = 0;
                    propeller.PlayPropellerAnimation(this, targetCell, (newtarget) =>
                    {
                        if (newtarget != null && newtarget.CanUseBooster())
                        {
                            propeller.gameObject.SetActive(false);
                            newtarget.OnBoosterUse(
                                () =>
                                {
                                    LevelManager.Instance.LoadedLevel.ArrangeStackItem();
                                    RemoveCell(newtarget);
                                    GameplayManager.Instance.SaveAllDataOfLevel();
                                }, true);
                            if (removeItemCount <= 0)
                                Unlock();
                        }
                        else
                        {
                            RemoveCell(newtarget);
                            removeItemCount++;
                            removeItemCount = Mathf.Clamp(removeItemCount, 0, propellers.Count);
                        }
                    });
                }
            }
        }

        public static void RemoveCell(BaseCell baseCell)
        {
            if (alredyUseCell.Contains(baseCell))
            {
                alredyUseCell.Remove(baseCell);
            }
        }

        public static void AddCell(BaseCell baseCell)
        {
            if (!alredyUseCell.Contains(baseCell))
            {
                alredyUseCell.Add(baseCell);
            }
        }

        public override bool IsDependendOnAdjacentCell()
        {
            return removeItemCount > 0;
        }

        public override void Unlock()
        {
            myCell.GetComponentInChildren<MeshRenderer>(true).gameObject.SetActive(true);
            parent.SetActive(removeItemCount > 0);
            base.Unlock();
        }

        public override void SaveData(LevelProgressData levelProgressData)
        {
            var playerData = levelProgressData;
            obstacalData.count = removeItemCount;
            playerData.UpdateObstacalCellData(myCell.CellId, SerializeUtility.SerializeObject(obstacalData));
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetLockerData(LevelProgressData levelProgressData)
        {
            SetObject(false);

            var playerData = levelProgressData;

            string data = "";

            if (playerData != null)
                data = playerData.GetObstacalData(myCell.CellId);

            if (!string.IsNullOrEmpty(data) && !string.IsNullOrWhiteSpace(data))
            {
                obstacalData = SerializeUtility.DeserializeObject<CountBaseObstacalData>(data);
                removeItemCount = obstacalData.count;
            }
            else
            {
                obstacalData = new CountBaseObstacalData();
                obstacalData.count = removeItemCount;
                playerData.UpdateObstacalCellData(myCell.CellId, SerializeUtility.SerializeObject(obstacalData));
            }

            for (int i = 0; i < removeItemCount; i++)
            {
                propellers[i].gameObject.SetActive(true);
            }

            parent.SetActive(removeItemCount > 0);
            if (removeItemCount <= 0)
                Unlock();
        }

        private void SetObject(bool isActive)
        {
            for (int i = 0; i < propellers.Count; i++)
            {
                propellers[i].gameObject.SetActive(isActive);
            }
        }

        public BaseCell GetBeseCellForPropellerUse()
        {
            List<BaseCell> cells = LevelManager.Instance.LoadedLevel.BaseCells;
            BaseCell bestCell = null;

            for (int i = 0; i < cells.Count; i++)
            {
                if (alredyUseCell.Contains(cells[i]) || cells[i] == myCell)
                    continue;

                if (!cells[i].CanUseBooster())
                    continue;

                if (bestCell == null)
                {
                    bestCell = cells[i];
                }
                else
                {
                    bestCell = GetBestCellBasedOnPriority(bestCell, cells[i]);
                }
            }

            if (!alredyUseCell.Contains(bestCell) && bestCell != null)
            {
                Debug.Log("Propeller Add Cell : " + bestCell.name);
                alredyUseCell.Add(bestCell);
            }

            return bestCell;
        }

        private BaseCell GetBestCellBasedOnPriority(BaseCell one, BaseCell two)
        {
            BaseCell bestCell = null;
            if (one != null && two != null)
            {
                if (one.GetPriority() < two.GetPriority())
                {
                    return one;
                }
                else if (one.GetPriority() > two.GetPriority())
                {
                    return two;
                }
                else
                {
                    int oneUniqueItem = one.HasItem ? one.ItemStack.GetUniqueItemCount() : 1000;
                    int twoUniqueItem = two.HasItem ? two.ItemStack.GetUniqueItemCount() : 1000;

                    if (oneUniqueItem > twoUniqueItem)
                    {
                        bestCell = one;
                    }
                    else if (twoUniqueItem > oneUniqueItem)
                    {
                        bestCell = two;
                    }
                    else
                    {
                        int oneTotoalItem = one.HasItem ? one.ItemStack.GetItems().Count : 1000;
                        int twoTotoalItem = two.HasItem ? two.ItemStack.GetItems().Count : 1000;

                        if (oneTotoalItem <= twoTotoalItem)
                        {
                            bestCell = one;
                        }
                        else
                        {
                            bestCell = two;
                        }
                    }

                    return bestCell;
                }
            }
            return bestCell;
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
