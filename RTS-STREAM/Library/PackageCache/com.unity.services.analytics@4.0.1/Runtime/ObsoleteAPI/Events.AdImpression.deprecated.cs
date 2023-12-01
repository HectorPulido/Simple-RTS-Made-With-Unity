using System;
using UnityEngine;

namespace Unity.Services.Analytics
{
    public static partial class Events
    {
        [Obsolete("This enum has been moved outside the Events class. Please use that instead. This enum will be removed in an upcoming release.")]
        public enum AdCompletionStatus
        {
            /// <summary>
            /// If the ad is fully viewed and therefore will count as an impression for the ad network.
            /// </summary>
            Completed = 0,
            /// <summary>
            /// If there is an option to exit the ad before generating revenue.
            /// </summary>
            Partial = 1,
            /// <summary>
            /// If the ad is not viewed at all (alternatively, don’t record the adImpression event in.
            /// </summary>
            Incomplete = 2
        }

        [Obsolete("This enum has been moved outside the Events class. Please use that instead. This enum will be removed in an upcoming release.")]
        public enum AdProvider
        {
            AdColony = 0,
            AdMob = 1,
            Amazon = 2,
            AppLovin = 3,
            ChartBoost = 4,
            Facebook = 5,
            Fyber = 6,
            Hyprmx = 7,
            Inmobi = 8,
            Maio = 9,
            Pangle = 10,
            Tapjoy = 11,
            UnityAds = 12,
            Vungle = 13,
            IrnSource = 14,
            Other = 15
        }

        /// <summary>
        /// Helper object to handle arguments for recording an AdImpression event.
        /// </summary>
        [Obsolete("This class has been aligned with other interfaces. Please use AdImpressionParameters with the AnalyticsService.Instance API instead. This class will be removed in an upcoming release")]
        public class AdImpressionArgs
        {
            /// <param name="adCompletionStatus">(Required) Indicates a successful Ad view. Select one of the `AdCompletionStatus` values.</param>
            /// <param name="adProvider">(Required) The Ad SDK that provided the Ad. Select one of the `AdProvider` values.</param>
            /// <param name="placementID">(Required) The unique identifier for the placement as integrated into the game.</param>
            /// <param name="placementName">(Required) If there is a place in the game that can show Ads from multiple networks, there won’t be a single placementId. This field compensates for that by providing a single name for your placement. Ideally, this would be an easily human-readable name such as ‘revive’ or ‘daily bonus’. This value is here for reporting purposes only.</param>
            public AdImpressionArgs(AdCompletionStatus adCompletionStatus, AdProvider adProvider, string placementID, string placementName)
            {
                AdCompletionStatus = adCompletionStatus;
                AdProvider = adProvider;
                PlacementID = placementID;
                PlacementName = placementName;
            }

            /// <summary>
            /// Indicates whether the Ad view was successful or not.
            /// </summary>
            public AdCompletionStatus AdCompletionStatus { get; set; }

            /// <summary>
            /// The Ad SDK that provided the Ad.
            /// </summary>
            public AdProvider AdProvider { get; set; }

            /// <summary>
            /// The unique identifier for the placement where the Ad appeared as integrated into the game.
            /// </summary>
            public string PlacementID { get; set; }

            /// <summary>
            /// If there is a place in the game that can show Ads from multiple networks, there won’t be a single placementId. This field compensates for that by providing a single name for your placement. Ideally, this would be an easily human-readable name such as ‘revive’ or ‘daily bonus’.
            /// This value is here for reporting purposes only.
            /// </summary>
            public string PlacementName { get; set; }

            /// <summary>
            /// Optional.
            /// The placementType should indicate what type of Ad is shown.
            /// This value is here for reporting purposes only.
            /// </summary>
            public string PlacementType { get; set; }

            /// <summary>
            /// Optional.
            /// The estimated ECPM in USD, you should populate this value if you can.
            /// </summary>
            public double? AdEcpmUsd { get; set; }

            /// <summary>
            /// Optional.
            /// The Ad SDK version you are using.
            /// </summary>
            public string SdkVersion { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            public string AdImpressionID { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            public string AdStoreDstID { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            public string AdMediaType { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            public Int64? AdTimeWatchedMs { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            public Int64? AdTimeCloseButtonShownMs { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            public Int64? AdLengthMs { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            public bool? AdHasClicked { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            public string AdSource { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            public string AdStatusCallback { get; set; }
        }

        /// <summary>
        /// Record an Ad Impression event.
        /// </summary>
        /// <param name="args">(Required) Helper object to handle arguments.</param>
        [Obsolete("The interface provided by this method has moved to AnalyticsService.Instance.AdImpression, and should be accessed from there instead. This API will be removed in an upcoming release.")]
        public static void AdImpression(AdImpressionArgs args)
        {
            // Enum.TryParse will fill out placementType if possible, but if it returns false or we get passed a null value we need to provide a default value to prevent runtime problems.
            if (!string.IsNullOrEmpty(args.PlacementType) || !Enum.TryParse(args.PlacementType, out AdPlacementType placementType))
            {
                placementType = AdPlacementType.BANNER;
            }

            var newParams = new AdImpressionParameters
            {
                AdCompletionStatus = (Analytics.AdCompletionStatus)(int)args.AdCompletionStatus,
                AdProvider = (Analytics.AdProvider)(int)args.AdProvider,
                AdEcpmUsd = args.AdEcpmUsd,
                AdHasClicked = args.AdHasClicked,
                AdImpressionID = args.AdImpressionID,
                AdLengthMs = args.AdLengthMs,
                AdMediaType = args.AdMediaType,
                AdSource = args.AdSource,
                AdStatusCallback = args.AdStatusCallback,
                AdStoreDstID = args.AdStoreDstID,
                AdTimeCloseButtonShownMs = args.AdTimeCloseButtonShownMs,
                AdTimeWatchedMs = args.AdTimeWatchedMs,
                PlacementID = args.PlacementID,
                PlacementName = args.PlacementName,
                PlacementType = placementType,
                SdkVersion = args.SdkVersion
            };

            AnalyticsService.Instance.AdImpression(newParams);
        }
    }
}
