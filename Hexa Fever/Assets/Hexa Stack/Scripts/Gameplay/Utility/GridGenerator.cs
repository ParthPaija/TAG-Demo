using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;

namespace Tag.HexaStack
{
    public class GridGenerator : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS
        public List<BaseCell> BaseCells { get => baseCells; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Grid grid;
        [SerializeField] private List<BaseCell> baseCells = new List<BaseCell>();
        [SerializeField] private BaseCell cellPrefab;
        [SerializeField] private bool isCirculerGrid;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init()
        {
            for (int i = 0; i < baseCells.Count; i++)
            {
                baseCells[i].Init();
            }
        }

        public void SetUndoBoosterData()
        {
            for (int i = 0; i < baseCells.Count; i++)
            {
                baseCells[i].SetUndoBoosterData();
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS  

        [Button(SdfIconType.Grid, IconAlignment.LeftOfText)]
        public void GenrateGrid(int gridSize = 1, bool isCirculerGrid = false)
        {
            baseCells = new List<BaseCell>();
            transform.Clear();

            int centerOffset = gridSize / 2;

            int count = 0;

            for (int i = -centerOffset; i <= centerOffset; i++)
            {
                for (int j = -centerOffset; j <= centerOffset; j++)
                {
                    Vector3 spawnPos = grid.CellToWorld(new Vector3Int(i, j, 0));

                    if (isCirculerGrid && spawnPos.magnitude > grid.CellToWorld(new Vector3Int(centerOffset, 0, 0)).magnitude)
                        continue;

                    BaseCell temp = Instantiate(cellPrefab, transform);
                    temp.transform.position = spawnPos;
                    temp.name = "Cell-" + count;
                    temp.CellId = count;
                    baseCells.Add(temp);
                    count++;
                }
            }

            SetCells();
            SetAdjacentCells();
        }

        [Button(SdfIconType.Google, IconAlignment.LeftOfText)]
        public void SetCells()
        {
            baseCells = new List<BaseCell>();
            baseCells.AddRange(GetComponentsInChildren<BaseCell>());
        }

        [Button(SdfIconType.Hammer, IconAlignment.LeftOfText)]
        public void SetAdjacentCells()
        {
            for (int i = 0; i < baseCells.Count; i++)
            {
                baseCells[i].SetAdjacentCells();
            }
        }

        [Button]
        public void SetCellId()
        {
            for (int i = 0; i < baseCells.Count; i++)
            {
                baseCells[i].CellId = i;
            }
        }

        #endregion
    }
}
