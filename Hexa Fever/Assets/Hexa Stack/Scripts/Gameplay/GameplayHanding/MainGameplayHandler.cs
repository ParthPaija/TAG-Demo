using Hexa_Stack.Scripts.CoreGameSDK.Puzzle;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class MainGameplayHandler : BaseGameplayHandler
    {
        #region PUBLIC_VARS

        public override int LevelNo { get => DataManager.PlayerData.playerGameplayLevel; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] internal bool isLogAdjustEvent = false;
        [SerializeField] internal Dictionary<LevelType, BaseReward> commonRewards = new Dictionary<LevelType, BaseReward>();
        [SerializeField] internal Dictionary<LevelType, GameObject> levelTheamGO = new Dictionary<LevelType, GameObject>();
        [SerializeField] internal List<BaseReward> levelReward = new List<BaseReward>();
        [SerializeField] private GameObject groundMain;
        [SerializeField] private GameObject groundLeage;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void OnNextOrCurrentLevelRestart()
        {
            base.OnNextOrCurrentLevelRestart();
            OnLevelStart(ResourceManager.Instance.GetLevelData(DataManager.PlayerData.playerGameplayLevel));
        }

        public override void OnLevelStart(LevelDataSO levelDataSO)
        {
            isGameComplate = false;
            groundMain.gameObject.SetActive(gameplayHandlerType == GameplayHandlerType.Main);
            groundLeage.gameObject.SetActive(gameplayHandlerType == GameplayHandlerType.VIPLeague);

            InputManager.StopInteraction = false;
            base.OnLevelStart(levelDataSO);
            reviveCountCoin = 0;
            reviveCountAd = 0;
            SetlevelTheam();
            LevelProgressData levelProgressData = DataManager.LevelProgressData;

            Log_LevelStartEvent(levelProgressData);

            levelProgressData.OnLevelStart(currentLevel.Level);
            levelRunTime = levelProgressData.currentRunningTime;
            DataManager.Instance.SaveLevelProgressData(levelProgressData);

            HandleViews();
            SetGoal();

            reviveCountCoin = levelProgressData.currentReviveCountCoin;
            reviveCountAd = levelProgressData.currentReviveCountAd;
            boosterUseData = levelProgressData.boosterUseData;
            SaveAllDataOfLevel();

            if (LevelManager.Instance.LoadedLevel.IsAllCellOcupied() && !GameplayGoalHandler.Instance.IsGameComplate)
            {
                GameplayManager.Instance.OnOutOfSpace();
            }

            //TutorialManager.Instance.CheckForTutorialsToStart();
        }

        public override void OnLevelWin()
        {
            isGameComplate = true;
            base.OnLevelWin();

            SetWinReward();
            ShowLevelWinView();

            AnalyticsManager.Instance.LogProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Complete);
            AnalyticsManager.Instance.LogGAEvent("LevelTime : Win : " + eventNameTag + LevelNo + " : " + levelRunTime);

            var playerData = DataManager.PlayerData;
            playerData.OnLevelWin();
            DataManager.Instance.SaveData(playerData);

            LevelProgressData levelProgressData = DataManager.LevelProgressData;
            Log_LevelComplateEvent(levelProgressData);
            levelProgressData.OnLevelEnd();
            DataManager.Instance.SaveLevelProgressData(levelProgressData);

            GameStatsCollector.Instance.GameplayManager_onGameplayLevelOver();
            VIPLeaderboardManager.Instance.GameplayManager_onGameplayLevelOver();
        }

        public override void OnOutOfSpace()
        {
            base.OnOutOfSpace();
            MainSceneUIManager.Instance.GetView<OutOfSpaceView>().Show();
        }

        public override void OnLevelFail()
        {
            isGameComplate = true;
            AnalyticsManager.Instance.LogProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Fail);

            levelRunTime = 0;

            base.OnLevelFail();

            var currency = DataManager.Instance.GetCurrency(CurrencyConstant.ENERGY);
            currency.Add(-1);

            var playerData = DataManager.PlayerData;
            playerData.OnLevelFail();
            DataManager.Instance.SaveData(playerData);


            Log_LevelFailEvent();

            MainSceneUIManager.Instance.GetView<LevelFailView>().Show();

            LevelProgressData levelProgressData = DataManager.LevelProgressData;
            levelProgressData.OnLevelEnd();
            levelProgressData.levelNo = currentLevel.Level;
            DataManager.Instance.SaveLevelProgressData(levelProgressData);
            AnalyticsManager.Instance.LogProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Start);
        }

        public override void OnLevelFailRetry()
        {
            AnalyticsManager.Instance.LogGAEvent("LevelStart : RetryFail : " + eventNameTag + LevelNo);
        }

        public override void OnExit()
        {
            base.OnExit();

            isGameComplate = true;
            LevelManager.Instance.UnloadLevel();
            MainSceneUIManager.Instance.GetView<GameplayTopbarView>().Hide();
            MainSceneUIManager.Instance.GetView<GameplayBottomView>().Hide();
            MainSceneUIManager.Instance.GetView<GameplayGoalView>().Hide();
            MainSceneUIManager.Instance.GetView<MainView>().Show();
            MainSceneUIManager.Instance.GetView<BottombarView>().Show();
        }

        public override void OnRetry()
        {
            isGameComplate = true;
            AnalyticsManager.Instance.LogProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Fail);
            levelRunTime = 0;
            base.OnRetry();
            AnalyticsManager.Instance.LogGAEvent("LevelStart : RetryManual : " + eventNameTag + LevelNo);
            AnalyticsManager.Instance.LogGAEvent("LevelTime : Retry : " + eventNameTag + LevelNo + " : " + levelRunTime);
            var currency = DataManager.Instance.GetCurrency(CurrencyConstant.ENERGY);

            var playerData = DataManager.PlayerData;
            playerData.OnLevelFail();
            DataManager.Instance.SaveData(playerData);

            currency.Add(-1);
            AdManager.Instance.OnLevelPlayed();
            AdManager.Instance.ShowInterstitial(InterstatialAdPlaceType.Reload_Level, "GameRestartInterstitial");
            GameplayManager.testPrintData.OnLevelComplate(LevelComplateState.Fail);
            LevelProgressData levelProgressData = DataManager.LevelProgressData;
            levelProgressData.ReSetData();
            levelProgressData.levelNo = currentLevel.Level;
            DataManager.Instance.SaveLevelProgressData(levelProgressData);
            AnalyticsManager.Instance.LogProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Start);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        internal void HandleViews()
        {
            MainSceneUIManager.Instance.GetView<MainView>().Hide();
            MainSceneUIManager.Instance.GetView<BottombarView>().Hide();
            MainSceneUIManager.Instance.GetView<LevelRestartView>().Hide();
            MainSceneUIManager.Instance.GetView<LevelFailView>().Hide();
            MainSceneUIManager.Instance.GetView<GameplayTopbarView>().Show();
            MainSceneUIManager.Instance.GetView<GameplayBottomView>().Show();
        }

        internal void SetlevelTheam()
        {
            foreach (var item in levelTheamGO)
            {
                item.Value.SetActive(false);
            }
            if (levelTheamGO.ContainsKey(currentLevel.LevelType))
            {
                levelTheamGO[currentLevel.LevelType].SetActive(true);
            }
        }

        protected virtual void SetWinReward()
        {
            levelReward.Clear();
            levelReward = new List<BaseReward>();
            BaseReward commonReward = null;
            if (commonRewards.ContainsKey(currentLevel.LevelType))
                commonReward = commonRewards[currentLevel.LevelType];

            if (commonReward != null)
                levelReward.Add(commonReward);

            for (int i = 0; i < currentLevel.LevelGoals.Count; i++)
            {
                BaseReward baseReward = currentLevel.LevelGoals[i].GetWinReward();
                if (baseReward != null)
                {
                    BaseReward reward = levelReward.Find(x => x.GetCurrencyId() == baseReward.GetCurrencyId());
                    if (reward != null)
                        reward.AddRewardValue(baseReward.GetAmount());
                    else
                        levelReward.Add(baseReward);
                }
            }

            for (int i = 0; i < levelReward.Count; i++)
            {
                levelReward[i].GiveReward();
                if (levelReward[i].GetCurrencyId() == CurrencyConstant.COINS)
                {
                    AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, AnalyticsConstants.CoinCurrency
                        , levelReward[i].GetAmount(), AnalyticsConstants.ItemType_Reward, AnalyticsConstants.ItemId_LevelWin);
                }
            }
        }

        internal void Log_LevelStartEvent(LevelProgressData levelProgressData)
        {
            if (currentLevel.Level != levelProgressData.levelNo)
            {
                AnalyticsManager.Instance.LogProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Start);
                if (isLogAdjustEvent)
                    PuzzleController.GetInstance().OnLevelStart(currentLevel.Level);
                AnalyticsManager.Instance.LogGAEvent("LevelStart : NewLevel : " + eventNameTag + LevelNo);
            }
        }

        internal void Log_LevelComplateEvent(LevelProgressData levelProgressData)
        {
            AnalyticsManager.Instance.LogGAEvent("LevelWin : " + eventNameTag + LevelNo);
            if (isLogAdjustEvent)
                PuzzleController.GetInstance().OnLevelComplete(levelProgressData.levelNo, levelProgressData.currentRunningTime);
        }

        internal void SetGoal()
        {
            GameplayGoalHandler.Instance.OnLevelStart(currentLevel);
            LevelManager.Instance.StartLevel(currentLevel);

            GoalStripUIManager.Instance.ShowGoalStripView(currentLevel.LevelType, () =>
            {
                ShowElementUnlockView();
            });
        }

        private void ShowElementUnlockView()
        {
            ElementIntroData elementIntroData = ResourceManager.Instance.ElementIntroDataSO.CanShowElementIntro(CurrentLevel.Level);
            if (elementIntroData != null)
            {
                MainSceneUIManager.Instance.GetView<ElementIntroView>().ShowView(elementIntroData, ShowBoosterUnlockView);
            }
            else
            {
                ShowBoosterUnlockView();
            }
        }

        private void ShowBoosterUnlockView()
        {
            BoosterData boosterData = ResourceManager.Instance.BoosterData.CanShowBoosterUnlock(CurrentLevel.Level);
            if (boosterData != null)
            {
                MainSceneUIManager.Instance.GetView<BoosterUnlockView>().ShowView(boosterData);
            }
            TutorialManager.Instance.CheckForTutorialsToStart();
        }

        internal void ShowLevelWinView()
        {
            MainSceneUIManager.Instance.GetView<GameWinAnimationView>().ShowView(() =>
            {
                MainSceneUIManager.Instance.GetView<LevelWinView>().ShowView(levelReward);
            });
        }

        internal void Log_LevelFailEvent()
        {
            AnalyticsManager.Instance.LogGAEvent("LevelTime : Fail : " + eventNameTag + LevelNo + " : " + levelRunTime);
            string eventName = "";
            int remaingScore = 0;

            List<BaseLevelGoal> baseLevelGoals = currentLevel.LevelGoals;


            if (!baseLevelGoals[0].IsGoalFullFilled())
            {
                eventName += "Score ";
                remaingScore = baseLevelGoals[0].GoalCount - baseLevelGoals[0].CurrentCount;
            }

            for (int i = 1; i < baseLevelGoals.Count; i++)
            {
                if (baseLevelGoals[i].goalType == GoalType.Obsatcal && !baseLevelGoals[i].IsGoalFullFilled())
                {
                    eventName += "and Obstacle";
                    break;
                }
            }

            AnalyticsManager.Instance.LogGAEvent("LevelFail : " + eventNameTag + LevelNo + " : " + eventName + " : " + remaingScore);
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
