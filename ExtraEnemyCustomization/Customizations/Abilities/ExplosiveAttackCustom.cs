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
            DoExplosive(player, inflictor, MeleeData);
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            DoExplosive(player, inflictor, TentacleData);
        }

        private void DoExplosive(PlayerAgent player, Agent inflictor, ExplosionSetting setting)
        {
            if (inflictor.TryCastToEnemyAgent(out var enemy))
            {
                if (!IsTarget(enemy))
                    return;

                setting.DoExplode(player);

                if (!setting.KillInflictor)
                    return;

                if (inflictor.Type != AgentType.Enemy)
                    return;

                enemy.Damage.ExplosionDamage(enemy.Damage.HealthMax, Vector3.zero, Vector3.zero);
            }
        }
    }
}