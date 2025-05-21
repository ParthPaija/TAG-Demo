using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "LevelData", menuName = Constant.GAME_NAME + "/LevelData")]
    public class LevelDataSO : SerializedScriptableObject
    {
        #region PUBLIC_VARS
        public Level LevelPrefab { get => levelPrefab; set => levelPrefab = value; }
        public int Level { get => level; set => level = value; }
        public List<BaseLevelGoal> LevelGoals { get => levelGoals; }
        public int SetIndex { get => setIndex; set => setIndex = value; }
        public List<LevelSpwanerConfig> LevelSpwanerConfigs { get => levelSpwanerConfigs; }
        public Dictionary<int, BaseItemSpawnerConfig> BaseItemSpawnerConfigs { get => baseItemSpawnerConfigs; }
        public BaseItemSpawnerConfig DefaultConfig { get => defaultConfig; set => defaultConfig = value; }
        public LevelType LevelType { get => levelType; set => levelType = value; }

        #endregion

        #region PRIVATE_VARS

        [ShowInInspector] private int setIndex;
        [SerializeField] private int level;
        [SerializeField] private LevelType levelType = LevelType.Normal;
        [SerializeField] private Level levelPrefab;
        [SerializeField] private List<BaseLevelGoal> levelGoals = new List<BaseLevelGoal>();
        [SerializeField] private List<LevelSpwanerConfig> levelSpwanerConfigs = new List<LevelSpwanerConfig>();
        [SerializeField] private Dictionary<int, BaseItemSpawnerConfig> baseItemSpawnerConfigs = new Dictionary<int, BaseItemSpawnerConfig>();

        [SerializeField] private BaseItemSpawnerConfig defaultConfig = new RandomItemSpawnerConfigNew();

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void OnLevelStart()
        {
            setIndex = 0;
            if (GameplayManager.Instance.CurrentHandler.gameplayHandlerType == GameplayHandlerType.VIPLeague)
                defaultConfig.SetData();
        }

        public Dictionary<int, List<int>> GetItem()
        {
            if (!baseItemSpawnerConfigs.ContainsKey(setIndex))
            {
                defaultConfig.GenrateItem();
                return defaultConfig.GetItem();
            }
            baseItemSpawnerConfigs[setIndex].GenrateItem();
            return baseItemSpawnerConfigs[setIndex].GetItem();
        }

        public Dictionary<int, List<int>> GetItemForBooster()
        {
            if (!baseItemSpawnerConfigs.ContainsKey(setIndex))
            {
                defaultConfig.GenrateItemForBooster();
                return defaultConfig.GetItem();
            }
            baseItemSpawnerConfigs[setIndex].GenrateItemForBooster();
            return baseItemSpawnerConfigs[setIndex].GetItem();
        }

        public List<int> GetUnlockItem()
        {
            List<int> itemTypes = new List<int>();
            int goalAmount = levelGoals[0].CurrentCount;

            for (int i = 0; i < levelSpwanerConfigs.Count; i++)
            {
                if (levelSpwanerConfigs[i].goalValue <= goalAmount)
                {
                    itemTypes.Add(levelSpwanerConfigs[i].itemId);
                }
            }
            return itemTypes;
        }

        public void AddGoal(BaseLevelGoal baseLevelGoal)
        {
            levelGoals = new List<BaseLevelGoal>();
            if (baseLevelGoal != null)
            {
                levelGoals.Add(baseLevelGoal);
            }
        }

        public void RemoveGoal(BaseLevelGoal baseLevelGoal)
        {
            if (levelGoals.Contains(baseLevelGoal))
            {
                levelGoals.Remove(baseLevelGoal);
            }
        }

        public void AddLevelSpwanerConfig(LevelSpwanerConfig levelSpwanerConfig)
        {
            if (levelSpwanerConfig != null)
            {
                levelSpwanerConfigs.Add(levelSpwanerConfig);
            }
        }

        public void RemoveLevelSpwanerConfig(LevelSpwanerConfig levelSpwanerConfig)
        {
            if (levelSpwanerConfigs.Contains(levelSpwanerConfig))
            {
                levelSpwanerConfigs.Remove(levelSpwanerConfig);
            }
        }

        public void DefaultDataLevelSpwanerConfigs(LevelType levelType)
        {
            levelSpwanerConfigs = new List<LevelSpwanerConfig>();
            if (levelType == LevelType.Bonus)
            {
                levelSpwanerConfigs.Add(new LevelSpwanerConfig { itemId = 1, goalValue = 0 });
                levelSpwanerConfigs.Add(new LevelSpwanerConfig { itemId = 2, goalValue = 0 });
                levelSpwanerConfigs.Add(new LevelSpwanerConfig { itemId = 3, goalValue = 0 });
                levelSpwanerConfigs.Add(new LevelSpwanerConfig { itemId = 9, goalValue = 0 });
            }
            else
            {
                levelSpwanerConfigs.Add(new LevelSpwanerConfig { itemId = 1, goalValue = 0 });
                levelSpwanerConfigs.Add(new LevelSpwanerConfig { itemId = 2, goalValue = 0 });
                levelSpwanerConfigs.Add(new LevelSpwanerConfig { itemId = 3, goalValue = 0 });
                levelSpwanerConfigs.Add(new LevelSpwanerConfig { itemId = 4, goalValue = 0 });
                levelSpwanerConfigs.Add(new LevelSpwanerConfig { itemId = 7, goalValue = 50 });
                levelSpwanerConfigs.Add(new LevelSpwanerConfig { itemId = 8, goalValue = 120 });
                levelSpwanerConfigs.Add(new LevelSpwanerConfig { itemId = 5, goalValue = 160 });
                levelSpwanerConfigs.Add(new LevelSpwanerConfig { itemId = 6, goalValue = 200 });
            }
        }

        public void DefaultSpwanerConfig(LevelType levelType)
        {
            defaultConfig = new RandomItemSpawnerConfigNew();
            defaultConfig.SetDefaultData(levelType);
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

    public enum LevelType
    {
        Normal,
        Medium,
        Hard,
        SuperHard,
        Bonus
    }

    public enum LevelTestingType
    {
        DefaultLevel, // First 
        TestLevel1, //Sort Level
        TestLevel2,//Easy Level
        TestLevel3//Lion Level
    }

    public class LevelSpwanerConfig
    {
        [ItemId] public int itemId;
        public int goalValue;
    }

    public abstract class BaseItemSpawnerConfig
    {
        [HideInInspector] public ItemSpawnerType itemType;
        [HideInInspector] public Dictionary<int, List<int>> items = new Dictionary<int, List<int>>();

        public virtual Dictionary<int, List<int>> GetItem()
        {
            return items;
        }

        public virtual void SetDefaultData(LevelType levelType) { }

        public virtual void GenrateItem() { }

        public virtual void SetData()
        {
        }

        public virtual void GenrateItemForBooster()
        {
        }
    }

    public class StaticItemSpawnerConfig : BaseItemSpawnerConfig
    {
        [ItemId] public List<List<int>> staticItem = new List<List<int>>();

        public StaticItemSpawnerConfig()
        {
            itemType = ItemSpawnerType.Static;
        }

        public override void GenrateItem()
        {
            base.GenrateItem();
            items = new Dictionary<int, List<int>>();

            MainSceneUIManager.Instance.GetView<GameplayBottomView>().SetSpawnerModeText("Static Spwan");
            for (int i = 0; i < staticItem.Count; i++)
            {
                if (!items.ContainsKey(i))
                    items.Add(i, staticItem[i]);
                else
                    items[i] = staticItem[i];
            }
        }

        public override void GenrateItemForBooster()
        {
            base.GenrateItemForBooster();
            GenrateItem();
        }

        public override Dictionary<int, List<int>> GetItem()
        {
            return base.GetItem();
        }
    }

    public class RandomItemSpawnerConfigNew : BaseItemSpawnerConfig
    {
        [ReadOnly, HideInInspector] public int uniqueItemCount = 0;
        [HideInInspector] public RandomItemSpawnerConfigData currentConfigData = new RandomItemSpawnerConfigData();

        public RandomItemSpawnerConfigData defautData = new RandomItemSpawnerConfigData();
        public RandomItemSpawnerConfigData lessTileModeDefaultData = new RandomItemSpawnerConfigData();
        public RandomItemSpawnerConfigData lessTileModeEasyMode = new RandomItemSpawnerConfigData();
        public RandomItemSpawnerConfigData lessTileModeHardMode = new RandomItemSpawnerConfigData();

        public override void SetData()
        {
            base.SetData();
            Debug.LogError("Data Set");
            defautData.stackSize[1] = new Vector2Int(defautData.stackSize[1].x, defautData.stackSize[1].y - 1);
            defautData.stackSize[2] = new Vector2Int(defautData.stackSize[1].x, defautData.stackSize[2].y - 1);
            defautData.stackSize[3] = new Vector2Int(defautData.stackSize[1].x, defautData.stackSize[3].y - 1);

            lessTileModeDefaultData.stackSize[1] = new Vector2Int(lessTileModeDefaultData.stackSize[1].x, lessTileModeDefaultData.stackSize[1].y - 1);
            lessTileModeDefaultData.stackSize[2] = new Vector2Int(lessTileModeDefaultData.stackSize[1].x, lessTileModeDefaultData.stackSize[2].y - 1);
            lessTileModeDefaultData.stackSize[3] = new Vector2Int(lessTileModeDefaultData.stackSize[1].x, lessTileModeDefaultData.stackSize[3].y - 1);
        }

        public override void SetDefaultData(LevelType levelType)
        {
            base.SetDefaultData(levelType);

            if (levelType == LevelType.Bonus)
            {
                defautData.DefaultConfigDataBouns(new SataticItemLevelConfigItemSelectionMode() { items = new List<int>() { 9 } });
                lessTileModeDefaultData.DefaultConfigDataBouns(new SataticItemLevelConfigItemSelectionMode() { items = new List<int>() { 9 } });
            }
            else
            {
                defautData.DefaultConfigData(new RandomLevelConfigItemSelectionMode());
                lessTileModeDefaultData.DefaultConfigData(new RandomLevelConfigItemSelectionMode());
            }
        }

        public RandomItemSpawnerConfigNew()
        {
            itemType = ItemSpawnerType.Random;
        }

        public override void GenrateItem()
        {
            int count = 0;
            List<BaseCell> cell = LevelManager.Instance.LoadedLevel.BaseCells;

            for (int i = 0; i < cell.Count; i++)
            {
                if (!cell[i].HasItem && !cell[i].IsCellLocked())
                    count++;
            }

            if (count <= SpawnAlgoConstant.lessTileCount)
            {
                if (DataManager.PlayerData.playerGameplayLevel <= SpawnAlgoConstant.forceEasyModeLevel)
                {
                    Debug.LogError("lessTileModeEasyMode : ForceFully For Level Less Then " + SpawnAlgoConstant.forceEasyModeLevel);
                    MainSceneUIManager.Instance.GetView<GameplayBottomView>().SetSpawnerModeText("lessTileModeEasyMode : ForceFully For Level Less Then " + SpawnAlgoConstant.forceEasyModeLevel);
                    currentConfigData = lessTileModeEasyMode;
                }
                else
                {
                    LevelType levelType = GameplayManager.Instance.CurrentLevel.LevelType;

                    if (levelType == LevelType.Normal || levelType == LevelType.Bonus)
                    {
                        currentConfigData = lessTileModeEasyMode;
                        MainSceneUIManager.Instance.GetView<GameplayBottomView>().SetSpawnerModeText("lessTileModeEasyMode : LevelType : " + levelType);
                    }
                    else
                    {
                        if (LevelManager.Instance.LoadedLevel.IsAllAdLockerAdWatched() ||
                            GameplayManager.Instance.ReviveCountAd > 0 ||
                            GameplayManager.Instance.ReviveCountCoin > 0 || GameplayManager.Instance.CurrentHandler.CanUseHammerOrSwapBoosterInLevel())
                        {
                            Debug.LogError("lessTileModeEasyMode : Booster Use");
                            MainSceneUIManager.Instance.GetView<GameplayBottomView>().SetSpawnerModeText("lessTileModeEasyMode : Booster Use");
                            currentConfigData = lessTileModeEasyMode;
                        }
                        else
                        {
                            int winStreakCount = DataManager.PlayerData.winStreak;
                            int loseStreakCount = DataManager.PlayerData.loseStreak;

                            if (loseStreakCount >= SpawnAlgoConstant.loseStreakCount)
                            {
                                Debug.LogError("lessTileModeEasyMode : Loose Streak " + loseStreakCount);
                                MainSceneUIManager.Instance.GetView<GameplayBottomView>().SetSpawnerModeText("lessTileModeEasyMode : Loose Streak " + loseStreakCount);
                                currentConfigData = lessTileModeEasyMode;
                            }
                            else if (winStreakCount >= SpawnAlgoConstant.winStreakCount)
                            {
                                Debug.LogError("lessTileModeHardMode : Win Streak " + winStreakCount);
                                MainSceneUIManager.Instance.GetView<GameplayBottomView>().SetSpawnerModeText("lessTileModeHardMode : Win Streak " + winStreakCount);
                                currentConfigData = lessTileModeHardMode;
                            }
                            else
                            {
                                Debug.LogError("lessTileMode : Default Mode Without Streak " + loseStreakCount + " " + winStreakCount);
                                MainSceneUIManager.Instance.GetView<GameplayBottomView>().SetSpawnerModeText("lessTileModeEasyMode : Default Mode Without Streak " + loseStreakCount + " " + winStreakCount);
                                currentConfigData = lessTileModeDefaultData;
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("defautData : Default Mode " + count);
                MainSceneUIManager.Instance.GetView<GameplayBottomView>().SetSpawnerModeText("defautData : Default Mode " + count);
                currentConfigData = defautData;
            }

            currentConfigData.GenrateItem();
            items = new Dictionary<int, List<int>>();
            items = currentConfigData.items;
        }

        public override void GenrateItemForBooster()
        {
            base.GenrateItemForBooster();
            MainSceneUIManager.Instance.GetView<GameplayBottomView>().SetSpawnerModeText("lessTileModeEasyMode : Refresh Booster ");
            Debug.LogError("lessTileModeEasyMode : Refresh Booster ");
            currentConfigData = lessTileModeEasyMode;
            currentConfigData.GenrateItem();
            items = new Dictionary<int, List<int>>();
            items = currentConfigData.items;
        }

        public override Dictionary<int, List<int>> GetItem()
        {
            return base.GetItem();
        }
    }

    public enum ItemSpawnerType
    {
        None,
        Static,
        Random
    }

    public class RandomItemSpawnerConfigData
    {
        [Header("Set Config")]
        public Vector2Int maxMinUniqueItemCount = new Vector2Int(2, 3);
        public BaseLevelConfigItemSelectionMode itemSelectionMode = new RandomLevelConfigItemSelectionMode();

        [Space(10)]
        [Header("Stack Config")]
        public Dictionary<int, Vector2Int> stackSize = new Dictionary<int, Vector2Int>();

        protected List<int> selectedItemInSet = new List<int>();
        protected int uniqueItemCount = 0;
        [HideInInspector] public List<int> unlockedItemInLevel = new List<int>();
        [HideInInspector] public Dictionary<int, List<int>> items = new Dictionary<int, List<int>>();

        public void DefaultConfigData(BaseLevelConfigItemSelectionMode baseLevelConfigItemSelectionMode)
        {
            maxMinUniqueItemCount = new Vector2Int(2, 5);
            itemSelectionMode = baseLevelConfigItemSelectionMode;
            stackSize = new Dictionary<int, Vector2Int>{
            { 1, new Vector2Int(2, 6) },
            { 2, new Vector2Int(2, 6) },
            { 3, new Vector2Int(3, 6) }};
        }

        public void DefaultConfigDataBouns(BaseLevelConfigItemSelectionMode baseLevelConfigItemSelectionMode)
        {
            maxMinUniqueItemCount = new Vector2Int(2, 4);
            itemSelectionMode = baseLevelConfigItemSelectionMode;
            stackSize = new Dictionary<int, Vector2Int>{
            { 1, new Vector2Int(2, 6) },
            { 2, new Vector2Int(2, 9) },
            { 3, new Vector2Int(3, 11) }};
        }

        public virtual void GenrateItem()
        {
            items = new Dictionary<int, List<int>>();
            selectedItemInSet = new List<int>();

            uniqueItemCount = Random.Range(maxMinUniqueItemCount.x, maxMinUniqueItemCount.y);
            unlockedItemInLevel = GameplayManager.Instance.CurrentLevel.GetUnlockItem();
            uniqueItemCount = Mathf.Clamp(uniqueItemCount, 1, unlockedItemInLevel.Count);

            selectedItemInSet = itemSelectionMode.FilterItem(unlockedItemInLevel, uniqueItemCount);

            for (int i = 0; i < 3; i++)
            {
                int noOfUniqueItem = Random.Range(1, stackSize.Count + 1);

                Vector2Int stackSizeTemp = stackSize[noOfUniqueItem];

                int totalStackSize = Random.Range(stackSizeTemp.x, stackSizeTemp.y + 1);

                for (int j = 0; j < noOfUniqueItem; j++)
                {
                    int remaingItemCount = noOfUniqueItem - j;

                    int itemCount;

                    if (j == noOfUniqueItem - 1)
                    {
                        itemCount = totalStackSize;
                    }
                    else
                    {
                        int currentItemCount = totalStackSize / remaingItemCount;

                        if (remaingItemCount == totalStackSize)
                        {
                            currentItemCount = remaingItemCount / totalStackSize;
                            itemCount = currentItemCount;
                        }
                        else
                        {
                            currentItemCount = Mathf.RoundToInt(currentItemCount);
                            int minusItem = currentItemCount - 1;
                            int plusItem = currentItemCount + 1;
                            itemCount = Random.Range(minusItem, plusItem + 1);
                        }
                    }

                    int itemId = GetUniqueItem(i);

                    for (int k = 0; k < itemCount; k++)
                    {
                        if (items.ContainsKey(i))
                        {
                            items[i].Add(itemId);
                        }
                        else
                        {
                            items.Add(i, new List<int>() { itemId });
                        }
                    }
                    totalStackSize -= itemCount;
                }
            }
        }

        public virtual int GetUniqueItem(int stackIndex)
        {
            selectedItemInSet.Shuffle();
            for (int i = 0; i < selectedItemInSet.Count; i++)
            {
                int itemId = selectedItemInSet[i];

                if (!items.ContainsKey(stackIndex))
                    return itemId;

                if (items.ContainsKey(stackIndex) && !items[stackIndex].Contains(itemId))
                    return itemId;
            }
            return selectedItemInSet[0];
        }
    }

    public class RandomItemSpawnerConfigDataEasyMode : RandomItemSpawnerConfigData
    {
        public List<int> topItem = new List<int>();

        public override void GenrateItem()
        {
            topItem = new List<int>();

            List<BaseCell> emptyCells = LevelManager.Instance.LoadedLevel.FindEmptyCells();

            Debug.Log("Eampty Cell Count :- " + emptyCells.Count);

            for (int i = 0; i < emptyCells.Count; i++)
            {
                for (int j = 0; j < emptyCells[i].AdjacentCells.Count; j++)
                {
                    if (emptyCells[i].AdjacentCells[j].HasItem && !emptyCells[i].AdjacentCells[j].IsCellLocked() &&
                        !topItem.Contains(emptyCells[i].AdjacentCells[j].ItemStack.GetTopItem().ItemId))
                    {
                        topItem.Add(emptyCells[i].AdjacentCells[j].ItemStack.GetTopItem().ItemId);
                        Debug.Log(emptyCells[i].AdjacentCells[j].ItemStack.GetTopItem().ItemId);
                    }
                }
            }

            items = new Dictionary<int, List<int>>();
            selectedItemInSet = new List<int>();

            uniqueItemCount = Random.Range(maxMinUniqueItemCount.x, maxMinUniqueItemCount.y);
            unlockedItemInLevel = GameplayManager.Instance.CurrentLevel.GetUnlockItem();
            uniqueItemCount = Mathf.Clamp(uniqueItemCount, 1, unlockedItemInLevel.Count);

            selectedItemInSet = itemSelectionMode.FilterItem(unlockedItemInLevel, uniqueItemCount);

            bool isLastItem = false;

            for (int i = 0; i < 3; i++)
            {
                int noOfUniqueItem = Random.Range(1, stackSize.Count + 1);

                Vector2Int stackSizeTemp = stackSize[noOfUniqueItem];

                int totalStackSize = Random.Range(stackSizeTemp.x, stackSizeTemp.y + 1);

                for (int j = 0; j < noOfUniqueItem; j++)
                {
                    int remaingItemCount = noOfUniqueItem - j;

                    int itemCount;

                    if (j == noOfUniqueItem - 1)
                    {
                        isLastItem = true;
                        itemCount = totalStackSize;
                    }
                    else
                    {
                        isLastItem = false;
                        int currentItemCount = totalStackSize / remaingItemCount;

                        if (remaingItemCount == totalStackSize)
                        {
                            currentItemCount = remaingItemCount / totalStackSize;
                            itemCount = currentItemCount;
                        }
                        else
                        {
                            currentItemCount = Mathf.RoundToInt(currentItemCount);
                            int minusItem = currentItemCount - 1;
                            int plusItem = currentItemCount + 1;
                            itemCount = Random.Range(minusItem, plusItem + 1);
                        }
                    }

                    int itemId = GetUniqueItem(i);

                    if (isLastItem && topItem.Count > 0)
                    {
                        itemId = topItem[Random.Range(0, topItem.Count)];
                    }

                    for (int k = 0; k < itemCount; k++)
                    {
                        if (items.ContainsKey(i))
                        {
                            items[i].Add(itemId);
                        }
                        else
                        {
                            items.Add(i, new List<int>() { itemId });
                        }
                    }
                    totalStackSize -= itemCount;
                }
            }
        }
    }

    public class RandomItemSpawnerConfigDataHardMode : RandomItemSpawnerConfigData
    {
        public List<int> bottomItem = new List<int>();
        public List<int> adLockerTileItem = new List<int>();

        public override void GenrateItem()
        {
            bottomItem = new List<int>();
            adLockerTileItem = new List<int>();

            List<BaseCell> emptyCells = LevelManager.Instance.LoadedLevel.FindEmptyCells();

            for (int i = 0; i < emptyCells.Count; i++)
            {
                for (int j = 0; j < emptyCells[i].AdjacentCells.Count; j++)
                {
                    if (emptyCells[i].AdjacentCells[j].HasItem && !emptyCells[i].AdjacentCells[j].IsCellLocked() &&
                        !bottomItem.Contains(emptyCells[i].AdjacentCells[j].ItemStack.GetTopItem().ItemId))
                    {
                        bottomItem.Add(emptyCells[i].AdjacentCells[j].ItemStack.GetTopItem().ItemId);
                        Debug.Log(emptyCells[i].AdjacentCells[j].ItemStack.GetTopItem().ItemId);
                    }
                }
            }

            List<BaseCell> cells = LevelManager.Instance.LoadedLevel.BaseCells;

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].IsCellLocked() && cells[i].BaseCellUnlocker as FreeCellUnlocker)
                {
                    for (int j = 0; j < cells[i].AdjacentCells.Count; j++)
                    {
                        if (cells[i].AdjacentCells[j].HasItem && !cells[i].AdjacentCells[j].IsCellLocked() &&
                            !adLockerTileItem.Contains(cells[i].AdjacentCells[j].ItemStack.GetTopItem().ItemId))
                        {
                            adLockerTileItem.Add(cells[i].AdjacentCells[j].ItemStack.GetTopItem().ItemId);
                            Debug.Log(cells[i].AdjacentCells[j].ItemStack.GetTopItem().ItemId);
                        }
                    }
                }
            }

            items = new Dictionary<int, List<int>>();
            selectedItemInSet = new List<int>();

            uniqueItemCount = Random.Range(maxMinUniqueItemCount.x, maxMinUniqueItemCount.y);
            unlockedItemInLevel = GameplayManager.Instance.CurrentLevel.GetUnlockItem();
            uniqueItemCount = Mathf.Clamp(uniqueItemCount, 1, unlockedItemInLevel.Count);

            selectedItemInSet = itemSelectionMode.FilterItem(unlockedItemInLevel, uniqueItemCount);

            for (int i = 0; i < bottomItem.Count; i++)
            {
                if (adLockerTileItem.Contains(bottomItem[i]))
                    adLockerTileItem.Remove(bottomItem[i]);

                if (selectedItemInSet.Contains(bottomItem[i]))
                    selectedItemInSet.Remove(bottomItem[i]);
            }

            bool isFirstItem = false;
            bool isLastItem = false;

            for (int i = 0; i < 3; i++)
            {
                int noOfUniqueItem = Random.Range(1, stackSize.Count + 1);

                Vector2Int stackSizeTemp = stackSize[noOfUniqueItem];

                int totalStackSize = Random.Range(stackSizeTemp.x, stackSizeTemp.y + 1);

                for (int j = 0; j < noOfUniqueItem; j++)
                {
                    int remaingItemCount = noOfUniqueItem - j;

                    int itemCount;

                    if (j == noOfUniqueItem - 1)
                    {
                        isLastItem = true;
                        isFirstItem = false;
                        itemCount = totalStackSize;
                    }
                    else if (j == 0)
                    {
                        isFirstItem = true;
                        isLastItem = false;
                        int currentItemCount = totalStackSize / remaingItemCount;

                        if (remaingItemCount == totalStackSize)
                        {
                            currentItemCount = remaingItemCount / totalStackSize;
                            itemCount = currentItemCount;
                        }
                        else
                        {
                            currentItemCount = Mathf.RoundToInt(currentItemCount);
                            int minusItem = currentItemCount - 1;
                            int plusItem = currentItemCount + 1;
                            itemCount = Random.Range(minusItem, plusItem + 1);
                        }
                    }
                    else
                    {
                        isFirstItem = false;
                        isLastItem = false;
                        int currentItemCount = totalStackSize / remaingItemCount;

                        if (remaingItemCount == totalStackSize)
                        {
                            currentItemCount = remaingItemCount / totalStackSize;
                            itemCount = currentItemCount;
                        }
                        else
                        {
                            currentItemCount = Mathf.RoundToInt(currentItemCount);
                            int minusItem = currentItemCount - 1;
                            int plusItem = currentItemCount + 1;
                            itemCount = Random.Range(minusItem, plusItem + 1);
                        }
                    }

                    int itemId = GetUniqueItem(i);

                    if (isFirstItem && bottomItem.Count > 0)
                    {
                        itemId = bottomItem[Random.Range(0, bottomItem.Count)];
                    }

                    if (isLastItem && adLockerTileItem.Count > 0)
                    {
                        itemId = adLockerTileItem[Random.Range(0, adLockerTileItem.Count)];
                    }

                    for (int k = 0; k < itemCount; k++)
                    {
                        if (items.ContainsKey(i))
                        {
                            items[i].Add(itemId);
                        }
                        else
                        {
                            items.Add(i, new List<int>() { itemId });
                        }
                    }
                    totalStackSize -= itemCount;
                }
            }
        }

        public override int GetUniqueItem(int stackIndex)
        {
            selectedItemInSet.Shuffle();
            for (int i = 0; i < selectedItemInSet.Count; i++)
            {
                int itemId = selectedItemInSet[i];

                if (!items.ContainsKey(stackIndex))
                    return itemId;

                if (items.ContainsKey(stackIndex) && !items[stackIndex].Contains(itemId))
                    return itemId;
            }

            if (selectedItemInSet.Count > 0)
                return selectedItemInSet[0];
            if (bottomItem.Count > 0)
                return bottomItem[0];
            if (adLockerTileItem.Count > 0)
                return adLockerTileItem[0];
            else return 1;
        }
    }

    public abstract class BaseLevelConfigItemSelectionMode
    {
        public virtual List<int> FilterItem(List<int> items, int count)
        {
            return items;
        }
    }

    public class RandomLevelConfigItemSelectionMode : BaseLevelConfigItemSelectionMode
    {
        public override List<int> FilterItem(List<int> items, int count)
        {
            List<int> filteredItem = new List<int>();
            for (int i = 0; i < count; i++)
            {
                int itemId = items[Random.Range(0, items.Count)];

                if (!filteredItem.Contains(itemId))
                    filteredItem.Add(itemId);
                else
                    i--;
            }
            return filteredItem;
        }
    }

    public class EasyLevelConfigItemSelectionMode : BaseLevelConfigItemSelectionMode
    {
        public override List<int> FilterItem(List<int> items, int count)
        {
            List<int> filteredItem = new List<int>();
            BaseCell emptyCell = LevelManager.Instance.LoadedLevel.FindEmptyCell();

            List<int> itemsNew = new List<int>();

            for (int i = 0; i < emptyCell.AdjacentCells.Count; i++)
            {
                if (emptyCell.HasItem && !emptyCell.IsCellLocked() && !itemsNew.Contains(emptyCell.AdjacentCells[i].ItemStack.GetTopItem().ItemId))
                {
                    itemsNew.Add(emptyCell.AdjacentCells[i].ItemStack.GetTopItem().ItemId);
                }
            }

            filteredItem.AddRange(itemsNew);
            count -= itemsNew.Count;

            for (int i = 0; i < count; i++)
            {
                int itemId = items[Random.Range(0, items.Count)];

                if (!filteredItem.Contains(itemId))
                    filteredItem.Add(itemId);
                else
                    i--;
            }
            return filteredItem;
        }
    }

    public class HardLevelConfigItemSelectionMode : BaseLevelConfigItemSelectionMode
    {
        public override List<int> FilterItem(List<int> items, int count)
        {
            List<int> filteredItem = new List<int>();
            BaseCell emptyCell = LevelManager.Instance.LoadedLevel.FindEmptyCell();

            List<int> itemsNew = new List<int>();

            if (emptyCell != null)
            {

                for (int i = 0; i < emptyCell.AdjacentCells.Count; i++)
                {
                    if (emptyCell.HasItem && !emptyCell.IsCellLocked() && !itemsNew.Contains(emptyCell.AdjacentCells[i].ItemStack.GetTopItem().ItemId))
                    {
                        itemsNew.Add(emptyCell.AdjacentCells[i].ItemStack.GetTopItem().ItemId);
                    }
                }


                itemsNew.Remove(0);
                filteredItem.AddRange(itemsNew);
                count -= itemsNew.Count;
            }

            for (int i = 0; i < count; i++)
            {
                int itemId = items[Random.Range(0, items.Count)];

                if (!filteredItem.Contains(itemId))
                    filteredItem.Add(itemId);
                else
                    i--;
            }
            return filteredItem;
        }
    }

    public class SataticItemLevelConfigItemSelectionMode : BaseLevelConfigItemSelectionMode
    {
        [ItemId] public List<int> items = new List<int>();

        public override List<int> FilterItem(List<int> items, int count)
        {
            List<int> filteredItem = new List<int>();

            filteredItem.AddRange(items);
            count -= items.Count;

            for (int i = 0; i < count; i++)
            {
                int itemId = items[Random.Range(0, items.Count)];

                if (!filteredItem.Contains(itemId))
                    filteredItem.Add(itemId);
                else
                    i--;
            }
            return filteredItem;
        }
    }
}
