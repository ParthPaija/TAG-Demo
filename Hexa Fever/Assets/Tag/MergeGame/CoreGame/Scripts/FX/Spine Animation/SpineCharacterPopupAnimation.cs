using Spine.Unity;
using System;
using Tag.HexaStack;
using UnityEngine;

namespace Tag.MetaGame
{
    public class SpineCharacterPopupAnimation : MonoBehaviour
    {
        #region PUBLIC_VARS

        public SkeletonGraphic charactorToAnimate;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string inAnimId;
        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string actionToPerformAnimId;
        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string idleAnimId;
        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string outAnimationId;

        public RectTransform characterParent;
        #endregion

        #region PRIVATE_VARS
        private CoroutineRunner CoroutineRunner { get { return CoroutineRunner.Instance; } }
        #endregion

        #region UNITY_CALLBACKS
        #endregion

        #region PUBLIC_FUNCTIONS

        public void PlayInAnimation(Action OnEnd)
        {
            charactorToAnimate.gameObject.SetActive(true);
            charactorToAnimate.AnimationState.SetAnimation(0, inAnimId, false).Complete += (tE) =>
            {
                OnEnd.Invoke();
            };
        }

        //public void PlayActionToPerformAnimation()
        //{
        //    charactorToAnimate.gameObject.SetActive(true);
        //    if (charactorToAnimate.AnimationState.ToString() != inAnimId)
        //        PlayInAnimation();
        //    charactorToAnimate.AnimationState.SetAnimation(0, actionToPerformAnimId, false);
        //    CoroutineRunner.Wait(GetAnimationTime(actionToPerformAnimId), PlayIdleAnimation);
        //}

        public void PlayIdleAnimation()
        {
            charactorToAnimate.gameObject.SetActive(true);
            charactorToAnimate.AnimationState.SetAnimation(0, idleAnimId, true);
        }

        public void PlayOutAnimation()
        {
            if (charactorToAnimate == null)
                return;
            charactorToAnimate.AnimationState.SetAnimation(0, outAnimationId, false);
            StopAllCoroutines();
            ResetAfterAnimationComplete();
        }

        public float GetAnimationTime(string id)
        {
            return charactorToAnimate.Skeleton.Data.FindAnimation(id).Duration;
        }

        public float GetOutAnimationTime()
        {
            return charactorToAnimate.Skeleton.Data.FindAnimation(outAnimationId).Duration;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void ResetAfterAnimationComplete()
        {
            float time = GetOutAnimationTime();
            if (gameObject.activeInHierarchy)
                CoroutineRunner.Wait(time, () => charactorToAnimate.gameObject.SetActive(false));
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}
