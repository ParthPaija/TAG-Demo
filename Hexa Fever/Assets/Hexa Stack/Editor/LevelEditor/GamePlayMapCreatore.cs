using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    [HideLabel, TabGroup("Level Creator"), HideReferenceObjectPicker]
    public class GamePlayMapCreatore
    {
        #region PUBLIC_VARS

        [TabGroup("Basic", "Level Info")]
        [Header("Level Settings")]
        [LabelText("Level Number")]
        public int levelNo;

        [TabGroup("Basic", "Level Info")]
        [Header("Level Settings")]
        [LabelText("Level Testing Type")]
        [EnumPaging] public LevelTestingType levelTestingType = LevelTestingType.TestLevel3;

        [TabGroup("Basic", "Grid Info")]
        [Header("Grid Settings")]
        [LabelText("Grid Size")]
        public int gridSize;

        [TabGroup("Basic", "Grid Info")]
        public bool isCirculerGrid;

        [TabGroup("Basic", "Level Info")]
        [Space(20)]
        public LevelType levelType;

        [TabGroup("Level", "Goals")]
        [HideIf("@levelData == null")]
        public List<BaseLevelGoal> levelGoals = new List<BaseLevelGoal>();

        [TabGroup("Level", "Spawner")]
        [HideIf("@levelData == null")]
        public List<LevelSpwanerConfig> levelSpwanerConfigs = new List<LevelSpwanerConfig>();

        [TabGroup("Level", "Spawner")]
        [HideIf("@levelData == null")]
        public Dictionary<int, BaseItemSpawnerConfig> baseItemSpawnerConfigs = new Dictionary<int, BaseItemSpawnerConfig>();

        [TabGroup("Level", "Spawner")]
        [HideIf("@levelData == null")]
        public BaseItemSpawnerConfig defaltConfig = new RandomItemSpawnerConfigNew();

        [HideInInspector] public BaseCell selectedCell;

        [TabGroup("Cell", "Items")]
        [Header("Cell Default Data")]
        [HideIf("@selectedCell == null")]
        [BoxGroup("Cell/Items/Default Items", ShowLabel = false)]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<DefaultItemData> defaultCellItems = new List<DefaultItemData>();

        [TabGroup("Cell", "Items")]
        [BoxGroup("Cell/Items/Actions")]
        [ButtonGroup("Cell/Items/Actions/Buttons")]
        [Button("Set Default Data", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
        [HideIf("@selectedCell == null")]
        private void SetDefaultDataButton()
        {
            SetDefalutDataToCell();
        }

        [ButtonGroup("Cell/Items/Actions/Buttons")]
        [Button("Clear Items", ButtonSizes.Large), GUIColor(1f, 0.6f, 0.4f)]
        [HideIf("@selectedCell == null")]
        private void ClearItemsButton()
        {
            defaultCellItems.Clear();
            SetDefalutDataToCell();
        }

        [TabGroup("Cell", "Locker")]
        [Header("Cell Locker Data")]
        [HideIf("@selectedCell == null")]
        [BoxGroup("Cell/Locker/Settings", ShowLabel = false)]
        [ValueDropdown("GetLockerTypes")]
        public BaseCellLockerSelector cellLockerSelector;

        [TabGroup("Cell", "Locker")]
        [BoxGroup("Cell/Locker/Actions")]
        [ButtonGroup("Cell/Locker/Actions/Buttons")]
        [Button("Set Locker", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
        [HideIf("@selectedCell == null")]
        private void SetLockerButton()
        {
            SetCellLocker();
        }

        [ButtonGroup("Cell/Locker/Actions/Buttons")]
        [Button("Remove Locker", ButtonSizes.Large), GUIColor(1f, 0.6f, 0.4f)]
        [HideIf("@selectedCell == null")]
        private void RemoveLockerButton()
        {
            cellLockerSelector = null;
            SetCellLocker();
        }

        // Hidden variables
        [HideInInspector] public Level gamePlayLevel;
        [HideInInspector] public LevelDataSO levelData;
        [HideInInspector] public LayerMask cellLayerMask;
        [HideInInspector] public bool isCellRemoveActive = false;
        private Level gamePlayLevelPrefab;
        [HideInInspector, SerializeField] private List<BaseItem> baseItems = new List<BaseItem>();
        [HideInInspector, SerializeField] private List<IceTile> iceTiles = new List<IceTile>();
        private CellLockerSelcetorDataSO cellLockerSelcetorData;
        [HideInInspector] public BaseCell copiedCell;
        private int tempLevelNo;
        private LevelDataSO tempLevelData;

        #endregion

        #region UNITY_CALLBACKS

        public GamePlayMapCreatore()
        {
            GetLevelDemoPrefab();
            cellLayerMask = LayerMask.GetMask("Cell");
            baseItems = new List<BaseItem>();
            iceTiles = new List<IceTile>();

            string path = "Assets/Hexa Stack/LevelEditor/CellLockerSelcetorData.asset";
            cellLockerSelcetorData = AssetDatabase.LoadAssetAtPath<CellLockerSelcetorDataSO>(path);
        }

        #endregion

        #region GUI_LAYOUT

        [OnInspectorGUI]
        private void SelecteDeselectButton()
        {
            // Basic Tab
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Basic Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("CREATE LEVEL", GUILayout.Height(30)))
                CreateLevel();
            if (GUILayout.Button("CREATE GRID", GUILayout.Height(30)))
                CreateGrid();
            if (GUILayout.Button("SELECT TILESET", GUILayout.Height(30)))
                PrefabPreviewWindow.ShowWindow();
            EditorGUILayout.EndHorizontal();

            // Only show Remove Cell button
            EditorGUILayout.BeginHorizontal();
            Color oldColor = GUI.color;
            if (isCellRemoveActive)
                GUI.color = Color.red;
            if (GUILayout.Button("REMOVE CELL", GUILayout.Height(30)))
            {
                isCellRemoveActive = !isCellRemoveActive;
            }
            GUI.color = oldColor;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            // Cell Tab
            if (selectedCell != null)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Cell Settings", EditorStyles.boldLabel);

                // Copy Paste buttons
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("COPY CELL DATA", GUILayout.Height(30)))
                {
                    copiedCell = selectedCell;
                }

                GUI.enabled = copiedCell != null;
                if (GUILayout.Button("PASTE CELL DATA", GUILayout.Height(30)))
                {
                    PasteCellData();
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            // Level Tab
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("SET LEVEL DEFAULT DATA", GUILayout.Height(30)))
                SetLevelDefaultData();
            if (GUILayout.Button("SAVE LEVEL", GUILayout.Height(30)))
                SaveToPrefab();
            if (GUILayout.Button("TEST LEVEL", GUILayout.Height(30)))
                SaveAndTest();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void OnSceneGui()
        {
            GameObject cell = null;
            Event current = Event.current;

            // Add keyboard shortcut handling
            if (current.type == EventType.KeyDown && current.shift)
            {
                if (current.keyCode == KeyCode.C && selectedCell != null)
                {
                    // Copy cell data
                    copiedCell = selectedCell;
                    Debug.Log("Cell data copied");
                    current.Use(); // Prevent the event from being processed further
                    return;
                }
                else if (current.keyCode == KeyCode.V && selectedCell != null && copiedCell != null)
                {
                    // Paste cell data
                    PasteCellData();
                    Debug.Log("Cell data pasted");
                    current.Use(); // Prevent the event from being processed further
                    return;
                }
            }

            if (current.type == EventType.MouseDown)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;

                if (!Physics.Raycast(ray, out hit, Mathf.Infinity, cellLayerMask))
                {
                    selectedCell = null;
                    return;
                }

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
                                gamePlayLevel.SetCells();
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
                                defaultCellItems = new List<DefaultItemData>();
                                int currentItemId = selectedCell.CellDefaultDataSO.itemTypes[0];
                                int currentCount = 1;

                                for (int i = 1; i < selectedCell.CellDefaultDataSO.itemTypes.Count; i++)
                                {
                                    if (selectedCell.CellDefaultDataSO.itemTypes[i] == currentItemId)
                                    {
                                        currentCount++;
                                    }
                                    else
                                    {
                                        defaultCellItems.Add(new DefaultItemData
                                        {
                                            itemId = currentItemId,
                                            count = currentCount
                                        });

                                        currentItemId = selectedCell.CellDefaultDataSO.itemTypes[i];
                                        currentCount = 1;
                                    }
                                }

                                defaultCellItems.Add(new DefaultItemData
                                {
                                    itemId = currentItemId,
                                    count = currentCount
                                });
                            }
                            else
                            {
                                defaultCellItems = new List<DefaultItemData>();
                            }

                            if (selectedCell.BaseCellUnlocker != null)
                            {
                                if (selectedCell.BaseCellUnlocker is FreeCellUnlocker)
                                {
                                    cellLockerSelector = new AdCellLockerSelector();
                                }
                                else if (selectedCell.BaseCellUnlocker is GrassCellUnlocker)
                                {
                                    cellLockerSelector = new GrassCellLockerSelector();
                                }
                                else if (selectedCell.BaseCellUnlocker is GoalPointCellUnlocker goalPointUnlocker)
                                {
                                    var pointSelector = new PointCellLockerSelector();
                                    pointSelector.baseLevelGoal = goalPointUnlocker.LevelGoal;
                                    cellLockerSelector = pointSelector;
                                }
                                else if (selectedCell.BaseCellUnlocker is ItemGoalPointCellUnlocker itemGoalUnlocker)
                                {
                                    var specificPointSelector = new SpecificItemPointCellLockerSelector();
                                    specificPointSelector.baseLevelGoal = itemGoalUnlocker.LevelGoal;
                                    cellLockerSelector = specificPointSelector;
                                }
                                else if (selectedCell.BaseCellUnlocker is BreadToasterUnlocker)
                                {
                                    cellLockerSelector = new BreadToasterLockerSelector();
                                }
                                else if (selectedCell.BaseCellUnlocker is IceCellUnlocker)
                                {
                                    cellLockerSelector = new IceLockerSelector();
                                }
                                else if (selectedCell.BaseCellUnlocker is PropellerCellUnlocker propellerUnlocker)
                                {
                                    var propellerSelector = new PropellerLockerSelector();
                                    propellerSelector.propellerCount = propellerUnlocker.name.EndsWith("1") ? 1 :
                                                                     propellerUnlocker.name.EndsWith("2") ? 2 : 3;
                                    cellLockerSelector = propellerSelector;
                                }
                                else if (selectedCell.BaseCellUnlocker is AdjacentCellStackCellUnlocker woodUnlocker)
                                {
                                    var woodSelector = new WoodCellLockerSelector();
                                    woodSelector.woodCount = woodUnlocker.name.EndsWith("1") ? 1 :
                                                           woodUnlocker.name.EndsWith("2") ? 2 : 3;
                                    cellLockerSelector = woodSelector;
                                }
                                else
                                {
                                    cellLockerSelector = null;
                                }
                            }
                            else
                            {
                                cellLockerSelector = null;
                            }
                            return;
                        }
                    }
                }
            }
        }

        public void PlaceNewTileSet(GameObject gameObject)
        {
            if (gamePlayLevel != null)
            {
                if (gamePlayLevel.GridGenerator != null)
                    GameObject.DestroyImmediate(gamePlayLevel.GridGenerator.gameObject);
                GridGenerator temp = GameObject.Instantiate(gameObject.GetComponent<GridGenerator>(), gamePlayLevel.transform);
                gamePlayLevel.GridGenerator = temp;
                temp.transform.position = Vector3.zero;
                ShowDefaultDataInCell();
            }
        }

        public void SetLevelDefaultData()
        {
            if (levelType == LevelType.Bonus)
            {
                levelData.AddGoal(new CoinItemCollectGoal() { GoalCount = 150, itemId = 9 });
            }
            else
            {
                levelData.AddGoal(new AllItemCollectGoal() { GoalCount = 250 });
            }
            levelData.DefaultDataLevelSpwanerConfigs(levelType);
            levelData.DefaultSpwanerConfig(levelType);

            levelSpwanerConfigs = levelData.LevelSpwanerConfigs;
            defaltConfig = levelData.DefaultConfig;
            levelGoals = levelData.LevelGoals;
            levelData.LevelType = levelType;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private string GetLevelPath()
        {
            return $"Assets/Hexa Stack/Asset/Resources/{levelTestingType}/Level ";
        }

        private void CreateLevel()
        {
            GetLevelDemoPrefab();
            if (gamePlayLevel == null)
            {
                if (levelData == null)
                {
                    string path = GetLevelPath() + levelNo + "/LevelData " + levelNo + ".asset";
                    string directory = Path.GetDirectoryName(path);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    LevelDataSO levelDataTemp = AssetDatabase.LoadAssetAtPath<LevelDataSO>(path);

                    if (levelDataTemp != null)
                    {
                        levelData = levelDataTemp;
                        levelData.Level = levelNo;

                        if (levelDataTemp.LevelPrefab != null)
                        {
                            gamePlayLevel = GameObject.Instantiate(levelDataTemp.LevelPrefab);
                            gamePlayLevel.name = "Level-" + levelNo;
                            gamePlayLevel.transform.position = Vector3.zero;
                            ShowDefaultDataInCell();
                        }
                        else
                        {
                            gamePlayLevel = GameObject.Instantiate(gamePlayLevelPrefab);
                            gamePlayLevel.name = "Level-" + levelNo;
                            gamePlayLevel.transform.position = Vector3.zero;

                            levelData.LevelPrefab = gamePlayLevel;
                        }
                    }
                    else
                    {
                        levelDataTemp = ScriptableObject.CreateInstance<LevelDataSO>();
                        levelData = levelDataTemp;
                        levelDataTemp.name = $"LevelData {levelNo}";
                        AssetDatabase.CreateAsset(levelDataTemp, path);

                        gamePlayLevel = GameObject.Instantiate(gamePlayLevelPrefab);
                        gamePlayLevel.name = "Level-" + levelNo;
                        gamePlayLevel.transform.position = Vector3.zero;

                        levelData.LevelPrefab = gamePlayLevel;
                    }
                }

                levelGoals = levelData.LevelGoals;
                levelSpwanerConfigs = levelData.LevelSpwanerConfigs;
                baseItemSpawnerConfigs = levelData.BaseItemSpawnerConfigs;
                defaltConfig = levelData.DefaultConfig;
                levelType = levelData.LevelType;
            }
        }

        private void CreateGrid()
        {
            if (gamePlayLevel == null)
            {
                EditorUtility.DisplayDialog("Error", "First Create Level", "OK");
                return;
            }

            // Create default tile set if none exists
            if (gamePlayLevel.GridGenerator == null)
            {
                string defaultTileSetPath = "Assets/Hexa Stack/Editor/LevelEditor/Prefab/TileSet.prefab";
                GameObject defaultTileSet = AssetDatabase.LoadAssetAtPath<GameObject>(defaultTileSetPath);

                if (defaultTileSet != null)
                {
                    GridGenerator temp = GameObject.Instantiate(defaultTileSet.GetComponent<GridGenerator>(), gamePlayLevel.transform);
                    gamePlayLevel.GridGenerator = temp;
                    temp.transform.position = Vector3.zero;
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Default TileSet not found. Please select a Tile Set first", "OK");
                    return;
                }
            }

            gamePlayLevel.GenrateGrid(gridSize, isCirculerGrid);
        }

        private void SetDefalutDataToCell()
        {
            if (selectedCell != null)
            {
                if (defaultCellItems.Count > 0)
                {
                    string path = GetLevelPath() + levelNo + "/CellDefaultData-" + selectedCell.name + ".asset";
                    string directory = Path.GetDirectoryName(path);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    CellDefaultDataSO cellDefaultDataSO = ScriptableObject.CreateInstance<CellDefaultDataSO>();

                    cellDefaultDataSO.itemTypes = new List<int>();

                    foreach (var itemData in defaultCellItems)
                    {
                        for (int i = 0; i < itemData.count; i++)
                        {
                            cellDefaultDataSO.itemTypes.Add(itemData.itemId);
                        }
                    }

                    AssetDatabase.CreateAsset(cellDefaultDataSO, path);
                    selectedCell.CellDefaultDataSO = AssetDatabase.LoadAssetAtPath<CellDefaultDataSO>(path);
                    selectedCell = null;
                    ShowDefaultDataInCell();
                    return;
                }
                else
                {
                    string path = GetLevelPath() + levelNo + "/CellDefaultData-" + selectedCell.name + ".asset";
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

            for (int i = 0; i < iceTiles.Count; i++)
            {
                if (iceTiles[i] != null)
                    GameObject.DestroyImmediate(iceTiles[i].gameObject);
            }

            baseItems.Clear();
            iceTiles.Clear();

            baseItems = new List<BaseItem>();
            iceTiles = new List<IceTile>();

            for (int i = 0; i < gamePlayLevel.BaseCells.Count; i++)
            {
                BaseCell baseCell = gamePlayLevel.BaseCells[i];
                if (baseCell.CellDefaultDataSO == null)
                    continue;

                Vector3 lastPos = Vector3.zero;

                for (int j = 0; j < baseCell.CellDefaultDataSO.itemTypes.Count; j++)
                {
                    BaseItem temp = GameObject.Instantiate(cellLockerSelcetorData.items[baseCell.CellDefaultDataSO.itemTypes[j]], baseCell.transform);
                    lastPos = gamePlayLevel.BaseCells[i].transform.localPosition.With(0,
                        baseCell.transform.localPosition.y + GamePlayConstant.TWO_ITEM_DISTANCE + (j * (GamePlayConstant.TWO_ITEM_DISTANCE)), 0);
                    temp.transform.localPosition = lastPos;
                    baseItems.Add(temp);
                }

                if (baseCell.BaseCellUnlocker != null && baseCell.BaseCellUnlocker.GetType() == typeof(GoalPointCellUnlocker))
                {
                    GoalPointCellUnlocker goalPointCellUnlocker = (GoalPointCellUnlocker)baseCell.BaseCellUnlocker;
                    goalPointCellUnlocker.SetTemp(lastPos.With(null, lastPos.y + GamePlayConstant.TWO_ITEM_DISTANCE, null));
                }

                if (baseCell.BaseCellUnlocker != null && baseCell.BaseCellUnlocker.GetType() == typeof(GrassCellUnlocker))
                {
                    GrassCellUnlocker grassCellUnlocker = (GrassCellUnlocker)baseCell.BaseCellUnlocker;
                    grassCellUnlocker.SetPosition(lastPos.With(null, 0, lastPos.y + GamePlayConstant.TWO_ITEM_DISTANCE));
                }

                if (baseCell.BaseCellUnlocker != null && baseCell.BaseCellUnlocker.GetType() == typeof(ItemGoalPointCellUnlocker))
                {
                    ItemGoalPointCellUnlocker goalPointCellUnlocker = (ItemGoalPointCellUnlocker)baseCell.BaseCellUnlocker;
                    goalPointCellUnlocker.SetTemp(lastPos.With(null, lastPos.y + GamePlayConstant.TWO_ITEM_DISTANCE, null));
                    goalPointCellUnlocker.SetLoakImage();
                }

                lastPos = Vector3.zero + new Vector3(0.15f, 0.2f, 0);

                if (baseCell.BaseCellUnlocker != null && baseCell.BaseCellUnlocker.GetType() == typeof(IceCellUnlocker))
                {
                    string path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Ice Tile.prefab";

                    for (int j = 0; j < baseCell.CellDefaultDataSO.itemTypes.Count; j++)
                    {
                        IceTile temp = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<IceTile>(path), baseCell.transform);
                        lastPos = gamePlayLevel.BaseCells[i].transform.localPosition.With(0,
                            baseCell.transform.localPosition.y + GamePlayConstant.TWO_ITEM_DISTANCE + (j * (GamePlayConstant.TWO_ITEM_DISTANCE)), 0);
                        temp.transform.localPosition = lastPos;
                        temp.transform.localEulerAngles = Vector3.zero;
                        iceTiles.Add(temp);
                    }
                }
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
                        GameObject.DestroyImmediate(baseCellUnlockerCurrentCell.gameObject);
                        selectedCell.BaseCellUnlocker = null;
                    }

                    cellLockerSelector.SelecteInCell(selectedCell);
                    selectedCell = null;
                    ShowDefaultDataInCell();
                    return;
                }
                else
                {
                    BaseCellUnlocker baseCellUnlocker = selectedCell.BaseCellUnlocker;
                    GameObject.DestroyImmediate(baseCellUnlocker.gameObject);
                    selectedCell.BaseCellUnlocker = null;
                    selectedCell = null;
                    ShowDefaultDataInCell();
                    return;
                }
            }
            EditorUtility.DisplayDialog("Selected Cell Not Found",
                "First Select Cell ", "OK");
        }

        private void GetLevelDemoPrefab()
        {
            gamePlayLevelPrefab = AssetDatabase.LoadAssetAtPath<Level>("Assets/Hexa Stack/Editor/LevelEditor/Prefab/Level Demo.prefab");
        }

        private void SaveToPrefab()
        {
            if (gamePlayLevel.GridGenerator != null)
                gamePlayLevel.GridGenerator.SetCells();

            EditorApplication.delayCall += () =>
            {
                if (gamePlayLevel.GridGenerator != null)
                {
                    gamePlayLevel.GridGenerator.SetAdjacentCells();
                }

                for (int i = 0; i < baseItems.Count; i++)
                {
                    GameObject.DestroyImmediate(baseItems[i].gameObject);
                }

                for (int i = 0; i < iceTiles.Count; i++)
                {
                    GameObject.DestroyImmediate(iceTiles[i].gameObject);
                }

                baseItems.Clear();
                baseItems = new List<BaseItem>();

                iceTiles.Clear();
                iceTiles = new List<IceTile>();

                if (gamePlayLevel != null)
                {
                    string path = GetLevelPath() + levelNo + "/Level " + levelNo + ".prefab";
                    string directory = Path.GetDirectoryName(path);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    PrefabUtility.SaveAsPrefabAsset(gamePlayLevel.gameObject, path);
                    GameObject.DestroyImmediate(gamePlayLevel.gameObject);
                    gamePlayLevel = null;
                    levelData.LevelPrefab = AssetDatabase.LoadAssetAtPath<Level>(path);
                    levelData.Level = levelNo;
                    EditorUtility.SetDirty(levelData);

                    string pathReviveData = "Assets/Hexa Stack/Data/ReviveDataConfig.asset";

                    AssetDatabase.LoadAssetAtPath<ReviveDataSO>(pathReviveData).lastEditLevel = levelNo;
                    AssetDatabase.LoadAssetAtPath<ReviveDataSO>(pathReviveData).levelTestingType = levelTestingType;
                    levelData = null;
                    levelNo++;
                }
            };
        }

        public void SaveAndTest()
        {
            // Store current level info
            tempLevelNo = levelNo;
            tempLevelData = levelData;

            if (gamePlayLevel.GridGenerator != null)
                gamePlayLevel.GridGenerator.SetCells();

            // Save current level synchronously
            if (gamePlayLevel != null)
            {
                if (gamePlayLevel.GridGenerator != null)
                {
                    gamePlayLevel.GridGenerator.SetAdjacentCells();
                }

                // Clear existing items
                foreach (var item in baseItems)
                {
                    if (item != null)
                        GameObject.DestroyImmediate(item.gameObject);
                }
                foreach (var tile in iceTiles)
                {
                    if (tile != null)
                        GameObject.DestroyImmediate(tile.gameObject);
                }
                baseItems.Clear();
                iceTiles.Clear();

                // Save the level
                string path = GetLevelPath() + levelNo + "/Level " + levelNo + ".prefab";
                string directory = Path.GetDirectoryName(path);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                PrefabUtility.SaveAsPrefabAsset(gamePlayLevel.gameObject, path);
                GameObject.DestroyImmediate(gamePlayLevel.gameObject);
                gamePlayLevel = null;
                levelData.LevelPrefab = AssetDatabase.LoadAssetAtPath<Level>(path);
                levelData.Level = levelNo;
                EditorUtility.SetDirty(levelData);

                string pathReviveData = "Assets/Hexa Stack/Data/ReviveDataConfig.asset";
                AssetDatabase.LoadAssetAtPath<ReviveDataSO>(pathReviveData).lastEditLevel = levelNo;
                AssetDatabase.LoadAssetAtPath<ReviveDataSO>(pathReviveData).levelTestingType = levelTestingType;

                // Force Unity to save all assets
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            // Now enter play mode
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.EnterPlaymode();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                // Save current state before entering play mode
                EditorPrefs.SetInt("TempLevelNo", tempLevelNo);
                EditorPrefs.SetString("TempLevelDataPath", AssetDatabase.GetAssetPath(tempLevelData));
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                // Restore the level when returning to edit mode
                EditorApplication.delayCall += () =>
                {
                    // Restore saved values
                    levelNo = EditorPrefs.GetInt("TempLevelNo");
                    string levelDataPath = EditorPrefs.GetString("TempLevelDataPath");
                    levelData = AssetDatabase.LoadAssetAtPath<LevelDataSO>(levelDataPath);

                    // Load the saved level
                    string path = GetLevelPath() + levelNo + "/Level " + levelNo + ".prefab";
                    if (File.Exists(path))
                    {
                        gamePlayLevel = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<Level>(path));
                        gamePlayLevel.name = "Level-" + levelNo;
                        gamePlayLevel.transform.position = Vector3.zero;
                        ShowDefaultDataInCell();
                    }

                    // Clear saved editor prefs
                    EditorPrefs.DeleteKey("TempLevelNo");
                    EditorPrefs.DeleteKey("TempLevelDataPath");

                    // Cleanup
                    EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                };
            }
        }

        private void PasteCellData()
        {
            if (selectedCell != null && copiedCell != null)
            {
                // Copy default data
                if (copiedCell.CellDefaultDataSO != null)
                {
                    string path = GetLevelPath() + levelNo + "/CellDefaultData-" + selectedCell.name + ".asset";
                    string directory = Path.GetDirectoryName(path);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    CellDefaultDataSO cellDefaultDataSO = ScriptableObject.CreateInstance<CellDefaultDataSO>();
                    cellDefaultDataSO.itemTypes = new List<int>(copiedCell.CellDefaultDataSO.itemTypes);
                    AssetDatabase.CreateAsset(cellDefaultDataSO, path);
                    selectedCell.CellDefaultDataSO = AssetDatabase.LoadAssetAtPath<CellDefaultDataSO>(path);
                }
                else
                {
                    string path = GetLevelPath() + levelNo + "/CellDefaultData-" + selectedCell.name + ".asset";
                    if (File.Exists(path))
                    {
                        AssetDatabase.DeleteAsset(path);
                    }
                    selectedCell.CellDefaultDataSO = null;
                }

                // Copy locker
                if (selectedCell.BaseCellUnlocker != null)
                {
                    GameObject.DestroyImmediate(selectedCell.BaseCellUnlocker.gameObject);
                    selectedCell.BaseCellUnlocker = null;
                }

                if (copiedCell.BaseCellUnlocker != null)
                {
                    // Set appropriate locker selector based on copied cell's locker
                    if (copiedCell.BaseCellUnlocker is FreeCellUnlocker)
                    {
                        cellLockerSelector = new AdCellLockerSelector();
                    }
                    else if (copiedCell.BaseCellUnlocker is GrassCellUnlocker)
                    {
                        cellLockerSelector = new GrassCellLockerSelector();
                    }
                    else if (copiedCell.BaseCellUnlocker is GoalPointCellUnlocker goalPointUnlocker)
                    {
                        var pointSelector = new PointCellLockerSelector();
                        // Create deep copy of the goal
                        if (goalPointUnlocker.LevelGoal is AllItemCollectGoal allItemGoal)
                        {
                            pointSelector.baseLevelGoal = new AllItemCollectGoal
                            {
                                GoalCount = allItemGoal.GoalCount
                            };
                        }
                        else if (goalPointUnlocker.LevelGoal is CoinItemCollectGoal coinGoal)
                        {
                            pointSelector.baseLevelGoal = new CoinItemCollectGoal
                            {
                                GoalCount = coinGoal.GoalCount,
                                itemId = coinGoal.itemId
                            };
                        }
                        else if (goalPointUnlocker.LevelGoal is SpecificItemCollectGoal specificGoal)
                        {
                            pointSelector.baseLevelGoal = new SpecificItemCollectGoal
                            {
                                GoalCount = specificGoal.GoalCount,
                                itemId = specificGoal.itemId
                            };
                        }
                        cellLockerSelector = pointSelector;
                    }
                    else if (copiedCell.BaseCellUnlocker is ItemGoalPointCellUnlocker itemGoalUnlocker)
                    {
                        var specificPointSelector = new SpecificItemPointCellLockerSelector();
                        // Create deep copy of the goal
                        if (itemGoalUnlocker.LevelGoal is AllItemCollectGoal allItemGoal)
                        {
                            specificPointSelector.baseLevelGoal = new AllItemCollectGoal
                            {
                                GoalCount = allItemGoal.GoalCount
                            };
                        }
                        else if (itemGoalUnlocker.LevelGoal is CoinItemCollectGoal coinGoal)
                        {
                            specificPointSelector.baseLevelGoal = new CoinItemCollectGoal
                            {
                                GoalCount = coinGoal.GoalCount,
                                itemId = coinGoal.itemId
                            };
                        }
                        else if (itemGoalUnlocker.LevelGoal is SpecificItemCollectGoal specificGoal)
                        {
                            specificPointSelector.baseLevelGoal = new SpecificItemCollectGoal
                            {
                                GoalCount = specificGoal.GoalCount,
                                itemId = specificGoal.itemId
                            };
                        }
                        cellLockerSelector = specificPointSelector;
                    }
                    else if (copiedCell.BaseCellUnlocker is BreadToasterUnlocker)
                    {
                        cellLockerSelector = new BreadToasterLockerSelector();
                    }
                    else if (copiedCell.BaseCellUnlocker is IceCellUnlocker)
                    {
                        cellLockerSelector = new IceLockerSelector();
                    }
                    else if (copiedCell.BaseCellUnlocker is PropellerCellUnlocker propellerUnlocker)
                    {
                        var propellerSelector = new PropellerLockerSelector();
                        propellerSelector.propellerCount = propellerUnlocker.name.EndsWith("1") ? 1 :
                                                         propellerUnlocker.name.EndsWith("2") ? 2 : 3;
                        cellLockerSelector = propellerSelector;
                    }
                    else if (copiedCell.BaseCellUnlocker is AdjacentCellStackCellUnlocker woodUnlocker)
                    {
                        var woodSelector = new WoodCellLockerSelector();
                        woodSelector.woodCount = woodUnlocker.name.EndsWith("1") ? 1 :
                                               woodUnlocker.name.EndsWith("2") ? 2 : 3;
                        cellLockerSelector = woodSelector;
                    }

                    if (cellLockerSelector != null)
                    {
                        cellLockerSelector.SelecteInCell(selectedCell);
                    }
                }

                ShowDefaultDataInCell();
            }
        }

        private IEnumerable GetLockerTypes()
        {
            var values = new Sirenix.OdinInspector.ValueDropdownList<BaseCellLockerSelector>();
            values.Add("None", null);
            values.Add("Ad Cell Locker", new AdCellLockerSelector());
            values.Add("Grass Cell Locker", new GrassCellLockerSelector());
            values.Add("Point Cell Locker", new PointCellLockerSelector());
            values.Add("Specific Item Point Locker", new SpecificItemPointCellLockerSelector());
            values.Add("Bread Toaster Locker", new BreadToasterLockerSelector());
            values.Add("Ice Cell Locker", new IceLockerSelector());
            values.Add("Propeller Cell Locker", new PropellerLockerSelector());
            values.Add("Wood Cell Locker", new WoodCellLockerSelector());
            return values;
        }

        #endregion
    }

    [System.Serializable]
    public class DefaultItemData
    {
        [HorizontalGroup("Item")]
        [LabelText("Item Type")]
        [ItemId]
        public int itemId;

        [HorizontalGroup("Item")]
        [LabelText("Count")]
        [Min(1)]
        public int count = 1;

        [HorizontalGroup("Item")]
        [Button("X", ButtonSizes.Small)]
        private void ClearItem()
        {
            itemId = 0;
            count = 1;
        }
    }
}