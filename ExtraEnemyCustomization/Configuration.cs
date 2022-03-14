﻿using BepInEx;
using BepInEx.Configuration;
using EECustom.Managers;
using System.IO;

namespace EECustom
{
    public static class Configuration
    {
        public const string SECTION_USER = "1. User-End";
        public const string SECTION_RUNDEVS = "2. Rundown Developer";
        public const string SECTION_LOGGING = "3. Logging";
        public const string SECTION_DEV = "4. DEBUG";
        public const int CONFIG_VERSION = 1;

        //USER CONFIGS
        public static ConfigEntry<bool> ShowMarkerText { get; private set; }
        public static ConfigEntry<bool> ShowMarkerDistance { get; private set; }
        public static ConfigEntry<bool> ShowExplosionEffect { get; private set; }

        //RUNDOWN DEVELOPER CONFIGS
        public static ConfigEntry<bool> UseLiveEdit { get; private set; }
        public static ConfigEntry<bool> LinkMTFOHotReload { get; private set; }

        //LOGGING CONFIGS
        public static ConfigEntry<bool> UseDebugLog { get; private set; }
        public static ConfigEntry<bool> UseVerboseLog { get; private set; }
        public static ConfigEntry<AssetCacheManager.OutputType> AssetCacheBehaviour { get; private set; }

        //DEVELOPER CONFIGS
        public static ConfigEntry<bool> DumpConfig { get; private set; }

        private static ConfigFile _currentContext;

        public static void CreateAndBindAll()
        {
            var path = Path.Combine(Paths.ConfigPath, "EEC.cfg");
            var config = new ConfigFile(path, true);
            var version = BindConfigVersion(config);
            if (version.Value < CONFIG_VERSION)
            {
                //Rebuild Config On Major Config version change happened
                File.Delete(path);
                config = new ConfigFile(path, true);
                _ = BindConfigVersion(config);
            }
            BindAll(config);
        }

        public static void BindAll(ConfigFile context)
        {
            _currentContext = context;

            ShowMarkerText = BindUserConfig("Marker Text", "Display Enemy Marker Texts? (if set by rundown devs)", true);
            ShowMarkerDistance = BindUserConfig("Marker Distance", "Display Enemy Marker Distance? (if set by rundown devs)", true);
            ShowExplosionEffect = BindUserConfig("Explosion Flash", "(Accessibility) Display Light flash effect for explosion abilities?", true);

            UseLiveEdit = BindRdwDevConfig("Live Edit", "Reload Config when they are edited while in-game", false);
            LinkMTFOHotReload = BindRdwDevConfig("Reload on MTFO HotReload", "Reload Configs when MTFO's HotReload button has pressed?", true);

            UseDebugLog = BindLoggingConfig("UseDevMessage", "Using Dev Message for Debugging your config?", false);
            UseVerboseLog = BindLoggingConfig("Verbose", "Using Much more detailed Message for Debugging?", false);
            AssetCacheBehaviour = BindLoggingConfig("Cached Asset Result Output", "How does your cached material/texture result be returned?", AssetCacheManager.OutputType.None);

            DumpConfig = BindDevConfig("DumpConfig", "Dump Empty Config file?", false);
        }

        private static ConfigEntry<int> BindConfigVersion(ConfigFile context)
        {
            return context.Bind(new ConfigDefinition("Version", "Config Version"), CONFIG_VERSION);
        }

        private static ConfigEntry<T> BindUserConfig<T>(string name, string description, T defaultValue)
        {
            return BindItem(_currentContext, SECTION_USER, name, description, defaultValue);
        }

        private static ConfigEntry<T> BindRdwDevConfig<T>(string name, string description, T defaultValue)
        {
            return BindItem(_currentContext, SECTION_RUNDEVS, name, description, defaultValue);
        }

        private static ConfigEntry<T> BindLoggingConfig<T>(string name, string description, T defaultValue)
        {
            return BindItem(_currentContext, SECTION_LOGGING, name, description, defaultValue);
        }

        private static ConfigEntry<T> BindDevConfig<T>(string name, string description, T defaultValue)
        {
            return BindItem(_currentContext, SECTION_DEV, name, description, defaultValue);
        }

        private static ConfigEntry<T> BindItem<T>(ConfigFile context, string section, string name, string description, T defaultValue)
        {
            return context.Bind(new ConfigDefinition(section, name), defaultValue, new ConfigDescription(description));
        }
    }
}