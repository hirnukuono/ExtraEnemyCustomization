using EECustom.Customizations;
using EECustom.Customizations.Properties;
using System;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public sealed class SpawnCostCustomConfig : CustomizationConfig
    {
        public SpawnCostCustom[] SpawnCostCustom { get; set; } = Array.Empty<SpawnCostCustom>();

        public override string FileName => "SpawnCost";
        public override CustomizationConfigType Type => CustomizationConfigType.Property;

        public override void Loaded()
        {
            Logger.Error("SpawnCost.json(c) will no longer avilable after 2.x.x version");
            Logger.Error(" - Suggestion: Migrate to Property.json(c)");
        }
    }
}