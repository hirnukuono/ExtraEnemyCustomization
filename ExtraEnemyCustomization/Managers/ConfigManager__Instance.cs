using EECustom.Configs;
using EECustom.Configs.Customizations;
using EECustom.Customizations;
using Enemies;
using GameData;
using System.Collections.Generic;
using System.Linq;

namespace EECustom.Managers
{
    public partial class ConfigManager
    {
        private readonly List<EnemyCustomBase> _customizationBuffer = new();

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

        internal void RegisterTargetEnemyLookup(EnemyDataBlock enemy)
        {
            foreach (var custom in _customizationBuffer)
            {
                custom.RegisterTargetEnemyLookup(enemy);
            }
        }
    }
}