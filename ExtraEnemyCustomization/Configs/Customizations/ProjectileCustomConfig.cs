using EECustom.Customizations.Shooters;
using EECustom.CustomSettings;
using EECustom.CustomSettings.DTO;
using System;

namespace EECustom.Configs.Customizations
{
    public sealed class ProjectileCustomConfig : CustomizationConfig
    {
        public ShooterFireCustom[] ShooterFireCustom { get; set; } = Array.Empty<ShooterFireCustom>();
        public CustomProjectile[] ProjectileDefinitions { get; set; } = Array.Empty<CustomProjectile>();

        public override string FileName => "Projectile";
        public override CustomizationConfigType Type => CustomizationConfigType.Projectile;

        public override void Loaded()
        {
            if (!CustomProjectileManager.AssetLoaded)
                return;

            foreach (var proj in ProjectileDefinitions)
            {
                CustomProjectileManager.GenerateProjectile(proj);
            }
        }

        public override void Unloaded()
        {
            CustomProjectileManager.DestroyAllProjectile();
        }
    }
}