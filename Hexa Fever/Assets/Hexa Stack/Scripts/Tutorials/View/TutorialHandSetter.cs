using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class TutorialHandSetter : MonoBehaviour
    {
        #region PUBLIC_VARIABLES

        public Transform handTargetTransform;
        public Vector3 offset;
        public bool canPlayTranslateAnimation;

        #endregion

        #region PRIVATE_VARIABLES

        #endregion

        #region PROPERTIES

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_METHODS

        public void SetHandAnimation()
        {
            TutorialElementHandler.Instance.SetActiveTapHandUI(true, handTargetTransform.position + offset, canPlayTranslateAnimation);
        }

        public void ResetHandAnimation()
        {
            TutorialElementHandler.Instance.tutorialHandAnimationUI.gameObject.SetActive(false);
        }

        #endregion

        #region PRIVATE_METHODS

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region COROUTINES

        #endregion

        #region UI_CALLBACKS

        #endregion
    }
}