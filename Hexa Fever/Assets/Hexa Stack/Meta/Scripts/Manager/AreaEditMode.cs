using System;
using System.Collections;
using Tag.CoreGame;
using Tag.HexaStack;
using Tag.MetaGame.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tag.MetaGame
{
    public class AreaEditMode : Manager<AreaEditMode>, IPointerDownHandler, IPointerUpHandler
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private BoxCollider2D _boxCollider2d;
        [SerializeField] private DecoreIconToggle _decoreIconToggle;
        [SerializeField] private Button _backButton;
        [SerializeField] private LongPressAnimatipn _longPressAnimation;
        private float _clickTime;
        private float _waitTime = 1f;
        private Action _exitAction;
        private AreaManager AreaManager { get { return AreaManager.Instance; } }
        private MainView MainView { get { return MainSceneUIManager.Instance.GetView<MainView>(); } }
        private BottombarView BottombarView { get { return MainSceneUIManager.Instance.GetView<BottombarView>(); } }
        private AreasView AreasView { get { return MainSceneUIManager.Instance.GetView<AreasView>(); } }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void AreaEditModeOn(Action exitAction = null)
        {
            _exitAction = exitAction;
            DisableAreaCollider();
        }

        public void AreaEditModeOff()
        {
            EnableAreaCollider();
            HideIcons();
            AreaManager areaManager = AreaManager;
            areaManager.UnloadVisitedArea();
            _exitAction?.Invoke();
        }

        public void ShowIcons()
        {
            //BackHandler.Add(this);
            //_decoreIconToggle.ShowIcon();
            _backButton.gameObject.SetActive(true);
        }

        public void HideIcons()
        {
            //BackHandler.Remove(this);
            //_decoreIconToggle.HideIcon();
            _backButton.gameObject.SetActive(false);
        }

        public void EnableAreaCollider()
        {
            _boxCollider2d.enabled = true;
        }

        public void DisableAreaCollider()
        {
            _boxCollider2d.enabled = false;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        private IEnumerator DoWaitAndCallOnClick()
        {
            yield return new WaitForSeconds(0.1f);
            _longPressAnimation.StartArrowAnimation(Input.mousePosition, (Action)delegate
            {
                //HomeView.HideView();
                //TopView.HideTopBar();
                //AreaEditModeOn((Action)delegate
                //{
                //    HomeView.ShowView();
                //    TopView.ShowTopBar();
                //} );
                ShowIcons();
                AreaManager.RaiseShowIconsAction();
            });
        }

        #endregion

        #region EVENT_HANDLERS
        public void OnPointerDown(PointerEventData eventData)
        {
           /* _clickTime = Time.time;
            StartCoroutine(DoWaitAndCallOnClick());*/
        }

        public void OnPointerUp(PointerEventData eventData)
        {
           /* if (Mathf.Abs(_clickTime - Time.time) < _waitTime)
            {
                _longPressAnimatipn.StopArrowAnimation();
                StopAllCoroutines();
            }*/
        }

        #endregion

        #region UI_CALLBACKS

        public void OnBackButtonClick()
        {
            BottombarView.Show();
            AreasView.Show();
            //SoundHandler.PlaySound(SoundType.ButtonClick);
            AreaEditModeOff();
        }

        public void OnBack()
        {
            if (_backButton.gameObject.activeInHierarchy)
            {
                OnBackButtonClick();
            }
        }

        #endregion
    }
}
