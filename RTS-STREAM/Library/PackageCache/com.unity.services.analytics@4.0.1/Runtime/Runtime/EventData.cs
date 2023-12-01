using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Services.Analytics.Platform;
using UnityEngine;

namespace Unity.Services.Analytics.Internal
{
    public class EventData
    {
        internal EventData()
        {
            Data = new Dictionary<string, object>();
        }

        public void Set(string key, float value)
        {
            Data[key] = value;
        }

        public void Set(string key, double value)
        {
            Data[key] = value;
        }

        public void Set(string key, bool value)
        {
            Data[key] = value;
        }

        public void Set(string key, int value)
        {
            Data[key] = value;
        }

        public void Set(string key, object value)
        {
            Data[key] = value;
        }

        public void Set(string key, System.Int64 value)
        {
            Data[key] = value;
        }

        public void Set(string key, string value)
        {
            Data[key] = value;
        }

        public void AddPlatform()
        {
            Set("platform", Runtime.Name());
        }

        public void AddBatteryLoad()
        {
            Set("batteryLoad", 1.0);
        }

        public void AddConnectionType()
        {
            Set("connectionType", NetworkReachability.ReachableViaLocalAreaNetwork.ToString());
        }

        public void AddUserCountry()
        {
            Set("userCountry", Platform.UserCountry.Name());
        }

        public void AddBuildGuuid()
        {
            Set("buildGUUID", Application.buildGUID);
        }

        public void AddClientVersion()
        {
            Set("clientVersion", Application.version);
        }

        public void AddGameBundleID()
        {
            Set("gameBundleID", Application.identifier);
        }

        public void AddStdParamData(string sdkMethod, string uasID)
        {
            Debug.Assert(!string.IsNullOrEmpty(sdkMethod), "Standard Events expect this.");

            AddPlatform();
            AddBatteryLoad();
            AddConnectionType();
            AddUserCountry();
            AddBuildGuuid();
            AddClientVersion();
            AddGameBundleID();

            if (!string.IsNullOrEmpty(sdkMethod))
            {
                Set("sdkMethod", sdkMethod);
            }

            if (!string.IsNullOrEmpty(uasID))
            {
                Set("uasUserID", uasID);
            }
        }

        public Dictionary<string, object> Data { get; private set; }
    }
}
