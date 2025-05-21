using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tag
{
    public class CompasAnimation : MonoBehaviour
    {
        #region PUBLIC_VARS

        public Vector3 initialRotation;

        [Header("Compas Data")]
        public MeshRenderer redNiddle;
        public MeshRenderer blueNiddle;
        [Header("Idle Animation Data")]
        public float idleAnimationTime = 1f;
        public float idleAnimationRotationOffset;
        public AnimationCurve idleAnimationCurve;
        [Header("Flip Animation Data")]
        public float flipAnimationTime = 1f;
        public Vector3 flipRotationOffset;
        public AnimationCurve flipAnimationCurve;

        #endregion

        #region PRIVATE_VARS

        private Vector3 redNiddleCurrentRotation;
        private Vector3 blueNiddleCurrentRotation;

        private Coroutine idleAnimationCoroutine;
        private Coroutine flipAnimationCoroutine;

        #endregion

        #region UNITY_CALLBACKS

        void Start()
        {
            SetInitialData();
        }

        #endregion

        #region PUBLIC_FUNCTIONS
        public void SetNiddles(bool isRedNiddleActive, bool isBlueNiddleActive)
        {
            redNiddle.enabled = isRedNiddleActive;
            blueNiddle.enabled = isBlueNiddleActive;
        }
        [Button]
        public void PlayNiddleIdleAnimation()
        {
            if (idleAnimationCoroutine != null)
                StopCoroutine(idleAnimationCoroutine);
            idleAnimationCoroutine = StartCoroutine(DoIdleAnimation());
        }
        [Button]
        public void PlayFlipRotation(int direction)
        {
            // direction must be 1 or -1
            // 1 for clockwise
            // -1 for anti-clockwise

            if (idleAnimationCoroutine != null)
                StopCoroutine(idleAnimationCoroutine);

            if (flipAnimationCoroutine != null)
                StopCoroutine(flipAnimationCoroutine);

            SetNiddleRotation();
            flipAnimationCoroutine = StartCoroutine(DoFlipRotation(direction));
        }

        public void SetNiddleRotation()
        {
            redNiddle.transform.localEulerAngles = redNiddleCurrentRotation;
            blueNiddle.transform.localEulerAngles = blueNiddleCurrentRotation;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetInitialData()
        {
            redNiddle.transform.localEulerAngles = initialRotation;
            blueNiddle.transform.localEulerAngles = initialRotation;

            redNiddleCurrentRotation = redNiddle.transform.localEulerAngles;
            blueNiddleCurrentRotation = blueNiddle.transform.localEulerAngles;
        }

        #endregion

        #region CO-ROUTINES
        IEnumerator DoIdleAnimation()
        {
            float i = 0;
            float rate = 1 / idleAnimationTime;
            Vector3 redNiddleTempRotation = redNiddle.transform.localEulerAngles;
            Vector3 blueNiddleTempRotation = blueNiddle.transform.localEulerAngles;
            while (i < 1)
            {
                i += Time.deltaTime * rate;

                redNiddleTempRotation.z = Mathf.LerpUnclamped(redNiddleCurrentRotation.z, redNiddleCurrentRotation.z + idleAnimationRotationOffset, idleAnimationCurve.Evaluate(i));
                blueNiddleTempRotation.z = Mathf.LerpUnclamped(blueNiddleCurrentRotation.z, blueNiddleCurrentRotation.z + idleAnimationRotationOffset, idleAnimationCurve.Evaluate(i));
                redNiddle.transform.localEulerAngles = redNiddleTempRotation;
                blueNiddle.transform.localEulerAngles = blueNiddleTempRotation;

                yield return null;
            }
            PlayNiddleIdleAnimation();
        }

        IEnumerator DoFlipRotation(int direction)
        {
            float i = 0;
            float rate = 1 / flipAnimationTime;

            while (i < 1)
            {
                i += Time.deltaTime * rate;
                redNiddle.transform.localEulerAngles = Vector3.LerpUnclamped(redNiddleCurrentRotation, redNiddleCurrentRotation + (direction * flipRotationOffset), flipAnimationCurve.Evaluate(i));
                blueNiddle.transform.localEulerAngles = Vector3.LerpUnclamped(blueNiddleCurrentRotation, blueNiddleCurrentRotation + (direction * flipRotationOffset), flipAnimationCurve.Evaluate(i));
                yield return null;
            }
            redNiddleCurrentRotation = redNiddle.transform.localEulerAngles;
            blueNiddleCurrentRotation = blueNiddle.transform.localEulerAngles;
            PlayNiddleIdleAnimation();
            StopCoroutine(flipAnimationCoroutine);
            flipAnimationCoroutine = null;
        }
        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
