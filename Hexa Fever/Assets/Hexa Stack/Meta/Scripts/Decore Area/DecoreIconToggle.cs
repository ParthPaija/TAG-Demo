using Tag.HexaStack;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.MetaGame
{
    public class DecoreIconToggle : MonoBehaviour
    {
        #region PUBLIC_VARS
        #endregion

        #region PRIVATE_VARS
        [SerializeField] Toggle _toggleIcons;
        private AreaManager AreaManager { get { return AreaManager.Instance; } }
        private SoundHandler SoundHandler { get { return SoundHandler.Instance; } }
        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowIcon()
        {
            _toggleIcons.isOn = false;
            gameObject.SetActive(true);
        }

        public void HideIcon()
        {
            _toggleIcons.isOn = true;
            gameObject.SetActive(false);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void ToggleIcon()
        {
            SoundHandler.PlaySound(SoundType.ButtonClick);
            AreaManager areaManager = AreaManager;
            if (_toggleIcons.isOn)
            {
                areaManager.RaiseHideIconsAction();
            }
            else
            {
                areaManager.RaiseShowIconsAction();
            }
        }

        #endregion
    }
}
