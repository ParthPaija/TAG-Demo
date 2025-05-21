using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class PlayerProfileFrameItemView : MonoBehaviour
    {
        #region PUBLIC_VARS

        public int Id
        {
            get { return playerProfileFrameData.id; }
        }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Image frameImage;
        [SerializeField] private GameObject selectedGO;
        private PlayerProfileFrameData playerProfileFrameData;
        private PlayerProfileview playerProfileview;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView(PlayerProfileFrameData playerProfileFrameData, PlayerProfileview playerProfileview)
        {
            this.playerProfileFrameData = playerProfileFrameData;
            this.playerProfileview = playerProfileview;
            frameImage.sprite = playerProfileFrameData.sprite;
            SetSelectionMark(false);
        }

        public void SetSelectionMark(bool isActive)
        {
            selectedGO.SetActive(isActive);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnClick()
        {
            playerProfileview.OnFrameSelect(this);
        }

        #endregion
    }
}
