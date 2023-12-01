using System;

namespace Unity.Services.Analytics
{
    public static class AnalyticsService
    {
        internal static AnalyticsServiceInstance internalInstance = new AnalyticsServiceInstance();

        public static IAnalyticsService Instance => internalInstance;
    }
}
