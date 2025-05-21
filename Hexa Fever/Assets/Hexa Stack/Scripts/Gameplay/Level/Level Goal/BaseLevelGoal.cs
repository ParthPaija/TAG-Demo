using Sirenix.OdinInspector;
using UnityEngine;

namespace Tag.HexaStack
{
    public abstract class BaseLevelGoal
    {
        [SerializeField] public GoalType goalType = GoalType.Item;
        [SerializeField] protected int goalCount;
        [ShowInInspector] protected int currentCount;

        public int GoalCount
        {
            get { return goalCount; }
            set { goalCount = value; }
        }

        public int CurrentCount { get => currentCount; set => currentCount = value; }

        public virtual void InitGoal()
        {
            currentCount = 0;
        }

        public virtual BaseReward GetWinReward()
        {
            return null;
        }

        public virtual void UpdateGoal(GoalType goalType, int itemId, int value)
        {

        }

        public virtual bool IsPlayAnimation(GoalType goalType, int itemId)
        {
            if (goalType != this.goalType)
                return false;
            return false;
        }

        public virtual Sprite GetRender()
        {
            return null;
        }

        public virtual bool IsGoalFullFilled()
        {
            return currentCount >= goalCount;
        }

        public virtual int GetGoalItemId()
        {
            return 0;
        }
    }

    public enum GoalType
    {
        None,
        Item,
        Obsatcal
    }
}
