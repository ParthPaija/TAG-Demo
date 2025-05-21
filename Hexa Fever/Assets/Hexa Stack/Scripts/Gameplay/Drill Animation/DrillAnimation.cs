using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Tag.HexaStack;
using UnityEngine;

namespace Tag.HexaStack
{
    public class DrillAnimation : MonoBehaviour
    {
        #region PUBLIC_VARS
        public List<BaseCell> cells;
        public List<GameObject> deactivatingObjects;

        [Header("Drill Animation Data")]
        public Transform drillMoveTransform;
        public Transform drillScaleTransform;
        public float drillAnimationTime = 2f; // Duration of the animation
        public int drillAnimationRotationSpeed;
        public AnimationCurve drillAnimationMoveCurve; // Animation curve for movement
        public AnimationCurve drillAnimationXCurve;
        public AnimationCurve drillAnimationYCurve;
        public AnimationCurve drillAnimationZCurve;
        public AnimationCurve drillAnimationRotationCurve; // Animation curve for scaling
        public ParticleSystem drillParticle;

        [Header("Drill Rotation Animation Data")]
        public Transform drillSpinTransform;
        public AnimationCurve rotationCurve; // Animation curve for rotation
        public float tileCollectAnimationTime = 2f; // Duration of the animation
        public int tileCollectAnimationRotationSpeed;
        #endregion

        #region PRIVATE_VARS
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        #endregion

        #region UNITY_CALLBACKS
        void Start()
        {
            initialPosition = drillMoveTransform.localPosition;
            initialRotation = drillSpinTransform.rotation;
        }
        #endregion

        #region PUBLIC_FUNCTIONS
        [Button]
        public void StartDrillAnimation()
        {
            StartCoroutine(DoDrillAnimation());
        }

        [Button]
        public void StartTileCollectAnimation()
        {
            StartCoroutine(DoTileCollectAnimation());
        }
        #endregion

        #region PRIVATE_FUNCTIONS
        private void ResetPosition()
        {
            drillMoveTransform.localPosition = initialPosition;
            drillSpinTransform.rotation = initialRotation;
            for (int j = 0; j < deactivatingObjects.Count; j++)
                deactivatingObjects[j].SetActive(true);
        }
        #endregion

        #region CO-ROUTINES
        private IEnumerator DoTileCollectAnimation()
        {
            float elapsedTime = 0f;
            Vector3 endrotation = new Vector3(0, 0, tileCollectAnimationRotationSpeed * -360) + initialRotation.eulerAngles;
            while (elapsedTime < tileCollectAnimationTime)
            {
                float t = elapsedTime / tileCollectAnimationTime;
                drillSpinTransform.eulerAngles = Vector3.LerpUnclamped(initialRotation.eulerAngles, endrotation, rotationCurve.Evaluate(t));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            drillSpinTransform.rotation = initialRotation;
        }

        private IEnumerator DoDrillAnimation()
        {
            drillParticle.Play();
            for (int j = 0; j < deactivatingObjects.Count; j++)
                deactivatingObjects[j].SetActive(false);

            float i = 0;
            Vector3 startPos = cells[0].transform.position;
            Vector3 endPos = cells[cells.Count - 1].transform.position;
            Vector3 endrotation = new Vector3(0, 0, drillAnimationRotationSpeed * -360) + initialRotation.eulerAngles;
            Vector3 startScale = Vector3.zero;
            Vector3 endScale = drillScaleTransform.localScale;
            Vector3 tempScale = Vector3.zero;
            while (i < 1)
            {
                i += Time.deltaTime / drillAnimationTime;
                drillMoveTransform.position = Vector3.LerpUnclamped(startPos, endPos, drillAnimationMoveCurve.Evaluate(i));
                tempScale.x = Mathf.LerpUnclamped(startScale.x, endScale.x, drillAnimationXCurve.Evaluate(i));
                tempScale.y = Mathf.LerpUnclamped(startScale.y, endScale.y, drillAnimationYCurve.Evaluate(i));
                tempScale.z = Mathf.LerpUnclamped(startScale.z, endScale.z, drillAnimationZCurve.Evaluate(i));
                drillScaleTransform.localScale = tempScale;
                drillSpinTransform.eulerAngles = Vector3.LerpUnclamped(initialRotation.eulerAngles, endrotation, drillAnimationRotationCurve.Evaluate(i));

                yield return null;
            }
            drillParticle.Stop();
            ResetPosition();

        }
        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
