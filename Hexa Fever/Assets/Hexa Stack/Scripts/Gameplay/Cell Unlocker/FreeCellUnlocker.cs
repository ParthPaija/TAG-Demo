using UnityEngine;

namespace Tag.HexaStack
{
    public class FreeCellUnlocker : BaseCellUnlocker
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GameObject lockedObj;
        private MeshRenderer meshRenderer;
        protected UnlockBaseObstacalData obstacalData;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Init(BaseCell baseCell, LevelProgressData levelProgressData)
        {
            meshRenderer = baseCell.GetComponentInChildren<MeshRenderer>(true);
            base.Init(baseCell, levelProgressData);
            SetObject();
            meshRenderer.material = ResourceManager.Instance.GetDefaultLockerMaterial();
            SetLockerData(levelProgressData);
        }

        public override void Unlock()
        {
            meshRenderer.material = ResourceManager.Instance.GetDefaultCellmaterial();
            base.Unlock();
            SoundHandler.Instance.PlaySound(SoundType.TileUnlock);
            SetObject();
            MainSceneUIManager.Instance.GetView<VFXView>().PlayItemAnimation(GoalType.Obsatcal, transform.position, 1, id);
            GameplayGoalHandler.Instance.UpdateGoals(GoalType.Obsatcal, id, 1);
            GameplayManager.Instance.CancelUndoBooster();
            GameplayManager.Instance.SaveAllDataOfLevel();
            VFXManager.Instance.PlayUnlockCellAnimation(myCell);
        }

        public override void OnClick()
        {
            base.OnClick();

            if (AdManager.Instance == null)
            {
                Unlock();
                GameplayManager.testPrintData.OnTileAdWatch();
                return;
            }

            AdManager.Instance.ShowRewardedAd(() =>
            {
                GameplayManager.Instance.AdTileWatchCount++;
                AnalyticsManager.Instance.LogGAEvent("AdTile : " + GameplayManager.Instance.CurrentHandler.GetCurrentLevelEventString() + " : " + GameplayManager.Instance.AdTileWatchCount);
                Unlock();
                GameplayManager.testPrintData.OnTileAdWatch();
            }, RewardAdShowCallType.AdLocker, "AdLocker");
        }

        public override void SaveData(LevelProgressData levelProgressData)
        {
            var playerData = levelProgressData;
            obstacalData.isUnlock = !isLocked;
            playerData.UpdateObstacalCellData(myCell.CellId, SerializeUtility.SerializeObject(obstacalData));
        }

        public override bool IsDependendOnAdjacentCell()
        {
            return false;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

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
                meshRenderer.material = ResourceManager.Instance.GetDefaultCellmaterial();
                base.Unlock();
                SetObject();
            }
        }

        private void SetObject()
        {
            lockedObj.SetActive(isLocked);
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }

    public class UnlockBaseObstacalData
    {
        public bool isUnlock;
    }
}
