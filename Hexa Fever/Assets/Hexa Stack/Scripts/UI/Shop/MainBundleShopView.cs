using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Tag.HexaStack
{
    public class MainBundleShopView : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private HorizontalScrollSnap horizontalScrollSnap;
        [SerializeField] private GameObject[] pagesMarker;
        [SerializeField] private GameObject pageGO;
        private int pageIndex;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowView(int pageIndex)
        {
            this.pageIndex = pageIndex;
            horizontalScrollSnap.InitScroll();
            horizontalScrollSnap.ImmidiateChangePage(pageIndex);
            OnPageChanged(pageIndex);
            horizontalScrollSnap.enabled = horizontalScrollSnap.ChildObjects.Length > 1;
            pageGO.SetActive(horizontalScrollSnap.ChildObjects.Length > 1);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        public void OnPageChanged(int index)
        {
            for (int i = 0; i < pagesMarker.Length; i++)
            {
                if (i == index)
                {
                    pagesMarker[i].SetActive(true);
                }
                else
                {
                    pagesMarker[i].SetActive(false);
                }
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
