using System;
using System.Collections;
using UnityEngine;

namespace Tag.HexaStack
{
    public class RewardToastAnimation : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private float _speed;
        [SerializeField] private float _negativeSpeed;
        [SerializeField] private RewardToast _rewardToast;
        [SerializeField] private AnimationCurve _alphaCurve;
        [SerializeField] private AnimationCurve _movementCurve;
        private Action action;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void PlayAnimation(Sprite sprite, int quantity, Vector3 startPosition, Action action = null, Vector3 scale = default)
        {
            this.action = action;
            _rewardToast.SetRewardData(sprite, quantity);
            Vector3 endPosition = startPosition;
            endPosition.y += 1.5f;
            StartCoroutine(GenerateToast(startPosition, endPosition, scale));
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        private void OnAnimationComplete()
        {
            if (action != null)
            {
                action();
            }
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator GenerateToast(Vector3 startPosition, Vector3 endPosition, Vector3 scale)
        {
            RewardToast rewardToast = ObjectPool.Spawn(_rewardToast, transform);
            if (scale == Vector3.zero)
            {
                scale = Vector3.one;
            }
            rewardToast.rectTransform.position = startPosition;
            rewardToast.rectTransform.localScale = scale;
            rewardToast.gameObject.SetActive(true);
            StartCoroutine(RewardToastMoveAnimation(rewardToast, startPosition, endPosition));
            yield return null;
            yield return new WaitForSeconds(0.2f);
        }

        private IEnumerator RewardToastMoveAnimation(RewardToast rewardToast, Vector3 startPosition, Vector3 endPosition)
        {
            CanvasGroup canvasGroup = rewardToast.canvasGroup;
            RectTransform transform = rewardToast.rectTransform;
            float i = 0;
            while (i < 1)
            {
                i += Time.deltaTime * _speed;
                Vector3 newposition = Vector3.Lerp(startPosition, endPosition, _movementCurve.Evaluate(i));
                float alpha = _alphaCurve.Evaluate(i);
                canvasGroup.alpha = alpha;
                transform.position = newposition;
                yield return null;
            }
            rewardToast.Recycle();
            OnAnimationComplete();
        }
        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
