using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.CoreGame
{
    public class ScaleWithCurve : MonoBehaviour
    {
        #region PUBLIC_VARS

        public GameObject SpriteObject;
        public float time;
        public float interval;
        public bool isLooping;
        public bool isPlayOnEnable;
        public bool canScaleZ = false;
        public float _delay = 0;

        [Space(2)][Header("Curves")] public AnimationCurve scaleCurveX;
        public AnimationCurve scaleCurveY;
        public AnimationCurve scaleCurveZ;

        #endregion

        #region PRIVATE_VARS

        private Vector3 scale;
        private Coroutine coroutine;

        #endregion

        #region UNITY_CALLBACKS

        private void Awake()
        {
            SetSprite();
            scale = SpriteObject.transform.localScale;
        }

        private void OnEnable()
        {
            if (isPlayOnEnable)
            {
                DOVirtual.DelayedCall(_delay, () => StartAnimation());
            }
        }

        private void Update()
        {
            /* if (Input.GetKeyDown(KeyCode.Space))
             {
                 isLooping = true;
                 StartAnimation();
             }*/

            /*  if (Input.GetKeyDown(KeyCode.S))
              {
                  isLooping = false;
                  //StopAnimation();
              }*/
        }
        private void OnDisable()
        {
            StopAnimation();
        }
        #endregion

        #region PUBLIC_FUNCTIONS
        public void SetInitialScale()
        {
            scale.x = scaleCurveX.Evaluate(0);
            scale.y = scaleCurveY.Evaluate(0);
            if (canScaleZ)
                scale.z = scaleCurveZ.Evaluate(0);
            SpriteObject.transform.localScale = scale;
        }
        public void StartAnimation(Action onCompleteAnimation = null)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            if (gameObject.activeInHierarchy && gameObject.activeSelf)
                coroutine = StartCoroutine(AnimateLerp(onCompleteAnimation));
        }

        // start animation if coroutine is null
        public void StartAnimationIfCoroutineIsNull(Action onCompleteAnimation = null)
        {
            if (coroutine == null && gameObject.activeInHierarchy && gameObject.activeSelf)
                coroutine = StartCoroutine(AnimateLerp(onCompleteAnimation));
        }
        public void StartReverseAnimation(Action onCompleteAnimation = null, float outSpeed = 1)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            if (gameObject.activeInHierarchy && gameObject.activeSelf)
                coroutine = StartCoroutine(AnimateLerpOut(outSpeed,onCompleteAnimation));
        }

        public void StopAnimation()
        {
            if (coroutine != null)
            { 
                StopCoroutine(coroutine); 
                coroutine = null;
            }
        }

        [ContextMenu("Set Sprite")]
        public void SetSprite()
        {
            if (SpriteObject == null)
                SpriteObject = gameObject;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        private IEnumerator AnimateLerp(Action onCompleteAnimation)
        {
            // yield return new WaitForSeconds(Random.Range(0,1f));

            float i = 0;
            float rate = 1 / time;
            while (i < 1)
            {
                i += rate * Time.deltaTime;

                scale.x = scaleCurveX.Evaluate(i);
                scale.y = scaleCurveY.Evaluate(i);
                if (canScaleZ)
                    scale.z = scaleCurveZ.Evaluate(i);
                SpriteObject.transform.localScale = scale;
                yield return 0;
            }

            yield return new WaitForSeconds(interval);

            if (isLooping)
                StartAnimation();
            else
            {
                StopAnimation();
                onCompleteAnimation?.Invoke();
            }
        }
        private IEnumerator AnimateLerpOut(float outSpeed, Action onCompleteAnimation)
        {
            // yield return new WaitForSeconds(Random.Range(0,1f));
            float i = 0;
            float rate = outSpeed / time;
            while (i < 1)
            {
                i += rate * Time.deltaTime;

                scale.x = scaleCurveX.Evaluate(1 - i);
                scale.y = scaleCurveY.Evaluate(1 - i);
                if (canScaleZ)
                    scale.z = scaleCurveZ.Evaluate(1 - i);
                SpriteObject.transform.localScale = scale;
                yield return 0;
            }

            yield return new WaitForSeconds(interval);

            StopAnimation();
            onCompleteAnimation?.Invoke();
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        #endregion
    }
}