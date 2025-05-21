using Spine.Unity;
using System.Collections;
using UnityEngine;

namespace Tag.MetaGame.Animation
{
    public class SpineAnimationPlayerBehaviour : BaseAnimatorPlayBehaviour
    {
        #region PUBLIC_VARIABLES
        public SkeletonAnimation animator;

        [SpineAnimation]
        public string playAnimationID;
        //[SpineAnimation]
        //public string playOffAnimationID;
        public bool isLoop;
        public bool isPlayOnEnable;
        #endregion

        #region PRIVATE_VARIABLES
        private void Start()
        {
            if(isPlayOnEnable)
            {
                OnPlayAnimation();
            }
        }
        #endregion

        #region UNITY_CALLBACKS
        #endregion

        #region PUBLIC_FUNCTIONS
        public override void OnPlayAnimation()
        {
            animator.AnimationState.SetAnimation(0, playAnimationID, isLoop);
            //StartCoroutine(WaitAndPlayAnimation());
        }
       /* public override void PlayOffAnimation()
        {
            animator.AnimationState.SetAnimation(0, playOffAnimationID, isLoop);
            base.PlayOffAnimation();
        }*/
        public override void OnStopAnimation()
        {
            animator.skeleton.SetToSetupPose();
        }

        public override float GetAnimationTime()
        {
            var myAnimation = animator.Skeleton.Data.FindAnimation(playAnimationID);

            return myAnimation.Duration / animator.timeScale;
        }

        public override void PlayParticles()
        {
            //throw new System.NotImplementedException();
        }

        public override void SetTaskStatus(bool isActive)
        {
            //throw new System.NotImplementedException();
        }

        public override float GetParticleTime()
        {
            return 0;
        }
        #endregion

        #region PRIVATE_FUNCTIONS
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region COROUTINES

        private IEnumerator WaitAndPlayAnimation()
        {
            //animator.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            animator.AnimationState.SetAnimation(0, playAnimationID, isLoop);
            //animator.gameObject.SetActive(true);
        }


        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}