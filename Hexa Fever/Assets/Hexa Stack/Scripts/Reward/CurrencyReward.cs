using System;
using UnityEngine;

namespace Tag.HexaStack
{
    public class CurrencyReward : BaseReward
    {
        [CurrencyId] public int currencyID;
        public int curruncyValue;

        public override void GiveReward()
        {
            DataManager.Instance.GetCurrency(currencyID).Add(curruncyValue);
        }

        public override void RemoveReward()
        {
            DataManager.Instance.GetCurrency(currencyID).Add(-curruncyValue);
        }

        public override int GetAmount()
        {
            return curruncyValue;
        }

        public override string GetAmountStringForUI()
        {
            return curruncyValue.ToString();
        }

        public override RewardType GetRewardType()
        {
            return RewardType.Curreny;
        }

        public override void AddRewardValue(int amount)
        {
            curruncyValue += amount;
        }

        public override int GetCurrencyId()
        {
            return currencyID;
        }

        public override bool IsEnoughItem()
        {
            return GetAmount() <= DataManager.Instance.GetCurrency(currencyID).Value;
        }

        public override Sprite GetRewardImageSprite()
        {
            return ResourceManager.Instance.GetCurrencySprite(currencyID);
        }

        public override string GetName()
        {
            return DataManager.Instance.GetCurrency(currencyID).currencyName;
        }

        public override BaseReward MultiplyReward(int multiplier)
        {
            return new CurrencyReward { currencyID = currencyID, curruncyValue = curruncyValue * multiplier };
        }

        public override void ShowRewardAnimation(CurrencyAnimation animation, Vector3 pos, bool isUiAnimation, Transform endPos = null, Sprite itemSprite = null)
        {
            if (animation == null)
                return;
            if (isUiAnimation)
            {
                animation.UIStartAnimation(pos, endPos, curruncyValue, false);
            }
            else
            {
                if (itemSprite == null)
                    animation.StartAnimation(pos, curruncyValue, null);
                else
                    animation.StartAnimation(pos, curruncyValue, itemSprite);
            }
        }
        public override void ShowRewardToastAnimation(Vector3 startPosition, Action action = null, Vector3 scale = default)
        {
            MainSceneUIManager.Instance.GetView<VFXView>().RewardToastAnimation.PlayAnimation(GetRewardImageSprite(), GetAmount(), startPosition, action, scale);
        }
    }
}
