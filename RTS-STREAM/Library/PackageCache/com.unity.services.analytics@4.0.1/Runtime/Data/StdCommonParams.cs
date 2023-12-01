using Unity.Services.Analytics.Internal;
using UnityEngine;

namespace Unity.Services.Analytics.Data
{
    // http://go/UA2_Spreadsheet
    // but they are not a) provided by us or b) an event param.
    // JIRA-193 - Fetch this data!
    // Some of the info can be got here: https://docs.unity3d.com/ScriptReference/SystemInfo.html
    /// <summary>
    /// All the common event params that exist in all Events.
    /// There is other info in this spreadsheet that is common
    /// </summary>
    class StdCommonParams
    {
        internal string GameStoreID { get; set; }
        internal string GameBundleID { get; set; }
        internal string Platform { get; set; }
        internal string UasUserID { get; set; }
        internal string Idfv { get; set; }
        internal double? DeviceVolume { get; set; }
        internal double? BatteryLoad { get; set; }
        internal string BuildGuuid { get; set; }
        internal string ClientVersion { get; set; }
        internal string UserCountry { get; set; }
        internal string ProjectID { get; set; }

        internal void SerializeCommonEventParams(ref IBuffer buf, string callingMethodIdentifier)
        {
            if (!string.IsNullOrEmpty(GameStoreID))
            {
                // Schema: Optional
                buf.PushString(GameStoreID, "gameStoreID");
            }

            if (!string.IsNullOrEmpty(GameBundleID))
            {
                // Schema: Optional
                buf.PushString(GameBundleID, "gameBundleID");
            }

            if (!string.IsNullOrEmpty(Platform))
            {
                // Schema: Optional, IsEnum
                buf.PushString(Platform, "platform");
            }

            if (!string.IsNullOrEmpty(Idfv))
            {
                // Schema: Optional
                buf.PushString(Idfv, "idfv");
            }

            if (!string.IsNullOrEmpty(UasUserID))
            {
                // Schema: Optional
                buf.PushString(UasUserID, "uasUserID");
            }

            if (!string.IsNullOrEmpty(BuildGuuid))
            {
                // Schema: Optional
                buf.PushString(BuildGuuid, "buildGUUID");
            }

            if (!string.IsNullOrEmpty(ClientVersion))
            {
                // Schema: Required
                buf.PushString(ClientVersion, "clientVersion");
            }

            if (!string.IsNullOrEmpty(UserCountry))
            {
                // Schema: Optional, IsEnum
                buf.PushString(UserCountry, "userCountry");
            }

            if (DeviceVolume != null)
            {
                buf.PushDouble(DeviceVolume.Value, "deviceVolume"); // Schema: Optional
            }

            if (BatteryLoad != null)
            {
                buf.PushDouble(BatteryLoad.Value, "batteryLoad"); // Schema: Optional
            }

            if (!string.IsNullOrEmpty(ProjectID))
            {
                buf.PushString(ProjectID, "projectID");
            }

            // Schema: Required
            buf.PushString(callingMethodIdentifier, "sdkMethod");
        }
    }
}
