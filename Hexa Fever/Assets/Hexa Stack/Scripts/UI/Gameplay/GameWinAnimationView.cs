using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class GameWinAnimationView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Animator animator;
        [SerializeField] private string inAnimationName;
        private Action onComplate;

        #endregion

        #region UNITY_CALLBACKS

        public void ShowView(Action onComplate)
        {
            MainSceneUIManager.Instance.GetView<GameplayTopbarView>().Hide();
            MainSceneUIManager.Instance.GetView<GameplayBottomView>().Hide();
            MainSceneUIManager.Instance.GetView<GameplayGoalView>().Hide();
            this.onComplate = onComplate;
            base.Show();
            SoundHandler.Instance.PlaySound(SoundType.LevelComplete);
            CoroutineRunner.Instance.Wait(animator.GetAnimationLength(inAnimationName), () =>
            {
                Hide();
            });
        }

        public override void OnHideComplete()
        {
            onComplate?.Invoke();
            base.OnHideComplete();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

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
