using EECustom.Customizations;
using EECustom.Customizations.Strikers;
using EECustom.CustomSettings.DTO;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public class TentacleCustomConfig : CustomizationConfig
    {
        public StrikerTentacleCustom[] StrikerTentacleCustom { get; set; } = new StrikerTentacleCustom[0];
        public CustomTentacle[] TentacleDefinitions { get; set; } = new CustomTentacle[0];

        public override CustomizationConfigType Type => CustomizationConfigType.Tentacle;

        public override EnemyCustomBase[] GetAllSettings()
        {
            var list = new List<EnemyCustomBase>();
            list.AddRange(StrikerTentacleCustom);
            return list.ToArray();
        }
    }
}