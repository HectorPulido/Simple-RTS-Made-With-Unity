namespace Unity.Services.Analytics
{
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
}
