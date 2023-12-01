using System;
using System.Globalization;
using UnityEngine;

#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace Unity.Services.Analytics.Internal
{
    static class Locale
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string unity_services_current_language_code();

        internal static string CurrentLanguageCode()
        {
            return unity_services_current_language_code();
        }

#elif UNITY_ANDROID && !UNITY_EDITOR
        internal static string CurrentLanguageCode()
        {
            AndroidJavaClass localeClass = new AndroidJavaClass("java.util.Locale");
            AndroidJavaObject defaultLocale = localeClass.CallStatic<AndroidJavaObject>("getDefault");
            return defaultLocale.Call<string>("getLanguage");
        }

#else
        internal static string CurrentLanguageCode()
        {
            return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        }

#endif
        public static string AnalyticsRegionLanguageCode()
        {
            // As we can't reliably report current country code (as the only country code we have access to is the region settings,
            // not the user's current country as expected by the Analytics service) then we return ZZ to have the Analytics service
            // infer country from GeoIP instead.
            return $"{CurrentLanguageCode()}_ZZ";
        }

        /// <summary>
        /// Returns the current culture info. Invokes native method on Android and iOS.
        /// </summary>
        /// <returns></returns>
        [Obsolete("The 'language-regionSettingsCountry' code used by Analytics is non-standard, so this method may throw exceptions when used on systems with non-ISO language/region combinations. Prefer using AnalyticsRegionLanguageCode instead.")]
        public static CultureInfo CurrentCulture()
        {
            return CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Returns the current culture info. Invokes native method on iOS.
        /// </summary>
        /// <returns></returns>
        [Obsolete("The 'language-regionSettingsCountry' code used by Analytics is non-standard, so this method may throw exceptions when used on systems with non-ISO language/region combinations. Prefer using AnalyticsRegionLanguageCode instead.")]
        public static CultureInfo SystemCulture()
        {
            return CultureInfo.InvariantCulture;
        }
    }
}
