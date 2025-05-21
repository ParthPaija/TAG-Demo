using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class PlayerProfileview : BaseView
    {
        #region PUBLIC_VARS
        public PlayerProfileDataSO PlayerProfileDataSO { get => playerProfileDataSO; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private PlayerProfileDataSO playerProfileDataSO;

        [Header("PlayerProfile")]
        [SerializeField] Image avtarImage;
        [SerializeField] Image frameImage;
        [SerializeField] private InputField playerNameIF;
        private string chanagePlayNameString;

        [Header("PlayerFrame")]
        [SerializeField] private PlayerProfileFrameItemView framePrefab;
        [SerializeField] private Transform frameParent;
        [SerializeField] private GameObject frameGO;
        [SerializeField] private GameObject frameActiveTabGO;
        private int selectedAvtarId;
        private List<PlayerProfileFrameItemView> playerProfileFrameItemViews = new List<PlayerProfileFrameItemView>();

        [Header("PlayerAvtar")]
        [SerializeField] private PlayerProfileAvtarItemView avtarPrefab;
        [SerializeField] private Transform avtarParent;
        [SerializeField] private GameObject avtarGO;
        [SerializeField] private GameObject avtarActiveTabGO;
        private List<PlayerProfileAvtarItemView> playerProfileAvtarItemViews = new List<PlayerProfileAvtarItemView>();
        private int selectedFrameId;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);
            var playerProfileData = PlayerPersistantData.GetPlayerProfileData();
            playerNameIF.text = playerProfileData.playerName;
            chanagePlayNameString = playerProfileData.playerName;
            selectedAvtarId = playerProfileData.avtarId;
            selectedFrameId = playerProfileData.frameId;
            avtarImage.sprite = playerProfileDataSO.GetAvtarSprite(selectedAvtarId);
            frameImage.sprite = playerProfileDataSO.GetFrameSprite(selectedFrameId);
            SetView();
            OnAvtarTabClick();
        }

        public void OnPlayerNameChange()
        {
            if (string.IsNullOrEmpty(playerNameIF.text) || string.IsNullOrWhiteSpace(playerNameIF.text))
                return;

            chanagePlayNameString = playerNameIF.text;
        }

        public void OnAvtarSelect(PlayerProfileAvtarItemView playerProfileAvtarItemView)
        {
            for (int i = 0; i < playerProfileAvtarItemViews.Count; i++)
            {
                playerProfileAvtarItemViews[i].SetSelectionMark(false);
            }
            playerProfileAvtarItemView.SetSelectionMark(true);
            selectedAvtarId = playerProfileAvtarItemView.Id;
        }

        public void OnFrameSelect(PlayerProfileFrameItemView playerProfileFrameItemView)
        {
            for (int i = 0; i < playerProfileFrameItemViews.Count; i++)
            {
                playerProfileFrameItemViews[i].SetSelectionMark(false);
            }
            playerProfileFrameItemView.SetSelectionMark(true);
            selectedFrameId = playerProfileFrameItemView.Id;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetView()
        {
            ClearCurrent();
        }

        private void ClearCurrent()
        {
            frameParent.Clear();
            avtarParent.Clear();
            playerProfileAvtarItemViews = new List<PlayerProfileAvtarItemView>();
            playerProfileFrameItemViews = new List<PlayerProfileFrameItemView>();

            for (int i = 0; i < playerProfileDataSO.playerProfileAvtarDatas.Count; i++)
            {
                PlayerProfileAvtarItemView temp = Instantiate(avtarPrefab, avtarParent);
                temp.SetView(playerProfileDataSO.playerProfileAvtarDatas[i], this);
                playerProfileAvtarItemViews.Add(temp);
            }

            for (int i = 0; i < playerProfileDataSO.playerProfileFrameDatas.Count; i++)
            {
                PlayerProfileFrameItemView temp = Instantiate(framePrefab, frameParent);
                temp.SetView(playerProfileDataSO.playerProfileFrameDatas[i], this);
                playerProfileFrameItemViews.Add(temp);
            }

            playerProfileAvtarItemViews.Find(x => x.Id == selectedAvtarId).SetSelectionMark(true);
            playerProfileFrameItemViews.Find(x => x.Id == selectedFrameId).SetSelectionMark(true);
        }

        private void SetTab(bool isActive)
        {
            avtarGO.SetActive(isActive);
            avtarActiveTabGO.SetActive(isActive);
            frameGO.SetActive(!isActive);
            frameActiveTabGO.SetActive(!isActive);
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        public void OnAvtarTabClick()
        {
            SetTab(true);
        }

        public void OnFrameTabClick()
        {
            SetTab(false);
        }

        public void OnSaveButtonClick()
        {
            var playerProfileData = PlayerPersistantData.GetPlayerProfileData();

            playerProfileData.playerName = chanagePlayNameString;
            playerProfileData.avtarId = selectedAvtarId;
            playerProfileData.frameId = selectedFrameId;

            PlayerPersistantData.SetPlayerProfileData(playerProfileData);
            MainSceneUIManager.Instance.GetView<MainView>().PlayerProfileButton.SetView();
            Hide();
        }

        #endregion
    }
}
