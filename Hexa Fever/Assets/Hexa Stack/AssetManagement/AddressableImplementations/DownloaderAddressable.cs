using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Tag.AssetManagement
{

    public class DownloadOperationHandle
    {
        internal AssetAddress assetAddress;
        public bool downloadStatus;
        public bool IsDone;
        public float progress;
        public double totalBytes;

        public static DownloadOperationHandle InvalidResponse()
        {
            DownloadOperationHandle downloadHandle = new DownloadOperationHandle()
            {
                downloadStatus = false,
                IsDone = true,
                progress = 0
            };
            return downloadHandle;
        }
    }

    public class DownloaderAddressable : MonoBehaviour, IDownloader
    {
        public DownloadOperationHandle DownloadAsset(AssetAddress assetAddress)
        {
            DownloadOperationHandle downloadHandle = new DownloadOperationHandle()
            {
                assetAddress = assetAddress,
                downloadStatus = false,
                IsDone = false,
                progress = 0
            };
            StartCoroutine(DownloadAsset(downloadHandle));
            return downloadHandle;
        }


        #region DOWNLOAD_ROUTINE
        private IEnumerator DownloadAsset(DownloadOperationHandle downloadHandle)
        {
            string assetKey = downloadHandle.assetAddress.assetKey;
            AsyncOperationHandle sizeFetchTask = Addressables.GetDownloadSizeAsync(assetKey);

            while (!sizeFetchTask.IsDone)
                yield return null;

            if (sizeFetchTask.OperationException != null)
            {
                Debug.LogError(sizeFetchTask.OperationException.ToString());
                yield break;
            }

            downloadHandle.totalBytes = (long)sizeFetchTask.Result;
            Addressables.Release(sizeFetchTask);

            AsyncOperationHandle loadTask = Addressables.DownloadDependenciesAsync(assetKey);
            while (!loadTask.IsDone)
            {
                yield return null;
                downloadHandle.progress = loadTask.GetDownloadStatus().Percent;
            }

            if (loadTask.OperationException != null)
            {
                downloadHandle.IsDone = true;
                downloadHandle.progress = 0;
                downloadHandle.downloadStatus = false;
                Debug.LogError(loadTask.OperationException.ToString());
                yield break;
            }

            downloadHandle.IsDone = true;
            downloadHandle.downloadStatus = true;
        }
        #endregion

    }
}
