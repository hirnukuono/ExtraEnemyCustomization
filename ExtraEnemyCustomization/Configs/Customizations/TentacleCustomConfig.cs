using EECustom.Customizations;
using EECustom.Customizations.Strikers;
using EECustom.CustomSettings.DTO;
using System;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public sealed class TentacleCustomConfig : CustomizationConfig
    {
        public StrikerTentacleCustom[] StrikerTentacleCustom { get; set; } = Array.Empty<StrikerTentacleCustom>();
        public CustomTentacle[] TentacleDefinitions { get; set; } = Array.Empty<CustomTentacle>();

        public override string FileName => "Tentacle";
        public override CustomizationConfigType Type => CustomizationConfigType.Tentacle;

        public override EnemyCustomBase[] GetAllSettings()
        {
            var list = new List<EnemyCustomBase>();
            list.AddRange(StrikerTentacleCustom);
            return list.ToArray();
        }
    }
}