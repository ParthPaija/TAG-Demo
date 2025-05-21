using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.CoreGame
{
    public class MaterialValueChanger : MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        public Image image;
        public string variableName;
        public Material material;
        [Range(-1f, 1f)]
        public float fill;
        #endregion

        #region PRIVATE_VARIABLES
        private int variablePropertyID;
        private float previousValue;
        #endregion

        #region UNITY_CALLBACKS
        private void OnEnable()
        {
            previousValue = fill;
            OnSetMaterialData();
        }
        private void Update()
        {

            if (previousValue != fill)
            {
                SetMaterialFill();
                previousValue = fill;
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void OnSetMaterialData()
        {
            variablePropertyID = Shader.PropertyToID(variableName);
            material = image.material;
        }
        
        [ContextMenu("SetMaterialFillValue")]
        public void SetMaterialFillValue()
        {
            SetMaterialFill();
        }
        #endregion

        #region PRIVATE_METHODS
        private void SetMaterialFill()
        {
            material.SetFloat(variablePropertyID, fill);
        }
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region CO-ROUTINES
        #endregion
    }
}
