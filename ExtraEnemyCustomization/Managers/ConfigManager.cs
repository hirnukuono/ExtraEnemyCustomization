using EECustom.Configs;
using EECustom.Configs.Customizations;
using EECustom.Customizations;
using EECustom.Customizations.EnemyAbilities;
using EECustom.CustomSettings;
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
        public static string BasePath { get; private set; }

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

        private static readonly Dictionary<Type, string> _configFileNameLookup = new();
        private static readonly Dictionary<string, Config> _configInstanceLookup = new();

        internal static void Initialize()
        {
            foreach (var configType in _configTypes)
            {
                var instance = Activator.CreateInstance(configType) as Config;
                _configFileNameLookup[configType] = instance.FileName;
                _configInstanceLookup[instance.FileName] = instance;
            }

            LevelEvents.LevelCleanup += ClearTargetLookup;

            Current = new();

            LoadConfigs();
            Current.GenerateBuffer();
        }

        internal static void DumpDefault()
        {

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

            UnloadConfig(doClear: true);
            LoadConfigs();
            Current.GenerateBuffer();

            //Rebuild Projectile
            foreach (var proj in Current.ProjectileCustom.ProjectileDefinitions)
            {
                CustomProjectileManager.GenerateProjectile(proj);
            }

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

        internal static void UnloadConfig(bool doClear)
        {
            foreach (var config in Current._customizationBuffer)
            {
                config.OnConfigUnloaded();
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

            foreach (var item in _configInstanceLookup)
            {
                _configInstanceLookup[item.Key] = Activator.CreateInstance(item.Value.GetType()) as Config;
            }

            CustomProjectileManager.DestroyAllProjectile();
            CustomScoutWaveManager.ClearAll();
            EnemyAbilityManager.Clear();
        }

        private static void LoadConfigs()
        {
            if (MTFOUtil.IsLoaded && MTFOUtil.HasCustomContent)
            {
                try
                {
                    BasePath = Path.Combine(MTFOUtil.CustomPath, "ExtraEnemyCustomization");

                    foreach (var configType in _configTypes)
                    {
                        if (!_configFileNameLookup.TryGetValue(configType, out var name))
                        {
                            continue;
                        }

                        var fileName = $"{name}.json";
                        var filePath = Path.Combine(BasePath, fileName);
                        Logger.Debug($"Loading {fileName}...");
                        Logger.Verbose($" - Full Path: {filePath}");

                        if (!TryLoadConfig(filePath, configType, out var config))
                        {
                            continue;
                        }

                        _configInstanceLookup[name] = config;

                        switch (config)
                        {
                            case ScoutWaveConfig scoutWaveConfig:
                                CustomScoutWaveManager.ClearAll();
                                CustomScoutWaveManager.AddScoutSetting(scoutWaveConfig.Expeditions);
                                CustomScoutWaveManager.AddTargetSetting(scoutWaveConfig.TargetSettings);
                                CustomScoutWaveManager.AddWaveSetting(scoutWaveConfig.WaveSettings);
                                break;

                            case CategoryConfig:
                                Current.Categories.Cache();
                                break;

                            case EnemyAbilityCustomConfig:
                                Current.EnemyAbilityCustom.Abilities.RegisterAll();
                                EnemyAbilityManager.Setup();
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"Error Occured While reading ExtraEnemyCustomization.json file: {e}");
                }
            }
            else
            {
                Logger.Warning("No Custom content were found, No Customization will be applied");
            }
        }

        internal static bool TryLoadConfig(string filePath, Type type, out Config config)
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

        public static ConfigManager Current { get; private set; }

        public GlobalConfig Global { get => GetConfig<GlobalConfig>(); }
        public CategoryConfig Categories { get => GetConfig<CategoryConfig>(); }
        public ModelCustomConfig ModelCustom { get => GetConfig<ModelCustomConfig>(); }
        public AbilityCustomConfig AbilityCustom { get => GetConfig<AbilityCustomConfig>(); }
        public ProjectileCustomConfig ProjectileCustom { get => GetConfig<ProjectileCustomConfig>(); }
        public TentacleCustomConfig TentacleCustom { get => GetConfig<TentacleCustomConfig>(); }
        public DetectionCustomConfig DetectionCustom { get => GetConfig<DetectionCustomConfig>(); }
        public SpawnCostCustomConfig SpawnCostCustom { get => GetConfig<SpawnCostCustomConfig>(); }
        public EnemyAbilityCustomConfig EnemyAbilityCustom { get => GetConfig<EnemyAbilityCustomConfig>(); }


        private readonly List<EnemyCustomBase> _customizationBuffer = new();

        private T GetConfig<T>() where T : Config
        {
            if (_configFileNameLookup.TryGetValue(typeof(T), out var filename))
            {
                if (_configInstanceLookup.TryGetValue(filename, out var config))
                {
                    if (config is T castedConfig)
                        return castedConfig;
                }
            }
            return null;
        }

        private void GenerateBuffer()
        {
            _customizationBuffer.Clear();
            var settingLists = _configInstanceLookup.Values
                .Where(x => x is CustomizationConfig)
                .Select(x => ((CustomizationConfig)x).GetAllSettings());

            foreach (var settings in settingLists)
            {
                _customizationBuffer.AddRange(settings);
            }

            _customizationBuffer.RemoveAll(x => !x.Enabled); //Remove Disabled Items
            foreach (var custom in _customizationBuffer)
            {
                custom.OnConfigLoaded();
                custom.LogDev("Initialized:");
                custom.LogVerbose(custom.Target.ToDebugString());
            }

            GenerateEventBuffer();
        }

        internal void RegisterTargetLookup(EnemyAgent agent)
        {
            foreach (var custom in _customizationBuffer)
            {
                custom.RegisterTargetLookup(agent);
            }
        }
    }
}