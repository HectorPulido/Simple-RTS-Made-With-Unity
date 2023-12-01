#if UNITY_IOS

using System;
using System.Runtime.InteropServices;
using AOT;

namespace UnityEngine.Advertisements.Platform.iOS
{
    internal class IosPlatform : IosNativeObject, INativePlatform, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private static IPlatform s_Platform;

        [DllImport("__Internal")]
        private static extern void UnityAdsInitialize(string gameId, bool testMode, IntPtr initializationListener);

        [DllImport("__Internal")]
        private static extern void UnityAdsLoad(string placementId, IntPtr loadListener);

        [DllImport("__Internal")]
        private static extern void UnityAdsShow(string placementId, IntPtr showListener);

        [DllImport("__Internal")]
        private static extern bool UnityAdsGetDebugMode();

        [DllImport("__Internal")]
        private static extern void UnityAdsSetDebugMode(bool debugMode);

        [DllImport("__Internal")]
        private static extern string UnityAdsGetVersion();

        [DllImport("__Internal")]
        private static extern bool UnityAdsIsInitialized();

        [DllImport("__Internal")]
        private static extern void UnityAdsSetMetaData(string category, string data);

        public void SetupPlatform(IPlatform iosPlatform)
        {
            s_Platform = iosPlatform;
        }

        public void Initialize(string gameId, bool testMode, IUnityAdsInitializationListener initializationListener)
        {
            UnityAdsInitialize(gameId, testMode, new IosInitializationListener(this, initializationListener).NativePtr);
        }

        public void Load(string placementId, IUnityAdsLoadListener loadListener)
        {
            UnityAdsLoad(placementId, new IosLoadListener(this, loadListener).NativePtr);
        }

        public void Show(string placementId, IUnityAdsShowListener showListener)
        {
            UnityAdsShow(placementId, new IosShowListener(this, showListener).NativePtr);
        }

        public void SetMetaData(MetaData metaData)
        {
            UnityAdsSetMetaData(metaData.category, metaData.ToJSON());
        }

        public bool GetDebugMode()
        {
            return UnityAdsGetDebugMode();
        }

        public void SetDebugMode(bool debugMode)
        {
            UnityAdsSetDebugMode(debugMode);
        }

        public string GetVersion()
        {
            return UnityAdsGetVersion();
        }

        public bool IsInitialized()
        {
            return UnityAdsIsInitialized();
        }

        public void OnInitializationComplete()
        { }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        { }

        public void OnUnityAdsAdLoaded(string placementId)
        { }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        { }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            s_Platform.OnUnityAdsShowFailure(placementId, error, message);
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            s_Platform.OnUnityAdsShowStart(placementId);
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            s_Platform.OnUnityAdsShowClick(placementId);
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState completionState)
        {
            s_Platform.OnUnityAdsShowComplete(placementId, completionState);
        }
    }
}
#endif
