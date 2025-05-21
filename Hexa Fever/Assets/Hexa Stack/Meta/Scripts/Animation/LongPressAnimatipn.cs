using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.MetaGame.Animation
{
    public class LongPressAnimatipn : MonoBehaviour
    {
        #region PUBLIC_VARIABLES

        #endregion

        #region PRIVATE_VARIABLES

        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Image _redererOfFillImage;
        [SerializeField] private AnimationCurve _fillCurve;
        [SerializeField] private Vector3 _inputPositionOffset;

        [SerializeField] private float _waitTime;
        private Coroutine timerCo;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void StartArrowAnimation(Vector3 inputPos, Action endAction)
        {
            _targetTransform.position = inputPos + _inputPositionOffset;
            gameObject.SetActive(true);
            if (timerCo != null)
            {
                StopCoroutine(timerCo);
            }
            timerCo = StartCoroutine(TimerCoroutine(_waitTime, endAction));
        }

        public void StopArrowAnimation()
        {
            if (timerCo != null)
            {
                StopCoroutine(timerCo);
            }
            timerCo = null;
            gameObject.SetActive(false);
        }

        #endregion

        #region PRIVATE_FUNCTIONS
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region COROUTINES

        private IEnumerator TimerCoroutine(float time, Action endAction)
        {
            float i = 0;
            while (i <= 1f)
            {
                i += Time.deltaTime / time;
                _redererOfFillImage.fillAmount = Mathf.Lerp(0f, 1f, _fillCurve.Evaluate(i));
                yield return 0;
            }
            timerCo = null;
            gameObject.SetActive(false);
            endAction?.Invoke();
        }

        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}

