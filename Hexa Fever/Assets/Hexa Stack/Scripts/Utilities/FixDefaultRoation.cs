using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class FixDefaultRoation : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        private Vector3 defaultRotation;

        #endregion

        #region UNITY_CALLBACKS

        private void Start()
        {
            defaultRotation = transform.rotation.eulerAngles;
        }

        private void Update()
        {
            transform.rotation = Quaternion.Euler(defaultRotation);
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
