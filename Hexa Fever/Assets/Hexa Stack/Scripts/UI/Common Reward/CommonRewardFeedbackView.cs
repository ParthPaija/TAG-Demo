using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class CommonRewardFeedbackView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private RewardItemView[] rewardItemViews;
        [SerializeField] private SetRewardChild[] rewardsChild;
        [SerializeField] Button tapToClaim;
        private Action onAnimationComplate;
        private List<BaseReward> baseRewards;
        private List<RewardItemView> generatedRewardViews = new List<RewardItemView>();

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        [Button]
        public void ShowView(List<BaseReward> baseRewards, Action onAnimationComplate)
        {
            this.onAnimationComplate = onAnimationComplate;
            this.baseRewards = baseRewards;
            tapToClaim.interactable = false;
            SetRewardView();
            base.Show();
        }

        public override void OnViewShowDone()
        {
            base.OnViewShowDone();
            tapToClaim.interactable = true;
        }

        public override void OnHideComplete()
        {
            base.OnHideComplete();
            onAnimationComplate?.Invoke();
        }

        public override void OnBackButtonPressed()
        {
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetRewardView()
        {
            generatedRewardViews = new List<RewardItemView>();
            for (int i = 0; i < rewardItemViews.Length; i++)
            {
                rewardItemViews[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < baseRewards.Count; i++)
            {
                rewardItemViews[i].SetView(baseRewards[i]);
                generatedRewardViews.Add(rewardItemViews[i]);
            }

            for (int i = 0; i < rewardsChild.Length; i++)
            {
                rewardsChild[i].Set();
            }
        }

        private IEnumerator PlayRewardViewsUpAnimation()
        {
            var openViews = generatedRewardViews;
            float animTranslateTime = 0.6f;  // Animation duration
            float delayBetweenItems = 0.1f;  // Slight delay between items

            for (int i = 0; i < openViews.Count; i++)
            {
                RewardItemView currentAnimView = openViews[i];
                
                // Subtle highlight of original item
                currentAnimView.transform.DOScale(1.05f, 0.15f).SetEase(Ease.OutQuad)
                    .OnComplete(() => currentAnimView.transform.DOScale(1f, 0.1f));
                
                yield return new WaitForSeconds(0.1f);
                
                // Create clone for animation
                RewardItemView current = Instantiate(currentAnimView, currentAnimView.transform);
                current.transform.position = currentAnimView.transform.position;
                current.AnimationParent.pivot = new Vector2(0.5f, 0.5f);
                current.AnimationParent.anchorMin = new Vector2(0.5f, 0.5f);
                current.AnimationParent.anchorMax = new Vector2(0.5f, 0.5f);
                current.AnimationParent.anchoredPosition = Vector2.zero;
                
                // Simple, smooth upward animation
                Sequence viewSequence = DOTween.Sequence();
                viewSequence.Append(current.CanvasGroup.DOFade(0f, animTranslateTime).SetEase(Ease.InQuad));
                viewSequence.Join(current.AnimationParent.DOAnchorPos(new Vector2(0f, 120f), animTranslateTime)
                    .SetEase(Ease.OutQuad));
                
                // Very subtle scale change
                viewSequence.Join(current.transform.DOScale(0.9f, animTranslateTime).SetEase(Ease.Linear));
                
                yield return new WaitForSeconds(delayBetweenItems);
                
                // Clean up
                Destroy(current.gameObject, animTranslateTime);
            }
            
            // Wait for all animations to complete
            yield return new WaitForSeconds(animTranslateTime);
            Hide();
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnTapToClaimButton()
        {
            tapToClaim.interactable = false;
            StartCoroutine(PlayRewardViewsUpAnimation());
        }

        #endregion
    }
}
