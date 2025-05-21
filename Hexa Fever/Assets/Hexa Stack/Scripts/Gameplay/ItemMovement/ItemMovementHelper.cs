using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class ItemMovementHelper : Manager<ItemMovementHelper>
    {
        #region PUBLIC_VARS

        public Vector3 ItemCurrentPosition { get; set; }
        public bool IsAnyThingPick { get => isAnyThingPick; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private List<BaseItemMovement> listOfMovement = new List<BaseItemMovement>();
        [ShowInInspector] private bool isAnyThingPick;
        private bool isUIClickEnable;
        [ShowInInspector] private BaseItemMovement itemMovement;

        #endregion

        #region UNITY_CALLBACKS

        private void Start()
        {
            InputManager.Instance.AddListenerMouseButtonDown(OnMouseDown);
            InputManager.Instance.AddListenerMouseButtonMove(OnMouseMove);
            InputManager.Instance.AddListenerMouseButtonUp(OnMouseUp);
            InputManager.Instance.AddListenerUIClick(OnUIClick);
            InputManager.Instance.AddListenerUIPointerUp(OnUIPointerUp);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            InputManager.Instance.RemoveListenerMouseButtonDown(OnMouseDown);
            InputManager.Instance.RemoveListenerMouseButtonMove(OnMouseMove);
            InputManager.Instance.RemoveListenerMouseButtonUp(OnMouseUp);
            InputManager.Instance.RemoveListenerUIClick(OnUIClick);
            InputManager.Instance.RemoveListenerUIPointerUp(OnUIPointerUp);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void OnMouseDown(Vector3 pos)
        {
            for (int i = 0; i < listOfMovement.Count; i++)
            {
                if (!isAnyThingPick && listOfMovement[i].ItemPick(pos))
                {
                    isAnyThingPick = true;
                    itemMovement = listOfMovement[i];
                }
            }
        }

        private void OnMouseUp(Vector3 pos)
        {
            if (itemMovement != null)
                itemMovement.ItemPut(pos);
            isAnyThingPick = false;
            isUIClickEnable = false;
            itemMovement = null;
        }

        private void OnMouseMove(Vector3 pos)
        {
            if (itemMovement != null)
                ItemCurrentPosition = itemMovement.ItemDrag(pos);
        }

        private void OnUIClick(Vector3 pos)
        {
            if (isUIClickEnable)
            {
                if (itemMovement != null)
                    itemMovement?.ItemDrag(pos);
                return;
            }
            if (itemMovement != null)
                itemMovement.OnTouchCancel();

            for (int i = 0; i < listOfMovement.Count; i++)
            {
                if (listOfMovement[i].IsUIMovement())
                {
                    if (!isAnyThingPick && listOfMovement[i].ItemPick(pos))
                    {
                        isAnyThingPick = true;
                        isUIClickEnable = true;
                        itemMovement = listOfMovement[i];
                    }
                }
            }
        }

        private void OnUIPointerUp(Vector3 pos)
        {
            if (itemMovement != null)
                itemMovement.ItemPut(pos);
            isAnyThingPick = false;
            isUIClickEnable = false;
            itemMovement = null;
        }

        #endregion
    }
}
