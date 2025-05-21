using Newtonsoft.Json;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "AdBreakRemoteConfig", menuName = Constant.GAME_NAME + "/Remote Config Data/AdBreakRemoteConfig")]
    public class AdBreakRemoteConfig : BaseConfig
    {
        #region PUBLIC_VARS

        public AdBreakRemoteData adBreakRemoteData;

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

        public override string GetDefaultString()
        {
            return JsonConvert.SerializeObject(adBreakRemoteData, JsonSerializerSettings);
        }

        public override T GetValue<T>()
        {
            if (!string.IsNullOrEmpty(remoteConfigValue))
                return JsonConvert.DeserializeObject<T>(remoteConfigValue,JsonSerializerSettings);
            return JsonConvert.DeserializeObject<T>(GetDefaultString(),JsonSerializerSettings);
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

    public class AdBreakRemoteData
    {
        public bool isActive;
        public int openAt;
        public float minGameMinimizedTime = 10f;
        public BaseReward rewards;
        public float adBreakCoolDownTime = 300f;
    }
}
