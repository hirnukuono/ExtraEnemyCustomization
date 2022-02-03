using EECustom.Configs;
using EECustom.Configs.Customizations;
using EECustom.Events;
using EECustom.Utils;
using EECustom.Utils.Integrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EECustom.Managers
{
    public partial class ConfigManager
    {
        public static bool UseLiveEdit { get; private set; }
        public static bool LinkMTFOHotReload { get; private set; }
        public static string BasePath => EntryPoint.BasePath;
        public static ConfigManager Current { get; private set; }

        public static GlobalConfig Global => GetConfig<GlobalConfig>();
        public static CategoryConfig Categories => GetConfig<CategoryConfig>();
        public static ModelCustomConfig ModelCustom => GetConfig<ModelCustomConfig>();
        public static AbilityCustomConfig AbilityCustom => GetConfig<AbilityCustomConfig>();
        public static ProjectileCustomConfig ProjectileCustom => GetConfig<ProjectileCustomConfig>();
        public static TentacleCustomConfig TentacleCustom => GetConfig<TentacleCustomConfig>();
        public static DetectionCustomConfig DetectionCustom => GetConfig<DetectionCustomConfig>();
        public static SpawnCostCustomConfig SpawnCostCustom => GetConfig<SpawnCostCustomConfig>();
        public static EnemyAbilityCustomConfig EnemyAbilityCustom => GetConfig<EnemyAbilityCustomConfig>();

        private static readonly Type[] _configTypes = new Type[]
        {
            //Normal Configs
            typeof(GlobalConfig),
            typeof(CategoryConfig),
            typeof(ScoutWaveConfig),

            //Customization Configs
            typeof(AbilityCustomConfig),
            typeof(DetectionCustomConfig),
            typeof(EnemyAbilityCustomConfig),
            typeof(ModelCustomConfig),
            typeof(ProjectileCustomConfig),
            typeof(SpawnCostCustomConfig),
            typeof(TentacleCustomConfig)
        };

        private static readonly Dictionary<Type, string> _configTypeToFileName = new();
        private static readonly Dictionary<string, Type> _configFileNameToType = new();
        private static readonly Dictionary<string, Config> _configInstances = new();

        internal static void Initialize()
        {
            UseLiveEdit = Configuration.UseLiveEdit.Value;
            LinkMTFOHotReload = Configuration.LinkMTFOHotReload.Value;

            foreach (var configType in _configTypes)
            {
                var instance = Activator.CreateInstance(configType) as Config;
                var fileName = instance.FileName;
                _configTypeToFileName[configType] = fileName;
                _configFileNameToType[fileName] = configType;
                _configFileNameToType[fileName.ToUpper()] = configType; //For Easier Access
                _configInstances[fileName] = instance;
            }

            Current = new();

            LoadAllConfig();
            Current.GenerateBuffer();

            if (LinkMTFOHotReload)
            {
                MTFOUtil.HotReloaded += ReloadConfig;
            }

            if (UseLiveEdit)
            {
                var watcher = new FileSystemWatcher
                {
                    Path = BasePath,
                    IncludeSubdirectories = false,
                    NotifyFilter = NotifyFilters.LastWrite,
                    Filter = "*.json"
                };
                watcher.Changed += new FileSystemEventHandler(OnConfigFileEdited_ReloadConfig);
                watcher.EnableRaisingEvents = true;
            }

            LevelEvents.LevelCleanup += OnLevelCleanup_ClearLookup;
        }

        internal static void DumpDefault()
        {
            var dumpPath = Path.Combine(BasePath, "Dump");
            Directory.CreateDirectory(dumpPath);

            foreach (var item in _configInstances)
            {
                var file = Path.Combine(dumpPath, $"{item.Key}.json");
                File.WriteAllText(file, JSON.Serialize(item.Value.CreateBlankConfig(), item.Value.GetType()));
            }
        }

        internal static void ReloadConfig()
        {
            Logger.Log("HOT RELOADING CONFIG!");

            UnloadAllConfig(doClear: true);
            LoadAllConfig();
            Current.GenerateBuffer();

            FireAssetLoaded();
            Current.FirePrefabBuildEventAll();
        }

        internal static void FireAssetLoaded()
        {
            foreach (var config in Current._customizationBuffer)
            {
                config.OnAssetLoaded();
            }
        }

        internal static void UnloadAllConfig(bool doClear)
        {
            foreach (var config in Current._customizationBuffer)
            {
                config.OnConfigUnloaded();
            }

            foreach (var item in _configInstances)
            {
                item.Value.Unloaded();
            }

            if (doClear)
            {
                ClearConfigs();
            }
        }

        internal static void ClearConfigs()
        {
            if (Current == null)
                return;

            foreach (var item in _configInstances.ToArray())
            {
                _configInstances[item.Key] = Activator.CreateInstance(item.Value.GetType()) as Config;
            }
        }

        internal static void LoadAllConfig()
        {
            if (MTFOUtil.IsLoaded && MTFOUtil.HasCustomContent)
            {
                foreach (var configType in _configTypes)
                {
                    LoadConfig(configType);
                }
            }
            else
            {
                Logger.Warning("No Custom content were found, No Customization will be applied");
            }
        }

        internal static void LoadConfig(Type configType)
        {
            try
            {
                if (!_configTypeToFileName.TryGetValue(configType, out var name))
                {
                    throw new ArgumentOutOfRangeException(nameof(configType));
                }

                var fileName = $"{name}.json";
                var filePath = Path.Combine(BasePath, fileName);
                Logger.Debug($"Loading {fileName}...");
                Logger.Verbose($" - Full Path: {filePath}");

                if (!TryLoadConfigData(filePath, configType, out var config))
                {
                    return;
                }

                _configInstances[name] = config;
                config.Loaded();
            }
            catch (Exception e)
            {
                Logger.Error($"Error Occured While reading Config from type: {configType.Name}\n{e}");
            }
        }

        private static T GetConfig<T>() where T : Config
        {
            if (_configTypeToFileName.TryGetValue(typeof(T), out var filename))
            {
                if (_configInstances.TryGetValue(filename, out var config))
                {
                    if (config is T castedConfig)
                        return castedConfig;
                }
            }
            return null;
        }

        private static bool TryLoadConfigData(string filePath, Type type, out Config config)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    config = JSON.Deserialize(type, File.ReadAllText(filePath)) as Config;
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error($"Exception Occured While reading {filePath} file: {e}");
                    config = default;
                    return false;
                }
            }
            else
            {
                Logger.Warning($"File: {filePath} is not exist, ignoring this config...");
                config = default;
                return false;
            }
        }
    }
}