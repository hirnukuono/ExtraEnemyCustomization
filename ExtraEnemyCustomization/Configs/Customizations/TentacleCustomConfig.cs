using EEC.CustomSettings.CustomTentacles;
using EEC.EnemyCustomizations.Strikers;
using System;

namespace EEC.Configs.Customizations
{
    public sealed class TentacleCustomConfig : CustomizationConfig
    {
        public StrikerTentacleCustom[] StrikerTentacleCustom { get; set; } = Array.Empty<StrikerTentacleCustom>();
        public CustomTentacle[] TentacleDefinitions { get; set; } = Array.Empty<CustomTentacle>();

        public override string FileName => "Tentacle";
        public override CustomizationConfigType Type => CustomizationConfigType.Tentacle;
    }
}