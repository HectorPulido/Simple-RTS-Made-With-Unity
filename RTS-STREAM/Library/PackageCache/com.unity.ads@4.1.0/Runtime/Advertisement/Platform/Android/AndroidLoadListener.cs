using UnityEngine.Advertisements.Platform;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Advertisements {
    internal class AndroidLoadListener : AndroidJavaProxy {
        private const string CLASS_REFERENCE = "com.unity3d.ads.IUnityAdsLoadListener";
        private IPlatform m_Platform;
        private IUnityAdsLoadListener m_ManagedListener;

        public AndroidLoadListener(IPlatform platform, IUnityAdsLoadListener loadListener) : base(CLASS_REFERENCE) {
            m_Platform = platform;
            m_ManagedListener = loadListener;
        }

        public void onUnityAdsAdLoaded(string placementId) {
            m_ManagedListener?.OnUnityAdsAdLoaded(placementId);
        }

        public void onUnityAdsFailedToLoad(string placementId, AndroidJavaObject error, string message) {
            m_ManagedListener?.OnUnityAdsFailedToLoad(placementId, EnumUtilities.GetEnumFromAndroidJavaObject(error, UnityAdsLoadError.UNKNOWN), message);
        }
    }
}
