using System;
using UnityEngine;

namespace Unity.Services.Analytics
{
    partial class AnalyticsServiceInstance
    {
        /// <summary>
        /// Record an Ad Impression event.
        /// </summary>
        /// <param name="adImpressionParameters">(Required) Helper object to handle arguments.</param>
        public void AdImpression(AdImpressionParameters adImpressionParameters)
        {
            if (!ServiceEnabled)
            {
                return;
            }

            if (string.IsNullOrEmpty(adImpressionParameters.PlacementID))
            {
                Debug.LogError("Required to have a value for placementID.");
            }

            if (string.IsNullOrEmpty(adImpressionParameters.PlacementName))
            {
                Debug.LogError("Required to have a value for placementName.");
            }

            dataGenerator.AdImpression(ref dataBuffer, DateTime.Now, m_CommonParams, "com.unity.services.analytics.events.adimpression", adImpressionParameters);
        }
    }
}
