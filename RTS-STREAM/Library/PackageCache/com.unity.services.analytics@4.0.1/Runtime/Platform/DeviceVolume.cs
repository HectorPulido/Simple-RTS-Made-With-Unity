using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.Services.Analytics.Platform
{
    static class DeviceVolumeProvider
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern float unity_services_analytics_get_device_volume();
#endif
        internal static float? GetDeviceVolume()
        {
#if UNITY_IOS && !UNITY_EDITOR
            // Provided by the plugin in Runtime/Plugins/iOS/VolumeIOSPlugin.mm
            return unity_services_analytics_get_device_volume();
#elif UNITY_ANDROID && !UNITY_EDITOR
            // The below code should be equivalent to the following Android code. Note that constants have been converted to their raw values,
            // as documented in the android docs.
            // ---
            // Activity activity = UnityPlayer.currentActivity;                                             // Get the current activity as provided by Unity
            // AudioManager audioManager = (AudioManager) activity.getSystemService(Context.AUDIO_SERVICE); // Get the system audio service from the activity
            // double volume = audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);                     // Get the music/media audio stream volume 
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject audioManager = activity.Call<AndroidJavaObject>("getSystemService", "audio");
            int STREAM_MUSIC_rawValue = 3; // See android docs for STREAM_MUSIC constant on AudioManager
            int volume = audioManager.Call<int>("getStreamVolume", STREAM_MUSIC_rawValue);
            int maxVolume = audioManager.Call<int>("getStreamMaxVolume", STREAM_MUSIC_rawValue);
            return (float) volume / (float) maxVolume;
#else
            // Other platforms don't support device volume at this time.
            return null;
#endif
        }
    }
}
