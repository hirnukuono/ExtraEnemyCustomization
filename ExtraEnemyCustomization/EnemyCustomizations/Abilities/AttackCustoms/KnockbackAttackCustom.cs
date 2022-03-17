using EECustom.EnemyCustomizations.Shared;
using Enemies;
using Player;

namespace EECustom.EnemyCustomizations.Abilities
{
    public sealed class KnockbackAttackCustom : AttackCustomBase<KnockbackSetting>
    {
        public override bool DisableProjectileDamageEvent => false;

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