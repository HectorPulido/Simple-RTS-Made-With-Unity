using System;
using UnityEngine;

namespace Unity.Services.Analytics.Platform
{
    interface IIDeviceIdentifiersInternal
    {
        string Idfv { get; }
    }

    class DeviceIdentifiersInternal : IIDeviceIdentifiersInternal
    {
        public string Idfv => SystemInfo.deviceUniqueIdentifier;
    }
}
