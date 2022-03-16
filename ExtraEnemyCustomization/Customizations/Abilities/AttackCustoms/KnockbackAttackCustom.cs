using EECustom.Customizations.Shared;
using Enemies;
using Player;

namespace EECustom.Customizations.Abilities
{
    public sealed class KnockbackAttackCustom : AttackCustomBase<KnockbackSetting>
    {
        public override string GetProcessName()
        {
            return "KnockbackAttack";
        }

        protected override void OnApplyEffect(KnockbackSetting setting, PlayerAgent player, EnemyAgent inflictor)
        {
            setting.DoKnockback(inflictor, player);
        }

        protected override void OnApplyProjectileEffect(KnockbackSetting setting, PlayerAgent player, EnemyAgent inflictor, ProjectileBase projectile)
        {
            OnApplyEffect(setting, player, inflictor);
        }
    }
}