using Agents;
using EECustom.Customizations.Shared;
using EECustom.Events;
using Enemies;
using Player;

namespace EECustom.Customizations.Abilities
{
    public sealed class KnockbackAttackCustom : EnemyCustomBase
    {
        public KnockbackSetting MeleeData { get; set; } = new();
        public KnockbackSetting TentacleData { get; set; } = new();

        public override string GetProcessName()
        {
            return "KnockbackAttack";
        }

        public override void OnConfigLoaded()
        {
            LocalPlayerDamageEvents.MeleeDamage += OnMelee;
            LocalPlayerDamageEvents.TentacleDamage += OnTentacle;
        }

        public override void OnConfigUnloaded()
        {
            LocalPlayerDamageEvents.MeleeDamage -= OnMelee;
            LocalPlayerDamageEvents.TentacleDamage -= OnTentacle;
        }

        public void OnMelee(PlayerAgent player, Agent inflictor, float damage)
        {
            DoKnockback(player, inflictor, MeleeData);
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            DoKnockback(player, inflictor, TentacleData);
        }

        private void DoKnockback(PlayerAgent player, Agent inflictor, KnockbackSetting setting)
        {
            if (inflictor.TryCastToEnemyAgent(out var enemy))
            {
                if (IsTarget(enemy))
                {
                    setting.DoKnockback(enemy, player);
                }
            }
        }
    }
}