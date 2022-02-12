﻿using BepInEx.Configuration;
using EECustom.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom
{
    public static class Configuration
    {
        public const string SECTION_GENERAL = "General";
        public const string SECTION_LOGGING = "Logging";
        public const string SECTION_DEV = "Developer";

        //GENERAL CONFIGS
        public static ConfigEntry<bool> UseLiveEdit { get; private set; }
        public static ConfigEntry<bool> LinkMTFOHotReload { get; private set; }

        //LOGGING CONFIGS
        public static ConfigEntry<bool> UseDebugLog { get; private set; }
        public static ConfigEntry<bool> UseVerboseLog { get; private set; }
        public static ConfigEntry<AssetCacheManager.OutputType> AssetCacheBehaviour { get; private set; }

        //DEVELOPER CONFIGS
        public static ConfigEntry<bool> DumpConfig { get; private set; }


        private static ConfigFile _currentContext;

        public static void BindAll(ConfigFile context)
        {
            _currentContext = context;

            UseLiveEdit = BindGeneralConfig("Live Edit", "Reload Config when they are edited while in-game", false);
            LinkMTFOHotReload = BindGeneralConfig("Reload on MTFO HotReload", "Reload Configs when MTFO's HotReload button has pressed?", true);

            UseDebugLog = BindLoggingConfig("UseDevMessage", "Using Dev Message for Debugging your config?", false);
            UseVerboseLog = BindLoggingConfig("Verbose", "Using Much more detailed Message for Debugging?", false);
            AssetCacheBehaviour = BindLoggingConfig("Cached Asset Result Output", "How does your cached material/texture result be returned?", AssetCacheManager.OutputType.None);

            DumpConfig = BindDevConfig("DumpConfig", "Dump Empty Config file?", false);
        }

        private static ConfigEntry<T> BindGeneralConfig<T>(string name, string description, T defaultValue)
        {
            return BindItem(_currentContext, SECTION_GENERAL, name, description, defaultValue);
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