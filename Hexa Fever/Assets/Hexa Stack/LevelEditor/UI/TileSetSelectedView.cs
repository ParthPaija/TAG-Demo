using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class TileSetSelectedView : BaseView
    {
        #region PUBLIC_VARS

        public CellLockerSelcetorDataSO cellLockerSelcetorData;

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Transform parent;
        [SerializeField] private TileSetSelectedItemView prefab;
        private List<TileSetSelectedItemView> tileSetSelectedItemViews = new List<TileSetSelectedItemView>();

        #endregion

        #region UNITY_CALLBACKS

        [Button]
        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);

            for (int i = 0; i < tileSetSelectedItemViews.Count; i++)
            {
                Destroy(tileSetSelectedItemViews[i].gameObject);
            }
            tileSetSelectedItemViews.Clear();
            tileSetSelectedItemViews = new List<TileSetSelectedItemView>();

            for (int i = 0; i < cellLockerSelcetorData.gridGenerators.Count; i++)
            {
                TileSetSelectedItemView temp = Instantiate(prefab, parent);
                temp.SetView(cellLockerSelcetorData.gridGenerators[i]);
                temp.gameObject.SetActive(true);
                tileSetSelectedItemViews.Add(temp);
            }
        }

        #endregion

        #region PUBLIC_FUNCTIONS

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
