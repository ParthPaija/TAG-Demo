using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexa_Stack
{
    public class LoadingViewGridTile : MonoBehaviour
    {
        #region PUBLIC_VARS

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
        }

        public void ResetData()
        {
            transform.localPosition = initPosition;
            transform.localRotation = Quaternion.identity;
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
