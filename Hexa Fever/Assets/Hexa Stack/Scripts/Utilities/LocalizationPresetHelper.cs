using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace I2.Loc
{
    public class LocalizationPresetHelper : MonoBehaviour
    {
        #region private veriables

        [SerializeField] private string term;
        private TMP_Text text;

        #endregion

        #region unity callback

        private void OnEnable()
        {
            OnLocalize();
            LocalizationManager.OnLocalizeEvent += OnLocalize;
        }

        private void OnDisable()
        {
            LocalizationManager.OnLocalizeEvent -= OnLocalize;
        }

        #endregion

        #region private methods

        public void OnLocalize()
        {
            if (text == null)
                text = GetComponent<TMP_Text>();
            if (text == null)
                return;
            // must be font metrial preset in I2l under the Font preset material (Example Font preset material/BalckOutline)
            try
            {
                text.fontSharedMaterial = LocalizationManager.GetTranslatedObjectByTermName<Material>("FontAssets/" + term);
            }
            catch (Exception e)
            {
                //Debug.Log("Outline Color : " + term + " text : " + text.text);
                //Debug.LogError("Term " + term + " not fount in Font preset material");
            }
        }

        public void OnValidate()
        {
            //text = GetComponent<TMP_Text>();
            //string materialName = text.fontSharedMaterial.name;
            //string fontName = text.font.name;
            //fontName = fontName.Replace(" SDF", "");
            //materialName = materialName.Replace(fontName + " Atlas-", "");
            //term = materialName;
        }

        [Button]
        public void Validate()
        {
            text = GetComponent<TMP_Text>();
            string materialName = text.fontSharedMaterial.name;
            string fontName = text.font.name;
            fontName = fontName.Replace(" SDF ", "");
            materialName = materialName.Replace(fontName + " Material ", "");
            term = materialName;
        }
        #endregion
    }
}