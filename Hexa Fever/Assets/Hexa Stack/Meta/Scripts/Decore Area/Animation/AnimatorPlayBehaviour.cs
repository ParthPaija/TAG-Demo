using System.Collections;
using Tag.HexaStack;
using UnityEngine;
using UnityForge.PropertyDrawers;

namespace Tag.MetaGame.Animation
{
    public class AnimatorPlayBehaviour : BaseAnimatorPlayBehaviour
    {
        #region PUBLIC_VARIABLES

        public Animator animator;
        [AnimatorStateName(animatorField: "animator")]
        public string animationID;
        [AnimatorStateName(animatorField: "animator")]
        public string resetAnimationID;
        [AnimatorStateName(animatorField: "animator")]
        public string onAnimationID;
        [AnimatorStateName(animatorField: "animator")]
        public string offAnimationID;

        public bool isIdelAnimation = false;
        [AnimatorStateName(animatorField: "animator")]
        public string idelAnimationID;

        //private AnimationHandler AnimationHandler { get { return FindObjectFromLibrary<AnimationHandler>(); } }

        #endregion

        #region PRIVATE_VARIABLES

        private bool isTaskCompleted;

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            if (isTaskCompleted)
            {
                SetTaskStatus(true);
                PlayOnAnimation();
                PlayIdelAnimation();
            }
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void OnPlayAnimation()
        {
            //SetVisibility(true);
            isTaskCompleted = true;
            StartCoroutine(PlayInAnimation());
        }

        public override void PlayIdelAnimation()
        {
            if (isIdelAnimation)
            {
                animator.Play(idelAnimationID, 0);
            }
        }

        public override void PlayOnAnimation()
        {
            animator.Play(onAnimationID);
        }

        public override void PlayOffAnimation()
        {
            animator.Play(offAnimationID);
        }

        public override void PlayParticles()
        {
            base.PlayParticles();
        }

        public override void SetTaskStatus(bool isActive)
        {
            isTaskCompleted = isActive;
        }

        public override void OnStopAnimation()
        {
            if (!string.IsNullOrEmpty(resetAnimationID))
                animator.Play(resetAnimationID, 0);
            else
                animator.StopPlayback();
        }

        public override float GetAnimationTime()
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == animationID)
                    return clips[i].length;
            }
            return 0;
        }

        public override float GetParticleTime()
        {
            ParticleSystem.MainModule main = _taskCompletionEffects[0].main;
            return main.duration + main.startLifetime.constantMax;
        }

        #endregion

        #region PRIVATE_FUNCTIONS
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region COROUTINES
        private IEnumerator PlayInAnimation()
        {
            animator.Play(animationID, 0);
            yield return new WaitForSeconds(animator.GetAnimatorClipLength(animationID));
            PlayIdelAnimation();
        }
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}