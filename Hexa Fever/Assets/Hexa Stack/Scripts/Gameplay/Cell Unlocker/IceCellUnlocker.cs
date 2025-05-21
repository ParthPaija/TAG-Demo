using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class IceCellUnlocker : BaseCellUnlocker
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private List<IceTile> iceTiles = new List<IceTile>();
        [ShowInInspector] protected CountBaseObstacalData obstacalData;
        private int icePicesesCount = 3;
        [SerializeField] private int currentIcePicesesCount = 0;

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
            base.Init(baseCell, levelProgressData);
            SetLockerData(levelProgressData);
        }

        public virtual void OnRemoveSatck(BaseCell baseCell, int itemId)
        {
            if (!isLocked)
                return;

            if (myCell.AdjacentCells.Contains(baseCell))
            {
                RemoveIceTilesPices(currentIcePicesesCount);
            }
        }

        public override bool CanUseBooster()
        {
            return isLocked;
        }

        public override void OnBoosterUse(Action action)
        {
            base.OnBoosterUse(action);
            if (CanUseBooster())
            {
                RemoveIceTilesPices(currentIcePicesesCount);
                onBoosterUse?.Invoke();
                onBoosterUse = null;
            }
        }

        public override void Unlock()
        {
            base.Unlock();
        }

        public override void SaveData(LevelProgressData levelProgressData)
        {
            var playerData = levelProgressData;
            obstacalData.count = currentIcePicesesCount;
            playerData.UpdateObstacalCellData(myCell.CellId, SerializeUtility.SerializeObject(obstacalData));
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void RemoveIceTilesPices(int index)
        {
            if (iceTiles == null)
                return;

            for (int i = 0; i < iceTiles.Count; i++)
            {
                iceTiles[i].RemoveIcePiceses(index);
            }

            currentIcePicesesCount--;

            if (currentIcePicesesCount <= 0)
            {
                GameRuleManager.Instance.OnStackPlace(MyCell);
                Unlock();
                MainSceneUIManager.Instance.GetView<VFXView>().PlayItemAnimation(GoalType.Obsatcal, transform.position, 1, id);
                GameplayGoalHandler.Instance.UpdateGoals(GoalType.Obsatcal, id, 1);
            }

            GameplayManager.Instance.SaveAllDataOfLevel();
        }

        private void SetLockerData(LevelProgressData levelProgressData)
        {
            var playerData = levelProgressData;

            string data = "";

            if (playerData != null)
                data = playerData.GetObstacalData(myCell.CellId);

            if (!string.IsNullOrEmpty(data) && !string.IsNullOrWhiteSpace(data))
            {
                obstacalData = SerializeUtility.DeserializeObject<CountBaseObstacalData>(data);
            }
            else
            {
                obstacalData = new CountBaseObstacalData();
                obstacalData.count = icePicesesCount;
                playerData.UpdateObstacalCellData(myCell.CellId, SerializeUtility.SerializeObject(obstacalData));
            }
            currentIcePicesesCount = obstacalData.count;

            if (currentIcePicesesCount <= 0)
            {
                Unlock();
            }
            else
            {
                if (myCell.HasItem)
                {
                    GenrateIceTile();
                }
                for (int i = icePicesesCount; i > currentIcePicesesCount; i--)
                {
                    for (int j = 0; j < iceTiles.Count; j++)
                    {
                        iceTiles[j].RemoveIcePiceses(i);
                    }
                }
            }
        }

        private void GenrateIceTile()
        {
            ClearIceTiles();

            List<BaseItem> baseItems = myCell.ItemStack.GetItems();
            for (int i = 0; i < baseItems.Count; i++)
            {
                IceTile tile = Instantiate(ResourceManager.Instance.IceTilePrefab, transform);
                tile.transform.position = baseItems[i].transform.position;
                iceTiles.Add(tile);
            }
        }

        private void ClearIceTiles()
        {
            if (iceTiles == null)
                return;

            for (int i = 0; i < iceTiles.Count; i++)
            {
                Destroy(iceTiles[i].gameObject);
            }
            iceTiles.Clear();
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
