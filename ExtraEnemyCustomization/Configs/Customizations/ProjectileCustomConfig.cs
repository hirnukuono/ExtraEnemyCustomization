using EECustom.Customizations;
using EECustom.Customizations.Shooters;
using EECustom.CustomSettings;
using EECustom.CustomSettings.DTO;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public sealed class ProjectileCustomConfig : CustomizationConfig
    {
        public ShooterFireCustom[] ShooterFireCustom { get; set; } = new ShooterFireCustom[0];
        public CustomProjectile[] ProjectileDefinitions { get; set; } = new CustomProjectile[0];

        public override string FileName => "Projectile";
        public override CustomizationConfigType Type => CustomizationConfigType.Projectile;

        public override void Loaded()
        {
#warning Regenerating Projectiles Somehow Crash the Game
            return;

            if (!CustomProjectileManager.AssetLoaded)
                return;

            foreach (var proj in ProjectileDefinitions)
            {
                CustomProjectileManager.GenerateProjectile(proj);
            }
        }

        public override void Unloaded()
        {
#warning Regenerating Projectiles Somehow Crash the Game
            return;

            CustomProjectileManager.DestroyAllProjectile();
        }

        public override EnemyCustomBase[] GetAllSettings()
        {
            var list = new List<EnemyCustomBase>();
            list.AddRange(ShooterFireCustom);
            return list.ToArray();
        }
    }
}