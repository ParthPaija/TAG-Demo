using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.CoreGame
{
    public class ScrollRectEnsureVisible : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private ScrollRect _sr;
        [SerializeField] private RectTransform _viewportTransform;
        [SerializeField] private RectTransform _content;
        [SerializeField] private bool Log;
        public AnimationCurve _animationCurve = new AnimationCurve();

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS
        public void Start()
        {
            if (_animationCurve == null || _animationCurve.length == 0)
            {
                // Create Keyframes
                Keyframe key0 = new Keyframe(0, 0); // Keyframe at (0, 0)
                Keyframe key1 = new Keyframe(1, 1); // Keyframe at (1, 1)

                // Add Keyframes to the Curve
                _animationCurve.AddKey(key0);
                _animationCurve.AddKey(key1);
            }


        }

        public void MoveToNormalizedPosition(Vector2 normalizedPosition, float duration = 0.5f, Action onScrollComplete = null)
        {
            StartCoroutine(DoScrollAnimation(_sr.normalizedPosition, normalizedPosition, duration, onScrollComplete));
        }

        public void CenterOnItem(RectTransform target, float duration = 0.5f, Action onScrollComplete = null)
        {
            if (Log) Debug.Log("Updating scrollrect for item: " + target);

            //this is the center point of the visible area
            var maskHalfSize = _viewportTransform.rect.size * 0.5f;
            var contentSize = _content.rect.size;
            //get object position inside content
            var targetRelativePosition =
                _content.InverseTransformPoint(target.position);
            //adjust for item size
            targetRelativePosition += new Vector3(target.rect.size.x, target.rect.size.y, 0f) * 0.25f;
            //get the normalized position inside content
            var normalizedPosition = new Vector2(
                Mathf.Clamp01(targetRelativePosition.x / (contentSize.x - maskHalfSize.x)),
                1f - Mathf.Clamp01(targetRelativePosition.y / -(contentSize.y - maskHalfSize.y))
                );
            //we want the position to be at the middle of the visible area
            //so get the normalized center offset based on the visible area width and height
            var normalizedOffsetPosition = new Vector2(maskHalfSize.x / contentSize.x, maskHalfSize.y / contentSize.y);
            //and apply it
            normalizedPosition.x -= (1f - normalizedPosition.x) * normalizedOffsetPosition.x;
            normalizedPosition.y += normalizedPosition.y * normalizedOffsetPosition.y;

            normalizedPosition.x = Mathf.Clamp01(normalizedPosition.x);
            normalizedPosition.y = Mathf.Clamp01(normalizedPosition.y);

            if (Log)
                Debug.Log(string.Format(
                    @"Target normalized position [{3}]
Mask half size [{0}]
Content size: [{1}]
Target relative position [{2}]",
                    maskHalfSize,
                    contentSize,
                    targetRelativePosition,
                    normalizedPosition
                    ));

            StartCoroutine(DoScrollAnimation(_sr.normalizedPosition, normalizedPosition, duration, onScrollComplete));
            //Go.to(_sr, AnimTime, new GoTweenConfig().vector2Prop("normalizedPosition", normalizedPosition));
        }

        public void CenterOnItemWithoutAnimation(RectTransform target)
        {
            if (Log) Debug.Log("Updating scrollrect for item: " + target);

            //this is the center point of the visible area
            var maskHalfSize = _viewportTransform.rect.size * 0.5f;
            var contentSize = _content.rect.size;
            //get object position inside content
            var targetRelativePosition =
                _content.InverseTransformPoint(target.position);
            //adjust for item size
            targetRelativePosition += new Vector3(target.rect.size.x, target.rect.size.y, 0f) * 0.25f;
            //get the normalized position inside content
            var normalizedPosition = new Vector2(
                Mathf.Clamp01(targetRelativePosition.x / (contentSize.x - maskHalfSize.x)),
                1f - Mathf.Clamp01(targetRelativePosition.y / -(contentSize.y - maskHalfSize.y))
                );
            //we want the position to be at the middle of the visible area
            //so get the normalized center offset based on the visible area width and height
            var normalizedOffsetPosition = new Vector2(maskHalfSize.x / contentSize.x, maskHalfSize.y / contentSize.y);
            //and apply it
            normalizedPosition.x -= (1f - normalizedPosition.x) * normalizedOffsetPosition.x;
            normalizedPosition.y += normalizedPosition.y * normalizedOffsetPosition.y;

            normalizedPosition.x = Mathf.Clamp01(normalizedPosition.x);
            normalizedPosition.y = Mathf.Clamp01(normalizedPosition.y);

            if (Log)
                Debug.Log(string.Format(
                    @"Target normalized position [{3}]
Mask half size [{0}]
Content size: [{1}]
Target relative position [{2}]",
                    maskHalfSize,
                    contentSize,
                    targetRelativePosition,
                    normalizedPosition
                    ));
            SetScrollValue(normalizedPosition);
            //Go.to(_sr, AnimTime, new GoTweenConfig().vector2Prop("normalizedPosition", normalizedPosition));
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        private IEnumerator DoScrollAnimation(Vector2 startValue, Vector2 endValue, float time, Action onScrollComplete)
        {
            float i = 0f;
            float rate = 1f / time;
            while (i < 1)
            {
                i += Time.deltaTime * rate;
                Vector2 tempPos = Vector2.LerpUnclamped(startValue, endValue, _animationCurve.Evaluate(i));
                _sr.normalizedPosition = tempPos;
                yield return 0;
            }
            onScrollComplete?.Invoke();
        }

        public void SetScrollValue(Vector2 value)
        {
            _sr.normalizedPosition = value;
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
