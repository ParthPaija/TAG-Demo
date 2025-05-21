using Spine;
using Spine.Unity;
using System;
using System.Collections;
using Tag.HexaStack;
using Tag.RewardSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityForge.PropertyDrawers;

namespace Tag.CoreGame.Animation
{
    public class GiftBoxAnimation : BaseView
    {
        #region PUBLIC_VARS

        public GiftBoxType ChestType;

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GiftboxRewardAnimation _giftboxRewardAnimation;

        [SerializeField] SkeletonGraphic skeletonAnimation;
        [SerializeField, SpineAnimation] private string _giftInAnimation;
        [SerializeField, SpineAnimation] private string _giftClaimAnimation;
        [SerializeField, SpineAnimation] private string _giftIdealAnimation;

        [SerializeField] private float giftboxInAnimationTime;
        [SerializeField] private RectTransform endTransform;
        [SerializeField] private RectTransform midTransform;
        [SerializeField] private Transform objectToAnimate;
        [SerializeField] private AnimationCurve positionCurve;
        [Space]

        [SerializeField] private Animator _rewardAnimation;
        [AnimatorStateName(animatorField: nameof(_rewardAnimation))][SerializeField] private string _rewardInAnimation;
        [AnimatorStateName(animatorField: nameof(_rewardAnimation))][SerializeField] private string _rewardIdleAnimation;

        [SerializeField] private Button _tapToClaimButton;
        [SerializeField] private Button _tapToOpenButton;
        [SerializeField] private ParticleSystem giftBoxOpenPs;
        [SerializeField] private GameObject _tapToContinueGO;

        [SerializeField] private Canvas _canvas;
        private Action _onRewardClaimed;
        private Action _onRewardClaimedAnimationCompleted;
        private RectTransform startGiftboxTransform;
        BaseReward[] rewardDatas;

        #endregion

        #region UNITY_CALLBACKS

        public override void OnBackButtonPressed()
        {
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void PlayGiftBoxInAnimation(BaseReward[] rewardDatas, RectTransform startTransform, Action onRewardClaimed, Action onRewardClaimedAnimationCompleted)
        {
            base.Show();
            _onRewardClaimed = onRewardClaimed;
            _onRewardClaimedAnimationCompleted = onRewardClaimedAnimationCompleted;
            this.rewardDatas = rewardDatas;

            _tapToClaimButton.interactable = false;
            //_tapToClaimButton.gameObject.SetActive(false);

            _tapToContinueGO.SetActive(false);
            _tapToOpenButton.gameObject.SetActive(false);

            _rewardAnimation.Play(_rewardIdleAnimation);
            //gameObject.SetActive(true);

            startGiftboxTransform = startTransform;
            TrackEntry trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, _giftInAnimation, false);

            trackEntry.Complete += (tE) =>
            {
                skeletonAnimation.AnimationState.SetAnimation(0, _giftIdealAnimation, true);
                _tapToOpenButton.gameObject.SetActive(true);
            };

            var giftBoxInAnimation = skeletonAnimation.Skeleton.Data.FindAnimation(_giftInAnimation);
            giftboxInAnimationTime = giftBoxInAnimation.Duration;
        }

        public void PlayGiftBoxClaimAnimation()
        {
            _tapToContinueGO.SetActive(false);
            _tapToClaimButton.interactable = false;
            //_tapToClaimButton.gameObject.SetActive(false);

            _giftboxRewardAnimation.PlayRewardCollectionAnimation((Action)delegate
            {
                _onRewardClaimedAnimationCompleted?.Invoke();
                Hide();
            });
        }

        #endregion

        #region PRIVATE_FUNCTIONS]

        #endregion

        #region CO-ROUTINES

        private IEnumerator PlayGiftboxInAnimation()
        {
            float i = 0;
            float rate = 1 / (giftboxInAnimationTime);
            Vector3 temp1, temp2;
            Vector3 tempPos;
            while (i < 1)
            {
                i += Time.deltaTime * rate;
                temp1 = Vector3.LerpUnclamped(startGiftboxTransform.position, midTransform.position, positionCurve.Evaluate(i));
                temp2 = Vector3.LerpUnclamped(midTransform.position, endTransform.position, positionCurve.Evaluate(i));
                tempPos = Vector3.LerpUnclamped(temp1, temp2, positionCurve.Evaluate(i));


                objectToAnimate.position = tempPos;
                yield return null;
            }
        }

        private IEnumerator PlayRewardClaimCoroutine(float delayTime)
        {
            //yield return new WaitForSeconds(delayTime);
            skeletonAnimation.AnimationState.SetAnimation(0, _giftClaimAnimation, false);
            CoroutineRunner.Instance.Wait(0.5f, () =>
            {
                giftBoxOpenPs.Play();
            });
            _rewardAnimation.Play(_rewardInAnimation);
            yield return new WaitForSeconds(_rewardAnimation.GetAnimationLength(_rewardInAnimation));
            _tapToClaimButton.interactable = true;
            //_tapToClaimButton.gameObject.SetActive(true);
            _tapToContinueGO.SetActive(true);
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnTapToOpenButtonClick()
        {
            _tapToOpenButton.gameObject.SetActive(false);
            StartCoroutine(PlayRewardClaimCoroutine(giftboxInAnimationTime));
            _giftboxRewardAnimation.SetRewardView(rewardDatas);
        }

        public void OnClaimClicked()
        {
            _onRewardClaimed?.Invoke();
            PlayGiftBoxClaimAnimation();
        }

        #endregion
    }
}
