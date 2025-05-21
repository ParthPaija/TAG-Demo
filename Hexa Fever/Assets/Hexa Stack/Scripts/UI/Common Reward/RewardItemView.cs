using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class RewardItemView : MonoBehaviour
    {
        #region PUBLIC_VARS

        public RewardType targetRewardType;
        public CanvasGroup CanvasGroup => canvasGroup;
        public Image RewardImage => rewardImage;
        public RectTransform AnimationParent => animationParent;

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Image rewardImage;
        [SerializeField] private Text rewardText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform animationParent;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView(BaseReward baseReward)
        {
            rewardImage.sprite = baseReward.GetRewardImageSprite();
            rewardText.text = "x" + baseReward.GetAmountStringForUI();
            gameObject.SetActive(true);
        }

        public void PlayRewardItemAnimation()
        {

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

        [Button]
        public void GetRef()
        {
            animationParent = transform.GetComponent<RectTransform>();
            canvasGroup = transform.GetComponent<CanvasGroup>();
            rewardImage = transform.GetComponent<Image>();
            rewardText = transform.GetComponentInChildren<Text>();
        }
    }
}
