using UnityEngine;

namespace Tag.HexaStack
{
    public class CellItemMovement : BaseItemMovement
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private StackSwapBooster stackSwapBooster;
        [SerializeField] private BaseCell pickedCell;
        [SerializeField] private BaseCell putCell;

        private BaseItemStack itemStack;
        private Vector3 itemPos = Vector3.zero;
        private RaycastHit hit;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override bool ItemPick(Vector3 pos)
        {
            if (!stackSwapBooster.IsActive)
                return false;

            if (GetRayHit(pos, out hit, itemLayerMask))
            {
                PickItem();
            }
            return itemStack != null;
        }

        public override Vector3 ItemDrag(Vector3 pos)
        {
            if (itemStack == null)
                return base.ItemDrag(pos);

            if (GetRayHit(pos, out hit, itemLayerMask))
            {
                //BaseCell baseCell = hit.collider.GetComponent<BaseCell>();
                //if (baseCell != null && !baseCell.HasItem && !baseCell.IsCellLocked())
                //    VFXManager.Instance.PlaySelecedCellAnimation(hit.transform.position);
                //else
                //    VFXManager.Instance.StopSelecedCellAnimation();
                MoveItemOnWorld();
            }
            else if (GetRayHit(pos, out hit, worldLayer))
            {
                MoveItemOnWorld();
            }
            return itemPos;
        }

        public override void ItemPut(Vector3 pos)
        {
            VFXManager.Instance.StopSelecedCellAnimation();
            if (itemStack == null)
            {
                pickedCell = null;
                return;
            }

            if (Physics.Raycast(pos, InputManager.eventTranform.forward, out hit, Mathf.Infinity, itemLayerMask))
            {
                putCell = hit.collider.GetComponent<BaseCell>();

                if (putCell != null)
                    SwitchStack();
                else
                    pickedCell.ResetItemPosition();
            }
            else
                pickedCell.ResetItemPosition();

            putCell = null;
            pickedCell = null;
            itemStack = null;
            base.ItemPut(pos);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void PickItem()
        {
            BaseCell baseCell = hit.collider.GetComponent<BaseCell>();
            if (baseCell != null)
            {
                //if (baseCell.IsCellLocked())
                //{
                //    baseCell.OnClick();
                //    Debug.LogError("Cell Clicked");
                //    return;
                //}
            }
            BaseCell cell = hit.collider.GetComponent<BaseCell>();
            if (cell != null)
            {
                if (cell.HasItem)
                {
                    if (cell.BaseCellUnlocker != null && cell.BaseCellUnlocker.IsLocked())
                    {
                        return;
                    }
                    SoundHandler.Instance.PlaySound(SoundType.TilePickup);
                    pickedCell = cell;
                    itemStack = pickedCell.ItemStack;
                }
            }
        }

        private void MoveItemOnWorld()
        {
            itemStack?.SyncPosition(hit.point);
            itemPos = hit.point;
        }

        private void MoveItemOnCell()
        {
            itemStack?.SyncPosition(hit.transform.position);
            itemPos = hit.point;
        }

        private void SwitchStack()
        {
            BaseItemStack item = pickedCell.ItemStack;
            BaseCell putCellTemp = putCell;
            BaseCell pickCellTemp = pickedCell;

            if (!putCell.IsCellLocked() && !pickedCell.IsCellLocked() && pickedCell.HasItem)
            {
                if (putCell != pickedCell)
                {
                    GameplayManager.Instance.CancelUndoBooster();
                    stackSwapBooster.OnSatckPlacedSucess();
                    CoroutineRunner.Instance.Wait(0.1f, () =>
                    {
                        pickCellTemp.ItemStack = null;
                        if (putCellTemp.HasItem)
                            pickCellTemp.ItemStack = putCellTemp.ItemStack;
                        pickCellTemp.ResetItemPosition();

                        putCellTemp.ItemStack = item;
                        putCellTemp.ResetItemPosition();
                        GameRuleManager.Instance.OnStackSwapBooster(putCellTemp, pickCellTemp);
                    });
                }
                else
                {
                    pickedCell.ResetItemPosition();
                }
            }
            else
            {
                pickedCell.ResetItemPosition();
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
}
