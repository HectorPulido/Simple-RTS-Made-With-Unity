using System;
using UnityEngine.Serialization;

namespace Unity.Services.Analytics.Internal
{
    [Serializable]
    class GeoIPResponse
    {
        public string identifier;
        public string country;
        public string region;
        public int ageGateLimit;
    }
}
