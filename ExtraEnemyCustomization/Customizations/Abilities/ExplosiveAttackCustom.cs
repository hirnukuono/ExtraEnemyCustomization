using Agents;
using EECustom.Customizations.Shared;
using EECustom.Events;
using Enemies;
using Player;
using UnityEngine;

namespace EECustom.Customizations.Abilities
{
    public sealed class ExplosiveAttackCustom : EnemyCustomBase
    {
        public ExplosionSetting MeleeData { get; set; } = new();
        public ExplosionSetting TentacleData { get; set; } = new();

        public override string GetProcessName()
        {
            return "ExplosiveAttack";
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
                    return;

                MeleeData.DoExplode(player);

                if (!MeleeData.KillInflictor)
                    return;

                if (inflictor.Type != AgentType.Enemy)
                    return;

                enemy.Damage.ExplosionDamage(enemy.Damage.HealthMax, Vector3.zero, Vector3.zero);
            }
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            if (inflictor is not null && inflictor.Type == AgentType.Enemy)
            {
                var enemy = inflictor.Cast<EnemyAgent>();
                if (!IsTarget(enemy))
                    return;

                TentacleData.DoExplode(player);

                if (!TentacleData.KillInflictor)
                    return;

                if (inflictor.Type != AgentType.Enemy)
                    return;

                enemy.Damage.ExplosionDamage(enemy.Damage.HealthMax, Vector3.zero, Vector3.zero);
            }
        }
    }
}