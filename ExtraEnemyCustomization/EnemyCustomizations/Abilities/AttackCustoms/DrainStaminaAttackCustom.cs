using EEC.EnemyCustomizations.Shared;
using Enemies;
using Player;

namespace EEC.EnemyCustomizations.Abilities
{
    public sealed class DrainStaminaAttackCustom : AttackCustomBase<DrainStaminaSetting>
    {
        public override bool DisableProjectileDamageEvent => false;

        public override string GetProcessName()
        {
            return "DrainStamina";
        }

        protected override void OnApplyEffect(DrainStaminaSetting setting, PlayerAgent player, EnemyAgent inflictor)
        {
            setting.DoDrain(player);
        }

        protected override void OnApplyProjectileEffect(DrainStaminaSetting setting, PlayerAgent player, EnemyAgent inflictor, ProjectileBase projectile)
        {
            OnApplyEffect(setting, player, inflictor);
        }
    }
}