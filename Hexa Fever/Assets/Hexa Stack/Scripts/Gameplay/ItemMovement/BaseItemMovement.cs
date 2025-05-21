using UnityEngine;

namespace Tag.HexaStack
{
    public class BaseItemMovement : MonoBehaviour
    {
        #region PUBLIC_VARS

        public LayerMask itemLayerMask;
        public LayerMask worldLayer;

        #endregion

        #region PUBLIC_FUNCTIONS

        public virtual bool ItemPick(Vector3 pos)
        {
            return false;
        }

        public virtual Vector3 ItemDrag(Vector3 pos)
        {
            return Vector3.zero;
        }

        public virtual void ItemPut(Vector3 pos)
        {
        }

        public virtual void OnTouchCancel()
        {

        }

        public virtual bool IsUIMovement()
        {
            return false;
        }

        #endregion

        #region UI_CALLBACKS       

        public bool GetRayHit(Vector3 pos, out RaycastHit hit, LayerMask layerMask)
        {
            Debug.DrawLine(pos, InputManager.eventTranform.forward, Color.cyan, 2, true);
            return Physics.Raycast(pos, InputManager.eventTranform.forward, out hit, Mathf.Infinity, layerMask);
        }

        #endregion
    }
}
