using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Tag.AssetManagement
{
    public class LoadOperationHandle<T> where T: UnityEngine.Object
    {
        public AssetAddress assetAddress;
        public T data => loadHandle.Result;
        public float progress;
        public bool IsDone;
        public AsyncOperationHandle<T> loadHandle;
    }

    public class LoaderAddressable : MonoBehaviour ,ILoader
    {

        public LoadOperationHandle<T> LoadAsset<T>(AssetAddress assetAddress) where T : UnityEngine.Object
        {
            LoadOperationHandle<T> loadChunk = new LoadOperationHandle<T>()
            {
                assetAddress = assetAddress,
            };
            StartCoroutine(LoadAsset(loadChunk));
            return loadChunk;
        }


        #region CLEANUP
        public void Unload<T>(LoadOperationHandle<T> loadHandle) where T : UnityEngine.Object
        {
            UnloadAsset(loadHandle);
        }
        private void UnloadAsset<T>(LoadOperationHandle<T> assetAddress) where T : UnityEngine.Object
        {
            StartCoroutine(UnloadRoutine(assetAddress));
        }
        #endregion


        #region LOAD_ROUTINE
        private IEnumerator UnloadRoutine<T>(LoadOperationHandle<T> operationHandle) where T : UnityEngine.Object
        {
            Addressables.Release(operationHandle.loadHandle);
            Resources.UnloadUnusedAssets();
            yield return null;
        }

        private IEnumerator LoadAsset<T>(LoadOperationHandle<T> loadChunk) where T: UnityEngine.Object
        {
            string assetKey = loadChunk.assetAddress.assetKey;

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetKey);

            while (!handle.IsDone) 
            {
                yield return null;
                loadChunk.progress = handle.PercentComplete;
            }
           
            if (handle.OperationException!=null) 
            {
                loadChunk.IsDone = true;
                Debug.LogError(handle.OperationException.ToString());
                yield break; 
            }

            loadChunk.IsDone = true;
            loadChunk.loadHandle = handle;
        }
        #endregion

    }
}
