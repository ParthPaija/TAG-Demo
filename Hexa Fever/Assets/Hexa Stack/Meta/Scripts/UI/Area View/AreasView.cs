using System;
using System.Collections.Generic;
using Tag.CoreGame;
using Tag.MetaGame.TaskSystem;
using UnityEngine;
using Tag.HexaStack;
using DG.Tweening;

namespace Tag.MetaGame
{
    public class AreasView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private ScrollRectEnsureVisible _scrollRectEnsureVisible;
        [SerializeField] private RectTransform _parentRectTransform;
        [SerializeField] private GameObject _comingSoonPrefab;
        [SerializeField] private AreaView _areaViewPrefab;
        [SerializeField] private AreaView[] _areaViews;
        [SerializeField] private AreaView _currentActiveAreaView;

        private MainView MainView { get { return MainSceneUIManager.Instance.GetView<MainView>(); } }
        private AreaManager AreaManager { get { return AreaManager.Instance; } }
        private AreaAssetStateHandler AreaAssetStateHandler { get { return AreaAssetStateHandler.Instance; } }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS
        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);
            gameObject.SetActive(true);
            MainView.Hide();
            if (_currentActiveAreaView == null)
            {
                if (_areaViews.Length == 0)
                {
                    SetViewData();
                }
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    if (_currentActiveAreaView == null)
                    {
                        string lastCompletedAreaId = AreaManager.GetLastCompleteAreaId();
                        for (int i = 0; i < _areaViews.Length; i++)
                        {
                            if (_areaViews[i].areaId == lastCompletedAreaId)
                            {
                                _scrollRectEnsureVisible.CenterOnItem(_areaViews[i].GetComponent<RectTransform>());
                                break;
                            }
                        }
                    }
                    else
                    {
                        _scrollRectEnsureVisible.CenterOnItem(_currentActiveAreaView.GetComponent<RectTransform>());
                    }
                });
            }
            else
            {
                _currentActiveAreaView.SetView(SetCurrentActiveAreaView);
                _scrollRectEnsureVisible.CenterOnItem(_currentActiveAreaView.GetComponent<RectTransform>());
            }
        }

        public override void OnViewShowDone()
        {
            base.OnViewShowDone();
        }

        public void UpdateView()
        {
            if (_currentActiveAreaView == null)
            {
                if (_areaViews.Length == 0)
                {
                    SetViewData();
                }
                else
                {
                    for (int i = 0; i < _areaViews.Length; i++)
                    {
                        _areaViews[i].SetView(SetCurrentActiveAreaView);
                    }
                }
            }
            else
            {
                _currentActiveAreaView.SetView(SetCurrentActiveAreaView);
            }
            int nextIndex = Array.IndexOf(_areaViews, _currentActiveAreaView) + 1;

            if (nextIndex < _areaViews.Length)
            {
                _areaViews[nextIndex].SetView(SetCurrentActiveAreaView);
            }
        }

        public void HideView()
        {
            gameObject.SetActive(false);
        }

        public void OnBack()
        {
            MainView.ShowView();
            HideView();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetViewData()
        {
            Instantiate(_comingSoonPrefab, _parentRectTransform);
            int totalArea = AreaAssetStateHandler.TotalAreaNo;
            _areaViews = new AreaView[totalArea];
            for (int i = totalArea - 1; i > -1; i--)
            {
                AreaView areaView = Instantiate(_areaViewPrefab, _parentRectTransform);
                areaView.areaId = AreaUtility.AreaNoToAreaId(i + 1);
                areaView.SetAreaNumberHeading(i);
                areaView.SetView(SetCurrentActiveAreaView);
                _areaViews[i] = areaView;
            }
        }

        private void SetCurrentActiveAreaView(AreaView areaView)
        {
            _currentActiveAreaView = areaView;
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
