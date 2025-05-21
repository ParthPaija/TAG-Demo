using UnityEngine;

namespace Tag.HexaStack
{
    public class SpecificObstacleCollectGoal : BaseLevelGoal
    {
        [ObstacalId] public int obstacleId;

        public SpecificObstacleCollectGoal()
        {
            goalType = GoalType.Obsatcal;
        }

        public override void UpdateGoal(GoalType goalType, int itemId, int value)
        {
            if (this.goalType != goalType)
                return;
            if (itemId == this.obstacleId )
                currentCount += value;
            if (LevelEditorManager.Instance == null)
                currentCount = Mathf.Clamp(currentCount, 0, GoalCount);
        }

        public override bool IsPlayAnimation(GoalType goalType, int itemId)
        {
            if (goalType != this.goalType)
                return false;
            if(LevelEditorManager.Instance != null)
                return true;
            if (IsGoalFullFilled())
                return false;
            return this.obstacleId == itemId;
        }

        public override Sprite GetRender()
        {
            return ResourceManager.Instance.GetGoalSprite(goalType, obstacleId);
        }

        public override int GetGoalItemId()
        {
            return obstacleId;
        }
    }
}
