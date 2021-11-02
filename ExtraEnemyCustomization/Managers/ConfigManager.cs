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
    public partial class ConfigManager
    {
        public static string BasePath { get; private set; }

        internal static void Initialize()
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

        internal static bool TryLoadConfig<T>(string basePath, string fileName, out T config)
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

        public CategoryConfig Categories { get; private set; } = new();
        public ModelCustomConfig ModelCustom { get; private set; } = new();
        public AbilityCustomConfig AbilityCustom { get; private set; } = new();
        public ProjectileCustomConfig ProjectileCustom { get; private set; } = new();
        public TentacleCustomConfig TentacleCustom { get; private set; } = new();
        public DetectionCustomConfig DetectionCustom { get; private set; } = new();
        public SpawnCostCustomConfig SpawnCostCustom { get; private set; } = new();

        private readonly List<EnemyCustomBase> _customizationBuffer = new();

        private void GenerateBuffer()
        {
            _customizationBuffer.Clear();
            _customizationBuffer.AddRange(ModelCustom.GetAllSettings());
            _customizationBuffer.AddRange(AbilityCustom.GetAllSettings());
            _customizationBuffer.AddRange(ProjectileCustom.GetAllSettings());
            _customizationBuffer.AddRange(TentacleCustom.GetAllSettings());
            _customizationBuffer.AddRange(DetectionCustom.GetAllSettings());
            _customizationBuffer.AddRange(SpawnCostCustom.GetAllSettings());
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