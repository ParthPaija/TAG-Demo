using Sirenix.OdinInspector;
using System;
using System.Collections;
using Tag.RewardSystem;
using UnityEngine;

namespace Tag.HexaStack
{
    public class AdBreakManager : SerializedManager<AdBreakManager>
    {
        #region PUBLIC_VARS

        public AdBreakRemoteConfig adBreakRemoteConfig;

        #endregion

        #region PRIVATE_VARS

        private AdBreakRemoteData adBreakRemoteData;
        private float _gameMinimizedTime = 0f;
        private bool _wasMinimized = false;
        private bool _canShowAdBreak = true;
        private float _timeSinceLastAdBreak = 0f;

        #endregion

        #region UNITY_CALLBACKS

        public override void Awake()
        {
            base.Awake();
            adBreakRemoteData = adBreakRemoteConfig.GetValue<AdBreakRemoteData>();
            adBreakRemoteData.rewards = adBreakRemoteConfig.adBreakRemoteData.rewards;
            _canShowAdBreak = false;
        }

#if UNITY_ANDROID 
        private void OnApplicationPause(bool pause)
        {
            if (!IsActive())
            {
                return;
            }

            if (pause)
            {
                Debug.LogError("Game Lost Focus");
                _wasMinimized = true;
                _gameMinimizedTime = Time.time;
            }
            else if (_wasMinimized)
            {
                Debug.LogError("Game Regained Focus");
                float minimizedDuration = Time.time - _gameMinimizedTime;
                Debug.LogError("Unfocused duration: " + minimizedDuration + " seconds" + _canShowAdBreak);

                if (minimizedDuration >= adBreakRemoteData.minGameMinimizedTime && _canShowAdBreak)
                {
                    Debug.LogError("Showing Ad Break after losing focus");
                    ShowAdBreak();
                }
                _wasMinimized = false;
            }
        }

#endif

#if UNITY_ANDROID

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!IsActive())
            {
                return;
            }

            if (!hasFocus)
            {
                Debug.LogError("Game Lost Focus");
                _wasMinimized = true;
                _gameMinimizedTime = Time.time;
            }
            else if (_wasMinimized)
            {
                Debug.LogError("Game Regained Focus");
                float minimizedDuration = Time.time - _gameMinimizedTime;
                Debug.LogError("Unfocused duration: " + minimizedDuration + " seconds" + _canShowAdBreak);

                if (minimizedDuration >= adBreakRemoteData.minGameMinimizedTime && _canShowAdBreak)
                {
                    Debug.LogError("Showing Ad Break after losing focus");
                    ShowAdBreak();
                }
                _wasMinimized = false;
            }
        }
#endif

        private void Update()
        {
            if (!IsActive())
            {
                return;
            }

            if (!_canShowAdBreak)
            {
                _timeSinceLastAdBreak += Time.deltaTime;
                if (_timeSinceLastAdBreak >= adBreakRemoteData.adBreakCoolDownTime)
                {
                    Debug.LogError("Ad Break Show True");
                    _canShowAdBreak = true;
                    _timeSinceLastAdBreak = 0f;
                }
            }
        }

#endregion

        #region PUBLIC_FUNCTIONS

        [Button]
        public void ShowAdBreak()
        {
            StartCoroutine(ShowAdBreakCoroutine());
        }

        public void GrantReward()
        {
            _canShowAdBreak = false;
            _timeSinceLastAdBreak = 0f;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private bool IsActive()
        {
            if (!adBreakRemoteData.isActive)
                return false;

            if (adBreakRemoteData.openAt > DataManager.PlayerData.playerGameplayLevel)
                return false;

            if(!MainSceneUIManager.Instance.GetView<GameplayTopbarView>().IsActive)
                return false;

            return true;
        }

        private void ShowInterstitialAd()
        {
            AdManager.Instance.ShowInterstitialWithoutConditions(InterstatialAdPlaceType.Ad_Break, "AdBreakInterstitial");
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator ShowAdBreakCoroutine()
        {
            MainSceneUIManager.Instance.GetView<AdBreakView>().ShowView(adBreakRemoteData);
            adBreakRemoteData.rewards.GiveReward();
            if (adBreakRemoteData.rewards.GetCurrencyId() == CurrencyConstant.COINS)
            {
                AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, AnalyticsConstants.CoinCurrency
                        , adBreakRemoteData.rewards.GetAmount(), AnalyticsConstants.ItemType_AdReward, AnalyticsConstants.ItemId_AdBreak);
            }
            yield return new WaitForSeconds(1f);
            ShowInterstitialAd();
            yield return new WaitForSeconds(0.5f);
            if (adBreakRemoteData.rewards.GetCurrencyId() == CurrencyConstant.COINS)
                MainSceneUIManager.Instance.GetView<AdBreakView>().PlayCurrencyCoinAnimation(adBreakRemoteData.rewards.GetAmount());
            GrantReward();
            MainSceneUIManager.Instance.GetView<AdBreakView>().Hide();
        }

        internal bool IsSystemActive()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        #endregion
    }
}
