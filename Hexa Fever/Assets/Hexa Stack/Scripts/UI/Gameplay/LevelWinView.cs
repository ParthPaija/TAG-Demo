using I2.Loc;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class LevelWinView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GameFlowConfigDataSO gameFlowConfigDataSO;
        public Localize levelTextLocalize;
        public LocalizationParamsManager levelNoLocalizedParam;

        [SerializeField] private Button doubleCoinButton;
        [SerializeField] private List<BaseRewadUI> rewadUI = new List<BaseRewadUI>();
        [SerializeField] List<BaseReward> baseRewards;
        [SerializeField] CurrencyTopbarComponents coinTopbar;
        [SerializeField] CurrencyTopbarComponents metaCurrencyTopbar;
        [SerializeField] private float waitCurrencyAnimation = 0.5f;
        private bool isCurrenctAnimationShowed = false;
        [SerializeField] private Button[] buttons;

        [SerializeField] private RectTransform coinStack;
        [SerializeField] private RectTransform objectToAnimate;
        [SerializeField] private float animationTime = 0.6f;
        [SerializeField] private AnimationCurve positionCurve;
        [SerializeField] private AnimationCurve scaleCurve;
        [SerializeField] private Vector3 positionOffset;

        private int animationObjectCount;

        private bool isMetaVisiteEnableOnNext
        {
            get
            {
                if(ResourceManager.Instance.CurrentTestingType == LevelTestingType.TestLevel3)
                    return true;
                return gameFlowConfigDataSO.GetValue<GameFlowConfigData>().isMetaVisiteEnableOnNext && DataManager.PlayerData.playerGameplayLevel >= gameFlowConfigDataSO.GetValue<GameFlowConfigData>().unlockAt;
            }
        }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowView(List<BaseReward> baseRewards)
        {
            isCurrenctAnimationShowed = false;
            SetButtonInteractableState(true);
            this.baseRewards = baseRewards;
            base.Show();

            levelTextLocalize.SetTerm(GameplayManager.Instance.CurrentHandler.eventNameTag);
            levelNoLocalizedParam.SetParameterValue(levelNoLocalizedParam._Params[0].Name, (GameplayManager.Instance.LevelNo - 1).ToString());

            for (int i = 0; i < rewadUI.Count; i++)
            {
                rewadUI[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < rewadUI.Count && i < this.baseRewards.Count; i++)
            {
                rewadUI[i].SetReward(this.baseRewards[i]);
            }
            doubleCoinButton.gameObject.SetActive(GameplayManager.Instance.CurrentLevel.LevelType != LevelType.Bonus);
        }

        public override void OnViewShowDone()
        {
            base.OnViewShowDone();
            for (int i = 0; i < this.baseRewards.Count; i++)
            {
                if (this.baseRewards[i].GetCurrencyId() == 1)
                    coinTopbar.SetCurrencyValue(DataManager.Instance.GetCurrency(CurrencyConstant.COINS).Value - this.baseRewards[i].GetAmount());
                else
                    metaCurrencyTopbar.SetCurrencyValue(DataManager.Instance.GetCurrency(CurrencyConstant.META_CURRENCY).Value - this.baseRewards[i].GetAmount());
            }
        }

        public override void OnBackButtonPressed()
        {
            if (isCurrenctAnimationShowed)
                return;
            OnClose();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        [Button]
        public void PlayCoinDoubleAnimation(Action onComplate)
        {
            int coinAmount = 0;
            for (int i = 0; i < baseRewards.Count; i++)
            {
                if (baseRewards[i].GetCurrencyId() == 1)
                    coinAmount = baseRewards[i].GetAmount() / 2;
            }
            animationObjectCount = Mathf.Clamp(coinAmount, 1, 5);
            StartCoroutine(CoinAddAnimation(onComplate));
        }

        private void OnNextAction()
        {
            var currency = DataManager.Instance.GetCurrency(CurrencyConstant.ENERGY);
            if (currency.Value < 1)
            {
                MainSceneUIManager.Instance.GetView<NotEnoughLifeView>().ShowView(() =>
                {
                    AdManager.Instance.ShowInterstitial(InterstatialAdPlaceType.Game_Win_Screen, "GameWinInterstitial");
                    Hide();
                    if (GameplayManager.Instance.CurrentHandler.gameplayHandlerType == GameplayHandlerType.Main && VIPLeagueManager.Instance.IsVIPLeageUnlocked())
                    {
                        ForcefullyCloseAction();
                    }
                    else if (GameplayManager.Instance.CurrentHandler.gameplayHandlerType == GameplayHandlerType.VIPLeague && !VIPLeaderboardManager.Instance.IsCurrentLeaderboardEventActive())
                    {
                        ForcefullyCloseAction();
                    }
                    else
                    {
                        GameplayManager.Instance.OnCurrentLevelRestartOrNext();
                    }
                }, () =>
                {
                    ForcefullyCloseAction();
                });
                return;
            }
            AdManager.Instance.ShowInterstitial(InterstatialAdPlaceType.Game_Win_Screen, "GameWinInterstitial");
            Hide();
            if (GameplayManager.Instance.CurrentHandler.gameplayHandlerType == GameplayHandlerType.Main && VIPLeagueManager.Instance.IsVIPLeageUnlocked())
            {
                ForcefullyCloseAction();
            }
            else if (GameplayManager.Instance.CurrentHandler.gameplayHandlerType == GameplayHandlerType.VIPLeague && !VIPLeaderboardManager.Instance.IsCurrentLeaderboardEventActive())
            {
                ForcefullyCloseAction();
            }
            else
            {
                GameplayManager.Instance.OnCurrentLevelRestartOrNext();
            }
        }

        private void ForcefullyCloseAction()
        {
            GlobalUIManager.Instance.GetView<InGameLoadingView>().ShowView(1f, () =>
            {
            });
            LevelManager.Instance.UnloadLevel();
            MainSceneUIManager.Instance.GetView<MainView>().Show();
            MainSceneUIManager.Instance.GetView<BottombarView>().Show();
            SetButtonInteractableState(true);
        }

        private void PlayCurrencyCollectAnimation()
        {
            isCurrenctAnimationShowed = true;
            for (int i = 0; i < baseRewards.Count; i++)
            {
                if (baseRewards[i].GetCurrencyId() == 1)
                    baseRewards[i].ShowRewardAnimation(coinTopbar.CurrencyAnimation, rewadUI[i].RewardTransform.position, true);
                else
                    baseRewards[i].ShowRewardAnimation(metaCurrencyTopbar.CurrencyAnimation, rewadUI[i].RewardTransform.position, true);
            }
        }

        private void SetButtonInteractableState(bool state)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = state;
            }
        }

        private void OnDoubleCoinAddSucess()
        {
            doubleCoinButton.interactable = false;
            BaseReward coinReward = null;
            for (int i = 0; i < baseRewards.Count; i++)
            {
                if (baseRewards[i].GetCurrencyId() == CurrencyConstant.COINS)
                {
                    coinReward = baseRewards[i];
                    baseRewards[i] = baseRewards[i].MultiplyReward(2);
                    break;
                }
            }
            if (coinReward != null)
            {
                int coinAmount = coinReward.GetAmount();
                DataManager.Instance.GetCurrency(CurrencyConstant.COINS).Add(coinAmount);

                AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, AnalyticsConstants.CoinCurrency
                        , coinAmount, AnalyticsConstants.ItemType_AdReward, AnalyticsConstants.ItemId_LevelWinExtraCoins);

                AnalyticsManager.Instance.LogGAEvent("AdExtraCoins : " + GameplayManager.Instance.CurrentHandler.GetCurrentLevelEventString());
            }
            PlayCoinDoubleAnimation(() =>
            {
                doubleCoinButton.gameObject.SetActive(false);
                doubleCoinButton.interactable = false;
            });
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator CoinAddAnimation(Action action)
        {
            for (int i = 0; i < animationObjectCount; i++)
            {
                RectTransform obj = Instantiate(objectToAnimate, this.transform);
                StartCoroutine(DoCoinAddAnimation(obj, i == (animationObjectCount - 1)));
                yield return new WaitForSeconds(0.15f);
            }
            action?.Invoke();
        }

        private IEnumerator DoCoinAddAnimation(RectTransform obj, bool isLastObject)
        {
            float i = 0;
            float rate = 1 / animationTime;

            Vector3 startPos = coinStack.localPosition + positionOffset;
            Vector3 endPos = coinStack.localPosition - (positionOffset / 2);

            obj.gameObject.SetActive(true);
            while (i < 1)
            {
                i += Time.deltaTime * rate;

                obj.localPosition = Vector3.LerpUnclamped(startPos, endPos, positionCurve.Evaluate(i));

                coinStack.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, scaleCurve.Evaluate(i));
                yield return null;
            }
            Destroy(obj.gameObject);

            for (int j = 0; j < baseRewards.Count; j++)
            {
                if (baseRewards[j].GetCurrencyId() == 1)
                    if (!isLastObject)
                        rewadUI[j].IncreseRewardCount((baseRewards[j].GetAmount() / 2) / animationObjectCount);
                    else
                        rewadUI[j].SetRewardCount(baseRewards[j].GetAmount());
            }
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnNext()
        {
            //if (isCurrenctAnimationShowed)
            //{
            //    if (isCurrenctAnimationShowed) 
            //    {
            //        OnClose();
            //        return;
            //    }
            //    OnNextAction();
            //    return;
            //}

            if (isMetaVisiteEnableOnNext)
            {
                OnClose();
                return;
            }

            SetButtonInteractableState(false);
            PlayCurrencyCollectAnimation();
            CoroutineRunner.Instance.Wait(waitCurrencyAnimation, () =>
            {
                OnNextAction();
                SetButtonInteractableState(true);
            });
        }

        public void OnDoubleCoinButton()
        {
            AdManager.Instance.ShowRewardedAd(() =>
            {
                OnDoubleCoinAddSucess();
            }, RewardAdShowCallType.DoubleCoin, "DoubleCoin");
        }

        public void OnClose()
        {
            SetButtonInteractableState(false);
            PlayCurrencyCollectAnimation();
            CoroutineRunner.Instance.Wait(waitCurrencyAnimation, () =>
            {
                Hide();
                AdManager.Instance.ShowInterstitial(InterstatialAdPlaceType.Game_Win_Screen, "GameWinInterstitial");
                GlobalUIManager.Instance.GetView<InGameLoadingView>().ShowView(1f, () =>
                {
                });
                LevelManager.Instance.UnloadLevel();
                MainSceneUIManager.Instance.GetView<MainView>().Show();
                MainSceneUIManager.Instance.GetView<BottombarView>().Show();
                SetButtonInteractableState(true);
            });
        }

        #endregion
    }
}
