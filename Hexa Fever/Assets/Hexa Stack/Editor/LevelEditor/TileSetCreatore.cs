using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Tag.HexaStack.Editor
{
    [HideLabel, TabGroup("Tile Set Creator"), HideReferenceObjectPicker]
    public class TileSetCreatore
    {
        #region PUBLIC_VARS

        [Header("Grid Specific")]
        public int gridSize;
        public bool isCirculerGrid;
public int tileSetNo = 0;

        #endregion

        #region PRIVATE_VARS

        [HideInInspector] public GridGenerator grid;
        [HideInInspector] public BaseCell selectedCell;

        [Space(20)]
        [Header("Cell Defalut Data")]
        [HideIf("@selectedCell == null")]
        [ItemId] public List<int> defaultCellItem = new List<int>();

        [Space(20)]
        [Header("Cell Locker Data")]
        [HideIf("@selectedCell == null")]
        public BaseCellLockerSelector cellLockerSelector;
        [HideInInspector] public bool isCellRemoveActive = false;
        [HideInInspector] public LayerMask cellLayerMask;
        private GridGenerator gridPrefab;
        [HideInInspector, SerializeField] private List<BaseItem> baseItems = new List<BaseItem>();

        private CellLockerSelcetorDataSO cellLockerSelcetorData;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public TileSetCreatore()
        {
            if (gridPrefab == null)
                gridPrefab = AssetDatabase.LoadAssetAtPath<GridGenerator>("Assets/Hexa Stack/Editor/LevelEditor/Prefab/Grid.prefab");
            cellLayerMask = LayerMask.GetMask("Cell");
            baseItems = new List<BaseItem>();

            string path = "Assets/Hexa Stack/LevelEditor/CellLockerSelcetorData.asset";
            cellLockerSelcetorData = AssetDatabase.LoadAssetAtPath<CellLockerSelcetorDataSO>(path);

            //tileSetNo = cellLockerSelcetorData.gridGenerators.Count;
        }

        public void OnSceneGui()
        {
            GameObject cell = null;
            Event current = Event.current;

            if (current.type == EventType.MouseDown)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;

                if (isCellRemoveActive)
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, cellLayerMask))
                    {
                        cell = hit.collider.gameObject;

                        if (cell != null)
                        {
                            BaseCell baseCell = hit.collider.GetComponent<BaseCell>();
                            if (baseCell != null)
                            {
                                GameObject.DestroyImmediate(baseCell.gameObject);
                                grid.SetCells();
                                return;
                            }
                        }
                    }
                }
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, cellLayerMask))
                {
                    cell = hit.collider.gameObject;

                    if (cell != null)
                    {
                        BaseCell baseCell = hit.collider.GetComponent<BaseCell>();
                        if (baseCell != null)
                        {
                            selectedCell = baseCell;

                            if (selectedCell != null && selectedCell.CellDefaultDataSO != null)
                            {
                                defaultCellItem = new List<int>();
                                for (int i = 0; i < selectedCell.CellDefaultDataSO.itemTypes.Count; i++)
                                {
                                    defaultCellItem.Add(selectedCell.CellDefaultDataSO.itemTypes[i]);
                                }
                            }
                            else
                            {
                                defaultCellItem = new List<int>();
                            }
                            return;
                        }
                    }
                }
            }
        }

        [OnInspectorGUI]
        private void SelecteDeselectButton()
        {
            GUILayout.BeginVertical();//VER

            GUILayout.BeginHorizontal();//
            GUILayout.EndHorizontal();//

            GUILayout.BeginHorizontal();//
            GUILayout.Space(10);

            if (GUILayout.Button("CREATE TILESET"))
                CreateLevel();
            //if (GUILayout.Button("SET CELLS"))
            //{
            //    if (grid != null)
            //        grid.SetCells();
            //}
            //if (GUILayout.Button("SET ADJACENTCELLS"))
            //{
            //    if (grid != null)
            //        grid.SetAdjacentCells();
            //}
            GUILayout.EndHorizontal();//

            GUILayout.BeginHorizontal();//
            GUILayout.Space(10);

            Color oldColor = GUI.color;
            if (isCellRemoveActive)
                GUI.color = Color.red;
            if (GUILayout.Button("REMOVE CELL"))
            {
                isCellRemoveActive = !isCellRemoveActive;
            }
            GUI.color = oldColor;

            if (GUILayout.Button("SET DEFAULT DATA"))
            {
                SetDefalutDataToCell();
            }

            if (GUILayout.Button("SET CELL LOCKER DATA"))
            {
                SetCellLocker();
            }

            GUILayout.EndHorizontal();//

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();//
            if (GUILayout.Button("Save"))
                SaveToPrefab();
            GUILayout.EndHorizontal();//

            GUILayout.EndVertical();//VER
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void CreateLevel()
        {
            if (grid == null)
            {
                grid = GameObject.Instantiate(gridPrefab);
                grid.name = "TileSet-" + tileSetNo;
                grid.transform.position = Vector3.zero;
                CreateGrid();
                ShowDefaultDataInCell();
            }
        }

        private void CreateGrid()
        {
            if (grid != null)
            {
                grid.GenrateGrid(gridSize, isCirculerGrid);
            }
        }

        private void SetCellLocker()
        {
            if (selectedCell != null)
            {
                if (cellLockerSelector != null)
                {
                    BaseCellUnlocker baseCellUnlockerCurrentCell = selectedCell.BaseCellUnlocker;
                    if (baseCellUnlockerCurrentCell != null)
                    {
                        GameObject.DestroyImmediate(baseCellUnlockerCurrentCell);
                        selectedCell.BaseCellUnlocker = null;
                    }

                    cellLockerSelector.SelecteInCell(selectedCell);
                    selectedCell = null;
                    return;
                }
                else
                {
                    BaseCellUnlocker baseCellUnlocker = selectedCell.BaseCellUnlocker;
                    GameObject.DestroyImmediate(baseCellUnlocker.gameObject);
                    selectedCell.BaseCellUnlocker = null;
                    selectedCell = null;
                    return;
                }
            }

            EditorUtility.DisplayDialog("Selected Cell Not Found",
                "First Select Cell ", "OK");
        }

        private void SetDefalutDataToCell()
        {
            if (selectedCell != null)
            {
                if (defaultCellItem.Count > 0)
                {
                    string path = "Assets/Hexa Stack/Asset/TileSet/TileSet " + tileSetNo + "/CellDefaultData-" + selectedCell.name + ".asset";
                    string directory = Path.GetDirectoryName(path);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    CellDefaultDataSO cellDefaultDataSO = ScriptableObject.CreateInstance<CellDefaultDataSO>();

                    cellDefaultDataSO.itemTypes = new List<int>();

                    for (int i = 0; i < defaultCellItem.Count; i++)
                    {
                        cellDefaultDataSO.itemTypes.Add(defaultCellItem[i]);
                    }
                    AssetDatabase.CreateAsset(cellDefaultDataSO, path);
                    selectedCell.CellDefaultDataSO = AssetDatabase.LoadAssetAtPath<CellDefaultDataSO>(path);
                    selectedCell = null;
                    ShowDefaultDataInCell();
                    return;
                }
                else
                {
                    string path = "Assets/Hexa Stack/Asset/TileSet/TileSet " + tileSetNo + "/CellDefaultData-" + selectedCell.name + ".asset";
                    AssetDatabase.DeleteAsset(path);
                    ShowDefaultDataInCell();
                    selectedCell.CellDefaultDataSO = null;
                }
                ShowDefaultDataInCell();
            }
            EditorUtility.DisplayDialog("Selected Cell Not Found",
               "First Select Cell ", "OK");
        }

        private void ShowDefaultDataInCell()
        {
            for (int i = 0; i < baseItems.Count; i++)
            {
                if (baseItems[i] != null)
                    GameObject.DestroyImmediate(baseItems[i].gameObject);
            }

            baseItems.Clear();
            baseItems = new List<BaseItem>();

            for (int i = 0; i < grid.BaseCells.Count; i++)
            {
                if (grid.BaseCells[i].CellDefaultDataSO == null)
                    continue;

                for (int j = 0; j < grid.BaseCells[i].CellDefaultDataSO.itemTypes.Count; j++)
                {
                    BaseItem temp = GameObject.Instantiate(cellLockerSelcetorData.items[grid.BaseCells[i].CellDefaultDataSO.itemTypes[j]], grid.BaseCells[i].transform);
                    temp.transform.localPosition = grid.BaseCells[i].transform.localPosition.With(0,
                        grid.BaseCells[i].transform.localPosition.y + (j * (GamePlayConstant.TWO_ITEM_DISTANCE)),
                        0);
                    baseItems.Add(temp);
                }
            }
        }

        private void SaveToPrefab()
        {
            if (grid != null)
                grid.SetCells();

            EditorApplication.delayCall += () =>
            {
                if (grid != null)
                {
                    grid.SetAdjacentCells();
                }

                for (int i = 0; i < baseItems.Count; i++)
                {
                    GameObject.DestroyImmediate(baseItems[i].gameObject);
                }

                baseItems.Clear();
                baseItems = new List<BaseItem>();

                if (gridPrefab != null)
                {
                    string path = "Assets/Hexa Stack/Asset/TileSet/TileSet " + tileSetNo + "/TileSet " + tileSetNo + ".prefab";
                    string directory = Path.GetDirectoryName(path);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    PrefabUtility.SaveAsPrefabAsset(grid.gameObject, path);
                    GameObject.DestroyImmediate(grid.gameObject);
                    cellLockerSelcetorData.gridGenerators.Add(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                    EditorUtility.SetDirty(cellLockerSelcetorData);
                    AssetDatabase.SaveAssets();
                    grid = null;
                    tileSetNo++;
                }
            };
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
