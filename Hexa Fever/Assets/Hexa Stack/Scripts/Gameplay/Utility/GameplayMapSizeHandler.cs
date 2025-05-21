using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class GameplayMapSizeHandler : Manager<GameplayMapSizeHandler>
    {
        public RectTransform gameplayView;
        public float animationDuration = 1.0f;
        public float padding = 1.0f;
        public float yPosOffset = -2.5f;
        public Transform gameplayTransform; // Reference to your gameplay transform

        //private Vector3 originalScale;
        //private Vector3 originalPosition;
        //private Coroutine currentCoroutine;

        [Button]
        public void FitMap()
        {
            if (gameplayView == null || gameplayTransform == null)
            {
                Debug.LogError("Assign the Gameplay View RectTransform and Gameplay Transform in the inspector.");
                return;
            }

            // Get the world corners of the Gameplay View
            Vector3[] viewCorners = new Vector3[4];
            gameplayView.GetWorldCorners(viewCorners);

            // Calculate the center of the gameplay view
            //Vector3 viewCenter = (viewCorners[0] + viewCorners[2]) / 2;

            Vector3 viewCenter = gameplayView.TransformPoint(gameplayView.rect.center);

            // Calculate the width and height of the Gameplay View in world units
            float viewWidth = Vector3.Distance(viewCorners[0], viewCorners[3]);
            float viewHeight = Vector3.Distance(viewCorners[0], viewCorners[1]);

            // Calculate gameplay bounds
            Bounds gameplayBounds = CalculateGameplayBounds();

            // Determine the scaling factor to fit the gameplay inside the view
            float widthScale = viewWidth / gameplayBounds.size.x;
            float heightScale = viewHeight / gameplayBounds.size.z;
            float finalScale = Mathf.Min(widthScale, heightScale) * padding;

            // Calculate target scale
            Vector3 targetScale = Vector3.one * finalScale;

            // Calculate target position (center of the gameplay view)
            //Vector3 targetPosition = new Vector3(viewCenter.x, viewCenter.y, gameplayTransform.position.z);
            //Vector3 targetPosition = Camera.main.ScreenToWorldPoint(gameplayView.anchoredPosition);
            Vector3 targetPosition = viewCenter;
            targetPosition.z = 0;
            targetPosition.y += yPosOffset;

            // Start the smooth transition
            //if (currentCoroutine != null)
            //{
            //    StopCoroutine(currentCoroutine);
            //}
            //currentCoroutine = StartCoroutine(AnimateTransform(targetScale, targetPosition));

            gameplayTransform.localScale = targetScale;
            gameplayTransform.localPosition = targetPosition;
        }

        private Bounds CalculateGameplayBounds()
        {
            Bounds bounds = new Bounds(gameplayTransform.position, Vector3.zero);
            Renderer[] renderers = gameplayTransform.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
            return bounds;
        }

        //[Button]
        //public void ResetMap(Action action = null)
        //{
        //    if (currentCoroutine != null)
        //    {
        //        StopCoroutine(currentCoroutine);
        //    }
        //    currentCoroutine = StartCoroutine(AnimateTransform(originalScale, originalPosition, action));
        //}

        //private IEnumerator AnimateTransform(Vector3 targetScale, Vector3 targetPosition, Action action = null)
        //{
        //    Vector3 startScale = gameplayTransform.localScale;
        //    Vector3 startPosition = gameplayTransform.position;
        //    float timeElapsed = 0f;

        //    while (timeElapsed < animationDuration)
        //    {
        //        gameplayTransform.localScale = Vector3.Lerp(startScale, targetScale, timeElapsed / animationDuration);
        //        gameplayTransform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / animationDuration);

        //        timeElapsed += Time.deltaTime;
        //        yield return null;
        //    }

        //    // Ensure the final values are set correctly
        //    gameplayTransform.localScale = targetScale;
        //    gameplayTransform.position = targetPosition;
        //    action?.Invoke();
        //    currentCoroutine = null;
        //}
    }
}
