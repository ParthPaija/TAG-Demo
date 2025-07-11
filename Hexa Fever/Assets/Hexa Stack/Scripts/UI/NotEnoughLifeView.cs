using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class NotEnoughLifeView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private int oneLifeCoinAmount;
        [SerializeField, CurrencyId] private int currencyId;
        [SerializeField] Text timerText;
        [SerializeField] Text lifeCountText;
        [SerializeField] Text addLifeText;
        [SerializeField] Text addLifeCoinAmountText;

        private int defaultValue;
        private int currencyVaue;
        private int remaningCurrencyValue;
        private CurrencyTimeBase currencyTimeBaseEnergy;
        private Action positiveAction;
        private Action negativeAction;

        #endregion

        #region UNITY_CALLBACKS

        private void OnDisable()
        {
            if (currencyTimeBaseEnergy != null)
            {
                currencyTimeBaseEnergy.RegisterTimerTick(OnEnergyTimeUpdate);
                currencyTimeBaseEnergy.RegisterOnCurrencyUpdateByTimer(OnEnergyUpdateByTimer);
                currencyTimeBaseEnergy.RegisterTimerStartOrStop(OnEnergyTimerStart);
            }
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowView(Action positiveAction, Action negativeAction)
        {
            this.positiveAction = positiveAction;
            this.negativeAction = negativeAction;
            base.Show();
            currencyTimeBaseEnergy = DataManager.Instance.GetCurrency(currencyId).GetType<CurrencyTimeBase>();

            if (currencyTimeBaseEnergy != null)
            {
                currencyTimeBaseEnergy.RegisterTimerTick(OnEnergyTimeUpdate);
                currencyTimeBaseEnergy.RegisterOnCurrencyUpdateByTimer(OnEnergyUpdateByTimer);
                currencyTimeBaseEnergy.RegisterTimerStartOrStop(OnEnergyTimerStart);
            }
            SetView();
        }

        [Button]
        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);

        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetView()
        {
            defaultValue = currencyTimeBaseEnergy.defaultValue;
            currencyVaue = currencyTimeBaseEnergy.Value;
            remaningCurrencyValue = defaultValue - currencyVaue;

            lifeCountText.text = currencyVaue.ToString();
            addLifeText.text = $"+{remaningCurrencyValue}";
            addLifeCoinAmountText.text = $"{remaningCurrencyValue * oneLifeCoinAmount}";
        }

        private void OnEnergyTimeUpdate(TimeSpan timeSpan)
        {
            timerText.text = timeSpan.FormateTimeSpanForDaysInTwoDigit();
        }

        private void OnEnergyUpdateByTimer(int value)
        {
            currencyVaue = currencyTimeBaseEnergy.Value;
            SetView();
            if (currencyVaue >= defaultValue)
                Hide();
        }

        private void OnEnergyTimerStart(bool isStart, bool needToUpdate)
        {
            currencyVaue = currencyTimeBaseEnergy.Value;
            if (!isStart)
            {
                if (currencyVaue >= defaultValue)
                    Hide();
            }
        }

        public override void Hide()
        {
            base.Hide();
            if (currencyTimeBaseEnergy.Value > 0 || currencyTimeBaseEnergy.IsInfiniteEnergyActive)
            {
                positiveAction?.Invoke();
            }
            else
            {
                negativeAction?.Invoke();
            }
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnAdd()
        {
            AdManager.Instance.ShowRewardedAd(() =>
            {
                AnalyticsManager.Instance.LogGAEvent("AdLife : " + currencyTimeBaseEnergy.Value);
                currencyTimeBaseEnergy.Add(1);
                SetView();
            }, RewardAdShowCallType.AdLife, "AdLife");
        }

        public void OnCoin()
        {
            int coinAmount = remaningCurrencyValue * oneLifeCoinAmount;
            if (DataManager.Instance.GetCurrency(CurrencyConstant.COINS).Value >= coinAmount)
            {
                AnalyticsManager.Instance.LogResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Sink, AnalyticsConstants.CoinCurrency
                        , coinAmount, AnalyticsConstants.ItemType_Trade, AnalyticsConstants.ItemId_LifeRefill);

                DataManager.Instance.GetCurrency(CurrencyConstant.COINS).Add(-coinAmount);
                currencyTimeBaseEnergy.Add(remaningCurrencyValue);
                SetView();
                Hide();
            }
            else
            {
                MainSceneUIManager.Instance.GetView<ShopView>().Show();
            }
        }

        public void OnClose()
        {
            Hide();
        }

        #endregion
    }
}
