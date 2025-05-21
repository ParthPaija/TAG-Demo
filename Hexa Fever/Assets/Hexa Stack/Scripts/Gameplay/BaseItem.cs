using UnityEngine;
using Sirenix.OdinInspector;

namespace Tag.HexaStack
{
    public class BaseItem : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS
        public int ItemId { get => itemId; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField, ItemId] int itemId;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public virtual bool IsSameItem(int itemId)
        {
            return this.itemId == itemId;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
