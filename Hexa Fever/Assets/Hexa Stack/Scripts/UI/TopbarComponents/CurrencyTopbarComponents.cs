using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class CurrencyTopbarComponents : BaseTopbarComponents
    {
        #region PUBLIC_VARS
        public int CurrencyId { get => currencyId; }
        public CurrencyAnimation CurrencyAnimation { get => currencyAnimation; }

        public Transform EndPos
        {
            get { return endTransform; }
        }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private bool isCurrencyValueChangeOnEvent = false;
        [SerializeField, CurrencyId] protected int currencyId;
        [SerializeField] protected Text currencyText;
        [SerializeField] protected Transform endTransform;
        [SerializeField] protected CurrencyAnimation currencyAnimation;
        [SerializeField] protected int currencyVaue;

        #endregion

        #region UNITY_CALLBACKS

        public override void Start()
        {
            base.Start();
            RegisterCurrencyEvent();
            currencyVaue = DataManager.Instance.GetCurrency(currencyId).Value;
            SetCurrencyValue(currencyVaue);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            RegisterCurrencyEvent();
            currencyVaue = DataManager.Instance.GetCurrency(currencyId).Value;
            SetCurrencyValue(currencyVaue);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            DeRegisterCurrencyEvent();
        }

        public virtual void SetCurrencyValue(int value)
        {
            currencyVaue = value;
            currencyText.text = value.ToString();
        }

        public void SetCurrencyValue()
        {
            currencyText.text = DataManager.Instance.GetCurrency(currencyId).Value.ToString();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public virtual void RegisterCurrencyEvent()
        {
            DataManager.Instance.GetCurrency(currencyId).RegisterOnCurrencySpendEvent(SetDeductedCurrencyValue);
            if (isCurrencyValueChangeOnEvent)
                DataManager.Instance.GetCurrency(currencyId).RegisterOnCurrencyEarnedEvent(SetEarnCurrencyValue);
            //DataManager.Instance.GetCurrency(currencyId).RegisterOnCurrencyChangeEvent(SetCurrencyValue);
            currencyAnimation?.RegisterObjectAnimationComplete(SetCurrencyAmount);
        }

        public virtual void DeRegisterCurrencyEvent()
        {
            if (isCurrencyValueChangeOnEvent)
                DataManager.Instance.GetCurrency(currencyId).RemoveOnCurrencyEarnedEvent(SetEarnCurrencyValue);
            DataManager.Instance.GetCurrency(currencyId).RemoveOnCurrencySpendEvent(SetDeductedCurrencyValue);
            currencyAnimation?.DeregisterObjectAnimationComplete(SetCurrencyAmount);
        }

        public virtual void SetDeductedCurrencyValue(int value, Vector3 position)
        {
            StartCoroutine(DoAnimateTopBarValueChange(0.65f, currencyVaue, DataManager.Instance.GetCurrency(currencyId).Value, currencyText));
            currencyVaue -= value;
        }

        public virtual void SetEarnCurrencyValue(int value, Vector3 position)
        {
            SetCurrencyAmount(value,true);
        }

        #endregion

        #region PRIVATE_FUNCTIONS


        private void SetCurrencyAmount(int value, bool isLastObject)
        {
            currencyVaue += value;
            currencyVaue = Mathf.Clamp(currencyVaue, 0, DataManager.Instance.GetCurrency(currencyId).Value);
            currencyText.text = currencyVaue.ToString();
            if (isLastObject)
            {
                currencyVaue = DataManager.Instance.GetCurrency(currencyId).Value;
                currencyText.text = currencyVaue.ToString();
            }
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator DoAnimateTopBarValueChange(float time, int startValue, int targetValue, Text textTopBarComponent)
        {
            float i = 0;
            float rate = 1 / time;

            while (i < 1)
            {
                i += Time.deltaTime * rate;

                textTopBarComponent.text = "" + (int)Mathf.Lerp(startValue, targetValue, i);
                yield return null;
            }
            textTopBarComponent.text = "" + targetValue;
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public virtual void OnTryToBuyThisCurrency()
        {
            BottombarView bottombarView = MainSceneUIManager.Instance.GetView<BottombarView>();
            if (bottombarView.IsActive)
            {
                bottombarView.OnTapButton(BottomBarButtonsType.Shop);
                bottombarView.OnShopButtonClick();
            }
            else
            {
                MainSceneUIManager.Instance.GetView<ShopView>().Show();
            }
        }

        #endregion
    }
}
