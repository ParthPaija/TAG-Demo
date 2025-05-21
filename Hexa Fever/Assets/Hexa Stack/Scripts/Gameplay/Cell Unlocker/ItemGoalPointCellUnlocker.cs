using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class ItemGoalPointCellUnlocker : GoalPointCellUnlocker
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        protected CountBaseObstacalData obstacalData;
        [SerializeField] protected SpriteRenderer goalLoackImage;
        [SerializeField] private Dictionary<int, Sprite> goalLockSprite = new Dictionary<int, Sprite>();

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            GameplayManager.Instance.AddListenerOnItemRemove(OnItemRemove);
        }

        private void OnDisable()
        {
            GameplayManager.Instance.RemoveListenerOnItemRemove(OnItemRemove);
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Init(BaseCell baseCell, LevelProgressData levelProgressData)
        {
            meshRenderer = baseCell.GetComponentInChildren<MeshRenderer>(true);
            base.Init(baseCell, levelProgressData);
            myCell = baseCell;
            isLocked = true;
            levelGoal.InitGoal();

            SetObject();
            SetPosition();
            meshRenderer.material = ResourceManager.Instance.GetDefaultLockerMaterial();

            goalLoackImage.sprite = goalLockSprite[levelGoal.GetGoalItemId()];

            var playerData = levelProgressData;

            string data = "";

            if (playerData != null)
                data = playerData.GetObstacalData(myCell.CellId);

            if (!string.IsNullOrEmpty(data) && !string.IsNullOrWhiteSpace(data))
            {
                obstacalData = SerializeUtility.DeserializeObject<CountBaseObstacalData>(data);
                levelGoal.CurrentCount = obstacalData.count;
            }
            else
            {
                obstacalData = new CountBaseObstacalData();
                obstacalData.count = levelGoal.CurrentCount;
                playerData.UpdateObstacalCellData(myCell.CellId, SerializeUtility.SerializeObject(obstacalData));
            }

            levelGoal.CurrentCount = obstacalData.count;
            goalText.text = (levelGoal.GoalCount - levelGoal.CurrentCount).ToString();

            if (levelGoal.IsGoalFullFilled())
            {
                Unlock();
                GameRuleManager.Instance.AddOnRemoveItemCell(MyCell);
            }
        }

        public override void SaveData(LevelProgressData levelProgressData)
        {
            var playerData = levelProgressData;
            obstacalData.count = levelGoal.CurrentCount;
            playerData.UpdateObstacalCellData(myCell.CellId, SerializeUtility.SerializeObject(obstacalData));
        }

        public override void SetText()
        {
            base.SetText();
        }

        public void SetLoakImage()
        {
            goalLoackImage.sprite = goalLockSprite[levelGoal.GetGoalItemId()];
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        internal void OnItemRemove(int itemId, int amount)
        {
            levelGoal.UpdateGoal(GoalType.Item, itemId, amount);
            if (levelGoal.IsGoalFullFilled() && isLocked)
            {
                Unlock();
                VFXManager.Instance.PlayUnlockCellAnimation(myCell);
                GameRuleManager.Instance.AddOnRemoveItemCell(MyCell);
                return;
            }
            goalText.text = (levelGoal.GoalCount - levelGoal.CurrentCount).ToString();
            GameplayManager.Instance.SaveAllDataOfLevel();
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
