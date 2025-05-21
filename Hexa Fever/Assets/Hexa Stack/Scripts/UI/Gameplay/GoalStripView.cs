using DG.Tweening;
using I2.Loc;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class GoalStripView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        public Localize levelTextLocalize;
        public LocalizationParamsManager levelNoLocalizedParam;
        [SerializeField] private Image goalImage;
        [SerializeField] private Text goalValueText;
        [SerializeField] private Animator animator;
        [SerializeField] private string inAnimationName;

        private Action onComplate;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ShowView(Action onComplate)
        {
            this.onComplate = onComplate;
            base.Show();
            BaseLevelGoal baseLevelGoal = GameplayManager.Instance.CurrentLevel.LevelGoals[0];
            goalImage.sprite = baseLevelGoal.GetRender();
            goalValueText.text = baseLevelGoal.GoalCount.ToString();

            //if (levelNoText != null)
            //levelNoText.text = $"{GameplayManager.Instance.CurrentHandler.eventNameTag} {GameplayManager.Instance.LevelNo}";

            levelTextLocalize.SetTerm(GameplayManager.Instance.CurrentHandler.eventNameTag);
            levelNoLocalizedParam.SetParameterValue(levelNoLocalizedParam._Params[0].Name, (GameplayManager.Instance.LevelNo).ToString());
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
