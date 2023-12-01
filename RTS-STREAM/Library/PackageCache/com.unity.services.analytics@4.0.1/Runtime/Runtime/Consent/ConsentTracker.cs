using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

[assembly: InternalsVisibleTo("Unity.Services.Analytics.Internal.Tests")]

namespace Unity.Services.Analytics.Internal
{
    interface IConsentTracker
    {
        Task<GeoIPResponse> CheckGeoIP();
        void SetUserConsentStatus(string key, bool consentGiven);
        Dictionary<string, string> requiredHeaders { get; }
        void BeginOptOutProcess(string identifier);
        void BeginOptOutProcess();
        void FinishOptOutProcess();
        bool IsGeoIpChecked();
        bool IsConsentGiven(string identifier);
        bool IsConsentGiven();
        bool IsConsentDenied();
        bool IsOptingOutInProgress();
        void SetDenyConsentToAll();
    }

    static class Consent
    {
        public static String Pipl => "pipl";
        public static String None => "none";
    }

    class ConsentTracker : IConsentTracker
    {
        readonly IGeoAPI m_GeoAPI;
        internal ConsentStatus optInPiplConsentStatus { get; set; }
        internal ConsentStatus optOutConsentStatus { get; set; }

        private Dictionary<string, string> piplHeaders =>
            new Dictionary<string, string>
        {
            { "PIPL_EXPORT", "true" },
            { "PIPL_CONSENT", "true" }
        };

        /// <summary>
        /// Returns the required headers, based on the required legislation.
        /// If PIPL is not required it will return an empty Dictionary.
        /// In order to get the correct information, `CheckGeoIP` method has to be called beforehand (which updates
        /// the geolocation response for the current session).
        ///
        /// </summary>
        public Dictionary<string, string> requiredHeaders =>
            response.identifier == Consent.Pipl ? piplHeaders : new Dictionary<string, string>();

        internal GeoIPResponse response;

        internal const string optInPiplConsentStatusPrefKey = "unity.services.analytics.pipl_consent_status";
        internal const string optOutConsentStatusPrefKey = "unity.services.analytics.consent_status";

        public ConsentTracker()
        {
            m_GeoAPI = new GeoAPI();
            optOutConsentStatus = ConsentStatus.Unknown;
            optInPiplConsentStatus = ConsentStatus.Unknown;
            ReadOptInPiplConsentStatus();
            ReadOptOutConsentStatus();
        }

        internal ConsentTracker(IGeoAPI geoApi)
        {
            m_GeoAPI = geoApi ?? new GeoAPI();
            optOutConsentStatus = ConsentStatus.Unknown;
            optInPiplConsentStatus = ConsentStatus.Unknown;
            ReadOptInPiplConsentStatus();
            ReadOptOutConsentStatus();
        }

        /// <summary>
        /// Triggers the GeoIP call, if no geolocation data was already stored for this session, otherwise returns
        /// the last GeoIP response.
        /// Updates the consent status of PIPL legislation to reflect the current requirements (if it was previously
        /// set as `Unknown`, `Not Required` or `Required But Unchecked`)
        /// If the consent was already denied/given, but PIPL is not required anymore, the status is not changed.
        ///
        /// `ConsentCheckException` is thrown if the GeoIP call was unsuccessful.
        ///
        /// </summary>
        /// <returns>`GeoIPResponse` object which maps the GeoIP response.</returns>
        /// <exception cref="ConsentCheckException">Thrown if the GeoIP call was unsuccessful.</exception>
        public async Task<GeoIPResponse> CheckGeoIP()
        {
            try
            {
                if (IsGeoIpChecked())
                {
                    return response;
                }

                var newResponse = await GetGeoIPResponse();

                response = newResponse;

                if (optInPiplConsentStatus == ConsentStatus.Unknown || optInPiplConsentStatus == ConsentStatus.NotRequired
                    || optInPiplConsentStatus == ConsentStatus.RequiredButUnchecked)
                {
                    optInPiplConsentStatus = newResponse.identifier == Consent.Pipl
                        ? ConsentStatus.RequiredButUnchecked
                        : ConsentStatus.NotRequired;
                    PlayerPrefs.SetInt(optInPiplConsentStatusPrefKey, (int)optInPiplConsentStatus);
                    PlayerPrefs.Save();
                }

                return newResponse;
            }
            catch (ConsentCheckException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Sets the consent status for the specified legislation identifier and saves it in Player Preferences.
        /// At the moment, it only works for PIPL (as every other legislation we track is opt-out based only, so we
        /// do not need to track whether the consent was explicitly given, and other legislation statuses are not
        /// separately stored).
        ///
        /// </summary>
        /// <param name="identifier">The legislation identifier for which the consent status should be changed.</param>
        /// <param name="consentGiven">The consent status which should be set for the specified legislation.</param>
        /// <exception cref="ConsentCheckException">Thrown if the incorrect legislation was being provided or the required consent flow cannot be determined.</exception>
        public void SetUserConsentStatus(string identifier, bool consentGiven)
        {
            ValidateConsentFlow(identifier);

            if (identifier == Consent.Pipl)
            {
                SetOptInPiplConsentStatus(consentGiven);
            }
            else
            {
                optOutConsentStatus = consentGiven ? ConsentStatus.Unknown : ConsentStatus.OptedOut;
                PlayerPrefs.SetInt(optOutConsentStatusPrefKey, (int)optOutConsentStatus);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Checks whether the GeoIP response is already stored, which indicates that `CheckGeoIP` has been successfully
        /// called.
        ///
        /// </summary>
        /// <returns>A boolean value whether the GeoIP response is not null.</returns>
        public bool IsGeoIpChecked()
        {
            return response != null;
        }

        /// <summary>
        /// Checks if the consent was given.
        /// If the required legislation is PIPL, it takes priority over any other legislation. Even if the user opted out
        /// from CCPA/GDPR/etc, but gave consent for PIPL, this method will return true.
        /// If PIPL is not required, the method does not take into account PIPL consent status.
        /// It requires `CheckGeoIP` method to be called beforehand.
        ///
        /// </summary>
        /// <returns>A boolean value whether the required consent was given.</returns>
        /// <exception cref="ConsentCheckException">Thrown if the required consent flow cannot be determined.</exception>
        public bool IsConsentGiven()
        {
            ValidateConsentWasChecked();
            return IsConsentGiven(response.identifier);
        }

        /// <summary>
        /// Checks if the consent was given for the specified legislation (either explicitly given for PIPL or Unknown
        /// for opt-out flag).
        /// This method does not take into account the geolocation.
        ///
        /// As we currently do not store opt-out-based legislation as separate flags, if the identifier is not PIPL,
        /// the method checks the opt-out consent flag.
        /// </summary>
        /// <param name="identifier">The identifier of the legislation.</param>
        /// <returns>The boolean value whether the consent was given.</returns>
        public bool IsConsentGiven(string identifier)
        {
            if (identifier == Consent.Pipl)
            {
                return optInPiplConsentStatus == ConsentStatus.ConsentGiven;
            }

            return optOutConsentStatus == ConsentStatus.Unknown;
        }

        /// <summary>
        /// Checks if the consent was denied or the user already opted-out.
        ///
        /// If the required legislation is PIPL, it takes priority over any other legislation. Even if the user did not
        /// opt out from CCPA/GDPR/etc, but denied consent for PIPL, this method will return true.
        /// If PIPL is not required, the method does not take into account PIPL consent status.
        /// It requires `CheckGeoIP` method to be called beforehand.
        ///
        /// If the user already started the process of opting out, but it wasn't finished yet, the method will return
        /// false.
        ///
        /// </summary>
        /// <returns>A boolean value whether the required consent was either denied or the user opted out.</returns>
        /// <exception cref="ConsentCheckException">Thrown if the required consent flow cannot be determined.</exception>
        public bool IsConsentDenied()
        {
            ValidateConsentWasChecked();

            if (response.identifier == Consent.Pipl)
            {
                return optInPiplConsentStatus == ConsentStatus.ConsentDenied
                    || optInPiplConsentStatus == ConsentStatus.OptedOut;
            }

            return optOutConsentStatus == ConsentStatus.OptedOut;
        }

        /// <summary>
        /// Checks whether the opting out process already started for any legislation, regardless of the geolocation
        /// response.
        ///
        /// </summary>
        /// <returns>A boolean value whether any consent flag is set to Forgetting.</returns>
        /// <exception cref="ConsentCheckException">Thrown if the required consent flow cannot be determined.</exception>
        public bool IsOptingOutInProgress()
        {
            ValidateConsentWasChecked();

            return (response.identifier == Consent.Pipl)
                ? optInPiplConsentStatus == ConsentStatus.Forgetting
                : optOutConsentStatus == ConsentStatus.Forgetting;
        }

        /// <summary>
        /// Begins the process of opting out for the specified legislation.
        /// It sets the consent status for the specified legislation identifier to `Forgetting` and saves it in Player
        /// Preferences.
        /// It includes the validation of the specified legislation. It requires `CheckGeoIP` method to be called before
        /// calling this method.
        /// In case of PIPL, the opt-in-based legislation, it will do nothing if the consent was not explicitly given
        /// beforehand, as the opting-out process includes sending the ForgetMe event to the backend.
        /// In order to deny the consent immediately, use `SetUserConsentStatus(string identifier, bool consentGiven)`
        ///
        /// If PIPL legislation is required and the specified legislation identifier is not PIPL, or the other
        /// legislation is required, but the specified legislation identifier is PIPL, `ConsentCheckException` is thrown.
        ///
        /// </summary>
        /// <param name="identifier">The identifier of the legislation.</param>
        /// <exception cref="ConsentCheckException">Thrown if the incorrect legislation was being provided or the required consent flow cannot be determined.</exception>
        public void BeginOptOutProcess(string identifier)
        {
            ValidateConsentWasChecked();
            ValidateConsentFlow(identifier);

            if (identifier == Consent.Pipl && optInPiplConsentStatus == ConsentStatus.ConsentGiven)
            {
                optInPiplConsentStatus = ConsentStatus.Forgetting;
                PlayerPrefs.SetInt(optInPiplConsentStatusPrefKey, (int)optInPiplConsentStatus);
                PlayerPrefs.Save();
            }
            else if (identifier != Consent.Pipl && optOutConsentStatus == ConsentStatus.Unknown)
            {
                optOutConsentStatus = ConsentStatus.Forgetting;
                PlayerPrefs.SetInt(optOutConsentStatusPrefKey, (int)optOutConsentStatus);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Begins the process of opting out from the current legislation.
        /// It sets the consent status for the specified legislation identifier to `Forgetting` and saves it in Player
        /// Preferences.
        /// In case of PIPL, the opt-in-based legislation, it will do nothing if the consent was not explicitly given
        /// beforehand.
        /// In order to deny the consent immediately, use `SetUserConsentStatus(string identifier, bool consentGiven)`
        /// </summary>
        public void BeginOptOutProcess()
        {
            if (optInPiplConsentStatus == ConsentStatus.ConsentGiven)
            {
                optInPiplConsentStatus = ConsentStatus.Forgetting;
                PlayerPrefs.SetInt(optInPiplConsentStatusPrefKey, (int)optInPiplConsentStatus);
                PlayerPrefs.Save();
            }

            if (optOutConsentStatus == ConsentStatus.Unknown)
            {
                optOutConsentStatus = ConsentStatus.Forgetting;
                PlayerPrefs.SetInt(optOutConsentStatusPrefKey, (int)optOutConsentStatus);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Finishes the opting out process, whichever was triggered before.
        /// If both consent statuses were set to Forgetting (most likely due to technical problems), it will update both
        /// statuses to `OptedOut`.
        /// </summary>
        public void FinishOptOutProcess()
        {
            if (optInPiplConsentStatus == ConsentStatus.Forgetting)
            {
                optInPiplConsentStatus = ConsentStatus.OptedOut;
                PlayerPrefs.SetInt(optInPiplConsentStatusPrefKey, (int)optInPiplConsentStatus);
                PlayerPrefs.Save();
            }

            if (optOutConsentStatus == ConsentStatus.Forgetting)
            {
                optOutConsentStatus = ConsentStatus.OptedOut;
                PlayerPrefs.SetInt(optOutConsentStatusPrefKey, (int)optOutConsentStatus);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Sets opt-out-based consent flag to OptedOut and PIPL flag to either ConsentDenied or OptedOut - depending if
        /// the consent was previously given.
        /// </summary>
        public void SetDenyConsentToAll()
        {
            optOutConsentStatus = ConsentStatus.OptedOut;
            optInPiplConsentStatus = optInPiplConsentStatus == ConsentStatus.Forgetting
                ? ConsentStatus.OptedOut
                : ConsentStatus.ConsentDenied;
            PlayerPrefs.SetInt(optInPiplConsentStatusPrefKey, (int)optInPiplConsentStatus);
            PlayerPrefs.SetInt(optOutConsentStatusPrefKey, (int)optOutConsentStatus);
            PlayerPrefs.Save();
        }

        internal void SetOptInPiplConsentStatus(bool consentGiven)
        {
            optInPiplConsentStatus = consentGiven ? ConsentStatus.ConsentGiven : ConsentStatus.ConsentDenied;
            PlayerPrefs.SetInt(optInPiplConsentStatusPrefKey,
                consentGiven ? (int)ConsentStatus.ConsentGiven : (int)ConsentStatus.ConsentDenied);
            PlayerPrefs.Save();
        }

        internal void ReadOptInPiplConsentStatus()
        {
            if (PlayerPrefs.HasKey(optInPiplConsentStatusPrefKey))
            {
                optInPiplConsentStatus = (ConsentStatus)PlayerPrefs.GetInt(optInPiplConsentStatusPrefKey);
            }
        }

        internal void ReadOptOutConsentStatus()
        {
            if (PlayerPrefs.HasKey(optOutConsentStatusPrefKey))
            {
                optOutConsentStatus = (ConsentStatus)PlayerPrefs.GetInt(optOutConsentStatusPrefKey);
            }
        }

        internal async Task<GeoIPResponse> GetGeoIPResponse()
        {
            try
            {
                return await m_GeoAPI.MakeRequest();
            }
            catch (ConsentCheckException e)
            {
                throw e;
            }
        }

        private void ValidateConsentWasChecked()
        {
            if (!IsGeoIpChecked())
            {
                throw new ConsentCheckException(ConsentCheckExceptionReason.ConsentFlowNotKnown,
                    CommonErrorCodes.Unknown,
                    "The required consent flow cannot be determined. Make sure GeoIP was successfully called.",
                    null);
            }
        }

        private void ValidateConsentFlow(string identifier)
        {
            if (response.identifier == Consent.Pipl && identifier != response.identifier
                || response.identifier != Consent.Pipl && identifier == Consent.Pipl)
            {
                throw new ConsentCheckException(ConsentCheckExceptionReason.InvalidConsentFlow,
                    CommonErrorCodes.InvalidRequest,
                    "The requested action is unavailable for this legislation. Please check documentation for more details.",
                    null);
            }
        }
    }
}
