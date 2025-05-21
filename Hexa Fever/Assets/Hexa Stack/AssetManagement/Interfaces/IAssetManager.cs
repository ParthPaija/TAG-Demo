using System;
using System.Collections;
using System.Collections.Generic;

namespace Tag.AssetManagement
{
    public interface IAssetManager
    {
        public void Init();

        public void SetRemoteURL(string remoteURL);

        public AssetOperationHandle IsAssetDownloaded(AssetAddress assetAddress);

        public AssetOperationHandle IsValidAddress(AssetAddress assetAddress);
    }
}
