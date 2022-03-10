using EECustom.Customizations.Properties;
using System;

namespace EECustom.Configs.Customizations
{
    public sealed class PropertyCustomConfig : CustomizationConfig
    {
        public SpawnCostCustom[] SpawnCostCustom { get; set; } = Array.Empty<SpawnCostCustom>();
        public EventsCustom[] EventsCustom { get; set; } = Array.Empty<EventsCustom>();

        public override string FileName => "Property";
        public override CustomizationConfigType Type => CustomizationConfigType.Property;
    }
}