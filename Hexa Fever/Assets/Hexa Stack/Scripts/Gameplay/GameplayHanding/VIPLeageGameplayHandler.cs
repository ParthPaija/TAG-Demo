using System.Collections.Generic;

namespace Tag.HexaStack
{
    public class VIPLeageGameplayHandler : MainGameplayHandler
    {
        #region PUBLIC_VARS

        public override int LevelNo { get => PlayerPersistantData.GetVIPLeaderboardPlayerData().currentLevel; }

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void OnNextOrCurrentLevelRestart()
        {
            OnLevelStart(VIPLeagueManager.Instance.GetLevelData());
        }

        public override void OnLevelWin()
        {
            isGameComplate = true;
            AdManager.Instance.OnLevelPlayed();
            if (BoosterManager.Instance.IsAnyBoosterActive)
                BoosterManager.Instance.CurrentActiveBooster.OnUnUse();
            GameRuleManager.Instance.OnLevelComplate();

            var leaderBoardPlayerData = PlayerPersistantData.GetVIPLeaderboardPlayerData();
            leaderBoardPlayerData.currentLevel++;
            leaderBoardPlayerData.lastPlayedLevelIndex = -1;
            PlayerPersistantData.SetVIPLeaderboardPlayerData(leaderBoardPlayerData);

            VIPLeaderboardManager.Instance.GameplayManager_onGameplayLevelOver();

            var playerData = DataManager.PlayerData;
            playerData.OnLevelWin(true);
            DataManager.Instance.SaveData(playerData);

            SetWinReward();
            ShowLevelWinView();

            AnalyticsManager.Instance.LogProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Complete);
            AnalyticsManager.Instance.LogGAEvent("LevelTime : Win : " + eventNameTag + LevelNo + " : " + levelRunTime);
            LevelProgressData levelProgressData = DataManager.LevelProgressData;
            Log_LevelComplateEvent(levelProgressData);
            levelProgressData.OnLevelEnd();
            DataManager.Instance.SaveLevelProgressData(levelProgressData);
        }

        public override void OnLevelFail()
        {
            isGameComplate = true;
            AnalyticsManager.Instance.LogProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Fail);

            levelRunTime = 0;

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
            //AnalyticsManager.Instance.LogProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Start);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        protected override void SetWinReward()
        {
            levelReward.Clear();
            levelReward = new List<BaseReward>();
            BaseReward commonReward = null;

            if (commonRewards.ContainsKey(currentLevel.LevelType))
                commonReward = commonRewards[currentLevel.LevelType];

            if (commonReward != null)
                levelReward.Add(commonReward);

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

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        #endregion
    }
}
