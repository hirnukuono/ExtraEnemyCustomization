using Agents;
using EECustom.Customizations.Shared;
using EECustom.Events;
using Enemies;
using Player;
using UnityEngine;

namespace EECustom.Customizations.Abilities
{
    public sealed class ExplosiveAttackCustom : AttackCustomBase<ExplosionSetting>
    {
        public override string GetProcessName()
        {
            return "ExplosiveAttack";
        }

        protected override void OnApplyEffect(ExplosionSetting setting, PlayerAgent player, EnemyAgent inflictor)
        {
            setting.DoExplode(player);

            if (setting.KillInflictor)
            {
                var damage = inflictor.Damage;
                damage.ExplosionDamage(damage.HealthMax, Vector3.zero, Vector3.zero);
            }
        }
    }
}