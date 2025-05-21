using UnityEngine;
using UnityEngine.Events;

namespace Tag.HexaStack
{
    public class TapOnUITutorialStep : BaseTutorialStep
    {
        #region PUBLIC_VARIABLES

        [Space]
        public UnityEvent OnStartStep;
        public UnityEvent OnEndStep;
        public UnityEvent onTap;

        #endregion

        #region PRIVATE_VARIABLES

        #endregion

        #region PROPERTIES

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_METHODS

        public override void OnStartStep1()
        {
            LevelManager.Instance.LoadedLevel.GridRotator.isRotationActive = false;
            InputManager.StopInteraction = true;
            base.OnStartStep1();
            OnStartStep?.Invoke();
            TutorialElementHandler.RegisterOnHighlighterTap(OnTap);
        }

        public override void EndStep()
        {
            TutorialElementHandler.ResetHighLighters();
            TutorialElementHandler.SetActiveTapHandUI(false, Vector3.zero);
            OnEndStep?.Invoke();
            base.EndStep();
            LevelManager.Instance.LoadedLevel.GridRotator.isRotationActive = true;
            InputManager.StopInteraction = false;
        }

        #endregion

        #region PRIVATE_METHODS

        private void OnTap()
        {
            if (onTap != null) onTap.Invoke();
            EndStep();
        }

        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region COROUTINES
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}
