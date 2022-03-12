﻿using EECustom.Configs.Customizations;
using EECustom.Customizations;
using GameData;
using System.Collections.Generic;
using System.Linq;

namespace EECustom.Managers
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

        internal static void RegisterTargetEnemyLookup(EnemyDataBlock enemy)
        {
            foreach (var custom in _customizationBuffer)
            {
                custom.RegisterTargetEnemyLookup(enemy);
            }
        }
    }
}