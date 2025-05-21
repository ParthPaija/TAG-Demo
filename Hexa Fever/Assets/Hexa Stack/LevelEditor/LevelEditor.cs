using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class LevelEditor : SerializedManager<LevelEditor>
    {
        #region PUBLIC_VARS

        public LayerMask itemLayerMask;
        public LevelDataSO lastLevelEdit;

        public static LevelDataSO CurrentLevelForEdit { get => Instance.levelData; }

        public CellLockerSelcetorDataSO cellLockerSelcetorData;

        #endregion

        #region PRIVATE_VARS

        [SerializeField] Camera cameraMain;
        [SerializeField] private LevelDataSO levelData;
        [SerializeField] private Transform gamepPlay;

        private Level gamePlayLevelPrefab;
        private Level gamePlayLevel;
        private int levelNo;
        private RaycastHit hit;

        [HideInInspector, SerializeField] private List<BaseItem> baseItems = new List<BaseItem>();

        #endregion

        #region UNITY_CALLBACKS

        public override void Awake()
        {
            base.Awake();
            GetLevelDemoPrefab();
        }

        private void Update()
        {
            SetDirtyCurrentLevelData();

            if (Input.GetMouseButtonDown(0))
            {
                if (GetRayHit(cameraMain.ScreenToWorldPoint(Input.mousePosition), out hit, itemLayerMask))
                {
                    BaseCell baseCell = hit.collider.GetComponent<BaseCell>();
                    if (baseCell != null)
                    {

                    }
                }
            }
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void OnLoadLevel(int level)
        {
            levelNo = level;
            CreateLevel();
        }

        public void ChnageTileSet(GridGenerator gridGenerator)
        {
            gamePlayLevel.transform.Clear();
            GridGenerator temp = Instantiate(gridGenerator, gamePlayLevel.transform);
            gamePlayLevel.GridGenerator = temp;
            ShowDefaultDataInCell();
        }

        public void SetDirtyCurrentLevelData()
        {
            if (levelData == null)
                return;
#if UNITY_EDITOR
            EditorUtility.SetDirty(levelData);
#endif
        }

        public void Save()
        {
            if (gamePlayLevel != null)
            {
                for (int i = 0; i < baseItems.Count; i++)
                {
                    if (baseItems[i] != null)
                        GameObject.DestroyImmediate(baseItems[i].gameObject);
                }

                baseItems.Clear();
                baseItems = new List<BaseItem>();

                string path = "Assets/Hexa Stack/Asset/Levels/Level " + levelNo + "/Level " + levelNo + ".prefab";
                string directory = Path.GetDirectoryName(path);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                gamePlayLevel.GetComponent<GridRotator>().enabled = true;
#if UNITY_EDITOR
                PrefabUtility.SaveAsPrefabAsset(gamePlayLevel.gameObject, path);
                GameObject.DestroyImmediate(gamePlayLevel.gameObject);
                gamePlayLevel = null;
                levelData.LevelPrefab = AssetDatabase.LoadAssetAtPath<Level>(path);
                levelData.Level = levelNo;
                EditorUtility.SetDirty(levelData);
                levelData = null;
#endif
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void GetLevelDemoPrefab()
        {
#if UNITY_EDITOR
            if (gamePlayLevelPrefab == null)
                gamePlayLevelPrefab = AssetDatabase.LoadAssetAtPath<Level>("Assets/Hexa Stack/Editor/LevelEditor/Prefab/Level Demo.prefab");
#endif
        }

        private void CreateLevel()
        {
#if UNITY_EDITOR
            if (gamePlayLevel == null)
            {
                if (levelData == null)
                {
                    string path = "Assets/Hexa Stack/Asset/Levels/Level " + levelNo + "/LevelData " + levelNo + ".asset";
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
                            gamePlayLevel = GameObject.Instantiate(levelDataTemp.LevelPrefab, gamepPlay);
                            gamePlayLevel.name = "Level-" + levelNo;
                            gamePlayLevel.transform.localPosition = Vector3.zero;
                        }
                        else
                        {
                            gamePlayLevel = GameObject.Instantiate(gamePlayLevelPrefab, gamepPlay);
                            gamePlayLevel.name = "Level-" + levelNo;
                            gamePlayLevel.transform.localPosition = Vector3.zero; ;

                            levelData.LevelPrefab = gamePlayLevel;
                        }
                    }
                    else
                    {
                        levelDataTemp = ScriptableObject.CreateInstance<LevelDataSO>();
                        levelData = levelDataTemp;
                        levelDataTemp.name = $"LevelData {levelNo}";
                        AssetDatabase.CreateAsset(levelDataTemp, path);

                        gamePlayLevel = GameObject.Instantiate(gamePlayLevelPrefab, gamepPlay);
                        gamePlayLevel.name = "Level-" + levelNo;
                        gamePlayLevel.transform.localPosition = Vector3.zero;

                        levelData.LevelPrefab = gamePlayLevel;
                    }
                }
            }
            if (gamePlayLevel != null)
                gamePlayLevel.GetComponent<GridRotator>().enabled = false;
            ShowDefaultDataInCell();
#endif
        }

        private bool GetRayHit(Vector3 pos, out RaycastHit hit, LayerMask layerMask)
        {
            return Physics.Raycast(pos, InputManager.eventTranform.forward, out hit, Mathf.Infinity, layerMask);
        }

        private void ShowDefaultDataInCell()
        {
            if (gamePlayLevel.GridGenerator == null)
                return;

            for (int i = 0; i < baseItems.Count; i++)
            {
                if (baseItems[i] != null)
                    GameObject.DestroyImmediate(baseItems[i].gameObject);
            }

            baseItems.Clear();
            baseItems = new List<BaseItem>();

            for (int i = 0; i < gamePlayLevel.BaseCells.Count; i++)
            {
                if (gamePlayLevel.BaseCells[i].CellDefaultDataSO == null)
                    continue;

                for (int j = 0; j < gamePlayLevel.BaseCells[i].CellDefaultDataSO.itemTypes.Count; j++)
                {
                    BaseItem temp = GameObject.Instantiate(cellLockerSelcetorData.items[gamePlayLevel.BaseCells[i].CellDefaultDataSO.itemTypes[j]], gamePlayLevel.BaseCells[i].transform);
                    temp.transform.localPosition = gamePlayLevel.BaseCells[i].transform.localPosition.With(0,
                        gamePlayLevel.BaseCells[i].transform.localPosition.y+ GamePlayConstant.TWO_ITEM_DISTANCE + (j * (GamePlayConstant.TWO_ITEM_DISTANCE)),
                        0);
                    baseItems.Add(temp);
                }
            }
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
