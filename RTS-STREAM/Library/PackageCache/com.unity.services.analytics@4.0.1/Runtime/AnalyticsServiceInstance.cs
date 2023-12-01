using System;
using System.Threading.Tasks;
using Unity.Services.Analytics.Data;
using Unity.Services.Analytics.Internal;
using Unity.Services.Analytics.Platform;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Device.Internal;
using UnityEngine;
using Event = Unity.Services.Analytics.Internal.Event;

namespace Unity.Services.Analytics
{
    partial class AnalyticsServiceInstance : IAnalyticsService
    {
        public string PrivacyUrl => "https://unity3d.com/legal/privacy-policy";

        const string k_CollectUrlPattern = "https://collect.analytics.unity3d.com/api/analytics/collect/v1/projects/{0}/environments/{1}";
        const string k_ForgetCallingId = "com.unity.services.analytics.Events." + nameof(OptOut);

        internal IPlayerId PlayerId { get; private set; }
        internal IInstallationId InstallId { get; private set; }

        internal string CustomAnalyticsId { get; private set; }

        internal IBuffer dataBuffer = new Internal.Buffer();

        internal IDataGenerator dataGenerator = new DataGenerator();

        internal IDispatcher dataDispatcher { get; set; }

        string m_CollectURL;
        readonly string m_SessionID;
        readonly StdCommonParams m_CommonParams = new StdCommonParams();
        readonly string m_StartUpCallingId = "com.unity.services.analytics.Events.Startup";

        internal IIDeviceIdentifiersInternal deviceIdentifiersInternal = new DeviceIdentifiersInternal();

        internal bool ServiceEnabled { get; private set; } = true;

        internal AnalyticsServiceInstance()
        {
            // The docs say nothing about Application.cloudProjectId being guaranteed or not,
            // we add a check just to be sure.
            if (string.IsNullOrEmpty(Application.cloudProjectId))
            {
                Debug.LogError("No Cloud ProjectID Found for Analytics");
                return;
            }

            dataDispatcher = new Dispatcher(dataBuffer, new WebRequestHelper(), ConsentTracker);

            m_SessionID = Guid.NewGuid().ToString();

            m_CommonParams.ClientVersion = Application.version;
            m_CommonParams.ProjectID = Application.cloudProjectId;
            m_CommonParams.GameBundleID = Application.identifier;
            m_CommonParams.Platform = Runtime.Name();
            m_CommonParams.BuildGuuid = Application.buildGUID;
            m_CommonParams.Idfv = deviceIdentifiersInternal.Idfv;
        }

        public void Flush()
        {
            if (!ServiceEnabled)
            {
                return;
            }

            if (string.IsNullOrEmpty(Application.cloudProjectId))
            {
                return;
            }

            if (InstallId == null)
            {
#if UNITY_ANALYTICS_DEVELOPMENT
                Debug.Log("The Core callback hasn't yet triggered.");
#endif

                return;
            }

            if (ConsentTracker.IsGeoIpChecked() && ConsentTracker.IsConsentGiven())
            {
                dataBuffer.InstallID = InstallId.GetOrCreateIdentifier();
                dataBuffer.PlayerID = PlayerId?.PlayerId;

                dataBuffer.UserID = !string.IsNullOrEmpty(CustomAnalyticsId) ? CustomAnalyticsId : dataBuffer.InstallID;

                dataBuffer.SessionID = m_SessionID;
                dataDispatcher.CollectUrl = m_CollectURL;
                dataDispatcher.Flush();
            }

            if (ConsentTracker.IsOptingOutInProgress())
            {
                analyticsForgetter.AttemptToForget();
            }
        }

        public void RecordInternalEvent(Event eventToRecord)
        {
            if (!ServiceEnabled)
            {
                return;
            }

            dataBuffer.PushEvent(eventToRecord);
        }

        internal void SetDependencies(IInstallationId installId, IPlayerId playerId, string environment, string customAnalyticsId)
        {
            InstallId = installId;
            PlayerId = playerId;
            CustomAnalyticsId = customAnalyticsId;

            m_CollectURL = string.Format(k_CollectUrlPattern, Application.cloudProjectId, environment.ToLowerInvariant());
        }

        internal async Task Initialize(IInstallationId installId, IPlayerId playerId, string environment, string customAnalyticsId)
        {
            SetDependencies(installId, playerId, environment, customAnalyticsId);

            if (!ServiceEnabled)
            {
                return;
            }

            await InitializeUser();
        }

        async Task InitializeUser()
        {
            SetVariableCommonParams();

#if UNITY_ANALYTICS_DEVELOPMENT
            Debug.LogFormat("UA2 Setup\nCollectURL:{0}\nSessionID:{1}", m_CollectURL, m_SessionID);
#endif

            try
            {
                await ConsentTracker.CheckGeoIP();

                if (ConsentTracker.IsGeoIpChecked() && (ConsentTracker.IsConsentDenied() || ConsentTracker.IsOptingOutInProgress()))
                {
                    OptOut();
                }
            }
#if UNITY_ANALYTICS_EVENT_LOGS
            catch (ConsentCheckException e)
            {
                Debug.Log("Initial GeoIP lookup fail: " + e.Message);
            }
#else
            catch (ConsentCheckException) {}
#endif
        }

        /// <summary>
        /// Sets up the internals of the Analytics SDK, including the regular sending of events and assigning
        /// the userID to be used in further event recording.
        /// </summary>
        internal void Startup()
        {
            if (!ServiceEnabled)
            {
                return;
            }

            // Startup Events.
            dataGenerator.SdkStartup(ref dataBuffer, DateTime.Now, m_CommonParams, m_StartUpCallingId);
            dataGenerator.ClientDevice(ref dataBuffer, DateTime.Now, m_CommonParams, m_StartUpCallingId, SystemInfo.processorType, SystemInfo.graphicsDeviceName, SystemInfo.processorCount, SystemInfo.systemMemorySize, Screen.width, Screen.height, (int)Screen.dpi);

#if UNITY_DOTSRUNTIME
            var isTiny = true;
#else
            var isTiny = false;
#endif

            dataGenerator.GameStarted(ref dataBuffer, DateTime.Now, m_CommonParams, m_StartUpCallingId, Application.buildGUID, SystemInfo.operatingSystem, isTiny, DebugDevice.IsDebugDevice(), Locale.AnalyticsRegionLanguageCode());
        }

        internal void NewPlayerEvent()
        {
            if (!ServiceEnabled)
            {
                return;
            }

            if (InstallId != null && new InternalNewPlayerHelper(InstallId).IsNewPlayer())
            {
                dataGenerator.NewPlayer(ref dataBuffer, DateTime.Now, m_CommonParams, m_StartUpCallingId, SystemInfo.deviceModel);
            }
        }

        /// <summary>
        /// Records the gameEnded event, and flushes the buffer to upload all events.
        /// </summary>
        internal void GameEnded()
        {
            if (!ServiceEnabled)
            {
                return;
            }

            dataGenerator.GameEnded(ref dataBuffer, DateTime.Now, m_CommonParams, "com.unity.services.analytics.Events.Shutdown", DataGenerator.SessionEndState.QUIT);
            if (ConsentTracker.IsGeoIpChecked())
            {
                Flush();
            }
        }

        // <summary>
        // Internal tick is called by the Heartbeat at set intervals.
        // </summary>
        internal void InternalTick()
        {
            if (!ServiceEnabled)
            {
                return;
            }

            SetVariableCommonParams();
            dataGenerator.GameRunning(ref dataBuffer, DateTime.Now, m_CommonParams, "com.unity.services.analytics.Events.InternalTick");
            if (ConsentTracker.IsGeoIpChecked())
            {
                Flush();
            }
        }

        void SetVariableCommonParams()
        {
            m_CommonParams.Idfv = deviceIdentifiersInternal.Idfv;
            m_CommonParams.DeviceVolume = DeviceVolumeProvider.GetDeviceVolume();
            m_CommonParams.BatteryLoad = SystemInfo.batteryLevel;
            m_CommonParams.UasUserID = PlayerId?.PlayerId;
        }

        void GameEnded(DataGenerator.SessionEndState quitState)
        {
            if (!ServiceEnabled)
            {
                return;
            }

            dataGenerator.GameEnded(ref dataBuffer, DateTime.Now, m_CommonParams, "com.unity.services.analytics.Events.GameEnded", quitState);
        }

        public async Task SetAnalyticsEnabled(bool enabled)
        {
            if (enabled && !ServiceEnabled)
            {
                dataBuffer = new Internal.Buffer();
                dataDispatcher = new Dispatcher(dataBuffer, new WebRequestHelper(), ConsentTracker);
                await InitializeUser();

                ServiceEnabled = true;
            }
            else if (!enabled && ServiceEnabled)
            {
                dataBuffer.ClearBuffer();
                dataBuffer.ClearDiskCache();
                dataBuffer = new BufferRevoked();

                ServiceEnabled = false;
            }
        }
    }
}
