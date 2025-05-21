using System;
using System.Collections;
using System.Collections.Generic;
using Tag.AssetManagement;
using Tag.HexaStack;
using UnityEngine;

namespace Tag.MetaGame
{
    public class AreaSpriteHandler : Manager<AreaSpriteHandler>
    {
        #region PUBLIC_VARS
        public bool IsSpritesDownloaded { private set; get; }
        #endregion

        #region PRIVATE_VARS

        private int _totalRetryCount = 3;
        private int _currentRetryCount;
        private float _waitMinutesForRetryLoad = 10;

        private Dictionary<string, LoadOperationHandle<AreaExploreSpriteDataSO>> _areaExploreSpriteDatas = new Dictionary<string, LoadOperationHandle<AreaExploreSpriteDataSO>>();
        private int _totalAreaNo;
        [SerializeField] private Sprite defaultSprite;
        private IDownloader Downloader { get { return AssetManagerAddressable.Instance.downloaderAddressable; } }
        private ILoader Loader { get { return AssetManagerAddressable.Instance.loaderAddressable; } }
        private InternetManager InternetManager { get { return InternetManager.Instance; } }
        private Action _onDownloadDone;
        private AreaAssetStateHandler AreaAssetStateHandler { get { return AreaAssetStateHandler.Instance; } }
        #endregion

        #region UNITY_CALLBACKS
        public override void Awake()
        {
            base.Awake();
            Init();
        }
        #endregion

        #region PUBLIC_FUNCTIONS
        public void Init()
        {
            IsSpritesDownloaded = false;
            _totalAreaNo = AreaAssetStateHandler.TotalAreaNo;
            LoadSprite();
        }
        public void RegisterDownloadDoneAction(Action action)
        {
            _onDownloadDone = action;
        }
        public Sprite GetAreaEmptySprite(string areaId)
        {
            if (IsSpritesDownloaded)
            {
                return _areaExploreSpriteDatas[areaId].data.areaEmptySprite;
            }
            return defaultSprite;
        }
        public Sprite GetAreaCompleteSprite(string areaId)
        {
            if (IsSpritesDownloaded)
            {
                return _areaExploreSpriteDatas[areaId].data.areaCompleteSprite;
            }
            return defaultSprite;
        }
        public string GetAreaName(string areaId)
        {
            /*     if (IsSpritesDownloaded)
                 {
                     return _areaExploreSpriteDatas[areaId].data.areaName;
                 }*/
            return "Area : " + AreaUtility.AreaIdToAreaNo(areaId);
        }

        #endregion

        #region PRIVATE_FUNCTIONS
        private void LoadSprite()
        {
            InternetManager.Instance.CheckNetConnection((IsConnectionAvailable) =>
            {
                if (IsConnectionAvailable)
                {
                    StartCoroutine(LoadSpriteCO());
                }
                else
                {
                    DownloadFail();
                }
            });
        }
        private void OnLoadData()
        {
            _onDownloadDone?.Invoke();
            IsSpritesDownloaded = true;
        }
        private void DownloadFail()
        {
            StopAllCoroutines();
            if (_currentRetryCount < _totalRetryCount)
            {
                _currentRetryCount++;
                LoadSprite();
            }
            else
            {
                StartCoroutine(WaitAndLoadSprites(_waitMinutesForRetryLoad * 60));
            }
        }
        #endregion

        #region CO-ROUTINES
        private IEnumerator LoadSpriteCO()
        {
            yield return DownloadAllAreaSpriteData(_totalAreaNo);
            yield return LoadAllAreaSpriteData(_totalAreaNo);
            OnLoadData();
        }
        private IEnumerator DownloadAllAreaSpriteData(int totalAreaNo)
        {
            AssetAddress assetAddress;
            for (int i = 0; i < totalAreaNo; i++)
            {
                assetAddress = AssetAddress.GenerateAreaExploreSpriteDataAddress(AreaUtility.AreaNoToAreaId(i + 1));
                yield return DownloadAreaSpriteData(assetAddress);
            }
        }
        private IEnumerator DownloadAreaSpriteData(AssetAddress assetAddress)
        {
            if (AreaAssetStateHandler.GetAssetState(assetAddress.assetKey) == AssetState.DOWNLOADED)
                yield break;
            else if (AreaAssetStateHandler.GetAssetState(assetAddress.assetKey) == AssetState.NOT_DOWNLOAD)
            {
                DownloadOperationHandle downloadOperationHandle = Downloader.DownloadAsset(assetAddress);
                while (!downloadOperationHandle.IsDone)
                    yield return null;

                if (downloadOperationHandle.downloadStatus)
                    AreaAssetStateHandler.SetAssetState(assetAddress.assetKey, AssetState.DOWNLOADED);
                else
                    DownloadFail();
            }
            else
                throw new Exception($"Not Valid AreaSpriteDataId {assetAddress.assetKey}");

        }

        private IEnumerator LoadAllAreaSpriteData(int totalAreaNo)
        {
            string areaId;
            AssetAddress assetAddress;
            LoadOperationHandle<AreaExploreSpriteDataSO> loadOperationHandle;
            for (int i = 0; i < totalAreaNo; i++)
            {
                areaId = AreaUtility.AreaNoToAreaId(i + 1);
                assetAddress = AssetAddress.GenerateAreaExploreSpriteDataAddress(areaId);
                loadOperationHandle = Loader.LoadAsset<AreaExploreSpriteDataSO>(assetAddress);

                while (!loadOperationHandle.IsDone)
                    yield return null;

                _areaExploreSpriteDatas.Add(areaId, loadOperationHandle);
            }
        }
        private IEnumerator WaitAndLoadSprites(float time)
        {
            yield return new WaitForSeconds(time);
            LoadSprite();
        }
        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
