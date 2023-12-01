using System;
using UnityEngine.Advertisements.Platform;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Advertisements {
    internal class AndroidInitializationListener : AndroidJavaProxy {

        private const string CLASS_REFERENCE = "com.unity3d.ads.IUnityAdsInitializationListener";
        private IPlatform m_Platform;
        private IUnityAdsInitializationListener m_ManagedListener;

        public AndroidInitializationListener(IPlatform platform, IUnityAdsInitializationListener initializationListener) : base(CLASS_REFERENCE) {
            m_Platform = platform;
            m_ManagedListener = initializationListener;
        }

        public void onInitializationComplete() {
            m_ManagedListener?.OnInitializationComplete();
        }

        public void onInitializationFailed(AndroidJavaObject error, string message) {
            m_ManagedListener?.OnInitializationFailed(EnumUtilities.GetEnumFromAndroidJavaObject(error, UnityAdsInitializationError.UNKNOWN), message);
        }
    }
}
