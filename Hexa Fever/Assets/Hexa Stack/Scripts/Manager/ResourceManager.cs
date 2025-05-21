using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tag.HexaStack
{
    public class ResourceManager : SerializedManager<ResourceManager>
    {
        #region PUBLIC_VARS
        public BaseItemStack ItemStack { get => itemStack; }
        public Sprite AllItemGoalSprite { get => allItemGoalSprite; }
        public BoosterDataSO BoosterData { get => boosterData; }

        public Sprite noAdsSprite;
        public Material grayScale;

        public int TotalLevelCount
        {
            get { return 100; }
        }

        public ElementIntroDataSO ElementIntroDataSO
        {
            get
            {
                if (elementIntroData.ContainsKey(CurrentTestingType))
                    return elementIntroData[CurrentTestingType];
                return elementIntroData[LevelTestingType.DefaultLevel];
            }
        }

        public IceTile IceTilePrefab { get => iceTilePrefab; }
        public LevelTestingType CurrentTestingType
        {
            get
            {
                return (LevelTestingType)PlayerPrefs.GetInt("LevelTestingTypeDataKey");
            }
            set
            {
                if (CurrentTestingType != value)
                {
                    loadedLevel = new LevelDataSO[0];
                    LoadLevelFromResources();
                    Debug.LogError("Reset Level Data due to Test type Change");
                    DataManager.Instance.ResetCurrentSaveLevelData();
                }
                PlayerPrefs.SetInt("LevelTestingTypeDataKey", (int)value);
            }
        }

        #endregion

        #region PRIVATE_VARS

        [Header("Levels")]
        [SerializeField] private LevelTestingConfigDataSO levelTestingConfigDataSO;
        //[SerializeField] private Dictionary<LevelTestingType, List<LevelDataSO>> levelDataDict = new Dictionary<LevelTestingType, List<LevelDataSO>>();

        [SerializeField] private LevelDataSO[] loadedLevel;

        [Space(20)]
        [SerializeField] private Dictionary<int, BaseItem> items = new Dictionary<int, BaseItem>();
        [SerializeField] private Dictionary<int, Sprite> goalItemSprite = new Dictionary<int, Sprite>();
        [SerializeField] private Dictionary<int, Sprite> goalObstacleIdSprite = new Dictionary<int, Sprite>();
        [SerializeField] private Dictionary<int, Sprite> currencySprite = new Dictionary<int, Sprite>();
        [SerializeField] private BaseItemStack itemStack;
        [SerializeField] private Sprite allItemGoalSprite;
        [SerializeField] private BoosterDataSO boosterData;
        [SerializeField] private Dictionary<LevelTestingType, ElementIntroDataSO> elementIntroData = new Dictionary<LevelTestingType, ElementIntroDataSO>();
        [SerializeField] private UnlockerPriorityDataSO unlockerPriorityData;
        [SerializeField] private IceTile iceTilePrefab;

        [SerializeField] private Dictionary<GameplayHandlerType, Material> baseCellMaterial = new Dictionary<GameplayHandlerType, Material>();
        [SerializeField] private Dictionary<GameplayHandlerType, Material> baseLockerMaterial = new Dictionary<GameplayHandlerType, Material>();
        [SerializeField] private Dictionary<GameplayHandlerType, Material> baseLockerSmallMaterial = new Dictionary<GameplayHandlerType, Material>();

        #endregion

        #region UNITY_CALLBACKS

        public override void Awake()
        {
            base.Awake();
            CurrentTestingType = levelTestingConfigDataSO.GetValue<LevelTestingConfigData>().levelTestingType;
            LoadLevelFromResources();
        }

        public void LoadLevelFromResources()
        {
            loadedLevel = Resources.LoadAll<LevelDataSO>(CurrentTestingType.ToString());
            if (loadedLevel == null)
            {
                loadedLevel = Resources.LoadAll<LevelDataSO>(LevelTestingType.DefaultLevel.ToString());
                Debug.LogError("Level Load Complate :- " + CurrentTestingType);
            }
        }

        public void ChnageLevelTestingType(LevelTestingType levelTestingType)
        {
            CurrentTestingType = levelTestingType;
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public Material GetDefaultCellmaterial()
        {
            GameplayHandlerType gameplayHandlerType = GameplayManager.Instance.CurrentHandler.gameplayHandlerType;

            if (baseCellMaterial.ContainsKey(gameplayHandlerType))
            {
                return baseCellMaterial[gameplayHandlerType];
            }
            return baseCellMaterial[GameplayHandlerType.Main];
        }

        public Material GetDefaultLockerMaterial()
        {
            GameplayHandlerType gameplayHandlerType = GameplayManager.Instance.CurrentHandler.gameplayHandlerType;

            if (baseLockerMaterial.ContainsKey(gameplayHandlerType))
            {
                return baseLockerMaterial[gameplayHandlerType];
            }
            return baseLockerMaterial[GameplayHandlerType.Main];
        }

        public Material GetDefaultSmallLockerMaterial()
        {
            GameplayHandlerType gameplayHandlerType = GameplayManager.Instance.CurrentHandler.gameplayHandlerType;

            if (baseLockerSmallMaterial.ContainsKey(gameplayHandlerType))
            {
                return baseLockerSmallMaterial[gameplayHandlerType];
            }
            return baseLockerSmallMaterial[GameplayHandlerType.Main];
        }

        public BaseItem GetItem(int itemType)
        {
            if (items.ContainsKey(itemType))
            {
                return items[itemType];
            }
            return null;
        }

        public Sprite GetGoalSprite(GoalType goalType, int itemId)
        {
            if (goalType == GoalType.Item)
            {
                if (goalItemSprite.ContainsKey(itemId))
                {
                    return goalItemSprite[itemId];
                }
                else
                {
                    return allItemGoalSprite;
                }
            }
            else
            {
                if (goalObstacleIdSprite.ContainsKey(itemId))
                {
                    return goalObstacleIdSprite[itemId];
                }
            }
            return null;
        }

        public BaseItem GetRandomItem()
        {
            return items.ElementAt(Random.Range(0, items.Count)).Value;
        }

        public LevelDataSO GetLevelData(int levelNo)
        {
            for (int i = 0; i < loadedLevel.Length; i++)
            {
                if (loadedLevel[i].Level == levelNo)
                    return loadedLevel[i];
            }
            return loadedLevel[loadedLevel.Length - 1];
        }

        public Sprite GetCurrencySprite(int currencyID)
        {
            if (currencySprite.ContainsKey(currencyID))
            {
                return currencySprite[currencyID];
            }
            return null;
        }

        public int GetPriority(int obstacalId)
        {
            return unlockerPriorityData.GetPriority(obstacalId);
        }

        //        [Button]
        //        public void AddLevelDataToDict(LevelTestingType levelTestingType)
        //        {
        //            // Create list for this testing type if it doesn't exist
        //            if (!levelDataDict.ContainsKey(levelTestingType))
        //            {
        //                levelDataDict[levelTestingType] = new List<LevelDataSO>();
        //            }
        //            else
        //            {
        //                // Clear existing data to avoid duplicates
        //                levelDataDict[levelTestingType].Clear();
        //            }

        //#if UNITY_EDITOR
        //            // Search for all LevelDataSO assets in the project
        //            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:LevelDataSO");
        //            foreach (string guid in guids)
        //            {
        //                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        //                LevelDataSO levelData = UnityEditor.AssetDatabase.LoadAssetAtPath<LevelDataSO>(path);

        //                // Check if this level data matches the desired testing type
        //                // You might need to modify this condition based on how your LevelDataSO identifies its type
        //                if (levelData != null && path.Contains(levelTestingType.ToString()))
        //                {
        //                    levelDataDict[levelTestingType].Add(levelData);
        //                }
        //            }
        //#endif

        //            // Sort levels by level number
        //            levelDataDict[levelTestingType] = levelDataDict[levelTestingType].OrderBy(x => x.Level).ToList();

        //            Debug.Log($"Added {levelDataDict[levelTestingType].Count} levels for testing type {levelTestingType}");
        //        }

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
