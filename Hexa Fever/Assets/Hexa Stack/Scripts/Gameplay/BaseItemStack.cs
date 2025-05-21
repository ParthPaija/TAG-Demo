using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class BaseItemStack : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS
        public BaseCell ParentCell { get => parentCell; set => parentCell = value; }

        #endregion

        #region PRIVATE_VARS

        [Header("UI")]
        [SerializeField] private Transform canvasTransform;
        [SerializeField] private Text itemCountText;

        [SerializeField] private Vector3 positionOffSet;
        [SerializeField] private List<BaseItem> baseItems = new List<BaseItem>();
        [SerializeField] private BaseCell parentCell;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SyncPosition(Vector3 pos)
        {
            transform.position = Vector3.Lerp(transform.position, pos + positionOffSet, Time.deltaTime * 28f);
        }

        public void SetItem(List<BaseItem> baseItems)
        {
            if (this.baseItems == null)
                this.baseItems = new List<BaseItem>();
            this.baseItems.AddRange(baseItems);
            ArrangeItem();
            parentCell?.OnItemAddInCell();
        }

        public BaseItem GetTopItem()
        {
            if (baseItems.Count > 0)
                return baseItems[baseItems.Count - 1];
            return null;
        }

        public List<BaseItem> GetItems()
        {
            return baseItems;
        }

        public List<BaseItem> GetItemReverse()
        {
            List<BaseItem> baseItemsReverse = new List<BaseItem>();
            for (int i = baseItems.Count - 1; i >= 0; i--)
            {
                baseItemsReverse.Add(baseItems[i]);
            }
            return baseItemsReverse;
        }

        public int GetUniqueItemCount()
        {
            List<int> items = new List<int>();
            for (int i = 0; i < baseItems.Count; i++)
            {
                if (!items.Contains(baseItems[i].ItemId))
                {
                    items.Add(baseItems[i].ItemId);
                }
            }
            return items.Count;
        }

        public List<BaseItem> GetTopSameTypeItem()
        {
            List<BaseItem> topSameTypeItem = new List<BaseItem>();
            if (baseItems.Count > 0)
            {
                int topItemId = GetTopItem().ItemId;
                for (int i = baseItems.Count - 1; i >= 0; i--)
                {
                    if (baseItems[i].ItemId != topItemId)
                    {
                        break;
                    }
                    topSameTypeItem.Add(baseItems[i]);
                }
            }
            return topSameTypeItem;
        }

        [Button]
        public List<int> GetBottomItem()
        {
            List<int> uniqueItem = new List<int>();
            if (baseItems.Count > 0)
            {
                for (int i = 0; i < baseItems.Count; i++)
                {
                    if (!uniqueItem.Contains(baseItems[i].ItemId))
                    {
                        uniqueItem.Add(baseItems[i].ItemId);
                    }
                }
            }
            return uniqueItem;
        }

        [Button]
        public Vector3 GetTopPosition()
        {
            return new Vector3(0, baseItems[0].transform.localPosition.y + ((baseItems.Count) * GamePlayConstant.TWO_ITEM_DISTANCE));
        }

        public void AddItem(BaseItem item)
        {
            baseItems.Add(item);
            SetItemCountAndPos();
            parentCell?.OnItemAddInCell();
        }

        public void RemoveItem(BaseItem item)
        {
            if (item == null)
                return;

            if (baseItems.Contains(item))
                baseItems.Remove(item);
            //parentCell?.OnItemRemoveInCell();
            //SetItemCountAndPos();
            if (baseItems.Count <= 0)
            {
                //parentCell?.OnRemoveAllItem();
                Destroy(gameObject);
                parentCell.ItemStack = null;
                parentCell = null;
            }
        }

        public void SetParentCell(BaseCell baseCell)
        {
            parentCell = baseCell;
            //transform.rotation = baseCell.transform.rotation;
        }

        public void ToggeleItemCount(bool isActive)
        {
            itemCountText.gameObject.SetActive(isActive);
        }

        public List<int> GetItemsId()
        {
            List<int> items = new List<int>();
            for (int i = 0; i < baseItems.Count; i++)
            {
                items.Add(baseItems[i].ItemId);
            }
            return items;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        public void ArrangeItem(float offset = 0)
        {
            if (baseItems == null)
                return;

            for (int i = 0; i < baseItems.Count; i++)
            {
                baseItems[i].transform.localPosition = baseItems[i].transform.localPosition.With(null, transform.localPosition.y + offset + (i * GamePlayConstant.TWO_ITEM_DISTANCE));
            }
            SetItemCountAndPos();
        }

        private void SetItemCountAndPos()
        {
            if (baseItems.Count <= 0)
            {
                itemCountText.gameObject.SetActive(false);
                return;
            }
            canvasTransform.localPosition = canvasTransform.position.With(0, GetTopPosition().y, 0);
            itemCountText.text = $"{GetTopSameTypeItem().Count}";
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
