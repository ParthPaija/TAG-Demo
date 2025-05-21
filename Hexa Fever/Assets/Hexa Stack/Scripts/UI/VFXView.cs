using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


namespace Tag.HexaStack
{
    public class VFXView : BaseView
    {
        #region private veribales

        [SerializeField] private int initPoolSize;
        [SerializeField] private float animationTime;
        [SerializeField] private CurrencyAnimation coinAnimation;
        [SerializeField] private CurrencyAnimation collectableItemAnimation;
        [SerializeField] private CurrencyAnimation breadAnimation;
        [SerializeField] private RewardToastAnimation rewardToastAnimation;

        private List<Action<int, int, bool>> onItemAnimationComplete = new List<Action<int, int, bool>>();

        #endregion

        #region propertice

        public CurrencyAnimation BreadItemAnimation
        {
            get
            {
                CurrencyAnimation currencyAnimation = ObjectPool.Spawn(breadAnimation, transform);
                currencyAnimation.RegisterObjectAnimationComplete(PlayOnItemAnimationComplete);
                return currencyAnimation;
            }
        }

        public CurrencyAnimation CoinItemAnimation
        {
            get
            {
                CurrencyAnimation currencyAnimation = ObjectPool.Spawn(coinAnimation, transform);
                currencyAnimation.RegisterObjectAnimationComplete(PlayOnItemAnimationComplete);
                return currencyAnimation;
            }
        }

        public CurrencyAnimation ItemAnimation
        {
            get
            {
                CurrencyAnimation currencyAnimation = ObjectPool.Spawn(collectableItemAnimation, transform);
                currencyAnimation.RegisterObjectAnimationComplete(PlayOnItemAnimationComplete);
                return currencyAnimation;
            }
        }

        public RewardToastAnimation RewardToastAnimation => rewardToastAnimation;
        #endregion

        #region public methods

        public void PlayItemAnimation(GoalType goalType, Vector3 startPos, int amount, int itemId)
        {
            if (itemId == 9)
            {
                List<Transform> endPosition = MainSceneUIManager.Instance.GetView<GameplayGoalView>().GetItemEndPosition(goalType, itemId);
                for (int i = 0; i < endPosition.Count; i++)
                {
                    CoinItemAnimation.StartAnimation(
                      startPos, endPosition[i], amount, ResourceManager.Instance.GetGoalSprite(goalType, itemId), itemId: itemId);
                }
            }
            else
            {
                List<Transform> endPosition = MainSceneUIManager.Instance.GetView<GameplayGoalView>().GetItemEndPosition(goalType, itemId);
                for (int i = 0; i < endPosition.Count; i++)
                {
                    ItemAnimation.StartAnimation(
                      startPos, endPosition[i], amount, ResourceManager.Instance.GetGoalSprite(goalType, itemId), itemId: itemId);
                }
            }
        }

        public void PlayBreadItemAnimation(GoalType goalType, Vector3 startPos, int amount, int itemId)
        {
            List<Transform> endPosition = MainSceneUIManager.Instance.GetView<GameplayGoalView>().GetItemEndPosition(goalType, itemId);
            for (int i = 0; i < endPosition.Count; i++)
            {
                BreadItemAnimation.StartAnimation(
                  startPos, endPosition[i], amount, ResourceManager.Instance.GetGoalSprite(goalType, itemId), itemId: itemId);
            }
        }

        public void PlayOnItemAnimationComplete(int itemId, int size, bool isLastObject)
        {
            for (int i = 0; i < onItemAnimationComplete.Count; i++)
            {
                if (onItemAnimationComplete[i] != null)
                    onItemAnimationComplete[i].Invoke(itemId, size, isLastObject);
            }
        }

        public void RagisterOnItemAnimationComplete(Action<int, int, bool> action)
        {
            if (!onItemAnimationComplete.Contains(action))
                onItemAnimationComplete.Add(action);
        }

        public void DeRagisterOnItemAnimationComplete(Action<int, int, bool> action)
        {
            if (onItemAnimationComplete.Contains(action))
                onItemAnimationComplete.Remove(action);
        }

        #endregion

        #region private methods

        #endregion

        #region Coroutine

        #endregion
    }
}