using System;
using Microsoft.Devices;

namespace ScribbleHunter
{
    public static class VibrationManager
    {
        private static SettingsManager settings = SettingsManager.GetInstance();

        public static void Vibrate(float seconds)
        {
            // TODO find a cross platform solution for vibration functionality
            //if (settings.GetVabrationValue())
            //    VibrateController.Default.Start(TimeSpan.FromSeconds(seconds));
        }
    }
}