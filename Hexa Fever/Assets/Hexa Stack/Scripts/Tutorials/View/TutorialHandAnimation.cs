using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tag.HexaStack
{
    public class TutorialHandAnimation : MonoBehaviour
    {
        #region PUBLIC_VARS

        public Transform startPosition;
        public Transform endPosition;
        public SortingGroup SortingGroup
        {
            get => _handSortingGroup;
            set
            {
                _handSortingGroup.sortingLayerID = value.sortingLayerID;
                _handSortingGroup.sortingOrder = value.sortingOrder;
            }
        }

        public Transform HandTransform { get { return handTransform; } }
        public RectTransform HandRectTransform { get { return handTransform.GetComponent<RectTransform>(); } }
        #endregion

        #region PRIVATE_SERIALIZE_FIELD_VARS

        [SerializeField] private Transform handTransform;
        [SerializeField] private float speed;
        [SerializeField] private float handInOutSpeed;
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private SortingGroup _handSortingGroup;
        [SerializeField] private SpriteRenderer handSpriteRenderer;
        [SerializeField] private Sprite handUpSprite;
        [SerializeField] private Sprite handDownSprite;

        #endregion

        #region PRIVATE_VARS

        private Action nextAnimation;
        private Action currentAnimation;
        private Coroutine moveAnimation;
        private Sequence tapSequence;

        #endregion

        #region UNITY_CALLBACKS

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void OnTapAnimation(bool isAnimation = false)
        {
            ResetHand();
            HandIn();
            nextAnimation = HandTapAnimation;
            if (!isAnimation)
                handTransform.position = endPosition.position;
        }

        public void PlayHandMove(bool isLoop = false)
        {
            ResetHand();
            HandIn();
            nextAnimation = HandMoveAnimation;
            currentAnimation = isLoop ? () => { PlayHandMove(true); } : null;
        }

        #endregion

        #region HandInAnimation

        public void HandIn()
        {
            StartCoroutine(HandInAnimation());
        }

        private IEnumerator HandInAnimation()
        {
            float i = 0;
            handTransform.position = startPosition.position;
            while (i < 1)
            {
                i += Time.deltaTime * handInOutSpeed;
                Vector3 newScale = Vector3.Lerp(Vector3.one * 1.4f, Vector3.one, i);
                handTransform.localScale = newScale;
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            nextAnimation.Invoke();
        }

        #endregion

        #region HandOutAnimation

        public void HandOut()
        {
            StartCoroutine(HandOutAnimation());
        }

        private IEnumerator HandOutAnimation()
        {
            float i = 0;
            while (i < 1)
            {
                i += Time.deltaTime * handInOutSpeed;
                Vector3 newRotation = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 25), i);
                yield return null;
            }
            StopCoroutine(moveAnimation);
            currentAnimation?.Invoke();
        }

        #endregion

        #region TapAnimation

        public void HandTapAnimation()
        {
            // Kill any existing tap sequence
            if (tapSequence != null)
            {
                tapSequence.Kill();
                tapSequence = null;
            }
            
            // Create new sequence
            tapSequence = DOTween.Sequence().SetTarget(handTransform);
            Vector3 startPos = handTransform.position;
            
            // Set it to loop indefinitely (-1)
            tapSequence.SetLoops(-1);
            
            tapSequence.AppendCallback(() => handSpriteRenderer.sprite = handUpSprite);
            tapSequence.AppendInterval(0.3f);
            tapSequence.AppendCallback(() => handSpriteRenderer.sprite = handDownSprite);
            tapSequence.AppendInterval(0.3f);
        }

        private IEnumerator OnHandUp()
        {
            yield return new WaitForSeconds(0.5f);
            HandOut();
        }

        public void OnStopTapping()
        {
            HandOut();
        }

        public void ResetHand()
        {
            StopAllCoroutines();
            handTransform.localRotation = Quaternion.Euler(Vector3.zero);
            
            // Kill the tap sequence if it exists
            if (tapSequence != null)
            {
                tapSequence.Kill();
                tapSequence = null;
            }
            
            DOTween.Kill(handTransform.transform);
            
            if (handSpriteRenderer != null && handDownSprite != null)
            {
                handSpriteRenderer.sprite = handDownSprite;
            }
        }

        #endregion

        #region HandMoveAnimation

        public void HandMoveAnimation()
        {
            moveAnimation = StartCoroutine(StartHandMoveAnimation());
        }

        private IEnumerator StartHandMoveAnimation()
        {
            float i = 0;
            while (i < 1)
            {
                i += Time.deltaTime * speed;
                Vector3 newPosition = Vector3.Lerp(startPosition.position, endPosition.position, animationCurve.Evaluate(i));
                handTransform.position = newPosition;
                yield return null;
            }
            StartCoroutine(OnHandUp());
        }

        #endregion
    }
}