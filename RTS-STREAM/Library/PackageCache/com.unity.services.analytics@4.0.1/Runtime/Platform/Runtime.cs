
using UnityEngine;

namespace Unity.Services.Analytics.Platform
{
    // Keep the enum values in Caps!
    // We stringify the values.
    // This enum is defined for all Std events.
    // http://go/ihu2c
    // JIRA-193 Talk to Jetpack about this.
    // Likely this can be compile time to some degree.
    // https://docs.unity3d.com/ScriptReference/RuntimePlatform.html
    // https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
    enum UA2PlatformCode
    {
        UNKNOWN,
        IOS, IOS_MOBILE, IOS_TABLET, IOS_TV,
        ANDROID, ANDROID_MOBILE, ANDROID_CONSOLE,
        WINDOWS_MOBILE, WINDOWS_TABLET,
        BLACKBERRY_MOBILE, BLACKBERRY_TABLET,
        FACEBOOK, AMAZON,
        WEB,
        PC_CLIENT, MAC_CLIENT,
        PS3, PS4, PSVITA,
        XBOX360, XBOXONE,
        WIIU, SWITCH,
    }

    public static class Runtime
    {
        /// <summary>
        /// Returns the name of the platform this app is running on.
        /// </summary>
        public static string Name()
        {
            return GetPlatform().ToString();
        }

        static UA2PlatformCode GetPlatform()
        {
            // NOTE: Assumes we're only supporting Unity LTS
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return UA2PlatformCode.MAC_CLIENT;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.LinuxPlayer:
                    return UA2PlatformCode.PC_CLIENT;
                case RuntimePlatform.IPhonePlayer:
                    return UA2PlatformCode.IOS;
                case RuntimePlatform.Android:
                    return UA2PlatformCode.ANDROID;
                case RuntimePlatform.WebGLPlayer:
                    return UA2PlatformCode.WEB;
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerX86:
                case RuntimePlatform.WSAPlayerARM:
                    return (SystemInfo.deviceType == DeviceType.Handheld)
                        ? UA2PlatformCode.WINDOWS_MOBILE
                        : UA2PlatformCode.PC_CLIENT;
                case RuntimePlatform.PS4:
                    return UA2PlatformCode.PS4;
                case RuntimePlatform.XboxOne:
                    return UA2PlatformCode.XBOXONE;
                case RuntimePlatform.tvOS:
                    return UA2PlatformCode.IOS_TV;
                case RuntimePlatform.Switch:
                    return UA2PlatformCode.SWITCH;
                default:
                    return UA2PlatformCode.UNKNOWN;
            }
        }
    }
}
