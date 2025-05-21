using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class PlayerProfileButton : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private PlayerProfileDataSO playerProfileDataSO;
        [SerializeField] private Image avtarImage;
        [SerializeField] private Image frameImage;

        #endregion

        #region UNITY_CALLBACKS

        private void OnEnable()
        {
            SetView();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView()
        {
            var data = PlayerPersistantData.GetPlayerProfileData();

            avtarImage.sprite = playerProfileDataSO.GetAvtarSprite(data.avtarId);
            frameImage.sprite = playerProfileDataSO.GetFrameSprite(data.frameId);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnPlayerProfileClick()
        {
            MainSceneUIManager.Instance.GetView<PlayerProfileview>().Show();
        }

        #endregion
    }
}
