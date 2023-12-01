using System;
using System.Collections.Generic;

namespace Unity.Services.Analytics
{
    public static class CustomEventSample
    {
        public static void RecordCustomEventWithNoParameters()
        {
            AnalyticsService.Instance.CustomData("myEvent", new Dictionary<string, object>());
        }

        public static void RecordCustomEventWithParameters()
        {
            var parameters = new Dictionary<string, object>
            {
                { "fabulousString", "hello there" },
                { "sparklingInt", 1337 },
                { "tremendousLong", Int64.MaxValue },
                { "spectacularFloat", 0.451f },
                { "incredibleDouble", 0.000000000000000031337 },
                { "peculiarBool", true },
                { "ultimateTimestamp", DateTime.UtcNow }
            };

            AnalyticsService.Instance.CustomData("myEvent", parameters);
        }
    }
}
