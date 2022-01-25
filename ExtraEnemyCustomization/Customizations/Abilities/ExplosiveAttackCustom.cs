using Agents;
using EECustom.Customizations.Shared;
using EECustom.Events;
using EECustom.Utils;
using EECustom.Utils.JsonElements;
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
            if (IsTarget(inflictor.GlobalID))
            {
                MeleeData.DoExplode(player);

                if (!MeleeData.KillInflictor)
                    return;

                if (inflictor.Type != AgentType.Enemy)
                    return;

                var enemyAgent = inflictor as EnemyAgent;
                enemyAgent.Damage.ExplosionDamage(enemyAgent.Damage.HealthMax, Vector3.zero, Vector3.zero);
            }
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            if (IsTarget(inflictor.GlobalID))
            {
                TentacleData.DoExplode(player);

                if (!TentacleData.KillInflictor)
                    return;

                if (inflictor.Type != AgentType.Enemy)
                    return;

                var enemyAgent = inflictor as EnemyAgent;
                enemyAgent.Damage.ExplosionDamage(enemyAgent.Damage.HealthMax, Vector3.zero, Vector3.zero);
            }
        }
    }
}