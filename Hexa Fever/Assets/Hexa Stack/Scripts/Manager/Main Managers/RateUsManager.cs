using Google.Play.Review;
using System;
using System.Collections;
using System.Collections.Generic;
using Tag.MetaGame.TaskSystem;
using UnityEngine;

namespace Tag.HexaStack
{
    public class RateUsManager : SerializedManager<RateUsManager>
    {
        #region PUBLIC_VARS

        public static bool IsRated
        {
            get => RatedState;
            set => RatedState = value;
        }

        private static bool RatedState { get { return PlayerPrefs.GetInt(RatedStateKey, 0) == 1; } set { PlayerPrefs.SetInt(RatedStateKey, value ? 1 : 0); } }
        private const string RatedStateKey = "GameRateUsPlayerPref";

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private List<RateUsConditionsData> conditions;
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;
        private Coroutine launchReviewCO;

        #endregion

        #region UNITY_CALLBACKS

        private void Start()
        {
            _reviewManager = new ReviewManager();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowInAppReview()
        {
            if (launchReviewCO == null)
            {
                StartCoroutine(LaunchInAppReviewInfo());
            }
        }

        public void OpenRateUsView(Action OnDone)
        {
            if (CanShowRateUsPopup())
            {
                MainSceneUIManager.Instance.GetView<RateUsView>().ShowWithHideAction(OnDone);
            }
            else
            {
                OnDone?.Invoke();
            }
        }

        public bool CanShowRateUsPopup()
        {
            if (IsRated)
                return false;

            for (int i = 0; i < conditions.Count; i++)
            {
                if (conditions[i].IsRated())
                    continue;

                for (int j = 0; j < conditions[i].baseRateUsConditions.Count; j++)
                {
                    if (conditions[i].baseRateUsConditions[j].CanOpen())
                    {
                        conditions[i].MarkAsRated();
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private IEnumerator LaunchInAppReviewInfo()
        {
            var requestFlowOperation = _reviewManager.RequestReviewFlow();

            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Debug.LogError("In app review Request :- " + requestFlowOperation.Error.ToString());
                yield break;
            }
            _playReviewInfo = requestFlowOperation.GetResult();

            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object

            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Debug.LogError("In app review Launch :- " + launchFlowOperation.Error.ToString());
                yield break;
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

    public enum RatePopupType
    {
        Native,
        Custome
    }

    public class RateUsConditionsData
    {
        public string prefsId;
        public List<BaseRateUsConditions> baseRateUsConditions = new List<BaseRateUsConditions>();

        public bool IsRated()
        {
            return PlayerPrefs.GetInt(prefsId, 0) == 1;
        }

        public void MarkAsRated()
        {
            PlayerPrefs.SetInt(prefsId, 1);
        }
    }

    public abstract class BaseRateUsConditions
    {
        public RatePopupType popupType;

        public abstract bool CanOpen();
    }

    public class LevelRateUsConditions : BaseRateUsConditions
    {
        public int level;

        public override bool CanOpen()
        {
            return PlayerPersistantData.GetMainPlayerProgressData().playerGameplayLevel >= level;
        }
    }

    public class AreaAndTaskRateUsConditions : BaseRateUsConditions
    {
        public string areaId;
        public int completedTaskCount;

        public override bool CanOpen()
        {
            AreaPlayerData areaPlayerData = TaskManager.Instance.GameplayerData;
            return areaPlayerData.areaid == areaId && areaPlayerData.completedTaskCount == completedTaskCount;
        }
    }
}
