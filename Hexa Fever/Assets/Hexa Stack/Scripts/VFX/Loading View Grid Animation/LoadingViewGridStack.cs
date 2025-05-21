using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexa_Stack
{
    public class LoadingViewGridStack : MonoBehaviour
    {
        #region PUBLIC_VARS

        public List<LoadingViewGridTile> tiles;

        #endregion

        #region PRIVATE_VARS
        private Vector3 initPosition;
        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetInitialData()
        {
            initPosition = transform.localPosition;
            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].SetInitialData();
            }
        }

        public void ResetData()
        {
            transform.localPosition = initPosition;
            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].ResetData();
            }

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
