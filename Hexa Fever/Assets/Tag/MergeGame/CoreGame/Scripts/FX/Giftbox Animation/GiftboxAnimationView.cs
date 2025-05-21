using System;
using System.Collections.Generic;
using Tag.HexaStack;
using Tag.RewardSystem;
using UnityEngine;

namespace Tag.CoreGame.Animation
{
    public class GiftboxAnimationView : BaseView
    {
        #region PUBLIC_VARS

        [SerializeField] private GiftBoxAnimation giftboxView;

        #endregion

        #region PRIVATE_VARS
        //[SerializeField] private List<GiftBoxSpriteData> giftBoxSpriteDatas;
        #endregion

        #region UNITY_CALLBACKS
        #endregion

        #region PUBLIC_FUNCTIONS

        public void PlayGiftboxAnimation(GiftBoxType chestType, RectTransform startTransform, BaseReward[] rewardDatas, Action onRewardClaimed, Action onRewardClaimedAnimationCompleted)
        {
            base.Show();
            giftboxView.PlayGiftBoxInAnimation(rewardDatas, startTransform, onRewardClaimed, () => { onRewardClaimedAnimationCompleted.Invoke(); Hide(); });
            //GiftBoxSpriteData giftBoxSpriteData = GetGiftBoxSpriteData(chestType);
            //giftboxView.SetGiftBoxSprite(giftBoxSpriteData.topSprite, giftBoxSpriteData.buttomSprite);
        }

        public override void OnBackButtonPressed()
        {
        }

        #endregion

        #region PRIVATE_FUNCTIONS
        //private GiftBoxSpriteData GetGiftBoxSpriteData(GiftBoxType giftBoxType)
        //{
        //    for (int i = 0; i < giftBoxSpriteDatas.Count; i++)
        //    {
        //        if (giftBoxSpriteDatas[i].giftBoxType == giftBoxType)
        //        {
        //            return giftBoxSpriteDatas[i];
        //        }
        //    }
        //    return null;
        //}
        #endregion

        #region CO-ROUTINES
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion
        //[Serializable]
        //public class GiftBoxSpriteData
        //{
        //    public GiftBoxType giftBoxType;
        //    public Sprite topSprite;
        //    public Sprite buttomSprite;
        //}
    }
}
