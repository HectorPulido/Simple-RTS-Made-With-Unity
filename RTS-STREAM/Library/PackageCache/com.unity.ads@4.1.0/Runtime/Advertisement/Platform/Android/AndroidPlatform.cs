#if UNITY_ANDROID
using System;

namespace UnityEngine.Advertisements.Platform.Android
{
    internal class AndroidPlatform : INativePlatform
    {
        private const string ADS_BASE_CLASS = "com.unity3d.ads.UnityAds";
        private const string ADS_METADATA_CLASS = "com.unity3d.ads.metadata.MetaData";
        private const string UNITY_PLAYER_CLASS = "com.unity3d.player.UnityPlayer";

        private IPlatform m_Platform;
        private AndroidJavaObject m_CurrentActivity;
        private AndroidJavaClass m_UnityAds;
        private AndroidJavaClass m_Placement;

        public void SetupPlatform(IPlatform platform)
        {
            m_Platform = platform;
            m_CurrentActivity = GetCurrentAndroidActivity();
            m_UnityAds = new AndroidJavaClass(ADS_BASE_CLASS);
        }

        public void Initialize(string gameId, bool testMode, IUnityAdsInitializationListener initializationListener)
        {
            m_UnityAds?.CallStatic("initialize", m_CurrentActivity, gameId, testMode, new AndroidInitializationListener(m_Platform, initializationListener));
        }

        public void Load(string placementId, IUnityAdsLoadListener loadListener)
        {
            m_UnityAds?.CallStatic("load", placementId, new AndroidLoadListener(m_Platform, loadListener));
        }

        public void Show(string placementId, IUnityAdsShowListener showListener)
        {
            m_UnityAds?.CallStatic("show", m_CurrentActivity, placementId, new AndroidShowListener(m_Platform, showListener));
        }

        public void SetMetaData(MetaData metaData)
        {
            var metaDataObject = new AndroidJavaObject(ADS_METADATA_CLASS, m_CurrentActivity);
            metaDataObject.Call("setCategory", metaData.category);
            foreach (var entry in metaData.Values())
            {
                metaDataObject.Call<bool>("set", entry.Key, entry.Value);
            }
            metaDataObject.Call("commit");
        }

        public bool GetDebugMode()
        {
            return m_UnityAds?.CallStatic<bool>("getDebugMode") ?? false;
        }

        public void SetDebugMode(bool debugMode)
        {
            m_UnityAds?.CallStatic("setDebugMode", debugMode);
        }

        public string GetVersion()
        {
            return m_UnityAds?.CallStatic<string>("getVersion") ?? "UnknownVersion";
        }

        public bool IsInitialized()
        {
            return m_UnityAds?.CallStatic<bool>("isInitialized") ?? false;
        }

        public static AndroidJavaObject GetCurrentAndroidActivity()
        {
            var unityPlayerClass = new AndroidJavaClass(UNITY_PLAYER_CLASS);
            return unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }
}
#endif
