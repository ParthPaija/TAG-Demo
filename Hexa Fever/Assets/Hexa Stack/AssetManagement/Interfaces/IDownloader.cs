
namespace Tag.AssetManagement
{
    public interface IDownloader
    {
        public DownloadOperationHandle DownloadAsset(AssetAddress assetAddress);

    }

}