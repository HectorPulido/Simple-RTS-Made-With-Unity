using System;
using System.Collections.Generic;
using UnityEngine.Advertisements.Events;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Advertisements.Platform
{
    internal interface IPlatform
    {
        IBanner Banner { get; }
        IUnityLifecycleManager UnityLifecycleManager { get; }
        INativePlatform NativePlatform { get; }

        bool IsInitialized { get; }
        bool IsShowing { get; }
        string Version { get; }
        bool DebugMode { get; set; }

        void Initialize(string gameId, bool testMode, IUnityAdsInitializationListener initializationListener);
        void Load(string placementId, IUnityAdsLoadListener loadListener);
        void Show(string placementId, ShowOptions showOptions, IUnityAdsShowListener showListener);
        void SetMetaData(MetaData metaData);
        void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message);
        void OnUnityAdsShowStart(string placementId);
        void OnUnityAdsShowClick(string placementId);
        void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState completionState);
    }
}
