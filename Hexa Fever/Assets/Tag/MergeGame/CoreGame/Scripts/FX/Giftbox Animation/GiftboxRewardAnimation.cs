using Sirenix.OdinInspector;
using System;
using System.Collections;
using Tag.HexaStack;
using UnityEngine;

namespace Tag.CoreGame.Animation
{
    public class GiftboxRewardAnimation : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private GiftboxReward[] _giftboxRewards;
        [SerializeField] private BaseReward[] _rewardDatas;
        private Action _onRewardClaimed;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetRewardView(BaseReward[] rewardDatas)
        {
            _rewardDatas = rewardDatas;

            int j = 0;
            switch (_rewardDatas.Length)
            {
                case 1:
                    for (int i = 0; i < _giftboxRewards.Length; i++)
                    {
                        if (i == 5)
                        {
                            SetReward(_giftboxRewards[i], _rewardDatas[j]);
                            j++;
                        }
                        else
                        {
                            _giftboxRewards[i].HideView();
                        }
                    }
                    StartCoroutine(PlayRewardIdleAnimation());
                    break;
                case 2:
                    for (int i = 0; i < _giftboxRewards.Length; i++)
                    {
                        if (i == 1 || i == 2)
                        {
                            SetReward(_giftboxRewards[i], _rewardDatas[j]);
                            j++;
                        }
                        else
                        {
                            _giftboxRewards[i].HideView();
                        }
                    }
                    StartCoroutine(PlayRewardIdleAnimation());
                    break;
                case 3:
                    for (int i = 0; i < _giftboxRewards.Length; i++)
                    {
                        if (i > 3)
                        {
                            SetReward(_giftboxRewards[i], _rewardDatas[j]);
                            j++;
                        }
                        else
                        {
                            _giftboxRewards[i].HideView();
                        }
                    }
                    StartCoroutine(PlayRewardIdleAnimation());
                    break;
                case 4:
                    for (int i = 0; i < _giftboxRewards.Length; i++)
                    {
                        if (i < 4)
                        {
                            SetReward(_giftboxRewards[i], _rewardDatas[j]);
                            j++;
                        }
                        else
                        {
                            _giftboxRewards[i].HideView();
                        }
                    }
                    StartCoroutine(PlayRewardIdleAnimation());
                    break;
                case 5:
                    for (int i = 0; i < _giftboxRewards.Length; i++)
                    {
                        if (i < 4 || i == 5)
                        {
                            SetReward(_giftboxRewards[i], _rewardDatas[j]);
                            j++;
                        }
                        else
                        {
                            _giftboxRewards[i].HideView();
                        }
                    }
                    StartCoroutine(PlayRewardIdleAnimation());
                    break;
                case 6:
                    for (int i = 0; i < _giftboxRewards.Length; i++)
                    {
                        if (i != 5)
                        {
                            SetReward(_giftboxRewards[i], _rewardDatas[j]);
                            j++;
                        }
                        else
                        {
                            _giftboxRewards[i].HideView();
                        }
                    }
                    StartCoroutine(PlayRewardIdleAnimation());
                    break;
                case 7:
                    for (int i = 0; i < _giftboxRewards.Length; i++)
                    {

                        SetReward(_giftboxRewards[i], _rewardDatas[j]);
                        j++;
                    }
                    StartCoroutine(PlayRewardIdleAnimation());
                    break;
                default:
                    Debug.Log("Enter reward Count between 1-7");
                    break;
            }
        }

        public void PlayRewardCollectionAnimation(Action onAnimationComplete)
        {
            _onRewardClaimed = onAnimationComplete;
            //  FilterReward();
            AnimateReward();
        }

        #endregion

        #region PRIVATE_FUNCTIONS
        private void SetReward(GiftboxReward giftboxReward, BaseReward rewardData)
        {
            giftboxReward.ShowView();
            giftboxReward.rewardImage.sprite = rewardData.GetRewardImageSprite();
            giftboxReward.rewardCount.text = "x" + rewardData.GetAmountStringForUI();
        }
        private void AnimateReward()
        {
            StartCoroutine(PlayRewardCollectionCoroutine());
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator PlayRewardIdleAnimation()
        {
            yield return new WaitForSeconds(1f);
            yield return new WaitForSeconds(0.45f);
            for (int i = 0; i < _giftboxRewards.Length; i++)
            {
                if (_giftboxRewards[i].gameObject.activeSelf)
                {
                    _giftboxRewards[i].PlayRewardIdleAnimation();
                    yield return new WaitForSeconds(0.2f);
                    _giftboxRewards[i].PlayParticle();
                }
            }
        }

        private IEnumerator PlayRewardCollectionCoroutine()
        {
            yield return new WaitForSeconds(1f);
            _onRewardClaimed?.Invoke();
            /* FxItemFactory fxItemFactory = FxItemFactory;
             int rewardIndex = 0;
             for (int i = 0; i < _normalReward.Count; i++)
             {
                 RewardData rewardData = _normalReward[rewardIndex++];
                 if (DataManager.IsIdManagedByCharacter(rewardData.rewardId.stringId))
                 {
                     if (GameManager.IsInMeta() || IsEventPopupOpen())
                         fxItemFactory.PlayRewardCollectionAnimation(rewardData.rewardId, _normalRewardViews[i].transform);
                     else
                         fxItemFactory.PlayCharacterRewardCollectionAnimation(rewardData.rewardId, _normalRewardViews[i].transform);
                 }
                 else
                 {
                     fxItemFactory.PlayCurrencyCollectAnimation(rewardData.rewardId, _normalRewardViews[i].transform.position, 5, 0f);
                 }
                 _normalRewardViews[i].HideView();
                 yield return new WaitForSeconds(0.1f);
             }
             yield return new WaitForSeconds(1f);
             _onRewardClaimed?.Invoke();*/
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
