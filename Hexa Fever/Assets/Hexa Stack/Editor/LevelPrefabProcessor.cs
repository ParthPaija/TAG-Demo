using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Tag.HexaStack;

namespace Tag.HexaStack.Editor
{

    public class LevelPrefabProcessor : EditorWindow
    {
        private List<GameObject> levelPrefabs = new List<GameObject>();
        private Vector2 scrollPosition;
        private bool processWoodLockers = false;
        private bool processFreeLockers = false;
        private bool processCameraLockers = false;
        private bool processGoalLockers = false;
        private bool processItemGoalLockers = false;
        private bool processPropellerLockers = false;
        private bool processGrassLockers = false;

        [MenuItem("Tools/Level Prefab Processor")]
        public static void ShowWindow()
        {
            GetWindow<LevelPrefabProcessor>("Level Prefab Processor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Level Prefab Processor", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            if (GUILayout.Button("Find All Level Prefabs"))
            {
                FindAllLevelPrefabs();
            }

            EditorGUILayout.Space();

            // Options for which lockers to process
            processWoodLockers = EditorGUILayout.Toggle("Process Wood Lockers", processWoodLockers);
            processPropellerLockers = EditorGUILayout.Toggle("Process Propeller Lockers", processPropellerLockers);
            processGrassLockers = EditorGUILayout.Toggle("Process Grass Lockers", processGrassLockers);
            processFreeLockers = EditorGUILayout.Toggle("Process Free Lockers", processFreeLockers);
            processCameraLockers = EditorGUILayout.Toggle("Process Camera Lockers", processCameraLockers);
            processGoalLockers = EditorGUILayout.Toggle("Process Goal Lockers", processGoalLockers);
            processItemGoalLockers = EditorGUILayout.Toggle("Process Item Goal Lockers", processItemGoalLockers);

            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < levelPrefabs.Count; i++)
            {
                EditorGUILayout.ObjectField($"Level {i + 1}", levelPrefabs[i], typeof(GameObject), false);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            if (GUILayout.Button("Process Selected Locker Types"))
            {
                ProcessAllLevelPrefabs();
            }
        }

        private void FindAllLevelPrefabs()
        {
            levelPrefabs.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Hexa Stack/Asset/Levels" });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab.GetComponent<Level>() != null)
                {
                    levelPrefabs.Add(prefab);
                }
            }

            Debug.Log($"Found {levelPrefabs.Count} Level prefabs");
        }

        private void ProcessAllLevelPrefabs()
        {
            foreach (GameObject levelPrefab in levelPrefabs)
            {
                ProcessLevelPrefab(levelPrefab);
            }

            Debug.Log("Completed processing all Level prefabs");
        }

        private void ProcessLevelPrefab(GameObject levelPrefab)
        {
            GameObject tempInstance = PrefabUtility.InstantiatePrefab(levelPrefab) as GameObject;
            Level level = tempInstance.GetComponent<Level>();

            if (level != null)
            {
                bool prefabModified = false;

                foreach (BaseCell baseCell in level.BaseCells)
                {
                    if (baseCell.BaseCellUnlocker != null)
                    {
                        if (processWoodLockers && ProcessWoodLocker(baseCell)) prefabModified = true;
                        if (processFreeLockers && ProcessFreeLocker(baseCell)) prefabModified = true;
                        if (processCameraLockers && ProcessCameraLocker(baseCell)) prefabModified = true;
                        if (processGoalLockers && ProcessGoalLocker(baseCell)) prefabModified = true;
                        if (processItemGoalLockers && ProcessItemGoalLocker(baseCell)) prefabModified = true;
                        if (processPropellerLockers && ProcessPropellerLocker(baseCell)) prefabModified = true;
                        if (processGrassLockers && ProcessGrassLocker(baseCell)) prefabModified = true;
                    }
                }

                if (prefabModified)
                {
                    PrefabUtility.SaveAsPrefabAsset(tempInstance, AssetDatabase.GetAssetPath(levelPrefab));
                    Debug.Log($"Updated cell lockers in Level prefab: {levelPrefab.name}");
                }
            }

            DestroyImmediate(tempInstance);
        }

        private bool ProcessWoodLocker(BaseCell baseCell)
        {
            if (baseCell.BaseCellUnlocker.GetType() != typeof(AdjacentCellStackCellUnlocker))
                return false;

            AdjacentCellStackCellUnlocker currentUnlocker = (AdjacentCellStackCellUnlocker)baseCell.BaseCellUnlocker;
            string newPath = GetWoodCellLockerPath(currentUnlocker.RemoveableItemObjs.Length);

            AdjacentCellStackCellUnlocker newUnlocker = AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(newPath) as AdjacentCellStackCellUnlocker;
            if (newUnlocker != null)
            {
                DestroyImmediate(currentUnlocker.gameObject);
                baseCell.BaseCellUnlocker = null;

                AdjacentCellStackCellUnlocker temp = (AdjacentCellStackCellUnlocker)PrefabUtility.InstantiatePrefab(newUnlocker, baseCell.transform);
                baseCell.BaseCellUnlocker = temp;
                return true;
            }
            return false;
        }

        private bool ProcessPropellerLocker(BaseCell baseCell)
        {
            if (baseCell.BaseCellUnlocker.GetType() != typeof(PropellerCellUnlocker))
                return false;

            PropellerCellUnlocker currentUnlocker = (PropellerCellUnlocker)baseCell.BaseCellUnlocker;
            string newPath = GetPropllerCellLockerPath(currentUnlocker.Propellers.Count);

            PropellerCellUnlocker newUnlocker = AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(newPath) as PropellerCellUnlocker;
            if (newUnlocker != null)
            {
                DestroyImmediate(currentUnlocker.gameObject);
                baseCell.BaseCellUnlocker = null;

                PropellerCellUnlocker temp = (PropellerCellUnlocker)PrefabUtility.InstantiatePrefab(newUnlocker, baseCell.transform);
                baseCell.BaseCellUnlocker = temp;
                return true;
            }
            return false;
        }

        private bool ProcessFreeLocker(BaseCell baseCell)
        {
            if (baseCell.BaseCellUnlocker.GetType() != typeof(FreeCellUnlocker))
                return false;

            string path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/AdCellLocker.prefab";
            FreeCellUnlocker freeCellUnlocker = AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path) as FreeCellUnlocker;

            DestroyImmediate(baseCell.BaseCellUnlocker.gameObject);
            baseCell.BaseCellUnlocker = null;

            FreeCellUnlocker temp = (FreeCellUnlocker)PrefabUtility.InstantiatePrefab(freeCellUnlocker, baseCell.transform);
            baseCell.BaseCellUnlocker = temp;
            return true;
        }

        private bool ProcessGrassLocker(BaseCell baseCell)
        {
            if (baseCell.BaseCellUnlocker.GetType() != typeof(GrassCellUnlocker))
                return false;

            string path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/GrassCellUnlocker.prefab";
            GrassCellUnlocker grassCellUnlocker = AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path) as GrassCellUnlocker;

            DestroyImmediate(baseCell.BaseCellUnlocker.gameObject);
            baseCell.BaseCellUnlocker = null;

            GrassCellUnlocker temp = (GrassCellUnlocker)PrefabUtility.InstantiatePrefab(grassCellUnlocker, baseCell.transform);
            baseCell.BaseCellUnlocker = temp;
            return true;
        }

        private bool ProcessCameraLocker(BaseCell baseCell)
        {
            if (baseCell.BaseCellUnlocker.GetType() != typeof(BreadToasterUnlocker))
                return false;

            string path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Mail Box.prefab";
            BreadToasterUnlocker cameraUnlocker = AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path) as BreadToasterUnlocker;

            DestroyImmediate(baseCell.BaseCellUnlocker.gameObject);
            baseCell.BaseCellUnlocker = null;

            BreadToasterUnlocker temp = (BreadToasterUnlocker)PrefabUtility.InstantiatePrefab(cameraUnlocker, baseCell.transform);
            baseCell.BaseCellUnlocker = temp;
            return true;
        }

        private bool ProcessGoalLocker(BaseCell baseCell)
        {
            if (baseCell.BaseCellUnlocker.GetType() != typeof(GoalPointCellUnlocker))
                return false;

            string path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/GoalPointCellLocker.prefab";

            GoalPointCellUnlocker goalPointCellUnlocker = (GoalPointCellUnlocker)AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path);

            GameObject temp = GameObject.Instantiate(goalPointCellUnlocker.gameObject, baseCell.transform);
            GoalPointCellUnlocker goalPointCellUnlocker1 = temp.GetComponent<GoalPointCellUnlocker>();

            GoalPointCellUnlocker tempOld = (GoalPointCellUnlocker)baseCell.BaseCellUnlocker;

            goalPointCellUnlocker1.LevelGoal = tempOld.LevelGoal;
            goalPointCellUnlocker1.SetText();

            BaseCellUnlocker baseCellUnlockerCurrentCell = baseCell.BaseCellUnlocker;
            if (baseCellUnlockerCurrentCell != null)
            {
                GameObject.DestroyImmediate(baseCellUnlockerCurrentCell.gameObject);
                baseCell.BaseCellUnlocker = null;
            }

            baseCell.BaseCellUnlocker = goalPointCellUnlocker1;

            tempOld = null;
            return true;
        }

        private bool ProcessItemGoalLocker(BaseCell baseCell)
        {
            if (baseCell.BaseCellUnlocker.GetType() != typeof(ItemGoalPointCellUnlocker))
                return false;

            string path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/ItemGoalPointCellUnlocker.prefab";

            ItemGoalPointCellUnlocker goalPointCellUnlocker = (ItemGoalPointCellUnlocker)AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path);

            GameObject temp = GameObject.Instantiate(goalPointCellUnlocker.gameObject, baseCell.transform);
            ItemGoalPointCellUnlocker goalPointCellUnlocker1 = temp.GetComponent<ItemGoalPointCellUnlocker>();

            ItemGoalPointCellUnlocker tempOld = (ItemGoalPointCellUnlocker)baseCell.BaseCellUnlocker;

            goalPointCellUnlocker1.LevelGoal = tempOld.LevelGoal;
            goalPointCellUnlocker1.SetText();
            goalPointCellUnlocker1.SetLoakImage();

            BaseCellUnlocker baseCellUnlockerCurrentCell = baseCell.BaseCellUnlocker;
            if (baseCellUnlockerCurrentCell != null)
            {
                GameObject.DestroyImmediate(baseCellUnlockerCurrentCell.gameObject);
                baseCell.BaseCellUnlocker = null;
            }

            baseCell.BaseCellUnlocker = goalPointCellUnlocker1;

            tempOld = null;
            return true;
        }

        private string GetWoodCellLockerPath(int removeableItemCount)
        {
            switch (removeableItemCount)
            {
                case 1:
                    return "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Wood/Wood-1/WoodCellStackCellLocker - 1.prefab";
                case 2:
                    return "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Wood/Wood-2/WoodCellStackCellLocker - 2.prefab";
                case 3:
                    return "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Wood/Wood-3/WoodCellStackCellLocker - 3.prefab";
                default:
                    return "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Wood/Wood-1/WoodCellStackCellLocker - 1.prefab";
            }
        }

        private string GetPropllerCellLockerPath(int propellerItemCount)
        {
            switch (propellerItemCount)
            {
                case 1:
                    return "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Propeller Cell Unlocker - 1.prefab";
                case 2:
                    return "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Propeller Cell Unlocker - 2.prefab";
                case 3:
                    return "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Propeller Cell Unlocker - 3.prefab";
                default:
                    return "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Propeller Cell Unlocker - 1.prefab";
            }
        }
    }
}