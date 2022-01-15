using EECustom.Customizations;
using EECustom.Customizations.Shooters;
using EECustom.CustomSettings.DTO;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public class ProjectileCustomConfig : CustomizationConfig
    {
        public ShooterFireCustom[] ShooterFireCustom { get; set; } = new ShooterFireCustom[0];
        public CustomProjectile[] ProjectileDefinitions { get; set; } = new CustomProjectile[0];

        public override CustomizationConfigType Type => CustomizationConfigType.Projectile;

        public override EnemyCustomBase[] GetAllSettings()
        {
            var list = new List<EnemyCustomBase>();
            list.AddRange(ShooterFireCustom);
            return list.ToArray();
        }
    }
}