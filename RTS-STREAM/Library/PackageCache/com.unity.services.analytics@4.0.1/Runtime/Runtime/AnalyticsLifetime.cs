using System;
using UnityEngine;

namespace Unity.Services.Analytics
{
    public class AnalyticsLifetime : MonoBehaviour
    {
        float m_Time = 0.0F;

        void Awake()
        {
            hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;

#if !UNITY_ANALYTICS_DEVELOPMENT
            hideFlags = hideFlags | HideFlags.HideInInspector;
#endif

            DontDestroyOnLoad(gameObject);

            // i hate this, but i don't know of a better way to know if we're running in a test..
            var mbs = FindObjectsOfType<MonoBehaviour>();
            foreach (var mb in mbs)
            {
                if (mb.GetType().FullName == "UnityEngine.TestTools.TestRunner.PlaymodeTestsController")
                {
                    return;
                }
            }

            AnalyticsService.internalInstance.Startup();
        }

        void Update()
        {
            // This is a very simple mechanism to flush the buffer, it might not be the most graceful,
            // but we can add the complexity later when its needed.
            // Once every 'n' flush the Events, then reset the timer.
            // Use unscaled time in case the user sets timeScale to anything other than 1 (e.g. to 0 to pause their game),
            // we always want to send events on the same real-time cadence regardless of framerate or user interference.
            m_Time += Time.unscaledDeltaTime;

            if (m_Time >= 60.0F)
            {
                AnalyticsService.internalInstance.InternalTick();
                m_Time = 0.0F;
            }
        }

        void OnDestroy()
        {
            AnalyticsService.internalInstance.GameEnded();
        }
    }

    public static class ContainerObject
    {
        static bool s_Created;
        static GameObject s_Container;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize()
        {
            if (!s_Created)
            {
#if UNITY_ANALYTICS_DEVELOPMENT
                Debug.Log("Created Analytics Container");
#endif

                s_Container = new GameObject("AnalyticsContainer");
                s_Container.AddComponent<AnalyticsLifetime>();

                s_Created = true;
            }
        }

        public static void DestroyContainer()
        {
            UnityEngine.Object.Destroy(s_Container);
            s_Created = false;
        }
    }
}
