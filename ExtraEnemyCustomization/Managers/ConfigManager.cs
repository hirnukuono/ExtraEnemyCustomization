using EECustom.Configs;
using EECustom.Configs.Customizations;
using EECustom.Events;
using EECustom.Utils;
using EECustom.Utils.Integrations;
using Enemies;
using GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EECustom.Managers
{
    public partial class ConfigManager
    {
        public static bool UseLiveEdit { get; set; }
        public static bool LinkMTFOHotReload { get; set; }
        public static string BasePath => EntryPoint.BasePath;
        public static ConfigManager Current { get; private set; }

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
                watcher.Changed += new FileSystemEventHandler(OnConfigFileEdited);
                watcher.EnableRaisingEvents = true;
            }

            LevelEvents.LevelCleanup += ClearTargetLookup;
        }

        private static void OnConfigFileEdited(object sender, FileSystemEventArgs e)
        {
            var filename = Path.GetFileNameWithoutExtension(e.Name);
            if (_configFileNameToType.TryGetValue(filename.ToUpper(), out var type))
            {
                filename = _configTypeToFileName[type];

                Logger.Log($"Config File Changed: {filename}");

                ReloadConfig();

                //TODO: Implement Reload Individual File

                /*
                _configInstances[filename].Unloaded();
                if (_configInstances[filename] is CustomizationConfig custom)
                {
                    var settings = custom.GetAllSettings();
                    foreach (var setting in settings)
                    {
                        setting.OnConfigUnloaded();
                    }
                }
                LoadConfig(type);
                Current.GenerateBuffer();
                */
            }
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

        private static void ClearTargetLookup()
        {
            if (Current == null)
                return;

            foreach (var custom in Current._customizationBuffer)
            {
                custom.ClearTargetLookup();
            }
        }

        internal static void ReloadConfig()
        {
            Logger.Log("HOT RELOADING CONFIG!");

            UnloadAllConfig(doClear: true);
            LoadAllConfig();
            Current.GenerateBuffer();

            foreach (var block in GameDataBlockBase<EnemyDataBlock>.GetAllBlocks())
            {
                var prefab = EnemyPrefabManager.GetEnemyPrefab(block.persistentID);
                if (prefab == null)
                    continue;

                var agent = prefab.GetComponent<EnemyAgent>();
                if (agent == null)
                    continue;

                Current.FirePrefabBuiltEvent(agent);
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