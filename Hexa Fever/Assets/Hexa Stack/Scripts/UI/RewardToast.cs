using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class RewardToast : MonoBehaviour
    {
        #region PUBLIC_VARS

        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;
        #endregion

        #region PRIVATE_VARS
        [SerializeField] private Image rewardImage;
        [SerializeField] private Text quantity;
        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetRewardData(Sprite sprite, int value)
        {
            rewardImage.sprite = sprite;
            quantity.text = "+" + value;
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
