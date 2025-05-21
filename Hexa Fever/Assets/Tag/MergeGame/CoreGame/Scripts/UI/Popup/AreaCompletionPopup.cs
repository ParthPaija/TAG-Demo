using I2.Loc;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using Tag.HexaStack;
using Tag.MetaGame.TaskSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.MetaGame
{
    public class AreaCompletionPopup : BaseView
    {
        #region PUBLIC_VARS

        public SpineCharacterPopupAnimation emmaPopupAnimation;
        //public TranslateObject emmaTranslateObject;
        public ParticleSystem _effect;
        public Animator completeFontAnimator;
        public CanvasGroup _cg;
        public Localize areaName;

        #endregion

        #region PRIVATE_VARS

        private SoundHandler SoundHandler { get { return SoundHandler.Instance; } }
        private AreaEditMode AreaEditMode { get { return AreaEditMode.Instance; } }
        private AreaManager AreaManager { get { return AreaManager.Instance; } }
        private MainView MainView { get { return MainSceneUIManager.Instance.GetView<MainView>(); } }
        private BottombarView BottombarView { get { return MainSceneUIManager.Instance.GetView<BottombarView>(); } }

        #endregion

        #region UNITY_CALLBACKS
        #endregion

        #region PUBLIC_FUNCTIONS

        [Button]
        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show();
            areaName.SetTerm(AreaSpriteHandler.Instance.GetAreaName(AreaManager.CurrentAreaId));
            SoundHandler.PlaySound(SoundType.AreaCompleteBackground);
            _effect.Play();
            emmaPopupAnimation.charactorToAnimate.gameObject.SetActive(true);
            emmaPopupAnimation.PlayInAnimation(() => { emmaPopupAnimation.PlayIdleAnimation(); });
            //emmaTranslateObject.OnTranslate(delegate { emmaPopupAnimation.PlayIdleAnimation(); });
            SoundHandler.PlaySound(SoundType.AreaCompleteText);
            completeFontAnimator.Play("In_Animation");
        }

        public override void Hide()
        {
            base.Hide();
            _effect.Stop();
        }

        public override void OnBackButtonPressed()
        {
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        private IEnumerator BGAlphaAnimation()
        {
            float i = 0;
            float rate = 1 / 0.5f;
            while (i < 1)
            {
                i += Time.deltaTime * rate;
                _cg.alpha = Mathf.Lerp(1f, 0f, i);
                yield return null;
            }
        }

        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS

        public void OnTapToContinueClicked()
        {
            Hide();
            SoundHandler.StopSound(SoundType.AreaCompleteBackground);
            AreaManager.OnAreaComplete(delegate
            {
                RateUsManager.Instance.OpenRateUsView(() =>
                {
                });
                AreaEditMode.EnableAreaCollider();
                MainView.Show();
                BottombarView.Show();
            });
        }

        #endregion
    }
}
