using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class PlayerProfileAvtarItemView : MonoBehaviour
    {
        #region PUBLIC_VARS

        public int Id
        {
            get { return playerProfileAvtarData.id; }
        }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Image avatarImage;
        [SerializeField] private GameObject selectedGO;
        private PlayerProfileAvtarData playerProfileAvtarData;
        private PlayerProfileview playerProfileview;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView(PlayerProfileAvtarData playerProfileAvtarData, PlayerProfileview playerProfileview)
        {
            this.playerProfileAvtarData = playerProfileAvtarData;
            this.playerProfileview = playerProfileview;
            avatarImage.sprite = playerProfileAvtarData.sprite;
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
            playerProfileview.OnAvtarSelect(this);
        }

        #endregion
    }
}
