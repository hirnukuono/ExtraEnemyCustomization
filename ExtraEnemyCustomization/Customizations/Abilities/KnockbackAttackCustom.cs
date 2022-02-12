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
            if (inflictor is not null && inflictor.Type == AgentType.Enemy)
            {
                var enemy = inflictor.Cast<EnemyAgent>();
                if (IsTarget(enemy))
                {
                    MeleeData.DoKnockback(enemy, player);
                }
            }
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            if (inflictor is not null && inflictor.Type == AgentType.Enemy)
            {
                var enemy = inflictor.Cast<EnemyAgent>();
                if (IsTarget(enemy))
                {
                    TentacleData.DoKnockback(enemy, player);
                }
            }
        }
    }
}