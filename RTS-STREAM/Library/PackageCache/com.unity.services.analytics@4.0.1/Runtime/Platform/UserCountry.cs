using System.Globalization;

namespace Unity.Services.Analytics.Internal.Platform
{
    public static class UserCountry
    {
        public static string Name()
        {
            // User country cannot be reliably deduced from any setting we have available here
            // without using location services, so we return ZZ so the Analytics service will use
            // GeoIP.
            return "";
        }
    }
}
