using System;
using Windows.Storage;

namespace Shared.Helpers
{
    public class SettingsHelper
    {

        public static Object GetSetting(String settingName, bool isRoaming = false)
        {
            ApplicationDataContainer settings;

            if (!isRoaming)
            {
                settings = ApplicationData.Current.LocalSettings;
            }
            else
            {
                settings = ApplicationData.Current.RoamingSettings;
            }

            return settings.Values[settingName];
        }

        public static void UpdateSetting(String settingName, Object settingValue, bool isRoaming = false)
        {

            ApplicationDataContainer settings;

            if (!isRoaming)
            {
                settings = ApplicationData.Current.LocalSettings;
            }
            else
            {
                settings = ApplicationData.Current.RoamingSettings;
            }

            settings.Values[settingName] = settingValue;

        }

    }
}