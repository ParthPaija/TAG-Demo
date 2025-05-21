using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityProj
{
    public class MaterialsFill : MonoBehaviour
    {
        #region PUBLIC_VARIABLES

        public string variableName;
        public List<SpriteRenderer> listSprites;
        public List<Material> listMaterials;
        public float fill;

        #endregion

        #region PRIVATE_VARIABLES

        private int variablePropertyID;

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            SetData();
            for (int i = 0; i < listSprites.Count; i++)
            {
                listMaterials.Add(listSprites[i].material);
            }

            variablePropertyID = Shader.PropertyToID(variableName);
            for (int i = 0; i < listMaterials.Count; i++)
            {
                listMaterials[i].SetFloat(variablePropertyID, fill);
            }
        }

        private void Update()
        {
            SetMaterialFill();
        }

        #endregion

        #region PUBLIC_METHODS
        [ContextMenu("Set Data")]
        public void SetData()
        {
            if (listSprites.Count != 0)
            {
                listSprites.Clear();
            }
            listSprites.Add(gameObject.GetComponent<SpriteRenderer>());
        }

        [ContextMenu("Set Material Fill Value")]
        public void SetMaterialFillValue()
        {
            SetMaterialFill();
        }

        #endregion

        #region PRIVATE_METHODS

        private void SetMaterialFill()
        {
            for (int i = 0; i < listMaterials.Count; i++)
            {
                listMaterials[i].SetFloat(variablePropertyID, fill);
            }
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region CO-ROUTINES

        #endregion
    }
}