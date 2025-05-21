using System;
using System.Collections;
using Tag.MetaGame.Animation;
using UnityEngine;

namespace Tag.MetaGame
{
    public class DecoreOptionObject : MonoBehaviour
    {
        #region PUBLIC_VARS

        public BaseAnimatorPlayBehaviour animatorPlayBehaviour;

        #endregion

        #region PRIVATE_VARS
        private AreaEditMode AreaEditMode { get { return AreaEditMode.Instance; } }
        //private BackHandler BackHandler { get { return FindObjectFromLibrary<BackHandler>(); } }
        //private AutoPopupHandler AutoPopupHandler { get { return FindObjectFromLibrary<AutoPopupHandler>(); } }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void PlayAnimation(Action action)
        {
            //BackHandler.SetCanBack(false);
            //AutoPopupHandler.SetCanShowAutoOpenPopup(false);
            AreaEditMode.HideIcons();
            StartCoroutine(DecoreAnimation(delegate
            {
                //BackHandler.SetCanBack(true);
                //AutoPopupHandler.SetCanShowAutoOpenPopup(true);
                action?.Invoke();
            }));
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        private IEnumerator DecoreAnimation(Action action)
        {
            animatorPlayBehaviour.OnPlayAnimation();
            yield return new WaitForSeconds(animatorPlayBehaviour.GetAnimationTime());
            animatorPlayBehaviour.PlayParticles();
            yield return new WaitForSeconds(animatorPlayBehaviour.GetParticleTime());
            action?.Invoke();
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
