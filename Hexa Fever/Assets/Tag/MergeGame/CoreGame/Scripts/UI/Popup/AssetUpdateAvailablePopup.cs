using System;
using System.Collections;
using System.Collections.Generic;
using Tag.CoreGame;
using Tag.HexaStack;
using Tag.MetaGame;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.AssetManagement
{
    public class AssetUpdateAvailablePopup : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS
        [SerializeField] private Button _downloadButton;
        [SerializeField] private ScaleWithCurve _downloadButtonAnimation;
        [SerializeField] private GameObject _downloadFillBarGO;
        [SerializeField] private RectFillBar _downloadFillBar;
        [SerializeField] private Text _downloadFillBarText;
        private string _areaId;
        private Action _onDownloadAction;
        private DownloadOperationHandle _currentDownloadHandle;
        private AssetAddress _assetAddress;



        private AssetDownloadFailedPopup AssetDownloadFailedPopup { get { return MainSceneUIManager.Instance.GetView<AssetDownloadFailedPopup>(); } }
        private IDownloader Downloader { get { return AssetManagerAddressable.Instance.downloaderAddressable; } }
        private InternetManager InternetManager { get { return InternetManager.Instance; } }
        private AreaAssetStateHandler AreaAssetStateHandler { get { return AreaAssetStateHandler.Instance; } }
        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS
        public void ShowPopup(string areaId, Action onDownload)
        {
            _areaId = areaId;
            _onDownloadAction = onDownload;
            base.ShowView();
        }
        public override void Hide()
        {
            base.Hide();
        }

        #endregion

        #region PRIVATE_FUNCTIONS
        private void DownloadArea()
        {
            InternetManager.CheckNetConnection((IsConnectionAvailable) =>
            {
                if (IsConnectionAvailable)
                    StartCoroutine(DownloadAreaCO());
                else
                    ActiveDownloadButton();
            });
        }
        private void Redownload()
        {
            gameObject.SetActive(true);
            DownloadArea();
        }
        private void ActiveDownloadButton()
        {
            _downloadButton.interactable = true;
            _downloadButton.gameObject.SetActive(true);
            _downloadButtonAnimation.StartAnimation();
        }
        #endregion

        #region CO-ROUTINES
        private IEnumerator DownloadAreaCO()
        {
            _assetAddress = AssetAddress.GenerateAreaAddress(_areaId);
            _currentDownloadHandle = Downloader.DownloadAsset(_assetAddress);
            yield return DownloadProgress();
        }

        private IEnumerator<WaitForSeconds> DownloadProgress()
        {
            DownloadOperationHandle downloadOperationHandle = _currentDownloadHandle;
            _downloadFillBarGO.SetActive(true);
            WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
            while (!downloadOperationHandle.IsDone)
            {
                _downloadFillBar.Fill(downloadOperationHandle.progress);
                _downloadFillBarText.text = (downloadOperationHandle.progress * 100).ToString("0.00") + "%";
                yield return waitForSeconds;
            }
            _downloadFillBarGO.SetActive(false);
            if (downloadOperationHandle.downloadStatus)
            {
                AreaAssetStateHandler.SetAssetState(_assetAddress.assetKey, AssetState.DOWNLOADED);
                _onDownloadAction?.Invoke();
                Hide();
            }
            else
            {
                AssetDownloadFailedPopup.ShowPopup(Redownload);
            }
            _currentDownloadHandle = null;
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       
        public void OnDownaloadButtonClick()
        {
            _downloadButton.interactable = false;
            _downloadButtonAnimation.StartReverseAnimation(delegate
            {
                _downloadButtonAnimation.gameObject.SetActive(false);
            });
            DownloadArea();
        }
        #endregion
    }
}
