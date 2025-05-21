using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Tag.HexaStack
{
    public class ShopFreeNotifications : BaseNotifications
    {
        #region PUBLIC_VARS

        [SerializeField] float moveAmount = 20f; // Ketlu upar niche javu che
        [SerializeField] float duration = 0.5f;  // Ketli vaar ma javu che
        [SerializeField] private RectTransform badgeTransform;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        void Start()
        {
            AnimateBadge();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Show()
        {
            base.Show();
            AnimateBadge();
        }

        public override bool CanShow()
        {
            return DailyDealsManager.Instance.IsFreeDealsCliamAvailable();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void AnimateBadge()
        {
            badgeTransform.DOAnchorPosY(badgeTransform.anchoredPosition.y + moveAmount, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
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
