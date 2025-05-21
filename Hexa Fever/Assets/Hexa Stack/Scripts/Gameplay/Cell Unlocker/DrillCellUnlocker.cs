using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class DrillCellUnlocker : BaseCellUnlocker
    {
        #region PUBLIC_VARS

        [SerializeField] private Transform drillModel;
        [SerializeField] private float drillRotationSpeed = 720f;
        [SerializeField] private ParticleSystem drillEffect;
        [SerializeField] private Transform drillDirection; // Direction the drill will operate in

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private bool isUnlockWithBooster = true;
        protected UnlockBaseObstacalData obstacalData;
        private Coroutine drillAnimationCoroutine;

        // Define an enum for the 6 directions in a hexagonal grid
        public enum HexDirection
        {
            North,      // Top
            NorthEast,  // Top Right
            SouthEast,  // Bottom Right
            South,      // Bottom
            SouthWest,  // Bottom Left
            NorthWest   // Top Left
        }

        [SerializeField] private HexDirection drillHexDirection;

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

        public override bool IsLocked()
        {
            return isLocked;
        }

        public override void Unlock()
        {
            base.Unlock();
            SoundHandler.Instance.PlaySound(SoundType.TileUnlock);
            
            // Start drilling animation and effect
            if (drillAnimationCoroutine == null)
                drillAnimationCoroutine = StartCoroutine(DrillAnimation());
            
            MainSceneUIManager.Instance.GetView<VFXView>().PlayItemAnimation(GoalType.Obsatcal, transform.position, 1, id);
            GameplayGoalHandler.Instance.UpdateGoals(GoalType.Obsatcal, id, 1);
            GameplayManager.Instance.SaveAllDataOfLevel();
            VFXManager.Instance.PlayUnlockCellAnimation(myCell);
        }

        public override bool CanUseBooster()
        {
            return isUnlockWithBooster && isLocked;
        }

        public override void OnBoosterUse(System.Action action = null)
        {
            base.OnBoosterUse(action);
            if (CanUseBooster())
            {
                Unlock();
                onBoosterUse?.Invoke();
                onBoosterUse = null;
            }
        }

        public override void SaveData(LevelProgressData levelProgressData)
        {
            var playerData = levelProgressData;
            obstacalData.isUnlock = !isLocked;
            playerData.UpdateObstacalCellData(myCell.CellId, SerializeUtility.SerializeObject(obstacalData));
        }

        public void OnRemoveSatck(BaseCell baseCell, int itemId)
        {
            if (!isLocked)
                return;

            if (myCell.AdjacentCells.Contains(baseCell))
            {
                Unlock();
            }
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
                base.Unlock();
            }
        }

        [Button]
        private List<BaseCell> GetCellsInDirection()
        {
            List<BaseCell> cellsInDirection = new List<BaseCell>();
            
            // Get the first cell in the direction
            BaseCell firstCell = GetAdjacentCellInDirection(myCell, drillHexDirection);
            if (firstCell != null)
            {
                cellsInDirection.Add(firstCell);
                
                // Get the second cell in the same direction
                BaseCell secondCell = GetAdjacentCellInDirection(firstCell, drillHexDirection);
                if (secondCell != null)
                {
                    cellsInDirection.Add(secondCell);
                }
            }
            
            return cellsInDirection;
        }

        private BaseCell GetAdjacentCellInDirection(BaseCell fromCell, HexDirection direction)
        {
            if (fromCell == null || fromCell.AdjacentCells == null || fromCell.AdjacentCells.Count == 0)
                return null;
            
            // Calculate the expected position based on the direction
            Vector3 expectedPosition = CalculatePositionInDirection(fromCell.transform.position, direction);
            
            // Find the closest cell to the expected position
            BaseCell closestCell = null;
            float closestDistance = float.MaxValue;
            
            foreach (BaseCell adjacentCell in fromCell.AdjacentCells)
            {
                float distance = Vector3.Distance(adjacentCell.transform.position, expectedPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCell = adjacentCell;
                }
            }
            
            // Only return if it's reasonably close to the expected position
            if (closestDistance < 2.0f) // Adjust threshold as needed
            {
                return closestCell;
            }
            
            return null;
        }

        private Vector3 CalculatePositionInDirection(Vector3 fromPosition, HexDirection direction)
        {
            // Assuming a regular hexagonal grid with these offsets
            // Adjust these values based on your actual grid spacing
            float horizontalSpacing = 1.0f;
            float verticalSpacing = 0.866f; // sqrt(3)/2
            
            switch (direction)
            {
                case HexDirection.North:
                    return fromPosition + new Vector3(0, 0, verticalSpacing * 2);
                    
                case HexDirection.NorthEast:
                    return fromPosition + new Vector3(horizontalSpacing, 0, verticalSpacing);
                    
                case HexDirection.SouthEast:
                    return fromPosition + new Vector3(horizontalSpacing, 0, -verticalSpacing);
                    
                case HexDirection.South:
                    return fromPosition + new Vector3(0, 0, -verticalSpacing * 2);
                    
                case HexDirection.SouthWest:
                    return fromPosition + new Vector3(-horizontalSpacing, 0, -verticalSpacing);
                    
                case HexDirection.NorthWest:
                    return fromPosition + new Vector3(-horizontalSpacing, 0, verticalSpacing);
                    
                default:
                    return fromPosition;
            }
        }

        private void DrillCellsInDirection(List<BaseCell> cellsToBreak)
        {
            foreach (BaseCell cell in cellsToBreak)
            {
                if (cell.HasItem)
                {
                    // Remove the top item
                    //cell.ItemStack.RemoveTopItem();
                    
                    // Play effect at the cell position
                    if (drillEffect != null)
                    {
                        ParticleSystem effect = Instantiate(drillEffect, cell.transform.position, Quaternion.identity);
                        effect.Play();
                        Destroy(effect.gameObject, effect.main.duration);
                    }
                    
                    // Play sound
                    SoundHandler.Instance.PlaySound(SoundType.WoodBreak);
                }
            }
        }
        #endregion

        #region CO-ROUTINES
        private IEnumerator DrillAnimation()
        {
            // Get cells in the drill direction
            List<BaseCell> cellsToBreak = GetCellsInDirection();
            
            // Start drill rotation animation
            float duration = 1.5f;
            float timer = 0f;
            
            // Play drill sound
            
            while (timer < duration)
            {
                timer += Time.deltaTime;
                
                // Rotate the drill model
                if (drillModel != null)
                {
                    drillModel.Rotate(Vector3.forward, drillRotationSpeed * Time.deltaTime);
                }
                
                yield return null;
            }
            
            // Break stones in the direction
            DrillCellsInDirection(cellsToBreak);
            
            // End animation
            drillAnimationCoroutine = null;
        }
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}
