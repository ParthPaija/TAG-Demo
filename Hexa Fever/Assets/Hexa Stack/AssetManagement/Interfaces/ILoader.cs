
namespace Tag.AssetManagement { 

    public interface ILoader
    {
        public LoadOperationHandle<T> LoadAsset<T>(AssetAddress assetAddress) where T : UnityEngine.Object;

        public void Unload<T>(LoadOperationHandle<T> loadHandle) where T : UnityEngine.Object;
    }

}
