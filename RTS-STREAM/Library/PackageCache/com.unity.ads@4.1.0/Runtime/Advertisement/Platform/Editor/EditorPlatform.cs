#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine.Advertisements.Events;

namespace UnityEngine.Advertisements.Platform.Editor
{
    internal class EditorPlatform : INativePlatform
    {
        private const string k_BaseUrl = "http://editor-support.unityads.unity3d.com/games";
        private const string k_Version = "4.1.0";

        private IPlatform m_Platform;
        private Configuration m_Configuration;
        private Placeholder m_Placeholder;

        private bool                     m_StartedInitialization;
        private bool                     m_Initialized;
        private bool                     m_DebugMode;
        private string                   m_GameId;
        private Dictionary<string, bool> m_PlacementMap = new Dictionary<string, bool>();
        private Queue<string>            m_QueuedLoads  = new Queue<string>();

        private EventHandler<FinishEventArgs> m_ShowCompletionHandler;

        public void SetupPlatform(IPlatform platform)
        {
            m_Platform = platform;
        }

        public void Initialize(string gameId, bool testMode, IUnityAdsInitializationListener initializationListener)
        {
            if (m_StartedInitialization) return;
            m_StartedInitialization = true;
            m_GameId = gameId;
            if (m_Platform.DebugMode)
            {
                Debug.Log("UnityAdsEditor: Initialize(" + gameId + ", " + testMode + ");");
            }

            var placeHolderGameObject = new GameObject("UnityAdsEditorPlaceHolderObject")
            {
                hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector
            };

            GameObject.DontDestroyOnLoad(placeHolderGameObject);
            m_Placeholder = placeHolderGameObject.AddComponent<Placeholder>();


            var configurationUrl = string.Join("/", new string[]
            {
                k_BaseUrl,
                gameId,
                string.Join("&", new string[]
                {
                    "configuration?platform=editor",
                    "unityVersion=" + Uri.EscapeDataString(Application.unityVersion),
                    "sdkVersionName=" + Uri.EscapeDataString(m_Platform.Version)
                })
            });
            var request = WebRequest.Create(configurationUrl);
            request.BeginGetResponse(result =>
            {
                try
                {
                    var response = request.EndGetResponse(result);
                    var reader = new StreamReader(response.GetResponseStream() ?? throw new Exception("Null response stream fetching configuration"));
                    var responseBody = reader.ReadToEnd();
                    try
                    {
                        m_Configuration = new Configuration(responseBody);
                        if (!m_Configuration.enabled)
                        {
                            Debug.LogWarning("gameId " + gameId + " is not enabled");
                        }

                        //Add placements to load list
                        foreach (var placement in m_Configuration.placements)
                        {
                            m_PlacementMap.Add(placement.Key, false);
                        }

                        m_Initialized = true;
                        m_Platform.UnityLifecycleManager.Post(() =>
                        {
                            initializationListener?.OnInitializationComplete();
                        });


                    }
                    catch (Exception exception)
                    {
                        Debug.LogError($"Failed to parse configuration for gameId: {gameId}");
                        Debug.Log(responseBody);
                        Debug.LogError(exception.Message);
                        m_Platform.UnityLifecycleManager.Post(() =>
                        {
                            initializationListener?.OnInitializationFailed(UnityAdsInitializationError.INTERNAL_ERROR, $"Failed to parse configuration for gameId: {gameId}");
                        });
                    }
                    reader.Close();
                    response.Close();
                }
                catch  (Exception exception)
                {
                    Debug.LogError($"Invalid configuration request for gameId: {gameId}");
                    Debug.LogError(exception.Message);
                    m_Platform.UnityLifecycleManager.Post(() =>
                    {
                        initializationListener?.OnInitializationFailed(UnityAdsInitializationError.INTERNAL_ERROR, $"Invalid configuration request for gameId: {gameId}");
                    });
                }
            }, new object());
        }

        public void Load(string placementId, IUnityAdsLoadListener loadListener)
        {
            if (string.IsNullOrEmpty(placementId))
            {
                Debug.LogWarning("placementID cannot be null or empty, please set a placement");
                return;
            }

            if (!m_Initialized)
            {
                m_QueuedLoads?.Enqueue(placementId);
                return;
            }

            m_Platform.UnityLifecycleManager.Post(() => {
                if (m_PlacementMap.ContainsKey(placementId))
                {
                    m_PlacementMap[placementId] = true;
                    loadListener?.OnUnityAdsAdLoaded(placementId);
                }
                else
                {
                    string errorMessage = "Placement " + placementId + " does not exist for gameId: " + m_GameId;
                    loadListener?.OnUnityAdsFailedToLoad(placementId, UnityAdsLoadError.INVALID_ARGUMENT, errorMessage);
                }
            });
        }

        public void Show(string placementId, IUnityAdsShowListener showListener)
        {
            if (!m_Initialized)
            {
                Debug.LogWarning("Unity Ads must be initialized before calling show");
                return;
            }

            if (string.IsNullOrEmpty(placementId))
            {
                Debug.LogWarning("placementID cannot be null or empty, please set a placement");
                return;
            }

            m_Platform.UnityLifecycleManager.Post(() => {
                if (m_Initialized && m_Configuration.placements.ContainsKey((placementId)))
                {
                    showListener?.OnUnityAdsShowStart(placementId);
                    bool allowSkip = m_Configuration.placements[placementId];
                    m_Placeholder.Show(placementId, allowSkip);
                    m_PlacementMap[placementId] = false;
                    m_ShowCompletionHandler = (sender, e) =>
                    {
                        var completionState = GetShowCompletionStateFromShowResult(e.showResult);
                        showListener?.OnUnityAdsShowComplete(placementId, completionState);
                        m_Placeholder.OnFinish -= m_ShowCompletionHandler;
                    };
                    m_Placeholder.OnFinish += m_ShowCompletionHandler;
                }
                else
                {
                    showListener?.OnUnityAdsShowFailure(placementId, UnityAdsShowError.NOT_READY, $"Placement {placementId} is not ready");
                }
            });
        }

        public void SetMetaData(MetaData metaData)
        {
        }

        public bool GetDebugMode()
        {
            return m_DebugMode;
        }

        public void SetDebugMode(bool debugMode)
        {
            m_DebugMode = debugMode;
        }

        public string GetVersion()
        {
            return k_Version;
        }

        public bool IsInitialized()
        {
            return m_Initialized;
        }

        private UnityAdsShowCompletionState GetShowCompletionStateFromShowResult(ShowResult showResult)
        {
            switch (showResult)
            {
                case ShowResult.Finished:
                    return UnityAdsShowCompletionState.COMPLETED;
                case ShowResult.Skipped:
                    return UnityAdsShowCompletionState.SKIPPED;
                case ShowResult.Failed:
                default:
                    return UnityAdsShowCompletionState.UNKNOWN;
            }
        }
    }
}
#endif