using System;
using UnityEngine;

namespace Tag.HexaStack
{
    public class AdjacentCellStackCellUnlocker : BaseCellUnlocker
    {
        #region PUBLIC_VARS

        public GameObject[] RemoveableItemObjs { get => removeableItemObjs; set => removeableItemObjs = value; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private bool isUnlockWithBooster = false;
        [SerializeField] internal int removeItemCount;
        [SerializeField] GameObject[] removeableItemObjs;
        [SerializeField] private ParticleSystem[] removeableItemsFX;
        [SerializeField] private Transform particalParent;

        protected CountBaseObstacalData obstacalData;

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            if (GameplayManager.Instance != null)
                GameplayManager.Instance.AddListenerOnStackRemove(OnRemoveSatck);
        }

        private void OnDisable()
        {
            if (GameplayManager.Instance != null)
                GameplayManager.Instance.RemoveListenerOnStackRemove(OnRemoveSatck);
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Init(BaseCell baseCell, LevelProgressData levelProgressData)
        {
            baseCell.GetComponentInChildren<MeshRenderer>().gameObject.SetActive(false);
            base.Init(baseCell, levelProgressData);
            SetLockerData(levelProgressData);
        }

        public override bool IsLocked()
        {
            return removeItemCount > 0;
        }

        public override void Unlock()
        {
            myCell.GetComponentInChildren<MeshRenderer>(true).gameObject.SetActive(true);
            base.Unlock();
            SetObject(false);
        }

        public override bool CanUseBooster()
        {
            return isUnlockWithBooster && isLocked;
        }

        public override void OnBoosterUse(Action action)
        {
            base.OnBoosterUse(action);
            if (CanUseBooster())
            {
                RemoveLocked();
                onBoosterUse?.Invoke();
                onBoosterUse = null;
            }
        }

        public virtual void OnRemoveSatck(BaseCell baseCell, int itemId)
        {
            if (!isLocked)
                return;

            if (myCell.AdjacentCells.Contains(baseCell))
            {
                RemoveLocked();
            }
        }

        public override void SaveData(LevelProgressData levelProgressData)
        {
            var playerData = levelProgressData;
            obstacalData.count = removeItemCount;
            playerData.UpdateObstacalCellData(myCell.CellId, SerializeUtility.SerializeObject(obstacalData));
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetLockerData(LevelProgressData levelProgressData)
        {
            SetObject(false);

            var playerData = levelProgressData;

            string data = "";

            if (playerData != null)
                data = playerData.GetObstacalData(myCell.CellId);

            if (!string.IsNullOrEmpty(data) && !string.IsNullOrWhiteSpace(data))
            {
                obstacalData = SerializeUtility.DeserializeObject<CountBaseObstacalData>(data);
                removeItemCount = obstacalData.count;
            }
            else
            {
                obstacalData = new CountBaseObstacalData();
                obstacalData.count = removeItemCount;
                playerData.UpdateObstacalCellData(myCell.CellId, SerializeUtility.SerializeObject(obstacalData));
            }

            for (int i = 0; i < removeItemCount; i++)
            {
                removeableItemObjs[i].SetActive(true);
            }

            if (removeItemCount <= 0)
            {
                Unlock();
            }
        }

        private void RemoveLocked()
        {
            removeableItemObjs[removeItemCount - 1].SetActive(false);
            ParticleSystem particleSystem = Instantiate(removeableItemsFX[removeItemCount - 1], particalParent);
            particleSystem.Play();
            removeItemCount--;
            SoundHandler.Instance.PlaySound(SoundType.WoodBreak);
            if (removeItemCount <= 0)
            {
                Unlock();
                MainSceneUIManager.Instance.GetView<VFXView>().PlayItemAnimation(GoalType.Obsatcal, transform.position, 1, id);
                GameplayGoalHandler.Instance.UpdateGoals(GoalType.Obsatcal, id, 1);
            }
        }

        private void SetObject(bool isActive)
        {
            for (int i = 0; i < removeableItemObjs.Length; i++)
            {
                removeableItemObjs[i].SetActive(isActive);
            }
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }

    public class CountBaseObstacalData
    {
        public int count;
    }
}
