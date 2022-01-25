using EECustom.Configs;
using EECustom.Configs.Customizations;
using EECustom.Customizations;
using Enemies;
using System.Collections.Generic;
using System.Linq;

namespace EECustom.Managers
{
    public partial class ConfigManager
    {
        public GlobalConfig Global => GetConfig<GlobalConfig>();
        public CategoryConfig Categories => GetConfig<CategoryConfig>();
        public ModelCustomConfig ModelCustom => GetConfig<ModelCustomConfig>();
        public AbilityCustomConfig AbilityCustom => GetConfig<AbilityCustomConfig>();
        public ProjectileCustomConfig ProjectileCustom => GetConfig<ProjectileCustomConfig>();
        public TentacleCustomConfig TentacleCustom => GetConfig<TentacleCustomConfig>();
        public DetectionCustomConfig DetectionCustom => GetConfig<DetectionCustomConfig>();
        public SpawnCostCustomConfig SpawnCostCustom => GetConfig<SpawnCostCustomConfig>();
        public EnemyAbilityCustomConfig EnemyAbilityCustom => GetConfig<EnemyAbilityCustomConfig>();

        private readonly List<EnemyCustomBase> _customizationBuffer = new();

        private T GetConfig<T>() where T : Config
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

        private void GenerateBuffer()
        {
            _customizationBuffer.Clear();
            var settingLists = _configInstances.Values
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