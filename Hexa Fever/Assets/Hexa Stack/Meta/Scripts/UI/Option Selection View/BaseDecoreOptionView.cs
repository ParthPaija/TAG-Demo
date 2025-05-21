using System;
using System.Collections;
using Tag.CommonPurchaseSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.MetaGame
{
    public abstract class BaseDecoreOptionView : MonoBehaviour
    {
        #region PUBLIC_VARS

        public abstract PurchaseType PrefabType { get; }
        public int Index { get; private set; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Image _optionImage;
        [SerializeField] private GameObject _selectEffectGO;
        [SerializeField] private OptionData _optionData;
        protected Action<int> _onSelectAction;

        [SerializeField] private AnimationCurve _scaleCurve;
        private Vector3 _defaultScale;
        private Vector3 _endScale;

        private Coroutine _scaleUpCoroutine;
        private Coroutine _scaleDownCoroutine;
        //private SoundHandler SoundHandler { get { return CustomBehaviour.FindObjFromLibrary<SoundHandler>(); } }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS
        private void Awake()
        {
            _defaultScale = transform.localScale;
            _endScale = transform.localScale + new Vector3(0.1f, 0.1f, 0.1f);
        }

        public virtual void SetView(int index, OptionData optionData, Action<int> onSelectAction)
        {
            Index = index;
            _optionData = optionData;
            _onSelectAction = onSelectAction;
            _optionImage.sprite = _optionData.thumbnail;
        }

        public void SelectOption()
        {
            if (_scaleUpCoroutine == null)
            {
                _scaleUpCoroutine = StartCoroutine(ScaleUpAnimation());
            }
            _selectEffectGO.SetActive(true);
            _scaleDownCoroutine = null;
        }

        public void DeselectOption()
        {
            if (_scaleDownCoroutine == null)
            {
                _scaleDownCoroutine = StartCoroutine(ScaleDownAnimation());
            }
            _selectEffectGO.SetActive(false);
            _scaleUpCoroutine = null;
        }

        public abstract void BuyOption(Action onSuccessAction);

        #endregion

        #region PRIVATE_FUNCTIONS

        private IEnumerator ScaleUpAnimation()
        {
            float animationSpeed = 0.2f;
            float i = 0;
            float rate = 1 / animationSpeed;
            
            while (i < 1)
            {
                i += rate * Time.deltaTime;
                transform.localScale = Vector3.LerpUnclamped(_defaultScale, _endScale, _scaleCurve.Evaluate(i));
                yield return null;
            }
        }

        private IEnumerator ScaleDownAnimation()
        {
            float animationSpeed = 0.2f;
            float i = 0;
            float rate = 1 / animationSpeed;
            while (i < 1)
            {
                i += rate * Time.deltaTime;
                transform.localScale = Vector3.LerpUnclamped(_endScale, _defaultScale, _scaleCurve.Evaluate(i));
                yield return null;
            }
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnSelectView()
        {
            //SoundHandler.PlaySound(SoundType.ButtonClick);
            _onSelectAction.Invoke(Index);
        }

        #endregion
    }
}
