using UnityEngine;

namespace Tag.HexaStack
{
    public class RemoveAdsButton : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        private void Start()
        {
            SetState();
        }

        private void OnEnable()
        {
            RemoveAdsHandler.Instance.AddListenerOnRemoveAdRefresh(SetState);
        }

        private void OnDisable()
        {
            RemoveAdsHandler.Instance.RemoveListenerOnRemoveAdRefresh(SetState);
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetState()
        {
            gameObject.SetActive(!DataManager.PlayerData.IsNoAdsActive());
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnRemoveAdsButtonClick()
        {
            GlobalUIManager.Instance.GetView<RemoveAdsView>().Show();
        }

        #endregion
    }
}
