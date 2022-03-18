using BepInEx;
using BepInEx.Configuration;
using EEC.Managers.Assets;
using System.IO;

namespace EEC
{
    public static class Configuration
    {
        public const string SECTION_USER = "1. User-End";
        public const string SECTION_RUNDEVS = "2. Rundown Developer";
        public const string SECTION_LOGGING = "3. Logging";
        public const string SECTION_DEV = "4. DEBUG";
        public const int CONFIG_VERSION = 1;

        //USER CONFIGS
        public static bool ShowMarkerText { get; private set; }
        public static bool ShowMarkerDistance { get; private set; }
        public static bool ShowExplosionEffect { get; private set; }
        private static ConfigEntry<bool> _showMarkerText;
        private static ConfigEntry<bool> _showMarkerDistance;
        private static ConfigEntry<bool> _showExplosionEffect;

        //RUNDOWN DEVELOPER CONFIGS
        public static bool UseLiveEdit { get; private set; }
        public static bool LinkMTFOHotReload { get; private set; }
        private static ConfigEntry<bool> _useLiveEdit;
        private static ConfigEntry<bool> _linkMTFOHotReload;

        //LOGGING CONFIGS
        public static bool UseDebugLog { get; private set; }
        public static bool UseVerboseLog { get; private set; }
        public static AssetCacheManager.OutputType AssetCacheBehaviour { get; private set; }
        private static ConfigEntry<bool> _useDebugLog;
        private static ConfigEntry<bool> _useVerboseLog;
        private static ConfigEntry<AssetCacheManager.OutputType> _assetCacheBehaviour;

        //DEVELOPER CONFIGS
        public static bool DumpConfig { get; private set; }
        public static bool Profiler { get; private set; }
        private static ConfigEntry<bool> _dumpConfig;
        private static ConfigEntry<bool> _profiler;

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

            _showMarkerText = BindUserConfig("Marker Text", "Display Enemy Marker Texts? (if set by rundown devs)", true);
            _showMarkerDistance = BindUserConfig("Marker Distance", "Display Enemy Marker Distance? (if set by rundown devs)", true);
            _showExplosionEffect = BindUserConfig("Explosion Flash", "(Accessibility) Display Light flash effect for explosion abilities?", true);
            ShowMarkerText = _showMarkerText.Value;
            ShowMarkerDistance = _showMarkerDistance.Value;
            ShowExplosionEffect = _showExplosionEffect.Value;

            _useLiveEdit = BindRdwDevConfig("Live Edit", "Reload Config when they are edited while in-game", false);
            _linkMTFOHotReload = BindRdwDevConfig("Reload on MTFO HotReload", "Reload Configs when MTFO's HotReload button has pressed?", true);
            UseLiveEdit = _useLiveEdit.Value;
            LinkMTFOHotReload = _linkMTFOHotReload.Value;

            _useDebugLog = BindLoggingConfig("UseDevMessage", "Using Dev Message for Debugging your config?", false);
            _useVerboseLog = BindLoggingConfig("Verbose", "Using Much more detailed Message for Debugging?", false);
            _assetCacheBehaviour = BindLoggingConfig("Cached Asset Result Output", "How does your cached material/texture result be returned?", AssetCacheManager.OutputType.None);
            UseDebugLog = _useDebugLog.Value;
            UseVerboseLog = _useVerboseLog.Value;
            AssetCacheBehaviour = _assetCacheBehaviour.Value;

            _dumpConfig = BindDevConfig("DumpConfig", "Dump Empty Config file?", false);
            _profiler = BindDevConfig("Profiler", "Show Profiler Info for Spawned Event", false);
            DumpConfig = _dumpConfig.Value;
            Profiler = _profiler.Value;
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