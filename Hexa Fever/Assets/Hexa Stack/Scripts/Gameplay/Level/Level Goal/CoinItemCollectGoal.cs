using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class CoinItemCollectGoal : BaseLevelGoal
    {
        [ItemId, ReadOnly] public int itemId = 9;

        public CoinItemCollectGoal()
        {
            goalType = GoalType.Item;
        }

        public override void UpdateGoal(GoalType goalType, int itemId, int value)
        {
            if (this.goalType != goalType)
                return;
            if (itemId == this.itemId && !IsGoalFullFilled())
            {
                currentCount += value;
            }
            currentCount = Mathf.Clamp(currentCount, 0, GoalCount);
        }

        public override BaseReward GetWinReward()
        {
            return new CurrencyReward { currencyID = CurrencyConstant.COINS, curruncyValue = GoalCount };
        }

        public override bool IsPlayAnimation(GoalType goalType, int itemId)
        {
            if (goalType != this.goalType)
                return false;
            if (IsGoalFullFilled())
                return false;
            return this.itemId == itemId;
        }

        public override Sprite GetRender()
        {
            return ResourceManager.Instance.GetGoalSprite(goalType, itemId);
        }

        public override int GetGoalItemId()
        {
            return itemId;
        }
    }
}
