using System;
using System.Collections.Generic;

namespace Unity.Services.Analytics
{
    public static partial class Events
    {
        /// <summary>
        /// Record a custom event. A schema for this event must exist on the dashboard or it will be ignored.
        /// </summary>
        [Obsolete("The interface provided by this method has moved to AnalyticsService.Instance.CustomData, and should be accessed from there instead. This API will be removed in an upcoming release.")]
        public static void CustomData(string eventName, IDictionary<string, object> eventParams)
        {
            AnalyticsService.Instance.CustomData(eventName, eventParams);
        }
    }
}
