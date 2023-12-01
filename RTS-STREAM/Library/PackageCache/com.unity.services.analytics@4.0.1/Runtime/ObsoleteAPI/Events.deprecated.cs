using System;

namespace Unity.Services.Analytics
{
    [Obsolete("The interface provided by this static class has moved to AnalyticsService.Instance, and should be accessed from there instead. This API will be removed in an upcoming release.")]
    public static partial class Events
    {
        /// <summary>
        /// This is the URL for the Unity Analytics privacy policy. This policy page should
        /// be presented to the user in a platform-appropriate way along with the ability to
        /// opt out of data collection.
        /// </summary>
        [Obsolete("The interface provided by this field has moved to AnalyticsService.Instance.PrivacyUrl, and should be accessed from there instead. This API will be removed in an upcoming release.")]
        public static readonly string PrivacyUrl = "https://unity3d.com/legal/privacy-policy";

        /// <summary>
        /// Opts the user out of sending analytics from all legislations.
        /// To deny consent for a specific opt-in legislation, like PIPL, use `ProvideConsent(string key, bool consent)` method)
        /// All existing cached events and any subsequent events will be discarded immediately.
        /// A final 'forget me' signal will be uploaded which will trigger purge of analytics data for this user from the back-end.
        /// If this 'forget me' event cannot be uploaded immediately (e.g. due to network outage), it will be reattempted regularly
        /// until successful upload is confirmed.
        /// Consent status is stored in PlayerPrefs so that the opted-out status is maintained over app restart.
        /// This action cannot be undone.
        /// </summary>
        /// <exception cref="ConsentCheckException">Thrown if the required consent flow cannot be determined..</exception>
        [Obsolete("The interface provided by this method has moved to AnalyticsService.Instance.OptOut, and should be accessed from there instead. This API will be removed in an upcoming release.")]
        public static void OptOut()
        {
            AnalyticsService.Instance.OptOut();
        }

        /// <summary>
        /// Forces an immediately upload of all recorded events to the server, if there is an internet connection.
        /// </summary>
        /// <exception cref="ConsentCheckException">Thrown if the required consent flow cannot be determined..</exception>
        [Obsolete("The interface provided by this method has moved to AnalyticsService.Instance.Flush, and should be accessed from there instead. This API will be removed in an upcoming release.")]
        public static void Flush()
        {
            AnalyticsService.Instance.Flush();
        }
    }
}
