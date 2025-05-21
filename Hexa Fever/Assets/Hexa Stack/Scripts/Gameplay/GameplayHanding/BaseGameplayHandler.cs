using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class BaseGameplayHandler : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS

        public string eventNameTag = "Level";
        public GameplayHandlerType gameplayHandlerType;

        public LevelDataSO CurrentLevel { get => currentLevel; }
        public virtual int LevelNo { get => 0; }

        public UndoBoosterProgressData UndoBoosterData { get => undoBoosterData; set => undoBoosterData = value; }
        public int ReviveCountCoin { get => reviveCountCoin; set => reviveCountCoin = value; }
        public int ReviveCountAd { get => reviveCountAd; set => reviveCountAd = value; }
        public int AdTileWatchCountAd { get => adTileWatchCountAd; set => adTileWatchCountAd = value; }
        protected Dictionary<int, int> BoosterUseData { get => boosterUseData; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] protected LevelDataSO currentLevel;
        protected GameplayManager gameplayManager;
        protected int reviveCountCoin = 0;
        protected int reviveCountAd = 0;
        protected int adTileWatchCountAd = 0;
        protected bool isGameComplate = false;
        [SerializeField] protected int levelRunTime;
        [SerializeField] protected UndoBoosterProgressData undoBoosterData = new UndoBoosterProgressData();
        [SerializeField] protected Dictionary<int, int> boosterUseData = new Dictionary<int, int>();

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public virtual void Init(GameplayManager gameplayManager)
        {
            this.gameplayManager = gameplayManager;
        }

        public virtual void OnLevelStart(LevelDataSO levelDataSO)
        {
            isGameComplate = false;
            currentLevel = levelDataSO;
            currentLevel.OnLevelStart();
            ResetUndoBoosterData();
            GameRuleManager.Instance.OnLevelComplate();
        }

        public virtual void OnNextOrCurrentLevelRestart()
        {

        }

        public virtual void OnLevelWin()
        {
            AdManager.Instance.OnLevelPlayed();
            if (BoosterManager.Instance.IsAnyBoosterActive)
                BoosterManager.Instance.CurrentActiveBooster.OnUnUse();
            GameRuleManager.Instance.OnLevelComplate();
            MainSceneUIManager.Instance.GetView<GameplaySettingView>().Hide();
        }

        public virtual void OnOutOfSpace()
        {
            if (BoosterManager.Instance.IsAnyBoosterActive)
                BoosterManager.Instance.CurrentActiveBooster.OnUnUse();
            MainSceneUIManager.Instance.GetView<GameplaySettingView>().Hide();
        }

        public virtual void OnLevelFail()
        {
            GameRuleManager.Instance.OnLevelComplate();
            MainSceneUIManager.Instance.GetView<GameplaySettingView>().Hide();
        }

        public virtual void OnLevelFailRetry()
        {

        }

        public virtual void OnExit()
        {
            GameRuleManager.Instance.OnLevelComplate();
        }

        public virtual void OnRetry()
        {

        }

        public virtual void SaveDataForUndoBooster()
        {
            if (GameRuleManager.Instance.IsMovementInProgress)
                return;
            undoBoosterData.canUseUndoBooster = true;
            MainSceneUIManager.Instance.GetView<GameplayBottomView>().CheakUndoBoosterCondition();
            LevelManager.Instance.LoadedLevel.SaveCellData(undoBoosterData.progressDataUndoBooster);
            ItemStackSpawnerManager.Instance.SaveData(undoBoosterData.progressDataUndoBooster);
            GameplayGoalHandler.Instance.SaveGoalData(undoBoosterData.progressDataUndoBooster);
        }

        public virtual void OnUndoBoosterUse()
        {
            ItemStackSpawnerManager.Instance.SetUndoBoosterData(undoBoosterData.progressDataUndoBooster);
            GameplayGoalHandler.Instance.SetUndoBoosterGoalData(undoBoosterData.progressDataUndoBooster);
            LevelManager.Instance.LoadedLevel.SetUndoBoosterData();
            ResetUndoBoosterData();
            SaveAllDataOfLevel();
        }

        public virtual void CancelUndoBooster()
        {
            ResetUndoBoosterData();
        }

        public virtual void SaveAllDataOfLevel()
        {
            if (isGameComplate)
                return;
            LevelProgressData levelProgressData = DataManager.LevelProgressData;
            LevelManager.Instance.LoadedLevel.SaveCellData(levelProgressData);
            ItemStackSpawnerManager.Instance.SaveData(levelProgressData);
            GameplayGoalHandler.Instance.SaveGoalData(levelProgressData);
            levelProgressData.currentReviveCountCoin = reviveCountCoin;
            levelProgressData.currentReviveCountAd = reviveCountAd;
            levelProgressData.adTileWatchCount = adTileWatchCountAd;
            levelProgressData.boosterUseData = boosterUseData;
            DataManager.Instance.SaveLevelProgressData(levelProgressData);
        }

        public void IncreaseLevelRunTime()
        {
            levelRunTime++;
            var levelProgressData = DataManager.LevelProgressData;
            levelProgressData.currentRunningTime = levelRunTime;
            DataManager.Instance.SaveLevelProgressData(levelProgressData);
        }

        public string GetCurrentLevelEventString()
        {
            string levelString = eventNameTag + LevelNo;
            return levelString;
        }

        public void OnBoosterUse(int boosterId)
        {
            if (boosterUseData.ContainsKey(boosterId))
            {
                boosterUseData[boosterId] += 1;
            }
            else
            {
                boosterUseData.Add(boosterId, 1);
            }
            string boosteName = ResourceManager.Instance.BoosterData.GetBoosterData(boosterId).boosterName;
            AnalyticsManager.Instance.LogGAEvent(boosteName + "Used : " + GetCurrentLevelEventString() + " : " + boosterUseData[boosterId]);
        }

        public bool CanUseHammerOrSwapBoosterInLevel()
        {
            if (boosterUseData.ContainsKey(CurrencyConstant.HAMMER_BOOSTER) && boosterUseData[CurrencyConstant.HAMMER_BOOSTER] > 0)
                return true;
            if (boosterUseData.ContainsKey(CurrencyConstant.SWAP_BOOSTER) && boosterUseData[CurrencyConstant.SWAP_BOOSTER] > 0)
                return true;
            return false;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        protected void ResetUndoBoosterData()
        {
            if (undoBoosterData == null)
                undoBoosterData = new UndoBoosterProgressData();
            undoBoosterData.canUseUndoBooster = false;
            MainSceneUIManager.Instance.GetView<GameplayBottomView>().CheakUndoBoosterCondition();
            undoBoosterData.progressDataUndoBooster.ReSetData();
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        #endregion
    }

    public class UndoBoosterProgressData
    {
        public LevelProgressData progressDataUndoBooster = new LevelProgressData();
        public bool canUseUndoBooster = false;

        public UndoBoosterProgressData()
        {
            progressDataUndoBooster = new LevelProgressData();
        }
    }

    public enum GameplayHandlerType
    {
        None,
        Main,
        Editor,
        VIPLeague
    }
}
