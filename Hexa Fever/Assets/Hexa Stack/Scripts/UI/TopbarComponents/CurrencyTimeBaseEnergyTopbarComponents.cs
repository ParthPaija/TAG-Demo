using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class CurrencyTimeBaseEnergyTopbarComponents : CurrencyTopbarComponents
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GameObject infiniteLifeGO;
        [SerializeField] private Text timerText;
        [SerializeField] private GameObject timerObject;
        private Coroutine coroutineEnergyAnimation;
        private int defaultValue;
        private CurrencyTimeBase currencyTimeBaseEnergy;

        #endregion

        #region UNITY_CALLBACKS

        public override void OnEnable()
        {
            currencyTimeBaseEnergy = DataManager.Instance.GetCurrency(currencyId).GetType<CurrencyTimeBase>();
            defaultValue = currencyTimeBaseEnergy.defaultValue;

            base.OnEnable();
        }

        public override void Start()
        {
            base.Start();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void RegisterCurrencyEvent()
        {
            base.RegisterCurrencyEvent();
            if (currencyTimeBaseEnergy != null)
            {
                currencyTimeBaseEnergy.RegisterOnCurrencyEarnedEvent(SetEarnEnergyValue);
                currencyTimeBaseEnergy.RegisterTimerTick(OnEnergyTimeUpdate);
                currencyTimeBaseEnergy.RegisterOnCurrencyUpdateByTimer(OnEnergyUpdateByTimer);
                currencyTimeBaseEnergy.RegisterTimerStartOrStop(OnEnergyTimerStart);
                currencyTimeBaseEnergy.RegisterInfiniteTimerStartOrStop(OnInfiniteEnergyTimerStart);
                currencyTimeBaseEnergy.RegisterInfiniteEnergyTimerTick(OnInfiniteEnergyTimeUpdate);
            }
            currencyAnimation?.RegisterObjectAnimationComplete(SetEnergyAmount);
        }

        public override void DeRegisterCurrencyEvent()
        {
            base.DeRegisterCurrencyEvent();
            if (currencyTimeBaseEnergy != null)
            {
                currencyTimeBaseEnergy.RemoveOnCurrencyEarnedEvent(SetEarnEnergyValue);
                currencyTimeBaseEnergy.RemoveTimerTick(OnEnergyTimeUpdate);
                currencyTimeBaseEnergy.RemoveOnCurrencyUpdateByTimer(OnEnergyUpdateByTimer);
                currencyTimeBaseEnergy.RemoveTimerStartOrStop(OnEnergyTimerStart);
                currencyTimeBaseEnergy.RemoveInfiniteTimerStartOrStop(OnEnergyTimerStart);
                currencyTimeBaseEnergy.RemoveInfiniteEnergyTimerTick(OnInfiniteEnergyTimeUpdate);
            }
            currencyAnimation?.DeregisterObjectAnimationComplete(SetEnergyAmount);
        }

        public override void SetCurrencyValue(int value)
        {
            if (value >= defaultValue)
            {
                //currencyText.text = "<color=green>" + value + "</color>/" + defaultValue;
                currencyText.text = value.ToString();
                timerText.text = "Full";
                return;
            }
            currencyText.text = value.ToString();
        }

        public override void SetDeductedCurrencyValue(int value, Vector3 position)
        {
            if (coroutineEnergyAnimation != null)
                StopCoroutine(coroutineEnergyAnimation);
            coroutineEnergyAnimation = StartCoroutine(DoAnimateTopBarEnergyValueChange(0.65f, currencyVaue, currencyVaue - value));
            currencyVaue -= value;
        }

        public override void OnTryToBuyThisCurrency()
        {
            if (!currencyTimeBaseEnergy.IsFull())
                MainSceneUIManager.Instance.GetView<NotEnoughLifeView>().ShowView(null, null);
            //else
            //GlobalUIManager.Instance.GetView<ToastMessageView>().ShowMessage($"Max {DataManager.Instance.GetCurrency(CurrencyConstant.ENERGY).currencyName}");
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetEnergyAmount(int energyValue, bool isLastObject)
        {
            currencyVaue += energyValue;

            if (isLastObject)
            {
                SetCurrencyValue(currencyTimeBaseEnergy.Value);
                return;
            }
            SetCurrencyValue(currencyTimeBaseEnergy.Value);
        }

        private void SetEarnEnergyValue(int value, Vector3 position)
        {
            if (coroutineEnergyAnimation != null)
                StopCoroutine(coroutineEnergyAnimation);
            coroutineEnergyAnimation = StartCoroutine(DoAnimateTopBarEnergyValueChange(0.65f, currencyVaue, currencyVaue + value));
            currencyVaue += value;
        }

        private void OnEnergyTimeUpdate(TimeSpan timeSpan)
        {
            if (!timerObject.activeSelf)
                timerObject.SetActive(true);
            timerText.text = timeSpan.FormateTimeSpanForDaysInTwoDigit();
        }

        private void OnInfiniteEnergyTimeUpdate(TimeSpan timeSpan)
        {
            if (!infiniteLifeGO.activeSelf)
                infiniteLifeGO.SetActive(true);
            timerText.text = timeSpan.FormateTimeSpanForDaysInTwoDigit();
        }

        private void OnEnergyUpdateByTimer(int value)
        {
            currencyVaue = currencyTimeBaseEnergy.Value;
            SetCurrencyValue(currencyVaue);
        }

        private void OnEnergyTimerStart(bool isStart, bool needToUpdate)
        {
            timerObject.SetActive(isStart);
            if (!isStart)
            {
                //currencyVaue = currencyTimeBaseEnergy.Value;
                SetCurrencyValue(currencyVaue);
            }
        }

        private void OnInfiniteEnergyTimerStart(bool isStart, bool needToUpdate)
        {
            infiniteLifeGO.SetActive(isStart);
            if (!isStart)
            {
                currencyVaue = currencyTimeBaseEnergy.Value;
                SetCurrencyValue(currencyVaue);
            }
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator DoAnimateTopBarEnergyValueChange(float time, int startValue, int targetValue)
        {
            float i = 0;
            float rate = 1 / time;
            int tempValue = 0;
            while (i < 1)
            {
                i += Time.deltaTime * rate;
                tempValue = (int)Mathf.Lerp(startValue, targetValue, i);
                SetCurrencyValue(tempValue);
                yield return null;
            }
            SetCurrencyValue(targetValue);
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
