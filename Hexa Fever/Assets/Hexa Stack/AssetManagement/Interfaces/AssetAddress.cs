using System;

namespace Tag.AssetManagement
{
    public struct AssetAddress : IEquatable<AssetAddress>
    {
        public string assetKey;

        public AssetAddress(string AssetKey)
        {
            assetKey = AssetKey;
        }

        public static AssetAddress GenerateAreaAddress(string areaId)
        {
            return new AssetAddress()
            {
                assetKey = areaId,
            };
        }
        public static AssetAddress GenerateAreaExploreSpriteDataAddress(string areaId)
        {
            return new AssetAddress()
            {
                assetKey = $"{areaId}ExploreSpriteData",
            };
        }

        public static AssetAddress GenerateTokenEventAddress(string eventId)
        {
            return new AssetAddress()
            {
                assetKey = $"{eventId}_EventSO",
            };
        }
        public static AssetAddress GenerateCoreGamePlayAddress()
        {
            return new AssetAddress()
            {
                assetKey = "CoreGamePlay"
            };
        }
        public static AssetAddress GenerateMainSceneAddress()
        {
            return new AssetAddress()
            {
                assetKey = $"MainScene"
            };
        }
        public override int GetHashCode()
        {
            return assetKey.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is AssetAddress && Equals(obj);
        }

        public bool Equals(AssetAddress other)
        {
            return (other).assetKey == assetKey;
        }

        public static AssetAddress Default
        {
            get
            {
                return new AssetAddress()
                {
                    assetKey = String.Empty,
                };
            }
        }
    }
}