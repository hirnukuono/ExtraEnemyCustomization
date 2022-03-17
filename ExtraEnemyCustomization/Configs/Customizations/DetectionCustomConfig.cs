using EECustom.EnemyCustomizations.Detections;
using System;

namespace EECustom.Configs.Customizations
{
    public sealed class DetectionCustomConfig : CustomizationConfig
    {
        public ScreamingCustom[] ScreamingCustom { get; set; } = Array.Empty<ScreamingCustom>();
        public FeelerCustom[] FeelerCustom { get; set; } = Array.Empty<FeelerCustom>();
        public ScoutAnimCustom[] ScoutAnimCustom { get; set; } = Array.Empty<ScoutAnimCustom>();

        public override string FileName => "Detection";
        public override CustomizationConfigType Type => CustomizationConfigType.Detection;
    }
}