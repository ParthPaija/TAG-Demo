using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexa_Stack
{
    public class LoadingViewGridCell : MonoBehaviour
    {
        #region PUBLIC_VARS

        public bool IsOccupied { get => isOccupied; set { isOccupied = value; } }

        public LoadingViewGridStack stack;

        public List<LoadingViewGridCell> nearCells;

        #endregion

        #region PRIVATE_VARS
        private bool isOccupied = false;

        private LoadingViewGridStack initStack;
        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetInitialData()
        {
            if (stack != null)
            {
                initStack = stack;
                initStack.SetInitialData();
            }
        }

        public void ResetData()
        {
            stack = initStack;
            if (stack != null)
                stack.ResetData();
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
