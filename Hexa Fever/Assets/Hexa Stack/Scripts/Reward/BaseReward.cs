using System;
using UnityEngine;

namespace Tag.HexaStack
{
    public class BaseReward
    {
        public virtual RewardType GetRewardType()
        {
            return RewardType.Curreny;
        }

        public virtual int GetCurrencyId() { return 0; }

        public virtual int GetAmount() { return 0; }

        public virtual string GetAmountStringForUI() { return ""; }

        public virtual void GiveReward() { }

        public virtual void RemoveReward() { }

        public virtual void AddRewardValue(int amount) { }

        public virtual bool IsEnoughItem() { return false; }

        public virtual void ShowRewardAnimation(CurrencyAnimation animation, Vector3 pos, bool isUiAnimation, Transform endPos = null, Sprite itemSprite = null) { }

        public virtual BaseReward MultiplyReward(int multiplier) { return new BaseReward(); }

        public virtual string GetName() { return ""; }

        public virtual Sprite GetRewardImageSprite() { return null; }

        public virtual string GetDescription() { return ""; }

        public virtual void ShowRewardToastAnimation(Vector3 startPosition, Action action = null, Vector3 scale = default) { }
    }

    public enum RewardType
    {
        None,
        Curreny,
        NoAds,
        InfiniteLife
    }
}
