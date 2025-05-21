using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class CellDetailsView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        private BaseCell baseCell;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowView(BaseCell baseCell)
        {
            this.baseCell = baseCell;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetCellDefaultDataView()
        {

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
