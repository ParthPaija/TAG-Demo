using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class LeaderBoardUserPlayer : BaseLeaderBoardPlayer
    {
        #region PUBLIC_VARS
        #endregion

        #region PRIVATE_VARS
        #endregion

        #region KEY
        #endregion

        #region Propertices
        #endregion

        #region Overrided_Method
        public override int GetCurrentPoints()
        {
            return VIPLeaderboardManager.Instance.GetPlayerCurrentScore();
        }

        public override string GetPlayerName()
        {
            return PlayerPersistantData.GetPlayerProfileData().playerName;
        }

        public override Sprite GetPlayerAvtar()
        {
            int id = PlayerPersistantData.GetPlayerProfileData().avtarId;
            return MainSceneUIManager.Instance.GetView<PlayerProfileview>().PlayerProfileDataSO.GetAvtarSprite(id);
        }

        public override Sprite GetPlayerFrame()
        {
            int id = PlayerPersistantData.GetPlayerProfileData().frameId;
            return MainSceneUIManager.Instance.GetView<PlayerProfileview>().PlayerProfileDataSO.GetFrameSprite(id);
        }

        #endregion

        #region UNITY_CALLBACKS
        #endregion

        #region PUBLIC_FUNCTIONS
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