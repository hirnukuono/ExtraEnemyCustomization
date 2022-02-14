using EECustom.Customizations;
using EECustom.Customizations.SpawnCost;
using System;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public sealed class SpawnCostCustomConfig : CustomizationConfig
    {
        public SpawnCostCustom[] SpawnCostCustom { get; set; } = Array.Empty<SpawnCostCustom>();

        public override string FileName => "SpawnCost";
        public override CustomizationConfigType Type => CustomizationConfigType.SpawnCost;
    }
}