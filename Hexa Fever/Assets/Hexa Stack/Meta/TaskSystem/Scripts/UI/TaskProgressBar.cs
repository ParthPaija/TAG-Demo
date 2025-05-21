using System.Collections;
using System.Collections.Generic;
using Tag.CoreGame;
using Tag.HexaStack;
using Tag.MetaGame.Animation;
using Tag.RewardSystem;
using Tag.TaskSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.MetaGame.TaskSystem
{
    public class TaskProgressBar : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private RectTransform _progressTransform;
        [SerializeField] private Image[] _giftboxes;
        [SerializeField] private Sprite[] _giftboxLockedImages;
        [SerializeField] private Sprite[] _giftboxUnlockImages;

        [SerializeField] private RectFillBar _fillBar;
        [SerializeField] private Text progressText;

        private TaskManager TaskManager { get { return TaskManager.Instance; } }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetProgressBar(float percentage)
        {
            //progressText.text = percentage + "%";
            progressText.text = TaskManager.GetCompletedTaskProgressString();
            _fillBar.Fill(percentage / 100);
        }

        public void PrepareGifts(AreaRewardData[] areaRewardDatas)
        {
            //SetGiftBoxPosition(areaRewardDatas);
            SetGiftBoxSprite(areaRewardDatas);
        }

        public void ResetGift()
        {
            for (int i = 0; i < _giftboxes.Length; i++)
            {
                _giftboxes[i].gameObject.SetActive(false);
                _giftboxes[i].sprite = _giftboxLockedImages[i];
            }
        }
        public void SetGiftBoxSprite(AreaRewardData[] areaRewardDatas)
        {
            int lastRewardClaimedPercentage = TaskManager.LastRewardClaimedPercentage;
            for (int i = 0; i < areaRewardDatas.Length; i++)
            {
                Image chestImage = GetGiftboxByType(areaRewardDatas[i].chestType);
                chestImage.gameObject.SetActive(true);
                if (lastRewardClaimedPercentage >= areaRewardDatas[i].rewardClaimPercentage)
                    chestImage.sprite = _giftboxUnlockImages[(int)areaRewardDatas[i].chestType - 1];
            }
        }

        public Image GetGiftboxByType(GiftBoxType chestType)
        {
            switch (chestType)
            {
                case GiftBoxType.Small:
                    return _giftboxes[0];
                case GiftBoxType.Medium:
                    return _giftboxes[1];
                default:
                    return _giftboxes[2];
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetGiftBoxPosition(AreaRewardData[] areaRewardDatas)
        {
            float width = _progressTransform.rect.width;
            for (int i = 0; i < areaRewardDatas.Length; i++)
            {
                Image chestImage = GetGiftboxByType(areaRewardDatas[i].chestType);
                chestImage.transform.localPosition = new Vector3((width * areaRewardDatas[i].rewardClaimPercentage / 100) - (width / 2f), _giftboxes[i].transform.localPosition.y, 0);
            }
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
