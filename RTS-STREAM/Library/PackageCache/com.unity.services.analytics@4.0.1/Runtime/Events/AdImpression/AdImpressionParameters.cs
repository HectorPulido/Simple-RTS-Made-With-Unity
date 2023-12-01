using System;

namespace Unity.Services.Analytics
{
    /// <summary>
    /// Helper struct to handle arguments for recording an AdImpression event.
    /// </summary>
    public struct AdImpressionParameters
    {
        /// <summary>
        /// Indicates whether the Ad view was successful or not.
        /// </summary>
        public AdCompletionStatus AdCompletionStatus;

        /// <summary>
        /// The Ad SDK that provided the Ad.
        /// </summary>
        public AdProvider AdProvider;

        /// <summary>
        /// The unique identifier for the placement where the Ad appeared as integrated into the game.
        /// </summary>
        public string PlacementID;

        /// <summary>
        /// If there is a place in the game that can show Ads from multiple networks, there won’t be a single placementId. This field compensates for that by providing a single name for your placement. Ideally, this would be an easily human-readable name such as ‘revive’ or ‘daily bonus’.
        /// This value is here for reporting purposes only.
        /// </summary>
        public string PlacementName;

        /// <summary>
        /// Optional.
        /// The placementType should indicate what type of Ad is shown.
        /// This value is here for reporting purposes only.
        /// </summary>
        public AdPlacementType? PlacementType;

        /// <summary>
        /// Optional.
        /// The estimated ECPM in USD, you should populate this value if you can.
        /// </summary>
        public double? AdEcpmUsd;

        /// <summary>
        /// Optional.
        /// The Ad SDK version you are using.
        /// </summary>
        public string SdkVersion;

        /// <summary>
        /// Optional.
        /// </summary>
        public string AdImpressionID;

        /// <summary>
        /// Optional.
        /// </summary>
        public string AdStoreDstID;

        /// <summary>
        /// Optional.
        /// </summary>
        public string AdMediaType;

        /// <summary>
        /// Optional.
        /// </summary>
        public Int64? AdTimeWatchedMs;

        /// <summary>
        /// Optional.
        /// </summary>
        public Int64? AdTimeCloseButtonShownMs;

        /// <summary>
        /// Optional.
        /// </summary>
        public Int64? AdLengthMs;

        /// <summary>
        /// Optional.
        /// </summary>
        public bool? AdHasClicked;

        /// <summary>
        /// Optional.
        /// </summary>
        public string AdSource;

        /// <summary>
        /// Optional.
        /// </summary>
        public string AdStatusCallback;
    }
}
