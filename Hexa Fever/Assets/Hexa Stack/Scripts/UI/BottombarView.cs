using Sirenix.OdinInspector;
using System.Collections.Generic;
using Tag.MetaGame;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class BottombarView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private List<BottomBarButton> bottomBarButtons = new List<BottomBarButton>();
        private BottomBarButton currentBottomBarButton;
        [SerializeField] private List<RectTransform> updateLayoutRects;

        #endregion

        #region UNITY_CALLBACKS
        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Init()
        {
            base.Init();
        }

        [Button]
        public void InitView()
        {
            Show();
            OnTapButton(BottomBarButtonsType.Home);
            OnHomeButtonClick();
        }

        public void OnTapButton(BottomBarButtonsType bottomBarButtonType)
        {
            bottomBarButtons[(int)bottomBarButtonType].OnClick();
        }

        public void OnBottomBarButtonClick(BottomBarButton bottomBarButton)
        {
            if (currentBottomBarButton != null)
                currentBottomBarButton.OnDeselect();
            for (int i = 0; i < bottomBarButtons.Count; i++)
            {
                if (bottomBarButtons[i] == bottomBarButton)
                {
                    currentBottomBarButton = bottomBarButton;
                    bottomBarButtons[i].OnSelect();
                }
            }
            updateLayoutRects.ForEach(x => LayoutRebuilder.ForceRebuildLayoutImmediate(x));
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void HideAllView()
        {
            MainSceneUIManager.Instance.GetView<MainView>().Hide();
            MainSceneUIManager.Instance.GetView<AreasView>().Hide();
            MainSceneUIManager.Instance.GetView<ShopView>().Hide();
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnHomeButtonClick()
        {
            HideAllView();
            MainSceneUIManager.Instance.GetView<MainView>().Show();
        }
        
        public void OnAreaExploreButtonClick()
        {
            HideAllView();
            MainSceneUIManager.Instance.GetView<AreasView>().Show();
        }

        public void OnShopButtonClick()
        {
            HideAllView();
            MainSceneUIManager.Instance.GetView<ShopView>().Show(-1);
        }

        public void OnLockButtonClick(Transform transform)
        {
            GlobalUIManager.Instance.GetView<ToastMessageView>().ShowMessage("COMING SOON!", transform.position.With(null, null, 0));
        }

        #endregion
    }

    public enum BottomBarButtonsType
    {
        Skin,
        Shop,
        Home,
        LeaderBoard,
        Areas,
    }
}
