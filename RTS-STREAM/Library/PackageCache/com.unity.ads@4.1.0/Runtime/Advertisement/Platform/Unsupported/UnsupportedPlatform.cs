using System;

namespace UnityEngine.Advertisements.Platform.Unsupported
{
    internal sealed class UnsupportedPlatform : INativePlatform
    {
        public void SetupPlatform(IPlatform platform) {}

        public void Initialize(string gameId, bool testMode, IUnityAdsInitializationListener initializationListener) {}

        public void Load(string placementId, IUnityAdsLoadListener loadListener) {}

        public void Show(string placementId, IUnityAdsShowListener showListener) {}

        public void SetMetaData(MetaData metaData) {}

        public bool GetDebugMode()
        {
            return false;
        }

        public void SetDebugMode(bool debugMode) {}

        public string GetVersion()
        {
            return "UnsupportedPlatformVersion";
        }

        public bool IsInitialized()
        {
            return false;
        }
    }
}
