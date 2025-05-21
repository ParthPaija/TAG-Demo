using System;
using UnityEngine;

namespace Tag.MetaGame
{
    public class DecoreIcon : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        private Action _onClickAction;

        private AreaEditMode AreaEditMode { get { return AreaEditMode.Instance; } }
        private AreaManager AreaManager { get { return AreaManager.Instance; } }

        #endregion

        #region UNITY_CALLBACKS

        private void OnMouseDown()
        {
            AreaEditMode.HideIcons();
            _onClickAction.Invoke();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void RegisterOnClickAction(Action onClickAction)
        {
            _onClickAction = onClickAction;
            AreaManager.RegisterIconActions(ShowIcon, HideIcon);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void ShowIcon()
        {
            gameObject.SetActive(true);
        }

        private void HideIcon()
        {
            gameObject.SetActive(false);
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
