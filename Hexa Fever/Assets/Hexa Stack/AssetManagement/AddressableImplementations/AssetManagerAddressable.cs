using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Tag.HexaStack;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Tag.AssetManagement
{
    public class AssetManagerAddressable : Manager<AssetManagerAddressable>, IAssetManager
    {
        public DownloaderAddressable downloaderAddressable;
        public LoaderAddressable loaderAddressable;
        #region UNITY_CALLBACKS
        public override void Awake()
        {
            base.Awake();
            Init();
        }
        public void Init()
        {
            OnLoadingDone();
        }
        #endregion


        #region DYNAMIC_URL
        public void SetRemoteURL(string url)
        {
            StartCoroutine(SetURL(url));
        }

        IEnumerator SetURL(string url)
        {
            yield return null;
        }
        #endregion

        #region ASSET_TRACK

        public AssetOperationHandle IsValidAddress(AssetAddress assetAddress)
        {
            AssetOperationHandle handle = new AssetOperationHandle()
            {
                assetAddress = assetAddress,
                IsDone = false
            };

            StartCoroutine(ValidAddressCheck(handle));
            return handle;
        }
        #endregion


        #region CONTENT_UPDATE_CHECK

        public AssetOperationHandle IsAssetDownloaded(AssetAddress assetAddress)
        {
            AssetOperationHandle operationHandle = new AssetOperationHandle()
            {
                assetAddress = assetAddress,
                IsDone = false
            };

            StartCoroutine(CheckForPendingDownload(operationHandle));
            return operationHandle;
        }


        private IEnumerator CheckForPendingDownload(AssetOperationHandle handle)
        {
            string assetKey = handle.assetAddress.assetKey;
            AsyncOperationHandle pendingDownloadTask = Addressables.GetDownloadSizeAsync(assetKey);

            yield return pendingDownloadTask;

            if (!pendingDownloadTask.IsValid() || pendingDownloadTask.OperationException != null)
            {
                handle.IsValidAddress = false;
                handle.status = false;
                handle.IsDone = true;
                Debug.LogError(pendingDownloadTask.OperationException.ToString());
                yield break;
            }
            handle.IsDownloaded = (long)pendingDownloadTask.Result <= 0;
            handle.status = true;
            handle.IsDone = true;
        }

        private IEnumerator ValidAddressCheck(AssetOperationHandle handle)
        {
            string bundleName = handle.assetAddress.assetKey;
            AsyncOperationHandle<IList<IResourceLocation>> validAddressVerificationTask = Addressables.LoadResourceLocationsAsync(bundleName);

            while (!validAddressVerificationTask.IsDone)
                yield return null;

            if (validAddressVerificationTask.OperationException != null)
            {
                handle.IsValidAddress = false;
                handle.status = false;
                handle.IsDone = true;
                Debug.LogError(validAddressVerificationTask.OperationException.ToString());
                yield break;
            }

            handle.IsValidAddress = validAddressVerificationTask.Result.Count > 0;
            handle.status = true;
            handle.IsDone = true;

            Addressables.Release(validAddressVerificationTask);
        }

        #endregion

    }


    //TODO seperate by base
    public class AssetOperationHandle
    {
        public AssetAddress assetAddress;
        public bool status;
        public bool IsDone;
        public bool IsDownloaded;
        public bool IsValidAddress;
    }

}
