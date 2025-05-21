using I2.Loc;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class ElementIntroView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Dictionary<int, GameObject> elements = new Dictionary<int, GameObject>();
        [SerializeField] private Localize elementDesText;
        [SerializeField] private Localize elementNameText;
        private Action onHide;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowView(ElementIntroData elementIntroData, Action onHide = null)
        {
            this.onHide = onHide;
            HideAllElement();
            elements[elementIntroData.elementID].SetActive(true);
            elementDesText.SetTerm(elementIntroData.elementDes);
            elementNameText.SetTerm(elementIntroData.elementName);
            elementIntroData.SetAsShow();
            base.Show();
        }

        public override void OnHideComplete()
        {
            base.OnHideComplete();
            onHide?.Invoke();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void HideAllElement()
        {
            foreach (var item in elements)
            {
                item.Value.SetActive(false);
            }
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnClose()
        {
            Hide();
        }

        #endregion
    }
}
