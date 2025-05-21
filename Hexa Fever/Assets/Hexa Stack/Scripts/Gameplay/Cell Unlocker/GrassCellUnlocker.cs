using UnityEngine;

namespace Tag.HexaStack
{
    public class GrassCellUnlocker : BaseCellUnlocker
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GameObject lockedObj;
        [SerializeField] private GameObject token;
        protected UnlockBaseObstacalData obstacalData;

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            if (GameplayManager.Instance != null)
            {
                GameplayManager.Instance.AddListenerOnStackOrItemRemove(OnRemoveSatck);
                GameplayManager.Instance.AddListenerOnStackItemRemoveOrAdd(SetPosition);
            }
        }

        private void OnDisable()
        {
            if (GameplayManager.Instance != null)
            {
                GameplayManager.Instance.RemoveListenerOnStackOrItemRemove(OnRemoveSatck);
                GameplayManager.Instance.AddListenerOnStackItemRemoveOrAdd(SetPosition);
            }
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Init(BaseCell baseCell, LevelProgressData levelProgressData)
        {
            base.Init(baseCell, levelProgressData);
            SetObject();
            SetLockerData(levelProgressData);
            token.SetActive(false);
            SetPosition(false, myCell);
        }

        public override void SaveData(LevelProgressData levelProgressData)
        {
            var playerData = levelProgressData;
            obstacalData.isUnlock = !isLocked;
            playerData.UpdateObstacalCellData(myCell.CellId, SerializeUtility.SerializeObject(obstacalData));
        }

        public override bool IsBlocker()
        {
            return false;
        }

        public override bool CanUseBooster()
        {
            return false;
        }

        public override bool IsDependendOnAdjacentCell()
        {
            return false;
        }

        public override bool IsDependendOnOwnCell()
        {
            return isLocked;
        }

        public override void Unlock()
        {
            base.Unlock();
            SoundHandler.Instance.PlaySound(SoundType.TileUnlock);
            SetObject();
            MainSceneUIManager.Instance.GetView<VFXView>().PlayBreadItemAnimation(GoalType.Obsatcal, transform.position, 1, id);
            GameplayGoalHandler.Instance.UpdateGoals(GoalType.Obsatcal, id, 1);
            GameplayManager.Instance.SaveAllDataOfLevel();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        public void SetPosition(bool isStart, BaseCell baseCell)
        {
            if (MyCell == baseCell && MyCell.HasItem)
            {
                token.SetActive(myCell.HasItem && !isStart && isLocked);
                token.transform.localPosition = transform.position.With(0, 0, MyCell.ItemStack.GetTopPosition().y);
            }
        }

        public void SetPosition(Vector3 pos)
        {
            token.SetActive(true);
            token.transform.localPosition = pos;
        }

        private void OnRemoveSatck(BaseCell baseCell, int itemId)
        {
            if (baseCell == myCell)
            {
                if (isLocked)
                {
                    Unlock();
                }
            }
        }

        private void SetLockerData(LevelProgressData levelProgressData)
        {
            var playerData = levelProgressData;

            string data = "";

            if (playerData != null)
                data = playerData.GetObstacalData(myCell.CellId);

            if (!string.IsNullOrEmpty(data) && !string.IsNullOrWhiteSpace(data))
            {
                obstacalData = SerializeUtility.DeserializeObject<UnlockBaseObstacalData>(data);
            }
            else
            {
                obstacalData = new UnlockBaseObstacalData();
                obstacalData.isUnlock = false;
                playerData.UpdateObstacalCellData(myCell.CellId, SerializeUtility.SerializeObject(obstacalData));
            }

            if (obstacalData.isUnlock)
            {
                base.Unlock();
                SetObject();
            }
        }

        private void SetObject()
        {
            lockedObj.SetActive(isLocked);
            token.SetActive(isLocked);
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
