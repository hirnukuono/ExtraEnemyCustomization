using EECustom.Customizations.SpawnCost;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Configs.Customizations
{
    public sealed class PropertyCustomConfig : CustomizationConfig
    {
        public SpawnCostCustom[] SpawnCostCustom { get; set; } = Array.Empty<SpawnCostCustom>();

        public override string FileName => "Property";
        public override CustomizationConfigType Type => CustomizationConfigType.Property;
    }
}
