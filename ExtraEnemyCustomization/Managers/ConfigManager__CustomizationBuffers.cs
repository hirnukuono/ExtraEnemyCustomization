using EEC.Configs.Customizations;
using EEC.EnemyCustomizations;
using GameData;
using System.Collections.Generic;
using System.Linq;

namespace EEC.Managers
{
    public static partial class ConfigManager
    {
        public static IEnumerable<EnemyCustomBase> CustomizationBuffer => _customizationBuffer;

        private static readonly List<EnemyCustomBase> _customizationBuffer = new();

        private static void GenerateBuffer()
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
                if (Logger.DevLogAllowed)
                    custom.LogDev("Initialized:");
                if (Logger.VerboseLogAllowed)
                    custom.LogVerbose(custom.Target.ToDebugString());
            }

            GenerateEventBuffer();
        }

        private static void RegisterTargetEnemyLookup(EnemyDataBlock enemy)
        {
            foreach (var custom in _customizationBuffer)
            {
                custom.RegisterTargetEnemyLookup(enemy);
            }
        }

        private static void TargetEnemyLookupFullyBuilt()
        {
            foreach (var custom in _customizationBuffer)
            {
                custom.OnTargetIDLookupBuilt();
            }
        }
    }
}