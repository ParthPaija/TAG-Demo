using UnityEngine;

namespace Tag.HexaStack
{
    public class AllItemCollectGoal : BaseLevelGoal
    {
        public AllItemCollectGoal()
        {
            goalType = GoalType.Item;
        }

        public override void UpdateGoal(GoalType goalType, int itemId, int value)
        {
            if (this.goalType != goalType)
                return;
            currentCount += value;
            currentCount = Mathf.Clamp(currentCount, 0, GoalCount);
        }

        public override BaseReward GetWinReward()
        {
            return new CurrencyReward { currencyID = CurrencyConstant.META_CURRENCY, curruncyValue = GoalCount };
        }

        public override Sprite GetRender()
        {
            return ResourceManager.Instance.AllItemGoalSprite;
        }

        public override bool IsPlayAnimation(GoalType goalType, int itemId)
        {
            if (goalType != this.goalType)
                return false;
            if (currentCount >= goalCount)
                return false;
            return true;
        }
    }
}
