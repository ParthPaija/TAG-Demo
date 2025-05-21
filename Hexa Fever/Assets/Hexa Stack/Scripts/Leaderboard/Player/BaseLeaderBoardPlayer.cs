using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class BaseLeaderBoardPlayer
    {
        #region PUBLIC_VARS

        public LeaderboardPlayerType leaderboardPlayerType;

        #endregion

        #region PRIVATE_VARS

        [ShowInInspector, ReadOnly] protected string playerName;
        [ShowInInspector, ReadOnly] protected Sprite playerAvtar;
        [ShowInInspector, ReadOnly] protected Sprite playerFrame;
        [SerializeField] private LeaderBoardPlayerScoreInfoUIData leaderBoardPlayerScoreInfoUIData;

        #endregion

        #region KEY

        #endregion

        #region Propertices

        #endregion

        #region Overrided_Method

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public virtual void Init(string playerName, Sprite avtar, Sprite frame, LeaderboardPlayerType leaderboardPlayerType = LeaderboardPlayerType.BotPlayer)
        {
            this.playerName = playerName;
            playerAvtar = avtar;
            playerFrame = frame;
            this.leaderboardPlayerType = leaderboardPlayerType;

            InitializeLeaderboardPlayerScoreInfoUIData();
        }

        public virtual string GetPlayerName()
        {
            return playerName;
        }

        public virtual Sprite GetPlayerAvtar()
        {
            return playerAvtar;
        }

        public virtual Sprite GetPlayerFrame()
        {
            return playerFrame;
        }

        public virtual int GetCurrentPoints()
        {
            return 0;
        }
        public virtual bool IsUserPlayer()
        {
            return leaderboardPlayerType == LeaderboardPlayerType.UserPlayer;
        }
        public virtual LeaderBoardPlayerScoreInfoUIData GetLeaderboardPlayerScoreInfoUIData()
        {
            if (leaderBoardPlayerScoreInfoUIData == null)
                InitializeLeaderboardPlayerScoreInfoUIData();

            leaderBoardPlayerScoreInfoUIData.name = GetPlayerName();
            leaderBoardPlayerScoreInfoUIData.score = GetCurrentPoints();
            leaderBoardPlayerScoreInfoUIData.avtarSprite = GetPlayerAvtar();
            leaderBoardPlayerScoreInfoUIData.frameSprite = GetPlayerFrame();

            return leaderBoardPlayerScoreInfoUIData;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void InitializeLeaderboardPlayerScoreInfoUIData()
        {
            leaderBoardPlayerScoreInfoUIData = new LeaderBoardPlayerScoreInfoUIData();
            leaderBoardPlayerScoreInfoUIData.leaderboardPlayerType = leaderboardPlayerType;
            leaderBoardPlayerScoreInfoUIData.name = GetPlayerName();
            leaderBoardPlayerScoreInfoUIData.score = GetCurrentPoints();
            leaderBoardPlayerScoreInfoUIData.avtarSprite = GetPlayerAvtar();
            leaderBoardPlayerScoreInfoUIData.frameSprite = GetPlayerFrame();
        }

        #endregion

        #region CO-ROUTINES
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
    public enum LeaderboardPlayerType
    {
        UserPlayer,
        BotPlayer
    }
}