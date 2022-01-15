using EECustom.Configs;
using EECustom.Configs.Customizations;
using EECustom.Customizations;
using Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EECustom.Managers
{
    public partial class ConfigManager
    {
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
