using System;
using UnityEngine.Advertisements.Platform;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Advertisements {
    internal class AndroidShowListener : AndroidJavaProxy {
        private const string OBJECT_ID = "com.unity3d.ads.IUnityAdsShowListener";
        private IPlatform m_Platform;
        private IUnityAdsShowListener m_ManagedListener;

        public AndroidShowListener(IPlatform platform, IUnityAdsShowListener showListener) : base(OBJECT_ID) {
            m_Platform = platform;
            m_ManagedListener = showListener;
        }

        public void onUnityAdsShowFailure(string placementId, AndroidJavaObject error, string message)
        {
            var enumError = EnumUtilities.GetEnumFromAndroidJavaObject(error, UnityAdsShowError.UNKNOWN);
            m_Platform.OnUnityAdsShowFailure(placementId, enumError, message);
            m_ManagedListener?.OnUnityAdsShowFailure(placementId, enumError, message);
        }

        public void onUnityAdsShowStart(string placementId) {
            m_Platform.OnUnityAdsShowStart(placementId);
            m_ManagedListener?.OnUnityAdsShowStart(placementId);
        }

        public void onUnityAdsShowClick(string placementId) {
            m_Platform.OnUnityAdsShowClick(placementId);
            m_ManagedListener?.OnUnityAdsShowClick(placementId);
        }

        public void onUnityAdsShowComplete(string placementId, AndroidJavaObject state) {
            var showCompletionState = EnumUtilities.GetEnumFromAndroidJavaObject(state, UnityAdsShowCompletionState.UNKNOWN);
            m_Platform.OnUnityAdsShowComplete(placementId, showCompletionState);
            m_ManagedListener?.OnUnityAdsShowComplete(placementId, showCompletionState);
        }
    }
}
