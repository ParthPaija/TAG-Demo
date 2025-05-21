using System.Collections;
using System.Collections.Generic;
using Tag.AssetManagement;
using Tag.HexaStack;
using UnityEngine;

namespace Tag.MetaGame
{
    public class MetaCameraSizeController : Manager<MetaCameraSizeController>
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private new Camera camera;
        [SerializeField] private float newCameraSize;
        private float defaultCameraSize;

        #endregion

        #region UNITY_CALLBACKS
        private void Start()
        {
            defaultCameraSize = camera.orthographicSize;
        }
        #endregion

        #region PUBLIC_FUNCTIONS
        public void SetCameraSizeAccordingAreaNo(bool isRequiredNewResolution)
        {
            if (isRequiredNewResolution)
            {
                camera.orthographicSize = newCameraSize;
            }
            else
            {
                if (defaultCameraSize != 0)
                    camera.orthographicSize = defaultCameraSize;
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
