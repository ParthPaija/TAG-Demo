using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class Propeller : MonoBehaviour
    {
        #region PUBLIC_VARS

        [SerializeField] private GameObject propellerBase;

        [Header("Propeller Settings")]
        [SerializeField] private Transform propellerModel; // The spinning propeller object
        [SerializeField] private float spinSpeed = 360f;   // Rotation speed during animation
        [SerializeField] private float liftHeight = 2f;    // Height to lift off before flying
        [SerializeField] private AnimationCurve flightCurve; // Curve for smooth flight

        [Header("Animation Timing")]
        [SerializeField] private float liftDuration = 0.5f;  // Duration of spin-up
        [SerializeField] private float flightDuration = 1.5f; // Duration of flight

        [Header("Impact Effects")]
        [SerializeField] private ParticleSystem impactEffect; // Particle effect on impact
        [SerializeField] private float impactScale = 1.5f;    // Scale multiplier on impact

        [Header("Scale Animation")]
        [SerializeField] private AnimationCurve scaleCurve; // Curve to control scaling
        [SerializeField] private Vector3 startScale = Vector3.one; // Initial scale
        [SerializeField] private Vector3 endScale = Vector3.one * 0.5f; // Final scale (optional)

        private PropellerCellUnlocker cellUnlocker;
        private BaseCell targetCell;
        Vector3 finalTargetPosition;
        List<Vector3> flightPath = new List<Vector3>();
        private Coroutine moveCO;
        private Action<BaseCell> endAction;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        private void Update()
        {
            if (moveCO != null)
            {
                //if (!targetCell.CanUseBooster())
                //{
                //    Debug.Log("Chanage Target Propeller " + targetCell.name);
                //    PropellerCellUnlocker.RemoveCell(targetCell);
                //    BaseCell newTarget = cellUnlocker.GetBeseCellForPropellerUse();
                //    if (newTarget != null)
                //    {
                //        targetCell = newTarget;
                //        moveCO = null;
                //        moveCO = StartCoroutine(PropellerAnimation(5));
                //    }
                //}
            }
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        [Button]
        public void PlayPropellerAnimation(PropellerCellUnlocker cellUnlocker, BaseCell targetCell, Action<BaseCell> endAction)
        {
            this.endAction = endAction;
            this.cellUnlocker = cellUnlocker;
            this.targetCell = targetCell;
            propellerModel.transform.position = cellUnlocker.MyCell.transform.position;
            propellerModel.gameObject.SetActive(true);
            propellerBase.SetActive(false);
            if (moveCO == null)
                moveCO = StartCoroutine(PropellerAnimation(5));
        }
        
        public void PlayPropellerAnimation(Vector3 propellerModelPos, BaseCell targetCell, Action<BaseCell> endAction)
        {
            this.endAction = endAction;
            this.targetCell = targetCell;
            propellerModel.transform.position = propellerModelPos;
            propellerModel.gameObject.SetActive(true);
            propellerBase.SetActive(false);
            gameObject.SetActive(true);
            if (moveCO == null)
                moveCO = StartCoroutine(PropellerAnimation(5));
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private List<Vector3> CreateBezierPoints(Vector3 start, Vector3 end, float maxStackHeight)
        {
            // Define a safe Y-offset based on stack height
            float yOffset = maxStackHeight + liftHeight; // Ensure the path is above the highest stack

            // Calculate the initial midpoint with the Y-offset
            Vector3 midPoint = (start + end) / 2 + Vector3.up * yOffset;

            // Add a random point between start and midPoint
            float randomOffsetX = UnityEngine.Random.Range(-4f, 4f); // Reduced randomness to stay aligned
            if (randomOffsetX > -1 && randomOffsetX < 1)
            {
                randomOffsetX = -2f;
            }
            float randomOffsetZ = UnityEngine.Random.Range(-4f, 4f);
            if (randomOffsetZ > -1 && randomOffsetZ < 1)
            {
                randomOffsetZ = -2f;
            }
            Vector3 randomPoint1 = Vector3.Lerp(start, midPoint, 0.65f) + new Vector3(randomOffsetX, UnityEngine.Random.Range(0.5f, 1f), randomOffsetZ);

            // Add a random point between midPoint and end
            randomOffsetX = UnityEngine.Random.Range(-2f, 2f);
            randomOffsetZ = UnityEngine.Random.Range(-2f, 2f);
            Vector3 randomPoint2 = Vector3.Lerp(midPoint, end, 0.667f) + new Vector3(randomOffsetX, UnityEngine.Random.Range(0.5f, 1f), randomOffsetZ);

            end.y += 1;
            end.z -= 0.6f;
            // Return the control points
            return new List<Vector3> { start, randomPoint1/*, midPoint, randomPoint2*/, end };
        }

        private Vector3 CalculateBezierPoint(float t, List<Vector3> controlPoints)
        {
            int n = controlPoints.Count - 1;
            Vector3 point = Vector3.zero;

            for (int i = 0; i <= n; i++)
            {
                float binomialCoeff = BinomialCoefficient(n, i);
                float blend = binomialCoeff * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i);
                point += blend * controlPoints[i];
            }

            return point;
        }

        private float BinomialCoefficient(int n, int k)
        {
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        private float Factorial(int num)
        {
            if (num == 0) return 1;
            float result = 1;
            for (int i = 1; i <= num; i++) result *= i;
            return result;
        }

        private void PlayImpactEffects(Vector3 position)
        {
            if (impactEffect != null)
            {
                ParticleSystem effect = Instantiate(impactEffect, position, Quaternion.identity, transform);
                effect.transform.localScale *= impactScale;
                effect.Play();
            }
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator PropellerAnimation(float maxStackHeight)
        {
            float timer = 0;
            Vector3 startPosition = propellerBase.transform.position;

            // Keep the original target position intact

            finalTargetPosition = targetCell.IsCellLocked() ? targetCell.transform.position : targetCell.transform.position.With(null, targetCell.ItemStack.GetTopPosition().y);
            //finalTargetPosition = targetCell.transform.position;

            Debug.LogError("Change target pos Propeller " + targetCell.name);

            // Generate the Bezier curve with Y-offset to avoid stacks
            flightPath = new List<Vector3>();
            flightPath = CreateBezierPoints(startPosition, finalTargetPosition, maxStackHeight);

            while (timer < flightDuration)
            {
                timer += Time.deltaTime;
                float t = timer / flightDuration;

                // Smooth motion along the Bezier curve
                propellerModel.position = CalculateBezierPoint(flightCurve.Evaluate(t), flightPath);

                // Scale animation based on the scale curve
                float scaleValue = scaleCurve.Evaluate(t);
                propellerModel.localScale = Vector3.Lerp(startScale, endScale, scaleValue);

                yield return null;
            }

            // Step 3: Impact
            PlayImpactEffects(finalTargetPosition + new Vector3(0, 0.3f, 0));
            endAction?.Invoke(targetCell);
            endAction = null;

            // Reset or deactivate the propeller
            propellerModel.gameObject.SetActive(false);
            propellerBase.gameObject.SetActive(true);
            moveCO = null;
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}