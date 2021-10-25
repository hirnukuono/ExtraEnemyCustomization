using EECustom.Configs;
using EECustom.Configs.Customizations;
using EECustom.Customizations;
using EECustom.CustomSettings;
using EECustom.Utils;
using EECustom.Utils.Integrations;
using Enemies;
using System;
using System.Collections.Generic;
using System.IO;

namespace EECustom.Managers
{
    public class ConfigManager
    {
        public static string BasePath { get; private set; }

        public static void Initialize()
        {
            Current = new ConfigManager();

            if (MTFOUtil.IsLoaded && MTFOUtil.HasCustomContent)
            {
                try
                {
                    BasePath = Path.Combine(MTFOUtil.CustomPath, "ExtraEnemyCustomization");

                    Logger.Debug("Loading Category.json...");
                    if (TryLoadConfig(BasePath, "Category.json", out CategoryConfig categoryConfig))
                    {
                        Current.Categories = categoryConfig;
                        Current.Categories.Cache();
                    }

                    Logger.Debug("Loading ScoutWave.json");
                    if (TryLoadConfig(BasePath, "ScoutWave.json", out ScoutWaveConfig scoutWaveConfig))
                    {
                        CustomScoutWaveManager.AddScoutSetting(scoutWaveConfig.Expeditions);
                        CustomScoutWaveManager.AddTargetSetting(scoutWaveConfig.TargetSettings);
                        CustomScoutWaveManager.AddWaveSetting(scoutWaveConfig.WaveSettings);
                    }

                    Logger.Debug("Loading Model.json...");
                    if (TryLoadConfig(BasePath, "Model.json", out ModelCustomConfig modelConfig))
                        Current.ModelCustom = modelConfig;

                    Logger.Debug("Loading Ability.json...");
                    if (TryLoadConfig(BasePath, "Ability.json", out AbilityCustomConfig abilityConfig))
                        Current.AbilityCustom = abilityConfig;

                    Logger.Debug("Loading Projectile.json...");
                    if (TryLoadConfig(BasePath, "Projectile.json", out ProjectileCustomConfig projConfig))
                        Current.ProjectileCustom = projConfig;

                    Logger.Debug("Loading Tentacle.json...");
                    if (TryLoadConfig(BasePath, "Tentacle.json", out TentacleCustomConfig tentacleConfig))
                        Current.TentacleCustom = tentacleConfig;

                    Logger.Debug("Loading Detection.json...");
                    if (TryLoadConfig(BasePath, "Detection.json", out DetectionCustomConfig detectionConfig))
                        Current.DetectionCustom = detectionConfig;

                    Logger.Debug("Loading SpawnCost.json...");
                    if (TryLoadConfig(BasePath, "SpawnCost.json", out SpawnCostCustomConfig spawnCostConfig))
                        Current.SpawnCostCustom = spawnCostConfig;
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

            Current.GenerateBuffer();
        }

        public static bool TryLoadConfig<T>(string basePath, string fileName, out T config)
        {
            var path = Path.Combine(basePath, fileName);
            if (File.Exists(path))
            {
                try
                {
                    config = JSON.Deserialize<T>(File.ReadAllText(path));
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error($"Exception Occured While reading {path} file: {e}");
                    config = default;
                    return false;
                }
            }
            else
            {
                Logger.Warning($"File: {path} is not exist, ignoring this config...");
                config = default;
                return false;
            }
        }

        public static ConfigManager Current { get; private set; }

        public CategoryConfig Categories { get; private set; } = new CategoryConfig();
        public ModelCustomConfig ModelCustom { get; private set; } = new ModelCustomConfig();
        public AbilityCustomConfig AbilityCustom { get; private set; } = new AbilityCustomConfig();
        public ProjectileCustomConfig ProjectileCustom { get; private set; } = new ProjectileCustomConfig();
        public TentacleCustomConfig TentacleCustom { get; private set; } = new TentacleCustomConfig();
        public DetectionCustomConfig DetectionCustom { get; private set; } = new DetectionCustomConfig();
        public SpawnCostCustomConfig SpawnCostCustom { get; private set; } = new SpawnCostCustomConfig();

        private readonly List<EnemyCustomBase> _CustomizationBuffer = new List<EnemyCustomBase>();

        private void GenerateBuffer()
        {
            _CustomizationBuffer.Clear();
            _CustomizationBuffer.AddRange(ModelCustom.GetAllSettings());
            _CustomizationBuffer.AddRange(AbilityCustom.GetAllSettings());
            _CustomizationBuffer.AddRange(ProjectileCustom.GetAllSettings());
            _CustomizationBuffer.AddRange(TentacleCustom.GetAllSettings());
            _CustomizationBuffer.AddRange(DetectionCustom.GetAllSettings());
            _CustomizationBuffer.AddRange(SpawnCostCustom.GetAllSettings());
            foreach (var custom in _CustomizationBuffer)
            {
                custom.OnConfigLoaded();
                custom.LogDev("Initialized:");
                custom.LogVerbose(custom.Target.ToDebugString());
            }
        }

        public void FirePrefabBuiltEvent(EnemyAgent agent)
        {
            foreach (var custom in _CustomizationBuffer)
            {
                if (!custom.Enabled)
                    continue;

                if (custom is IEnemyPrefabBuiltEvent builtEvent && custom.Target.IsMatch(agent))
                {
                    custom.LogDev($"Apply PrefabBuilt Event: {agent.name}");
                    builtEvent.OnPrefabBuilt(agent);
                    custom.LogVerbose($"Finished!");
                }
            }
        }

        public void FireSpawnedEvent(EnemyAgent agent)
        {
            foreach (var custom in _CustomizationBuffer)
            {
                if (!custom.Enabled)
                    continue;

                if (custom is IEnemySpawnedEvent spawnedEvent && custom.Target.IsMatch(agent))
                {
                    custom.LogDev($"Apply Spawned Event: {agent.name}");
                    spawnedEvent.OnSpawned(agent);
                    custom.LogVerbose($"Finished!");
                }
            }
        }

        public void FireDespawnedEvent(EnemyAgent agent)
        {
            foreach (var custom in _CustomizationBuffer)
            {
                if (!custom.Enabled)
                    continue;

                if (custom is IEnemyDespawnedEvent despawnedEvent && custom.Target.IsMatch(agent))
                {
                    custom.LogDev($"Apply Despawned Event: {agent.name}");
                    despawnedEvent.OnDespawned(agent);
                    custom.LogVerbose($"Finished!");
                }
            }
        }
    }
}