using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using Tag.AssetManagement;
using Tag.CoreGame;
using Tag.HexaStack;
using Tag.MetaGame.TaskSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.MetaGame
{
    public class AreaView : MonoBehaviour
    {
        #region PUBLIC_VARS

        public string areaId;

        #endregion

        #region PRIVATE_VARS
        [Header("Download")]
        [SerializeField] private Button _downloadButton;
        [SerializeField] private ScaleWithCurve _downloadButtonAnimation;
        [SerializeField] private RectFillBar _downloadFillBar;
        [SerializeField] private Text _downloadFillProgressText;
        [SerializeField] private GameObject _downloadFillBarGO;
        [SerializeField] private GameObject _downloadViewGO;

        [Space(50)]
        [SerializeField] private GameObject _lockedAreaViewGO;
        [SerializeField] private GameObject _unlockedAreaViewGO;
        [SerializeField] private GameObject _completeAreaViewGO;
        [SerializeField] private GameObject _viewButtonGO;
        [SerializeField] private Image _lockedImage;
        [SerializeField] private Image _emptyImage;
        [SerializeField] private Image _completeImage;
        [SerializeField] private RectFillBar _editFillBar;
        [SerializeField] private Text _progressText;
        [SerializeField] private Localize[] _headersText;
        [SerializeField] private Text[] _areasNumberText;

        private DownloadOperationHandle _currentDownloadHandle;
        private AssetAddress _assetAddress;
        public bool IsRunningView { get { return _unlockedAreaViewGO.activeSelf; } }

        private TaskManager TaskManager { get { return TaskManager.Instance; } }
        private AreaManager AreaManager { get { return AreaManager.Instance; } }
        private AreasView AreasView { get { return MainSceneUIManager.Instance.GetView<AreasView>(); } }
        private AreaEditMode AreaEditMode { get { return AreaEditMode.Instance; } }
        private SoundHandler SoundHandler { get { return SoundHandler.Instance; } }
        private InGameLoadingView InGameLoadingView { get { return GlobalUIManager.Instance.GetView<InGameLoadingView>(); } }
        private AreaSpriteHandler AreaSpriteHandler { get { return AreaSpriteHandler.Instance; } }
        private IDownloader Downloader { get { return AssetManagerAddressable.Instance.downloaderAddressable; } }
        private InternetManager InternetManager { get { return InternetManager.Instance; } }
        private AreaAssetStateHandler AreaAssetStateHandler { get { return AreaAssetStateHandler.Instance; } }
        private MainView MainView { get { return MainSceneUIManager.Instance.GetView<MainView>(); } }
        private BottombarView BottombarView { get { return MainSceneUIManager.Instance.GetView<BottombarView>(); } }

        #endregion

        #region UNITY_CALLBACKS
        private void OnEnable()
        {
            if (_currentDownloadHandle != null)
            {
                ResumeDownload();
            }
        }
        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView(Action<AreaView> action)
        {
            SetHeaderText(AreaSpriteHandler.GetAreaName(areaId));
            AreaManager areaManager = AreaManager;
            bool isLocked = areaManager.IsAreaLocked(areaId);
            if (isLocked)
            {
                _lockedImage.sprite = AreaSpriteHandler.GetAreaEmptySprite(areaId);
                _lockedAreaViewGO.SetActive(true);
            }
            else
            {
                _lockedAreaViewGO.SetActive(false);
                AreaState areaState = areaManager.GetAreaState(areaId);
                switch (areaState)
                {
                    case AreaState.Running:
                        float progress = TaskManager.GetCompletedTaskPercentage();
                        _emptyImage.sprite = AreaSpriteHandler.GetAreaEmptySprite(areaId);
                        _progressText.text = progress + "%";
                        _editFillBar.Fill(progress / 100);
                        _unlockedAreaViewGO.SetActive(true);
                        _completeAreaViewGO.SetActive(false);
                        action.Invoke(this);
                        break;
                    case AreaState.Complete:
                        _completeImage.sprite = AreaSpriteHandler.GetAreaCompleteSprite(areaId);
                        _unlockedAreaViewGO.SetActive(false);
                        _completeAreaViewGO.SetActive(true);
                        CheckForDownloadArea();
                        break;
                }
            }


        }

        public void SetAreaNumberHeading(int index)
        {
            for (int i = 0; i < _areasNumberText.Length; i++)
            {
                _areasNumberText[i].text = (index + 1).ToString();
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS
        private void CheckForDownloadArea()
        {
            _assetAddress = AssetAddress.GenerateAreaAddress(areaId);
            AssetState assetState = AreaAssetStateHandler.GetAssetState(_assetAddress.assetKey);
            if (assetState == AssetState.INVALID_ADDRESS)
            {
                throw new Exception($"Not Valid AreaId {_assetAddress.assetKey}");
            }
            bool isAreaDownloaded = (assetState == AssetState.DOWNLOADED);
            _downloadViewGO.SetActive(!isAreaDownloaded);
            _downloadButtonAnimation.gameObject.SetActive(!isAreaDownloaded);
            _viewButtonGO.SetActive(isAreaDownloaded);
        }
        private void ResumeDownload()
        {
            InternetManager.Instance.CheckNetConnection((IsConnectionAvailable) =>
            {
                if (IsConnectionAvailable)
                    StartCoroutine(DownloadProgressCO());
                /* else
                     ActiveDownloadButton();*/
            });
        }
        private void DownloadArea()
        {
            InternetManager.Instance.CheckNetConnection((IsConnectionAvailable) =>
            {
                if (IsConnectionAvailable)
                    StartCoroutine(DownloadAreaCO());
                else
                    ActiveDownloadButton();
            });
        }

        private void SetHeaderText(string areaName)
        {
            for (int i = 0; i < _headersText.Length; i++)
            {
                _headersText[i].SetTerm(areaName);
            }
        }
        private void ActiveDownloadButton()
        {
            _downloadButton.interactable = true;
            _downloadButtonAnimation.gameObject.SetActive(true);
            _downloadButtonAnimation.StartAnimation();
        }
        #endregion

        #region CO-ROUTINES
        private IEnumerator DownloadAreaCO()
        {
            _currentDownloadHandle = Downloader.DownloadAsset(_assetAddress);
            yield return DownloadProgressCO();
        }

        private IEnumerator<WaitForSeconds> DownloadProgressCO()
        {
            _downloadFillBarGO.SetActive(true);
            DownloadOperationHandle downloadOperationHandle = _currentDownloadHandle;
            WaitForSeconds waitForSeconds = new WaitForSeconds(1);
            while (!downloadOperationHandle.IsDone)
            {
                yield return waitForSeconds;
                _downloadFillBar.Fill(downloadOperationHandle.progress);
                _downloadFillProgressText.text = (downloadOperationHandle.progress * 100).ToString("0.00") + "%";
            }
            _downloadFillBarGO.SetActive(false);
            if (downloadOperationHandle.downloadStatus)
            {
                AreaAssetStateHandler.SetAssetState(_assetAddress.assetKey, AssetState.DOWNLOADED);
                _downloadViewGO.SetActive(false);
                _viewButtonGO.SetActive(true);
            }
            else
            {
                ActiveDownloadButton();
            }
            _currentDownloadHandle = null;
        }
        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnEditButtonClick()
        {
            SoundHandler.PlaySound(SoundType.ButtonClick);
            MainView.Hide();
            BottombarView.Hide();
            AreasView areasView = AreasView;
            areasView.HideView();
            AreaEditMode.AreaEditModeOn(areasView.ShowView);
            AreaEditMode.ShowIcons();
        }

        public void OnViewButtonClick()
        {
            SoundHandler.PlaySound(SoundType.ButtonClick);
            MainView.Hide();
            BottombarView.Hide();
            InGameLoadingView.Show(
                delegate
                {
                    AreaManager.VisitArea(areaId, AreasView.ShowView);
                });
        }
        public void OnDownloadButtonClick()
        {
            _downloadButton.interactable = false;
            _downloadButtonAnimation.StartReverseAnimation(delegate { _downloadButtonAnimation.gameObject.SetActive(false); });
            DownloadArea();
        }
        #endregion
    }
}
