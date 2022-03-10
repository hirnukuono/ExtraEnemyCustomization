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

        protected override void OnApplyEffect(KnockbackSetting setting, PlayerAgent player, EnemyAgent inflicator)
        {
            setting.DoKnockback(inflicator, player);
        }
    }
}