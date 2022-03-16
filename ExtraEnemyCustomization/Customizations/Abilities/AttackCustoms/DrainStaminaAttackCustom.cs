using EECustom.Customizations.Shared;
using Enemies;
using Player;

namespace EECustom.Customizations.Abilities
{
    public sealed class DrainStaminaAttackCustom : AttackCustomBase<DrainStaminaSetting>
    {
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