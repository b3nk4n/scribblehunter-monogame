using System;
using Xamarin.Essentials;

namespace ScribbleHunter
{
    public static class VibrationManager
    {
        private static SettingsManager settings = SettingsManager.GetInstance();

        public static void Vibrate(float seconds)
        {
            if (!settings.GetVabrationValue())
            {
                return;
            }

            try
            {
                Vibration.Vibrate(TimeSpan.FromSeconds(seconds).TotalMilliseconds);
            }
            catch { /* Ignored */ }
        }
    }
}