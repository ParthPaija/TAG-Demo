using System;
using System.Collections;
using UnityEngine;

namespace Tag.MetaGame
{
    public class TranslateObject : MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        
        public GameObject objectToTranslate;
        public float time;
        public Transform startPos;
        public Transform endPos;
        public AnimationCurve animationCurve;
        
        #endregion

        #region PRIVATE_VARIABLES
        #endregion

        #region UNITY_CALLBACKS
        #endregion

        #region PUBLIC_FUNCTIONS
        
        public void OnTranslate()
        {
            StartCoroutine(Translate(startPos, endPos, null));
        }
        
        public void OnTranslate(Action actionToPerform)
        {
            StartCoroutine(Translate(startPos, endPos, actionToPerform));
        }
        
        public void OnReverseTranslate(Action actionToPerform)
        {
            StartCoroutine(Translate(endPos, startPos, actionToPerform));
        }
        public void OnReverseTranslate()
        {
            StartCoroutine(Translate(endPos, startPos, null));
        }
        #endregion

        #region PRIVATE_FUNCTIONS
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region COROUTINES
        
        private IEnumerator Translate(Transform startPos, Transform endPos, Action actionToPerform)
        {
            float t = 0;
            float rate = 1 / time;
            objectToTranslate.transform.position = startPos.position;
            while (t < 1f)
            {
                objectToTranslate.transform.position = Vector3.LerpUnclamped(startPos.position, endPos.position, animationCurve.Evaluate(t));
                t += Time.deltaTime * rate;
                yield return 0;
            }
            objectToTranslate.transform.position = endPos.position;
            actionToPerform?.Invoke();
        }

        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}
