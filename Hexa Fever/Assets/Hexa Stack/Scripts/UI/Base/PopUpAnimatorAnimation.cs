using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tag.HexaStack
{
    public class PopUpAnimatorAnimation : BaseUiAnimation
    {
        #region PUBLIC_VAR

        public float sizeofAnimationClip;
        public string inAnimation = "InAnimation";
        public string outAnimation = "OutAnimation";

        #endregion

        [SerializeField] private Animator PopUpAnimator;
        private Coroutine coroutine;

        #region overrided methods

        public override void ShowAnimation(Action action)
        {
            base.ShowAnimation(action);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            coroutine = StartCoroutine(DoShowFx());
        }

        public override void HideAnimation(Action action)
        {
            base.HideAnimation(action);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            coroutine = StartCoroutine(DoHideFx());
        }

        #region PUBLIC_METHOD

        public void PlayInAnimation()
        {
            PopUpAnimator.Play(inAnimation);
        }

        public void PlayOutAnimation()
        {
            PopUpAnimator.Play(outAnimation);
        }

        #endregion

        #endregion

        public IEnumerator DoShowFx()
        {
            PopUpAnimator.Play(inAnimation);
            yield return 0;
            onShowComplete?.Invoke();
        }

        public IEnumerator DoHideFx()
        {
            PopUpAnimator.Play(outAnimation);
            yield return new WaitForSeconds(sizeofAnimationClip);
            onHideComplete?.Invoke();
            coroutine = null;
        }
    }
}