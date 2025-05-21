using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Tag.HexaStack
{
    public class ItemStackSpawner : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS

        public bool HasItem { get { return itemStack != null; } }

        public BaseItemStack ItemStack { get => itemStack; set => itemStack = value; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] public BoxCollider boxCollider;
        [SerializeField] private int index;
        private BaseItemStack itemStack;
        [SerializeField] private List<int> itemIds = new List<int>();

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SpawnStack(List<int> itemIds)
        {
            ClearStack();
            this.itemIds = itemIds;
            if (itemIds == null || itemIds.Count <= 0)
            {
                return;
            }

            itemStack = Instantiate(ResourceManager.Instance.ItemStack, transform);
            itemStack.transform.localPosition = new Vector3(0, 0.2f, 0);   
            List<BaseItem> baseItems = new List<BaseItem>();
            for (int i = 0; i < itemIds.Count; i++)
            {
                BaseItem temp = Instantiate(ResourceManager.Instance.GetItem(itemIds[i]), itemStack.transform);
                baseItems.Add(temp);
            }
            itemStack.SetItem(baseItems);
            gameObject.SetActive(false);
            //SavaData(DataManager.LevelProgressData);
            //DataManager.Instance.SaveLevelProgressData(DataManager.LevelProgressData);
        }

        public void StakInAnimation()
        {
            if (itemStack == null) return;
            boxCollider.enabled = false;
            itemStack.transform.position = transform.position - new Vector3(-10, 0, 0);
            Tween moveTwen = itemStack.transform.DOMoveX(transform.position.x, 0.35f).SetEase(Ease.Linear);
            moveTwen.OnComplete(() => { boxCollider.enabled = true; });
            SoundHandler.Instance.PlaySound(SoundType.TileSpawn);
        }

        public void StakOutAnimation()
        {
            if (itemStack == null) return;
            itemStack.transform.position = transform.position - new Vector3(10, 0, 0);
            itemStack.transform.DOMoveX(transform.position.x, 0.35f).SetEase(Ease.Linear);
        }

        public void ResetItemPosition()
        {
            if (itemStack == null)
            {
                return;
            }
            itemStack.transform.SetParent(transform);
            itemStack.transform.DOMove(transform.position, 0.1f).OnComplete(() =>
            {
            });
        }

        public void ClearStack()
        {
            if (itemStack != null)
            {
                Destroy(itemStack.gameObject);
                itemStack = null;
            }
        }

        public void RemoveStack()
        {
            itemStack = null;
            itemIds = null;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        public void SavaData(LevelProgressData levelProgressData)
        {
            List<int> ints = null;
            if (itemIds != null)
            {
                ints = new List<int>();
                for (int i = 0; i < itemIds.Count; i++)
                {
                    ints.Add(itemIds[i]);
                }
            }
            levelProgressData.UpdateSpwanerData(index, ints);
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
