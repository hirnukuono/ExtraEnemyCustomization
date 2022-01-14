using EECustom.Customizations;
using EECustom.Customizations.Detections;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public class DetectionCustomConfig : CustomizationConfig
    {
        public ScreamingCustom[] ScreamingCustom { get; set; } = new ScreamingCustom[0];
        public FeelerCustom[] FeelerCustom { get; set; } = new FeelerCustom[0];
        public ScoutAnimCustom[] ScoutAnimCustom { get; set; } = new ScoutAnimCustom[0];

        public override EnemyCustomBase[] GetAllSettings()
        {
            var list = new List<EnemyCustomBase>();
            list.AddRange(ScreamingCustom);
            list.AddRange(FeelerCustom);
            list.AddRange(ScoutAnimCustom);
            return list.ToArray();
        }
    }
}