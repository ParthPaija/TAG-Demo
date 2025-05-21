using Sirenix.OdinInspector;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Tag.HexaStack
{
    public class Level : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS
        public List<BaseCell> BaseCells { get => GridGenerator.BaseCells; }
        public GridGenerator GridGenerator { get => gridGenerator; set => gridGenerator = value; }
        public GridRotator GridRotator { get => gridRotator; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GridGenerator gridGenerator;
        private GridRotator gridRotator;

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            if (gridRotator == null)
                gridRotator = GetComponent<GridRotator>();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init()
        {
            GridGenerator.Init();
            CoroutineRunner.Instance.Wait(0.1f, () => gridRotator.Init(DataManager.LevelProgressData.gridRotation));
        }

        public void SetUndoBoosterData()
        {
            GridGenerator.SetUndoBoosterData();
            CoroutineRunner.Instance.Wait(0.1f, () => gridRotator.Init(GameplayManager.Instance.UndoBoosterData.progressDataUndoBooster.gridRotation));
        }

        public void SaveCellData(LevelProgressData levelProgressData)
        {
            for (int i = 0; i < BaseCells.Count; i++)
            {
                BaseCells[i].SavaData(levelProgressData);
            }
            if (gridRotator != null)
            {
                levelProgressData.gridRotation = gridRotator.CurrentRotation;
            }
        }

        public void ArrangeStackItem()
        {
            //for (int i = 0; i < BaseCells.Count; i++)
            //{
            //    if (BaseCells[i] != null && BaseCells[i].HasItem)
            //    {
            //        BaseCells[i].ItemStack.ArrangeItem(0.2f);
            //    }
            //}
        }

        public bool IsAllCellOcupied()
        {
            for (int i = 0; i < BaseCells.Count; i++)
            {
                if (!BaseCells[i].HasItem && !BaseCells[i].IsCellLocked())
                    return false;
            }
            return true;
        }

        public BaseCell FindEmptyCell()
        {
            for (int i = 0; i < BaseCells.Count; i++)
            {
                if (!BaseCells[i].HasItem && !BaseCells[i].IsCellLocked())
                {
                    return BaseCells[i];
                }
            }
            return null;
        }

        public List<BaseCell> FindEmptyCells()
        {
            List<BaseCell> cells = new List<BaseCell>();

            for (int i = 0; i < BaseCells.Count; i++)
            {
                if (!BaseCells[i].HasItem && !BaseCells[i].IsCellLocked())
                {
                    cells.Add(BaseCells[i]);
                }
            }
            return cells;
        }

        public bool IsAllAdLockerAdWatched()
        {
            int adLockerCount = 0;
            for (int i = 0; i < BaseCells.Count; i++)
            {
                if (BaseCells[i].BaseCellUnlocker != null && BaseCells[i].BaseCellUnlocker as FreeCellUnlocker)
                {
                    adLockerCount++;
                }
            }

            if (adLockerCount <= 0)
            {
                return false;   
            }

            for (int i = 0; i < BaseCells.Count; i++)
            {
                if (BaseCells[i].BaseCellUnlocker != null && BaseCells[i].BaseCellUnlocker as FreeCellUnlocker)
                {
                    if (BaseCells[i].BaseCellUnlocker.IsLocked())
                        return false;
                }
            }
            return true;
        }

        public BaseCell FindCellForPropeller()
        {
            for (int i = 0; i < BaseCells.Count; i++)
            {
                if (BaseCells[i].HasItem)
                {
                    return BaseCells[i];
                }
            }
            return null;
        }

        public bool CanUseHammerBooster()
        {
            for (int i = 0; i < BaseCells.Count; i++)
            {
                if (BaseCells[i].CanUseBooster())
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanUseSwapBooster()
        {
            for (int i = 0; i < BaseCells.Count; i++)
            {
                if (BaseCells[i].HasItem)
                {
                    if (BaseCells[i].BaseCellUnlocker != null)
                    {
                        if (!BaseCells[i].BaseCellUnlocker.IsLocked())
                            return true;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void RemoveStackFromGrid(int count)
        {
            List<BaseCell> removeStackCell = new List<BaseCell>();

            for (int i = 0; i < count; i++)
            {
                BaseCell cell = BaseCells[Random.Range(0, BaseCells.Count)];

                if (!removeStackCell.Contains(cell) && cell.HasItem && !cell.IsCellLocked())
                {
                    removeStackCell.Add(cell);
                }
                else
                    i--;
            }
            for (int i = 0; i < removeStackCell.Count; i++)
            {
                GameRuleManager.Instance.RemoveSatck(removeStackCell[i], () => { GameplayManager.Instance.SaveAllDataOfLevel(); });
            }
        }

        public BaseCell GetCellById(int cellId)
        {
            for (int i = 0; i < BaseCells.Count; i++)
            {
                if (BaseCells[i].CellId == cellId)
                    return BaseCells[i];
            }
            return null;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void GenrateGrid(int gridSize = 1, bool isCirculerGrid = false)
        {
            GridGenerator.GenrateGrid(gridSize, isCirculerGrid);
        }

        public void SetCells()
        {
            GridGenerator.SetCells();
            GridGenerator.SetAdjacentCells();
        }

        [Button]
        public void SetCellId()
        {
            gridGenerator.SetCellId();
        }

#if UNITY_EDITOR

        [Button]
        public void ChangeCellRender(Mesh mesh)
        {

            for (int i = 0; i < gridGenerator.BaseCells.Count; i++)
            {
                MeshFilter meshFilter = gridGenerator.BaseCells[i].GetComponentInChildren<MeshFilter>();
                meshFilter.mesh = mesh;
                meshFilter.transform.localScale = Vector3.one * 1.78f;
                meshFilter.transform.localEulerAngles = new Vector3(-90, 0, 0);
            }

        }
#endif

        #endregion
    }
}
