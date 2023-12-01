using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Analytics.Internal;

namespace Unity.Services.Analytics
{
    public interface IAnalyticsService
    {
        /// <summary>
        /// This is the URL for the Unity Analytics privacy policy. This policy page should
        /// be presented to the user in a platform-appropriate way along with the ability to
        /// opt out of data collection.
        /// </summary>
        string PrivacyUrl { get; }

        /// <summary>
        /// Forces an immediately upload of all recorded events to the server, if there is an internet connection.
        /// </summary>
        /// <exception cref="ConsentCheckException">Thrown if the required consent flow cannot be determined..</exception>
        void Flush();

        void AdImpression(AdImpressionParameters parameters);

        /// <summary>
        /// Record a Transaction event.
        /// </summary>
        /// <param name="transactionParameters">(Required) Helper object to handle parameters.</param>
        void Transaction(TransactionParameters transactionParameters);

        /// <summary>
        /// Record a TransactionFailed event.
        /// </summary>
        /// <param name="parameters">(Required) Helper object to handle parameters.</param>
        void TransactionFailed(TransactionFailedParameters parameters);

        /// <summary>
        /// Record a custom event. A schema for this event must exist on the dashboard or it will be ignored.
        /// </summary>
        void CustomData(string eventName, IDictionary<string, object> eventParams);

        /// <summary>
        /// Returns identifiers of required consents we need to gather from the user
        /// in order to be allowed to sent analytics events.
        /// This method must be called every time the game starts - without checking the geolocation,
        /// no event will be sent (even if the consent was already given).
        /// If the required consent was already given, an empty list is returned.
        /// If the user already opted out from the current legislation, an empty list is returned.
        /// It involves the GeoIP call.
        /// `ConsentCheckException` is thrown if the GeoIP call was unsuccessful.
        ///
        /// </summary>
        /// <returns>A list of consent identifiers that are required for sending analytics events.</returns>
        /// <exception cref="ConsentCheckException">Thrown if the GeoIP call was unsuccessful.</exception>
        Task<List<string>> CheckForRequiredConsents();

        /// <summary>
        /// Sets the consent status for the specified opt-in-based legislation (PIPL etc).
        /// The required legislation identifier can be found by calling `CheckForRequiredConsents` method.
        /// If this method is tried to be used for the incorrect legislation (PIPL outside China etc),
        /// the `ConsentCheckException` is thrown.
        ///
        /// </summary>
        /// <param name="identifier">The legislation identifier for which the consent status should be changed.</param>
        /// <param name="consent">The consent status which should be set for the specified legislation.</param>
        /// <exception cref="ConsentCheckException">Thrown if the incorrect legislation was being provided or
        /// the required consent flow cannot be determined.</exception>
        void ProvideOptInConsent(string identifier, bool consent);

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
        void OptOut();

        /// <summary>
        /// Allows other sources to write events with common analytics parameters to the Analytics service. This is primarily for use
        /// by other packages - as this method adds common parameters that may not be expected in the general case, for custom events
        /// you should use the <c>CustomData</c> method instead.
        /// </summary>
        /// <param name="eventToRecord">Internal event to record</param>
        void RecordInternalEvent(Event eventToRecord);

        /// <summary>
        /// Record an acquisitionSource event.
        /// </summary>
        /// <param name="acquisitionSourceParameters">(Required) Helper object to handle parameters.</param>
        void AcquisitionSource(AcquisitionSourceParameters acquisitionSourceParameters);

        /// <summary>
        /// Allows you to disable the Analytics service. When the service gets disabled all currently cached data both in RAM and on disk
        /// will be deleted and any new events will be voided. By default the service is enabled so you do not need to call this method on start.
        /// Will return instantly when disabling, must be awaited when re-enabling.
        /// <example>
        /// To disable the Analytics Service before the game starts
        /// <code>
        /// [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        /// static void DisableAnalytics()
        /// {
        ///     AnalyticsService.Instance.SetAnalyticsEnabled(false);
        /// }
        /// </code>
        /// </example>
        /// </summary>
        Task SetAnalyticsEnabled(bool enabled);

        /// <summary>
        /// Converts an amount of currency to the minor units required for the objects passed to the Transaction method.
        /// This method uses data from ISO 4217. Note that this method expects you to pass in currency in the major units for
        /// conversion - if you already have data in the minor units you don't need to call this method.
        /// For example - 1.99 USD would be converted to 199, 123 JPY would be returned unchanged.
        /// </summary>
        /// <param name="currencyCode">The ISO4217 currency code for the input currency. For example, USD for dollars, or JPY for Japanese Yen</param>
        /// <param name="value">The major unit value of currency, for example 1.99 for 1 dollar 99 cents.</param>
        /// <returns>The minor unit value of the input currency, for example for an input of 1.99 USD 199 would be returned.</returns>
        long ConvertCurrencyToMinorUnits(string currencyCode, double value);
    }
}
