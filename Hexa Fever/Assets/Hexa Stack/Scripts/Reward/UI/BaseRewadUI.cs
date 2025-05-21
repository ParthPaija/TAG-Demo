using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class BaseRewadUI : MonoBehaviour
    {
        #region PUBLIC_VARS
        public RectTransform RewardTransform { get => rewardImage.rectTransform; }
        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Image rewardImage;
        [SerializeField] private Text rewardValueText;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetReward(BaseReward baseReward)
        {
            gameObject.SetActive(true);
            //rewardImage.sprite = baseReward.GetRewardImageSprite();
            rewardValueText.text = baseReward.GetAmount().ToString();
        }

        public void IncreseRewardCount(int value)
        {
            int count = int.Parse(rewardValueText.text) + value;
            rewardValueText.text = count.ToString();
        }

        public void SetRewardCount(int value)
        {
            rewardValueText.text = value.ToString();
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
