using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Tag.HexaStack
{
    [Serializable]
    public class DebugConfig
    {
        [Tooltip("Master toggle for all debug logs")]
        public bool enableDebugLogs = false;

        [Tooltip("Movement progress tracking logs")]
        public bool movementProgressLogs = false;

        [Tooltip("Cell selection algorithm logs")]
        public bool cellSelectionLogs = false;

        [Tooltip("Cell priority and locker logs")]
        public bool cellPriorityLogs = false;

        [Tooltip("Stack operation logs")]
        public bool stackOperationLogs = false;

        [Tooltip("Item merging logs")]
        public bool itemMergeLogs = false;
    }

    public class GameRuleManager : SerializedManager<GameRuleManager>
    {
        #region PUBLIC_VARS
        [BoxGroup("Debug Settings")]
        [SerializeField] private DebugConfig debugConfig = new DebugConfig();

        [BoxGroup("Debug Settings")]
        [Button("Enable All Debug Logs")]
        private void EnableAllDebugLogs()
        {
            debugConfig.enableDebugLogs = true;
            debugConfig.movementProgressLogs = true;
            debugConfig.cellSelectionLogs = true;
            debugConfig.cellPriorityLogs = true;
            debugConfig.stackOperationLogs = true;
            debugConfig.itemMergeLogs = true;
        }

        [BoxGroup("Debug Settings")]
        [Button("Disable All Debug Logs")]
        private void DisableAllDebugLogs()
        {
            debugConfig.enableDebugLogs = false;
            debugConfig.movementProgressLogs = false;
            debugConfig.cellSelectionLogs = false;
            debugConfig.cellPriorityLogs = false;
            debugConfig.stackOperationLogs = false;
            debugConfig.itemMergeLogs = false;
        }

        public bool IsMovementInProgress
        {
            get
            {
                if (debugConfig.enableDebugLogs && debugConfig.movementProgressLogs)
                    Debug.LogError("IsMovementInProgress :- " + stackCO != null + " --- " + isGoalAnimationInProgress);
                return stackCO != null || isGoalAnimationInProgress;
            }
        }
        public bool IsGoalAnimationInProgress { get => isGoalAnimationInProgress; set => isGoalAnimationInProgress = value; }

        // Event that triggers when items are cleared (itemId, count)
        public event System.Action<int, int> OnItemsCleared;

        // Event that triggers when a stack placement is initiated on a cell
        public event System.Action<BaseCell> OnStackPlaced;

        // Public getter for the number of items needed to trigger a clear
        public int ItemsNeededForClear => noOfItemToDestroy;

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private BaseItemMovementModeSelctor itemMovementMode;
        [SerializeField] private float mergeAnimationDuration = 0.5f;
        [SerializeField] private float destroyAnimationDuration = 0.5f;
        [SerializeField] private int noOfItemToDestroy = 10;
        [SerializeField] private float delayBetweenEachItem = 0.5f;
        [SerializeField] private float delayBetweenEachItemForRotatios = 0.5f;

        [SerializeField] private ParticleSystem destroyPS;

        [SerializeField] List<BaseItem> itemsToMerge = new List<BaseItem>();
        [SerializeField] private List<BaseCell> removeItemCells = new List<BaseCell>();
        private Coroutine stackCO;
        [ShowInInspector] private List<Tween> activeTweens = new List<Tween>();
        [SerializeField] private CellDataForMovement currentCellData = new CellDataForMovement();
        private bool isMovementInProgress = false;
        private bool isGoalAnimationInProgress = false;
        //private float movementStartTime = 0f; // Track when movement started
        //private float maxMovementDuration = 10f; // Maximum duration for any movement (in seconds)

        FMOD.Studio.EventInstance Tile_Remove;
        FMOD.Studio.PARAMETER_DESCRIPTION Tile_Remove_pd;
        FMOD.Studio.PARAMETER_ID Tile_Remove_pid;

        FMOD.Studio.EventInstance Tile_Move;
        FMOD.Studio.PARAMETER_DESCRIPTION Tile_Move_pd;
        FMOD.Studio.PARAMETER_ID Tile_Move_pid;

        #endregion

        #region UNITY_CALLBACKS

        public override void Awake()
        {
            base.Awake();
            activeTweens = new List<Tween>();
        }

        //private void Update()
        //{
        //    // Safety check: if movement has been in progress for too long, reset the flag
        //    if (isMovementInProgress && Time.time - movementStartTime > maxMovementDuration)
        //    {
        //        if (debugConfig.enableDebugLogs && debugConfig.movementProgressLogs)
        //            Debug.LogError("Movement in progress timeout - resetting flag after " + maxMovementDuration + " seconds");
        //        isMovementInProgress = false;
        //    }
        //}

        #endregion

        #region PUBLIC_FUNCTIONS

        public void OnStackPlace(BaseCell targetCell)
        {
            // Invoke the new event at the start
            OnStackPlaced?.Invoke(targetCell);

            if (stackCO == null && activeTweens.Count == 0)
            {
                if (!isMovementInProgress)
                {
                    isMovementInProgress = true;
                    if (debugConfig.enableDebugLogs && debugConfig.movementProgressLogs)
                        Debug.LogError("Is Movement  in Progress True On Stack Place Jayare Kai no hoy ");
                }
                isGoalAnimationInProgress = false;
                if (itemMovementMode.SelecteMode() == ItemMovementMode.Falcon)
                    stackCO = StartCoroutine(DoStack(targetCell));
                else
                    stackCO = StartCoroutine(DoStackNew(targetCell));
            }
            else
            {
                if (!isMovementInProgress)
                {
                    isMovementInProgress = true;
                    if (debugConfig.enableDebugLogs && debugConfig.movementProgressLogs)
                        Debug.LogError("Is Movement  in Progress True On Stack Place Jayer Movement Alaready Chalu hoy");
                }
                if (targetCell.ItemStack != null && !removeItemCells.Contains(targetCell))
                    removeItemCells.Add(targetCell);
            }
        }

        public void OnStackSwapBooster(BaseCell putCell, BaseCell pickCell)
        {
            removeItemCells.Add(putCell);
            removeItemCells.Add(pickCell);
            OnStackPlace(putCell);
        }

        public void OnLevelComplate()
        {
            isMovementInProgress = false;
            if (debugConfig.enableDebugLogs && debugConfig.movementProgressLogs)
                Debug.LogError("Is Movement  in Progress False LEvel Complate");
            activeTweens = new List<Tween>();
            if (stackCO != null)
            {
                StopCoroutine(stackCO);
            }
            StopAllCoroutines();
            stackCO = null;
        }

        public void RemoveSatck(BaseCell baseCell, Action action, bool isScoreCountOnItemRemove = false)
        {
            if (baseCell.ItemStack != null)
                StartCoroutine(RemoveExcessItems(baseCell, baseCell.ItemStack.GetItemReverse(), action, isScoreCountOnItemRemove));
        }

        public void AddOnRemoveItemCell(BaseCell targetCell)
        {
            if (targetCell.ItemStack != null && !removeItemCells.Contains(targetCell))
                removeItemCells.Add(targetCell);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private bool IsSameItemType(BaseItemStack stack1, BaseItemStack stack2)
        {
            BaseItem topItem1 = GetTopItem(stack1);
            BaseItem topItem2 = GetTopItem(stack2);

            if (topItem1 == null || topItem2 == null)
                return false;

            return topItem1.ItemId == topItem2.ItemId;
        }

        private BaseItem GetTopItem(BaseItemStack stack)
        {
            if (stack == null)
                return null;

            return stack.GetTopItem();
        }

        private BaseCell GetBestCell(BaseCell targetCell, BaseCell adjacentCell)
        {
            if (adjacentCell == null)
                return targetCell;

            if (adjacentCell.HasItem && !adjacentCell.IsCellLocked() && IsSameItemType(targetCell.ItemStack, adjacentCell.ItemStack))
            {
                List<BaseItem> topItemsTargetCell = targetCell.ItemStack.GetTopSameTypeItem();
                List<BaseItem> topItemsAdjacentCell = adjacentCell.ItemStack.GetTopSameTypeItem();

                int targetCellTopItem = topItemsTargetCell.Count;
                int targetCellTopItemAdjacent = topItemsAdjacentCell.Count;

                foreach (var cell in targetCell.AdjacentCells)
                {
                    if (IsSameItemType(targetCell.ItemStack, cell.ItemStack))
                        targetCellTopItem += cell.HasItem ? cell.ItemStack.GetTopSameTypeItem().Count : 0;
                }

                foreach (var cell in adjacentCell.AdjacentCells)
                {
                    if (IsSameItemType(adjacentCell.ItemStack, cell.ItemStack))
                        targetCellTopItemAdjacent += cell.HasItem ? cell.ItemStack.GetTopSameTypeItem().Count : 0;
                }

                if (debugConfig.enableDebugLogs && debugConfig.cellSelectionLogs)
                {
                    Debug.LogError("Target Cell Item :- " + targetCellTopItem + " -- " + targetCell.IsCellUnlockAdjacentAndOwnCellLocker() + " Cell Name " + targetCell.name);
                    Debug.LogError("Adjacent Cell Item :- " + targetCellTopItemAdjacent + " -- " + adjacentCell.IsCellUnlockAdjacentAndOwnCellLocker() + " Cell Name " + adjacentCell.name);
                }

                if ((targetCellTopItem >= noOfItemToDestroy || targetCellTopItemAdjacent >= noOfItemToDestroy) && ((targetCell.IsCellUnlockAdjacentAndOwnCellLocker()) ||
                     adjacentCell.IsCellUnlockAdjacentAndOwnCellLocker()))
                {
                    if (debugConfig.enableDebugLogs && debugConfig.cellSelectionLogs)
                        Debug.LogError("Locker Algo Target Cell");

                    CellDataForMovement tempTargetCellData = new CellDataForMovement()
                    {
                        cell = targetCell,
                        unlockerCount = targetCell.GetCellUnlockerCount(),
                        protity = targetCell.GetPriorityAdjacentCells()
                    };

                    if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                        Debug.LogError("Target Cell Data " + tempTargetCellData.cell.name + " Prority " + tempTargetCellData.protity);

                    CellDataForMovement tempAdjacentCellData = new CellDataForMovement()
                    {
                        cell = adjacentCell,
                        unlockerCount = adjacentCell.GetCellUnlockerCount(),
                        protity = adjacentCell.GetPriorityAdjacentCells()
                    };

                    if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                        Debug.LogError("Adjacent Cell Data " + tempAdjacentCellData.cell.name + " Prority " + tempAdjacentCellData.protity);

                    CellDataForMovement cellDataForMovement = new CellDataForMovement();
                    cellDataForMovement = GetBestCellMovementData(tempTargetCellData, tempAdjacentCellData);

                    if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                        Debug.LogError("Result of Given Cell Data " + cellDataForMovement.cell.name + " Prority " + cellDataForMovement.protity);

                    if (currentCellData.cell != null)
                    {
                        if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                            Debug.LogError("Current Cell Data " + currentCellData.cell.name + " Prority " + currentCellData.protity);
                        currentCellData = GetBestCellMovementData(currentCellData, cellDataForMovement);
                        if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                            Debug.LogError("Result Of Current And Given Cell Data " + currentCellData.cell.name + " Prority " + currentCellData.protity);
                        return currentCellData.cell;
                    }
                    currentCellData = cellDataForMovement;
                    if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                        Debug.LogError("Result of current Data Null Cell Data " + currentCellData.cell.name + " Prority " + currentCellData.protity);
                    return cellDataForMovement.cell;
                }
                else if (currentCellData.cell != null)
                    return currentCellData.cell;
                else
                {
                    if (debugConfig.enableDebugLogs && debugConfig.cellSelectionLogs)
                        Debug.LogError("Loker Vagar No Algo");
                    if (topItemsTargetCell.Count == targetCell.ItemStack.GetItems().Count)
                    {
                        return targetCell;
                    }
                    else
                    {
                        if (topItemsAdjacentCell.Count == adjacentCell.ItemStack.GetItems().Count)
                        {
                            return adjacentCell;
                        }
                        else
                        {
                            if (targetCell.ItemStack.GetUniqueItemCount() <= adjacentCell.ItemStack.GetUniqueItemCount())
                            {
                                return targetCell;
                            }
                            else
                            {
                                return adjacentCell;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private BaseCell GetBestCellNewMode(BaseCell targetCell, BaseCell adjacentCell)
        {
            if (adjacentCell == null)
                return targetCell;

            if (adjacentCell.HasItem && !adjacentCell.IsCellLocked() && IsSameItemType(targetCell.ItemStack, adjacentCell.ItemStack))
            {
                List<BaseItem> topItemsTargetCell = targetCell.ItemStack.GetTopSameTypeItem();
                List<BaseItem> topItemsAdjacentCell = adjacentCell.ItemStack.GetTopSameTypeItem();

                List<int> bottomItemTargetCell = targetCell.ItemStack.GetBottomItem();
                List<int> bottomItemAdjacentCell = adjacentCell.ItemStack.GetBottomItem();

                int targetCellTopItem = topItemsTargetCell.Count;
                int targetCellTopItemAdjacent = topItemsAdjacentCell.Count;

                foreach (var cell in targetCell.AdjacentCells)
                {
                    if (IsSameItemType(targetCell.ItemStack, cell.ItemStack))
                        targetCellTopItem += cell.HasItem ? cell.ItemStack.GetTopSameTypeItem().Count : 0;
                }

                foreach (var cell in adjacentCell.AdjacentCells)
                {
                    if (IsSameItemType(adjacentCell.ItemStack, cell.ItemStack))
                        targetCellTopItemAdjacent += cell.HasItem ? cell.ItemStack.GetTopSameTypeItem().Count : 0;
                }

                if ((targetCellTopItem >= noOfItemToDestroy || targetCellTopItemAdjacent >= noOfItemToDestroy) && ((targetCell.IsCellUnlockAdjacentAndOwnCellLocker()) ||
                     adjacentCell.IsCellUnlockAdjacentAndOwnCellLocker()))
                {
                    if (debugConfig.enableDebugLogs && debugConfig.cellSelectionLogs)
                        Debug.LogError("Locker Algo Target Cell");

                    CellDataForMovement tempTargetCellData = new CellDataForMovement()
                    {
                        cell = targetCell,
                        unlockerCount = targetCell.GetCellUnlockerCount(),
                        protity = targetCell.GetPriorityAdjacentCells()
                    };

                    if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                        Debug.LogError("Target Cell Data " + tempTargetCellData.cell.name + " Prority " + tempTargetCellData.protity);

                    CellDataForMovement tempAdjacentCellData = new CellDataForMovement()
                    {
                        cell = adjacentCell,
                        unlockerCount = adjacentCell.GetCellUnlockerCount(),
                        protity = adjacentCell.GetPriorityAdjacentCells()
                    };

                    if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                        Debug.LogError("Adjacent Cell Data " + tempAdjacentCellData.cell.name + " Prority " + tempAdjacentCellData.protity);

                    CellDataForMovement cellDataForMovement = new CellDataForMovement();
                    cellDataForMovement = GetBestCellMovementData(tempTargetCellData, tempAdjacentCellData);

                    if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                        Debug.LogError("Result of Given Cell Data " + cellDataForMovement.cell.name + " Prority " + cellDataForMovement.protity);

                    if (currentCellData.cell != null)
                    {
                        if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                            Debug.LogError("Current Cell Data " + currentCellData.cell.name + " Prority " + currentCellData.protity);
                        currentCellData = GetBestCellMovementData(currentCellData, cellDataForMovement);
                        if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                            Debug.LogError("Result Of Current And Given Cell Data " + currentCellData.cell.name + " Prority " + currentCellData.protity);
                        return currentCellData.cell;
                    }
                    currentCellData = cellDataForMovement;
                    if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                        Debug.LogError("Result of current Data Null Cell Data " + currentCellData.cell.name + " Prority " + currentCellData.protity);
                    return cellDataForMovement.cell;
                }
                else if (currentCellData.cell != null)
                    return currentCellData.cell;
                else if (targetCell.ItemStack.GetUniqueItemCount() <= 1)
                {
                    if (debugConfig.enableDebugLogs && debugConfig.cellSelectionLogs)
                        Debug.LogError("Loker Vagar No Algo");
                    List<int> topItemTemp = new List<int>();
                    foreach (BaseCell cell in adjacentCell.AdjacentCells)
                    {
                        if (cell.HasItem && !cell.IsCellLocked() && !topItemTemp.Contains(cell.ItemStack.GetTopItem().ItemId))
                        {
                            topItemTemp.Add(cell.ItemStack.GetTopItem().ItemId);
                        }
                    }
                    for (int i = 0; i < bottomItemAdjacentCell.Count; i++)
                    {
                        if (!topItemTemp.Contains(bottomItemAdjacentCell[i]))
                        {
                            return adjacentCell;
                        }
                    }
                    return targetCell;
                }
                else if (adjacentCell.ItemStack.GetUniqueItemCount() <= 1)
                {
                    if (debugConfig.enableDebugLogs && debugConfig.cellSelectionLogs)
                        Debug.LogError("Loker Vagar No Algo");
                    List<int> topItemTemp = new List<int>();
                    foreach (BaseCell cell in targetCell.AdjacentCells)
                    {
                        if (cell.HasItem && !cell.IsCellLocked() && !topItemTemp.Contains(cell.ItemStack.GetTopItem().ItemId))
                        {
                            topItemTemp.Add(cell.ItemStack.GetTopItem().ItemId);
                        }
                    }
                    for (int i = 0; i < bottomItemTargetCell.Count; i++)
                    {
                        if (!topItemTemp.Contains(bottomItemTargetCell[i]))
                        {
                            return targetCell;
                        }
                    }
                    return adjacentCell;
                }
                else
                {
                    if (debugConfig.enableDebugLogs && debugConfig.cellSelectionLogs)
                        Debug.LogError("Loker Vagar No Algo");
                    List<int> topItemTempTargetCell = new List<int>();
                    bool isAllItemClearedForAdjacentCell = true;

                    foreach (BaseCell cell in targetCell.AdjacentCells)
                    {
                        if (cell.HasItem && !cell.IsCellLocked() && !topItemTempTargetCell.Contains(cell.ItemStack.GetTopItem().ItemId))
                        {
                            topItemTempTargetCell.Add(cell.ItemStack.GetTopItem().ItemId);
                        }
                    }

                    for (int i = 0; i < bottomItemTargetCell.Count; i++)
                    {
                        if (!topItemTempTargetCell.Contains(bottomItemTargetCell[i]))
                        {
                            isAllItemClearedForAdjacentCell = false;
                            break;
                        }
                    }
                    if (isAllItemClearedForAdjacentCell)
                    {
                        return adjacentCell;
                    }

                    List<int> topItemTempAdjacentCell = new List<int>();
                    bool isAllItemClearedForTargetCell = true;

                    foreach (BaseCell cell in adjacentCell.AdjacentCells)
                    {
                        if (cell.HasItem && !cell.IsCellLocked() && !topItemTempAdjacentCell.Contains(cell.ItemStack.GetTopItem().ItemId))
                        {
                            topItemTempAdjacentCell.Add(cell.ItemStack.GetTopItem().ItemId);
                        }
                    }
                    for (int i = 0; i < bottomItemAdjacentCell.Count; i++)
                    {
                        if (!topItemTempAdjacentCell.Contains(bottomItemAdjacentCell[i]))
                        {
                            isAllItemClearedForTargetCell = false;
                            break;
                        }
                    }
                    if (isAllItemClearedForTargetCell)
                    {
                        return targetCell;
                    }

                }
                return GetBestCell(targetCell, adjacentCell);
            }
            return null;
        }

        private IEnumerator AddCoinItemInCell(BaseCell baseCell, int noOfDestroy, int itemId)
        {
            if (itemId == 9)
                yield break;

            int numberOfCoins = (noOfDestroy / noOfItemToDestroy) + (noOfDestroy % noOfItemToDestroy);

            if (baseCell.HasItem)
            {
                for (int i = 0; i < numberOfCoins; i++)
                {
                    BaseItem baseItem = Instantiate(ResourceManager.Instance.GetItem(9));
                    baseItem.transform.localScale = Vector3.zero;
                    baseItem.transform.SetParent(baseCell.ItemStack.transform);
                    baseCell.ItemStack.AddItem(baseItem);
                    baseItem.transform.localRotation = Quaternion.identity;
                    baseItem.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InBack);
                    baseItem.transform.DOLocalJump(baseCell.ItemStack.GetTopPosition(), 2, 1, 0.6f);
                    yield return new WaitForSeconds(0.03f);
                }
            }
            else
            {
                BaseItemStack itemStack = Instantiate(ResourceManager.Instance.ItemStack, baseCell.transform);
                itemStack.transform.localRotation = Quaternion.identity;
                itemStack.transform.localPosition = new Vector3(0, 0.1f, 0);
                baseCell.ItemStack = itemStack;
                List<BaseItem> baseItems = new List<BaseItem>();

                for (int i = 0; i < numberOfCoins; i++)
                {
                    BaseItem temp = Instantiate(ResourceManager.Instance.GetItem(9), itemStack.transform);
                    temp.transform.localScale = Vector3.zero;
                    temp.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InBack);
                    baseItems.Add(temp);
                    yield return new WaitForSeconds(0.03f);
                }

                itemStack.SetItem(baseItems);
                itemStack.transform.localScale = baseCell.transform.localScale;
            }

            if (baseCell.ItemStack != null && !removeItemCells.Contains(baseCell))
                removeItemCells.Add(baseCell);
        }

        private CellDataForMovement GetBestCellMovementData(CellDataForMovement one, CellDataForMovement two)
        {
            CellDataForMovement bestCell = null;
            if (one != null && two != null)
            {
                if (one.protity < two.protity)
                {
                    return one;
                }
                else if (one.protity > two.protity)
                {
                    return two;
                }
                else
                {
                    if (one.unlockerCount > two.unlockerCount)
                    {
                        return one;
                    }
                    else if (one.unlockerCount < two.unlockerCount)
                    {
                        return two;
                    }
                    else
                    {
                        List<BaseItem> topItemsTargetCell = one.cell.ItemStack.GetTopSameTypeItem();
                        List<BaseItem> topItemsAdjacentCell = two.cell.ItemStack.GetTopSameTypeItem();

                        if (debugConfig.enableDebugLogs && debugConfig.cellPriorityLogs)
                            Debug.LogError("Loker Vagar No Algo Locker Ni priority Sathe");
                        if (topItemsTargetCell.Count == one.cell.ItemStack.GetItems().Count)
                        {
                            return one;
                        }
                        else
                        {
                            if (topItemsAdjacentCell.Count == two.cell.ItemStack.GetItems().Count)
                            {
                                return two;
                            }
                            else
                            {
                                if (one.cell.ItemStack.GetUniqueItemCount() <= two.cell.ItemStack.GetUniqueItemCount())
                                {
                                    return one;
                                }
                                else
                                {
                                    return two;
                                }
                            }
                        }
                    }
                }
            }
            return bestCell;
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator DoStack(BaseCell targetCell)
        {
            List<BaseCell> adjacentCells = targetCell.AdjacentCells;

            List<BaseCell> inAdjacentCells = new List<BaseCell>();
            BaseCell outAdjacentCell = null;

            currentCellData = new CellDataForMovement();

            for (int i = adjacentCells.Count - 1; i >= 0; i--)
            {
                BaseCell bestTargetCell = GetBestCell(targetCell, adjacentCells[i]);
                if (bestTargetCell != null)
                {
                    if (bestTargetCell == targetCell)
                    {
                        inAdjacentCells.Add(adjacentCells[i]);
                    }
                    else
                    {
                        if (outAdjacentCell != null)
                        {
                            BaseCell bestOutAdjacentCell = GetBestCell(outAdjacentCell, adjacentCells[i]);
                            if (bestOutAdjacentCell != null)
                            {
                                if (bestOutAdjacentCell == outAdjacentCell)
                                {
                                    inAdjacentCells.Add(adjacentCells[i]);
                                }
                                else
                                {
                                    inAdjacentCells.Add(outAdjacentCell);
                                    outAdjacentCell = adjacentCells[i];
                                }
                            }
                        }
                        else
                        {
                            outAdjacentCell = adjacentCells[i];
                        }
                    }
                }
            }

            foreach (BaseCell adjacentCellTemp in inAdjacentCells)
            {
                foreach (var item in adjacentCellTemp.AdjacentCells)
                {
                    if (item != null && item.HasItem && !item.IsCellLocked() && !inAdjacentCells.Contains(item) && outAdjacentCell != item && targetCell != item && adjacentCellTemp != null && adjacentCellTemp.HasItem)
                    {
                        itemsToMerge = new List<BaseItem>();
                        List<BaseItem> topItems = item.ItemStack.GetTopSameTypeItem();
                        if (IsSameItemType(adjacentCellTemp.ItemStack, item.ItemStack) && topItems.Count > 0)
                        {
                            itemsToMerge.AddRange(topItems);
                            yield return StartCoroutine(DoMergeItems(adjacentCellTemp, item, itemsToMerge));
                        }
                    }
                }

                if (adjacentCellTemp != null && adjacentCellTemp.ItemStack != null)
                {
                    itemsToMerge = new List<BaseItem>();
                    List<BaseItem> topItems = adjacentCellTemp.ItemStack.GetTopSameTypeItem();
                    if (topItems.Count > 0)
                    {
                        itemsToMerge.AddRange(topItems);
                        yield return StartCoroutine(DoMergeItems(targetCell, adjacentCellTemp, itemsToMerge));
                    }
                }
            }

            if (outAdjacentCell != null && outAdjacentCell.ItemStack != null)
            {
                itemsToMerge = new List<BaseItem>();
                List<BaseItem> topItems = targetCell.ItemStack.GetTopSameTypeItem();
                if (topItems.Count > 0)
                {
                    itemsToMerge.AddRange(topItems);
                    yield return StartCoroutine(DoMergeItems(outAdjacentCell, targetCell, itemsToMerge));
                }

                stackCO = StartCoroutine(DoStack(outAdjacentCell));
                yield break;
            }
            yield return null;
            if (removeItemCells.Count > 0)
            {
                BaseCell removeCell = removeItemCells[0];
                removeItemCells.RemoveAt(0);
                stackCO = StartCoroutine(DoStack(removeCell));
                yield break;
            }
            else
            {
                yield return StartCoroutine(CheckAndRemoveExcessItems());
                yield return null;
                if (removeItemCells.Count > 0)
                {
                    if (debugConfig.enableDebugLogs && debugConfig.stackOperationLogs)
                        Debug.LogError("Do stack Remove Cell  " + removeItemCells.Count);
                    BaseCell removeCell = removeItemCells[0];
                    removeItemCells.RemoveAt(0);
                    stackCO = StartCoroutine(DoStack(removeCell));
                    yield return null;
                    yield return null;
                    yield break;
                }
                else
                {
                    if (debugConfig.enableDebugLogs && debugConfig.stackOperationLogs)
                        Debug.LogError("Do stack Remove Cell Zero Che  " + removeItemCells.Count);
                }
            }
            yield return null;
            yield return null;
            yield return new WaitForEndOfFrame();
            isMovementInProgress = false;
            if (debugConfig.enableDebugLogs && debugConfig.movementProgressLogs)
                Debug.LogError("Is Movement  in Progress False Do stack");
            GameplayManager.Instance.SaveAllDataOfLevel();
            if (LevelManager.Instance.LoadedLevel.IsAllCellOcupied() && !GameplayGoalHandler.Instance.IsGameComplate)
            {
                GameplayManager.Instance.OnOutOfSpace();
            }
            yield return null;
            stackCO = null;
        }

        private IEnumerator DoStackNew(BaseCell targetCell)
        {
            List<BaseCell> adjacentCells = targetCell.AdjacentCells;

            currentCellData = new CellDataForMovement();

            List<BaseCell> inAdjacentCells = new List<BaseCell>();
            BaseCell outAdjacentCell = null;

            for (int i = adjacentCells.Count - 1; i >= 0; i--)
            {
                BaseCell bestTargetCell = GetBestCellNewMode(targetCell, adjacentCells[i]);
                if (bestTargetCell != null)
                {
                    if (bestTargetCell == targetCell)
                    {
                        inAdjacentCells.Add(adjacentCells[i]);
                    }
                    else
                    {
                        if (outAdjacentCell != null)
                        {
                            BaseCell bestOutAdjacentCell = GetBestCellNewMode(outAdjacentCell, adjacentCells[i]);
                            if (bestOutAdjacentCell != null)
                            {
                                if (bestOutAdjacentCell == outAdjacentCell)
                                {
                                    inAdjacentCells.Add(adjacentCells[i]);
                                }
                                else
                                {
                                    inAdjacentCells.Add(outAdjacentCell);
                                    outAdjacentCell = adjacentCells[i];
                                }
                            }
                        }
                        else
                        {
                            outAdjacentCell = adjacentCells[i];
                        }
                    }
                }
            }

            foreach (BaseCell adjacentCellTemp in inAdjacentCells)
            {
                foreach (var item in adjacentCellTemp.AdjacentCells)
                {
                    if (item != null && item.HasItem && !item.IsCellLocked() && !inAdjacentCells.Contains(item) && outAdjacentCell != item && targetCell != item && adjacentCellTemp != null && adjacentCellTemp.HasItem)
                    {
                        itemsToMerge = new List<BaseItem>();
                        List<BaseItem> topItems = item.ItemStack.GetTopSameTypeItem();
                        if (IsSameItemType(adjacentCellTemp.ItemStack, item.ItemStack) && topItems.Count > 0)
                        {
                            itemsToMerge.AddRange(topItems);
                            yield return StartCoroutine(DoMergeItems(adjacentCellTemp, item, itemsToMerge));
                        }
                    }
                }

                if (adjacentCellTemp != null && adjacentCellTemp.ItemStack != null)
                {
                    itemsToMerge = new List<BaseItem>();
                    List<BaseItem> topItems = adjacentCellTemp.ItemStack.GetTopSameTypeItem();
                    if (topItems.Count > 0)
                    {
                        itemsToMerge.AddRange(topItems);
                        yield return StartCoroutine(DoMergeItems(targetCell, adjacentCellTemp, itemsToMerge));
                    }
                }
            }

            if (outAdjacentCell != null && outAdjacentCell.ItemStack != null)
            {
                itemsToMerge = new List<BaseItem>();
                List<BaseItem> topItems = targetCell.ItemStack.GetTopSameTypeItem();
                if (topItems.Count > 0)
                {
                    itemsToMerge.AddRange(topItems);
                    yield return StartCoroutine(DoMergeItems(outAdjacentCell, targetCell, itemsToMerge));
                }

                //if (stackCO != null)
                //    StopCoroutine(stackCO);
                stackCO = StartCoroutine(DoStackNew(outAdjacentCell));
                yield break;
            }
            yield return null;
            if (removeItemCells.Count > 0)
            {
                BaseCell removeCell = removeItemCells[0];
                removeItemCells.RemoveAt(0);
                stackCO = StartCoroutine(DoStackNew(removeCell));
                yield break;
            }
            else
            {
                yield return StartCoroutine(CheckAndRemoveExcessItems());
                yield return null;
                if (removeItemCells.Count > 0)
                {
                    BaseCell removeCell = removeItemCells[0];
                    removeItemCells.RemoveAt(0);
                    stackCO = StartCoroutine(DoStackNew(removeCell));
                    yield return null;
                    yield return null;
                    yield break;
                }
            }
            yield return null;
            yield return null;
            yield return new WaitForEndOfFrame();
            isMovementInProgress = false;
            GameplayManager.Instance.SaveAllDataOfLevel();
            if (debugConfig.enableDebugLogs && debugConfig.movementProgressLogs)
                Debug.LogError("Is Movement  in Progress False DoStack New");
            if (LevelManager.Instance.LoadedLevel.IsAllCellOcupied() && !GameplayGoalHandler.Instance.IsGameComplate)
            {
                GameplayManager.Instance.OnOutOfSpace();
            }
            yield return null;
            stackCO = null;
        }

        private IEnumerator DoMergeItems(BaseCell targetCell, BaseCell currentCell, List<BaseItem> itemsToMerge)
        {
            Tile_Move = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Tile_Move");
            Tile_Move.start();

            FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("PitchIncreaseOnMove", out Tile_Move_pd);

            Tile_Move_pid = Tile_Move_pd.id;
            float minimum = 0f;
            float maximum = 20f;
            float current = 0f;
            float increase = 1f;

            //GameplayManager.Instance.InvokeOnStackItemRemoveOrAdd(targetCell, true);
            //GameplayManager.Instance.InvokeOnStackItemRemoveOrAdd(currentCell, true);

            foreach (BaseItem item in itemsToMerge)
            {
                Vector3 startPosition = item.transform.position;
                Vector3 endPosition = targetCell.ItemStack.GetTopPosition();

                Vector3 startPos = currentCell.transform.position;
                Vector3 endPos = targetCell.transform.position;

                Vector3 startPosTemp = transform.TransformPoint(currentCell.transform.localPosition);
                Vector3 endPosTemp = transform.TransformPoint(targetCell.transform.localPosition);

                Vector3 direction = (endPosTemp - startPosTemp).normalized;

                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                angle += LevelManager.Instance.LoadedLevel.transform.localEulerAngles.y;
                angle = (angle + 360) % 360; // Ensure angle is within 0-360 degrees

                float zRotation = 180;

                Vector3 rotation = new Vector3(0, angle, zRotation);

                if (item == null)
                    continue;

                item.transform.SetParent(targetCell.ItemStack.transform);

                if (targetCell.HasItem)
                    targetCell.ItemStack.AddItem(item);
                if (currentCell.HasItem)
                    currentCell.ItemStack.RemoveItem(item);


                SoundHandler.Instance.PlaySound(SoundType.TileMove);

                current += increase;
                current = Mathf.Clamp(current, minimum, maximum);
                FMODUnity.RuntimeManager.StudioSystem.setParameterByID(Tile_Move_pid, current);

                Tween jumpTween = item.transform.DOLocalJump(endPosition, 2, 1, mergeAnimationDuration);
                activeTweens.Add(jumpTween);

                StartCoroutine(FlipAndMoveCoroutine(item.transform, currentCell.transform, targetCell.transform, mergeAnimationDuration));
                yield return new WaitForSeconds(mergeAnimationDuration - delayBetweenEachItem);
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
            }

            if (currentCell.ItemStack != null && !removeItemCells.Contains(currentCell))
                removeItemCells.Add(currentCell);
            //GameplayManager.Instance.InvokeOnStackItemRemoveOrAdd(currentCell, false);
            //GameplayManager.Instance.InvokeOnStackItemRemoveOrAdd(targetCell, false);

            while (activeTweens.Count > 0)
            {
                yield return activeTweens[0].WaitForCompletion();
                if (activeTweens.Count > 0 && activeTweens[0] != null)
                {
                    activeTweens.RemoveAt(0);
                }
            }
            yield return new WaitForEndOfFrame();
        }

        private IEnumerator FlipAndMoveCoroutine(Transform hexagoneItem, Transform startTransform, Transform endTransform, float flipDuration)
        {
            if (hexagoneItem == null || startTransform == null || endTransform == null)
                yield break;

            hexagoneItem.localRotation = Quaternion.identity;
            Vector3 localStartPosition = hexagoneItem.parent.InverseTransformPoint(startTransform.position);
            Quaternion localStartRotation = hexagoneItem.localRotation;  // Use actual rotation
            Vector3 localEndPosition = hexagoneItem.parent.InverseTransformPoint(endTransform.position);
            Vector3 direction = (localEndPosition - localStartPosition).normalized;

            Vector3 rotationAxis = GetPerpendicularRotationAxis(direction);

            Debug.DrawLine(localStartPosition, localEndPosition, Color.red, 5f);
            Debug.DrawLine(hexagoneItem.position, rotationAxis, Color.green, 5f);

            float elapsedTime = 0f;

            while (elapsedTime < flipDuration)
            {
                elapsedTime += Time.deltaTime;
                float percentComplete = elapsedTime / flipDuration;
                float smoothPercent = Mathf.SmoothStep(0, 1, percentComplete);

                float angle = Mathf.Lerp(0, 180f, smoothPercent);
                Quaternion localTargetRotation = localStartRotation * Quaternion.AngleAxis(angle, rotationAxis);
                if(hexagoneItem == null) 
                    yield break;
                hexagoneItem.localRotation = localTargetRotation;
                yield return null;
            }
            hexagoneItem.localRotation = localStartRotation;
        }

        private Vector3 GetPerpendicularRotationAxis(Vector3 moveDirection)
        {
            Vector3 projected = new Vector3(moveDirection.z, 0, -moveDirection.x);

            if (projected == Vector3.zero)
                return new Vector3(1, 0, 0);

            return projected.normalized;
        }

        private IEnumerator CheckAndRemoveExcessItems()
        {
            List<BaseCell> cellsToCheck = LevelManager.Instance.LoadedLevel.BaseCells;
            List<(BaseCell cell, List<BaseItem> items, int itemType, Vector3 startPos)> cellsToRemove = new List<(BaseCell, List<BaseItem>, int, Vector3)>();

            foreach (BaseCell cell in cellsToCheck)
            {
                if (cell.HasItem && !cell.IsCellLocked())
                {
                    List<BaseItem> topSameItems = cell.ItemStack.GetTopSameTypeItem();
                    if (topSameItems.Count >= noOfItemToDestroy)
                    {
                        Vector3 startPos = topSameItems[0].transform.position;
                        int itemType = topSameItems[0].ItemId;
                        cellsToRemove.Add((cell, topSameItems, itemType, startPos));
                    }
                }
            }

            if (cellsToRemove.Count > 0)
            {
                List<Coroutine> removeCoroutines = new List<Coroutine>();

                foreach (var cellData in cellsToRemove)
                {
                    removeCoroutines.Add(StartCoroutine(RemoveExcessItems(cellData.cell, cellData.items, () =>
                    {
                        MainSceneUIManager.Instance.GetView<VFXView>().PlayItemAnimation(
                            GoalType.Item,
                            cellData.startPos,
                            cellData.items.Count,
                            cellData.itemType
                        );

                        isGoalAnimationInProgress = true;
                        GameplayGoalHandler.Instance.UpdateGoals(GoalType.Item, cellData.itemType, cellData.items.Count);
                        GameplayManager.Instance.InvokeOnStackRemove(cellData.cell, cellData.itemType);
                        //GameplayManager.Instance.InvokeOnStackOrItemRemove(cellData.cell, cellData.itemType);
                    })));

                }
                foreach (var coroutine in removeCoroutines)
                {
                    yield return coroutine;
                }
            }
        }

        private IEnumerator RemoveExcessItems(BaseCell cell, List<BaseItem> itemsToRemove, Action action = null, bool isScoreCountOnItemRemove = false)
        {
            ParticleSystem particleSystem = Instantiate(destroyPS, transform);
            particleSystem.gameObject.SetActive(true);
            particleSystem.transform.position = itemsToRemove[0].transform.position;
            particleSystem.Play();

            Tile_Remove = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Tile_Remove");
            Tile_Remove.start();

            FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("pitchIncrease", out Tile_Remove_pd);

            Tile_Remove_pid = Tile_Remove_pd.id;
            float minimum = 0f;
            float maximum = 20f;
            float current = 0f;
            float increase = 1f;

            int itemId = itemsToRemove[0].ItemId;
            int topItemCount = cell.ItemStack.GetTopSameTypeItem().Count;

            //GameplayManager.Instance.InvokeOnStackItemRemoveOrAdd(cell, true);
            foreach (BaseItem item in itemsToRemove)
            {
                if (item == null)
                    continue;

                SoundHandler.Instance?.PlaySound(SoundType.RemoveItem);
                Vibrator.Vibrate(Vibrator.smallIntensity);
                if (particleSystem != null)
                    particleSystem.transform.position = item.transform.position;

                item.transform.DOScale(Vector3.zero, destroyAnimationDuration).SetEase(Ease.InBack);

                yield return new WaitForSeconds(destroyAnimationDuration - 0.2f);

                GameplayManager.Instance.InvokeOnItemRemove(item.ItemId, 1);

                if (cell.HasItem)
                    cell.ItemStack.RemoveItem(item);

                if (item != null)
                    Destroy(item.gameObject);

                current += increase;
                current = Mathf.Clamp(current, minimum, maximum);
                FMODUnity.RuntimeManager.StudioSystem.setParameterByID(Tile_Remove_pid, current);

                yield return new WaitForEndOfFrame();
            }
            //GameplayManager.Instance.InvokeOnStackOrItemRemove(cell, itemId);
            //GameplayManager.Instance.InvokeOnStackItemRemoveOrAdd(cell, false);


            if (cell.HasItem && cell.ItemStack.GetItems().Count > 0 && !removeItemCells.Contains(cell))
                removeItemCells.Add(cell);

            yield return new WaitForSeconds(0.2f);

            // Trigger the OnItemsCleared event
            //OnItemsCleared?.Invoke(itemId, itemsToRemove.Count);

            if (isScoreCountOnItemRemove)
            {
                MainSceneUIManager.Instance.GetView<VFXView>().PlayItemAnimation(
                    GoalType.Item,
                    cell.transform.position,
                    itemsToRemove.Count,
                    itemId
                );

                GameplayGoalHandler.Instance.UpdateGoals(GoalType.Item, itemId, itemsToRemove.Count);
                //GameplayManager.Instance.InvakeOnStackRemove(cell, itemId);
            }

            if (action != null)
                action.Invoke();

            if (GameplayManager.Instance.CurrentLevel.LevelType == LevelType.Bonus && topItemCount >= noOfItemToDestroy)
                yield return StartCoroutine(AddCoinItemInCell(cell, itemsToRemove.Count, itemId));
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }

    public abstract class BaseItemMovementModeSelctor
    {
        public ItemMovementMode defaultItemMovementMode;
        public ItemMovementMode currentItemMovementMode;

        public virtual ItemMovementMode SelecteMode() { return currentItemMovementMode; }
    }

    public class FixedItemMovementModeSelctor : BaseItemMovementModeSelctor
    {
        public override ItemMovementMode SelecteMode()
        {
            currentItemMovementMode = defaultItemMovementMode;
            return currentItemMovementMode;
        }
    }

    public class EmptyTileItemMovementModeSelctor : BaseItemMovementModeSelctor
    {
        public int emptyTileCount = 2;
        public ItemMovementMode newItemMovementMode;

        [Button]
        public override ItemMovementMode SelecteMode()
        {
            int count = 0;
            List<BaseCell> cell = LevelManager.Instance.LoadedLevel.BaseCells;

            for (int i = 0; i < cell.Count; i++)
            {
                if (!cell[i].HasItem && !cell[i].IsCellLocked())
                    count++;
            }

            if (count <= emptyTileCount)
                currentItemMovementMode = newItemMovementMode;
            else
                currentItemMovementMode = defaultItemMovementMode;

            return currentItemMovementMode;
        }
    }

    public enum ItemMovementMode
    {
        Falcon,
        Lion
    }

    public class CellDataForMovement
    {
        public BaseCell cell;
        public int protity = 1000;
        public int unlockerCount;
    }
}
