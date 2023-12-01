using System;

namespace UnityEngine.Advertisements.Utilities {
    public static class EnumUtilities {
        public static T GetEnumFromAndroidJavaObject<T>(AndroidJavaObject androidJavaObject, T defaultValue) {
            try {
                return (T) Enum.Parse(typeof(T), androidJavaObject.Call<string>("toString"), true);
            } catch (Exception) {
                Debug.LogError("Unable to map native enum to managed enum");
            }

            return defaultValue;
        }
    }
}
