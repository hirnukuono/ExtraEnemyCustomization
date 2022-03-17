using EECustom.EnemyCustomizations.Properties;
using System;

namespace EECustom.Configs.Customizations
{
    public sealed class PropertyCustomConfig : CustomizationConfig
    {
        public SpawnCostCustom[] SpawnCostCustom { get; set; } = Array.Empty<SpawnCostCustom>();
        public EventsCustom[] EventsCustom { get; set; } = Array.Empty<EventsCustom>();
        public DistantRoarCustom[] DistantRoarCustom { get; set; } = Array.Empty<DistantRoarCustom>();

        public override string FileName => "Property";
        public override CustomizationConfigType Type => CustomizationConfigType.Property;
    }
}