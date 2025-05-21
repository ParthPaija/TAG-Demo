using Tag.AssetManagement;
using System;
using System.Collections.Generic;
using Tag.CoreGame;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using I2.Loc;
using Tag.HexaStack;
using Sirenix.OdinInspector;

namespace Tag.MetaGame.TaskSystem
{
    public class AreaUnlockView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Button _downloadButton;
        [SerializeField] private ScaleWithCurve _downloadButtonAnimation;
        [SerializeField] private ScaleWithCurve _unlockButtonAnimation;
        [SerializeField] private Slider _downloadFillBar;
        [SerializeField] private Text _fillBarText;
        [SerializeField] private GameObject _downloadFillBarGO;
        [SerializeField] private Image areaImage;
        [SerializeField] private Localize areaText;
        private AreaManager AreaManager { get { return AreaManager.Instance; } }
        private InGameLoadingView InGameLoadingView { get { return GlobalUIManager.Instance.GetView<InGameLoadingView>(); } }
        private AreaSpriteHandler AreaSpriteHandler { get { return AreaSpriteHandler.Instance; } }
        private ILoader Loader { get { return AssetManagerAddressable.Instance.loaderAddressable; } }
        private IDownloader Downloader { get { return AssetManagerAddressable.Instance.downloaderAddressable; } }
        private BufferingView BufferingScreen { get { return GlobalUIManager.Instance.GetView<BufferingView>(); } }
        private AreaAssetStateHandler AreaAssetStateHandler { get { return AreaAssetStateHandler.Instance; } }

        private string _nextAreaId;
        private AssetAddress _assetAddress;
        private DownloadOperationHandle _currentDownloadHandle;
        private LoadOperationHandle<GameObject> _currentloadHandle;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        [Button]
        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);
            if (_currentDownloadHandle != null)
            {
                ResumeDownload();
                return;
            }

            _downloadButtonAnimation.gameObject.SetActive(false);
            _unlockButtonAnimation.gameObject.SetActive(false);
            _downloadFillBarGO.SetActive(false);
            _nextAreaId = AreaUtility.GetNextAreaId(AreaManager.CurrentAreaId);
            areaImage.sprite = AreaSpriteHandler.GetAreaEmptySprite(_nextAreaId);
            areaText.SetTerm(AreaSpriteHandler.GetAreaName(_nextAreaId));

            CheckForAreaDownload();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void ActiveDownloadButton()
        {
            _downloadButton.interactable = true;
            _downloadButtonAnimation.gameObject.SetActive(true);
            _downloadButtonAnimation.StartAnimation();
        }

        private void ActiveUnlockButton()
        {
            _unlockButtonAnimation.gameObject.SetActive(true);
            _unlockButtonAnimation.StartAnimation();
        }

        private void OnLoadArea()
        {
            AreaManager.UnlockNewArea(_currentloadHandle);
            Hide();
            InGameLoadingView.Hide();
        }

        private void DownloadArea()
        {
            BufferingScreen.Show();
            InternetManager.Instance.CheckNetConnection((IsConnectionAvailable) =>
            {
                BufferingScreen.Hide();
                if (IsConnectionAvailable)
                    StartCoroutine(DownloadAreaCO());
                else
                    ActiveDownloadButton();
            });
        }

        private void ResumeDownload()
        {
            BufferingScreen.Show();

            InternetManager.Instance.CheckNetConnection((IsConnectionAvailable) =>
            {
                BufferingScreen.Hide();
                if (IsConnectionAvailable)
                    StartCoroutine(DownloadProgress());
                else
                    Hide();
            });
        }

        private void CheckForAreaDownload()
        {
            _assetAddress = AssetAddress.GenerateAreaAddress(_nextAreaId);
            if (AreaAssetStateHandler.GetAssetState(_assetAddress.assetKey) == AssetState.DOWNLOADED)
            {
                _downloadButtonAnimation.gameObject.SetActive(false);
                ActiveUnlockButton();
            }
            else if (AreaAssetStateHandler.GetAssetState(_assetAddress.assetKey) == AssetState.NOT_DOWNLOAD)
            {
                ActiveDownloadButton();
                _unlockButtonAnimation.gameObject.SetActive(false);
            }
            else
                throw new Exception($"Not Valid AreaId {_assetAddress.assetKey}");
        }

        #endregion

        #region CO-ROUTINES
        private IEnumerator DownloadAreaCO()
        {
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
                float value = downloadOperationHandle.progress * 100;
                _downloadFillBar.value = value;
                _fillBarText.text = (value).ToString("0.00") + "%";
                yield return waitForSeconds;
            }
            _downloadFillBarGO.SetActive(false);

            if (downloadOperationHandle.downloadStatus)
            {
                AreaAssetStateHandler.SetAssetState(_assetAddress.assetKey, AssetState.DOWNLOADED);
                ActiveUnlockButton();
            }
            else
                ActiveDownloadButton();

            _currentDownloadHandle = null;
        }

        private IEnumerator<WaitForSeconds> LoadProgress(LoadOperationHandle<GameObject> loadOperationHandle, Action onLoad)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
            while (!loadOperationHandle.IsDone)
            {
                yield return waitForSeconds;
                Debug.Log(loadOperationHandle.progress);
            }
            onLoad();
            Debug.Log("Load Done");
            _currentloadHandle = null;
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnUnlockClick()
        {
            _unlockButtonAnimation.StartReverseAnimation(delegate { _unlockButtonAnimation.gameObject.SetActive(false); });
            InGameLoadingView.Show(delegate
            {
                _currentloadHandle = Loader.LoadAsset<GameObject>(AssetAddress.GenerateAreaAddress(_nextAreaId));
                StartCoroutine(LoadProgress(_currentloadHandle, OnLoadArea));
            });
        }

        public void OnDownloadButtonClick()
        {
            _downloadButtonAnimation.StartReverseAnimation(delegate { _downloadButtonAnimation.gameObject.SetActive(false); });
            _downloadButton.interactable = false;
            DownloadArea();
        }

        public void OnCloseClick()
        {
            Hide();
        }

        #endregion
    }
}
