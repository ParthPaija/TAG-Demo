using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "DailyAdPassData", menuName = Constant.GAME_NAME + "/DailyAdPasa/DailyAdPassData")]
    public class DailyAdPassDataSO : SerializedScriptableObject
    {
        #region PUBLIC_VARS

        public bool isActive;
        public int openAt;
        public int refreshFreeRewardResetTimeInSecond;
        public int allRewardResetTimeInhours;
        public DailyAdPassRewardData freeReward = new DailyAdPassRewardData();
        public List<DailyAdPassRewardData> adReward = new List<DailyAdPassRewardData>();

        #endregion

        #region PRIVATE_VARS

        private static JsonSerializerSettings settings;
        public static JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                if (settings == null)
                    settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        TypeNameHandling = TypeNameHandling.Auto,
                    };
                return settings;
            }
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

        [Button]
        public string Temp()
        {
            return JsonConvert.SerializeObject(adReward, JsonSerializerSettings);
        }

        [Button]
        public void Temp2(string json)
        {
            adReward = JsonConvert.DeserializeObject<List<DailyAdPassRewardData>>(json, JsonSerializerSettings);
        }

        #endregion
    }

    public class DailyAdPassRewardData
    {
        [JsonIgnore] public Sprite rewardSprite;
        public BaseReward baseReward;
    }
}
