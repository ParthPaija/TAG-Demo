using UnityEngine;

namespace Tag.HexaStack
{
    public class ItemMovement : BaseItemMovement
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private ItemStackSpawner itemStackSpawner;
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
            if (BoosterManager.Instance != null && BoosterManager.Instance.IsAnyBoosterActive)
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
                //MoveItemOnCell();
                BaseCell baseCell = hit.collider.GetComponent<BaseCell>();
                if (baseCell != null && !baseCell.HasItem && !baseCell.IsCellLocked())
                    VFXManager.Instance.PlaySelecedCellAnimation(baseCell);
                else
                    VFXManager.Instance.StopSelecedCellAnimation();
                MoveItemOnWorld();
                RaiseOnItemDrag();
            }
            else if (GetRayHit(pos, out hit, worldLayer))
            {
                VFXManager.Instance.StopSelecedCellAnimation();
                MoveItemOnWorld();
            }
            return itemPos;
        }

        public override void ItemPut(Vector3 pos)
        {
            VFXManager.Instance.StopSelecedCellAnimation();
            if (itemStack == null)
            {
                itemStackSpawner = null;
                return;
            }

            if (Physics.Raycast(pos, InputManager.eventTranform.forward, out hit, Mathf.Infinity, itemLayerMask))
            {
                putCell = hit.collider.GetComponent<BaseCell>();

                if (putCell != null)
                    SwitchStack();
                else
                    itemStackSpawner.ResetItemPosition();
            }
            else
                itemStackSpawner.ResetItemPosition();

            putCell = null;
            itemStackSpawner = null;
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
                if (baseCell.IsCellLocked())
                    baseCell.OnClick();
                return;
            }
            ItemStackSpawner stackSpawner = hit.collider.GetComponent<ItemStackSpawner>();
            if (stackSpawner != null)
            {
                SoundHandler.Instance.PlaySound(SoundType.TilePickup);
                itemStackSpawner = stackSpawner;
                itemStack = itemStackSpawner.ItemStack;
                RaiseOnItemPick();
                return;
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
            ItemStackSpawner itemStackSpawnerTemp = itemStackSpawner;
            BaseItemStack item = itemStackSpawner.ItemStack;
            BaseCell putCellTemp = putCell;
            if (!putCell.HasItem && !putCell.IsCellLocked())
            {
                GameplayManager.Instance.SaveDataForUndoBooster();
                CoroutineRunner.Instance.Wait(0.05f, () =>
                {
                    RaiseOnItemDrop();
                    itemStackSpawnerTemp.RemoveStack();
                    putCellTemp.ItemStack = item;
                    putCellTemp.ResetItemPosition();
                    GameRuleManager.Instance.OnStackPlace(putCellTemp);
                    GameplayManager.testPrintData.AddMove();
                    //GameplayManager.Instance.InvokeOnStackItemRemoveOrAdd(putCellTemp, false);
                });
            }
            else
            {
                itemStackSpawner.ResetItemPosition();
            }
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        public delegate void OnItemMovementVoidEvent();
        public static event OnItemMovementVoidEvent onItemPick;
        public static event OnItemMovementVoidEvent onItemDrag;
        public static event OnItemMovementVoidEvent onItemDrop;

        public static void RaiseOnItemPick()
        {
            onItemPick?.Invoke();
        }

        public static void RaiseOnItemDrag()
        {
            onItemDrag?.Invoke();
        }

        public static void RaiseOnItemDrop()
        {
            onItemDrop?.Invoke();
        }

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
