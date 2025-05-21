using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "PlayerProfileData", menuName = Constant.GAME_NAME + "/PlayerProfile/PlayerProfileData")]
    public class PlayerProfileDataSO : SerializedScriptableObject
    {
        #region PUBLIC_VARS

        public List<PlayerProfileAvtarData> playerProfileAvtarDatas;
        public List<PlayerProfileFrameData> playerProfileFrameDatas;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public Sprite GetAvtarSprite(int id)
        {
            return playerProfileAvtarDatas.Find(x => x.id == id).sprite;
        }

        public Sprite GetFrameSprite(int id)
        {
            return playerProfileFrameDatas.Find(x => x.id == id).sprite;
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

    public class PlayerProfileAvtarData
    {
        [PlayerProfileAvtarId] public int id;
        public string name;
        public Sprite sprite;
    }

    public class PlayerProfileFrameData
    {
        [PlayerProfileFrameId] public int id;
        public string name;
        public Sprite sprite;
    }
}
