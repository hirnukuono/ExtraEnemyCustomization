using Agents;
using Enemies;
using System;

namespace EEC.Events
{
    public delegate void EnemyTakeDamageHandler(EnemyAgent enemyAgent, Agent inflictor, float damage);
    public delegate void EnemyHealthUpdateHandler(EnemyAgent enemyAgent, float maxHealth, float health);

    public static class EnemyDamageEvents
    {
        public static event EnemyTakeDamageHandler Damage;

        [Obsolete("Disabled Due to Patching Error", true)]
        public static event EnemyTakeDamageHandler MeleeDamage;

        [Obsolete("Disabled Due to Patching Error", true)]
        public static event EnemyTakeDamageHandler BulletDamage;

        [Obsolete("Disabled Due to Patching Error", true)]
        public static event EnemyTakeDamageHandler ExplosionDamage;

        public static event EnemyHealthUpdateHandler HealthUpdated;

        internal static void OnDamage(EnemyAgent enemyAgent, Agent inflictor, float damage)
        {
            Damage?.Invoke(enemyAgent, inflictor, damage);
        }

        internal static void OnMeleeDamage(EnemyAgent enemyAgent, Agent inflictor, float damage)
        {
            MeleeDamage?.Invoke(enemyAgent, inflictor, damage);
        }

        internal static void OnBulletDamage(EnemyAgent enemyAgent, Agent inflictor, float damage)
        {
            BulletDamage?.Invoke(enemyAgent, inflictor, damage);
        }

        internal static void OnExplosionDamage(EnemyAgent enemyAgent, Agent inflictor, float damage)
        {
            ExplosionDamage?.Invoke(enemyAgent, inflictor, damage);
        }

        internal static void OnHealthUpdated(EnemyAgent enemyAgent, float maxHealth, float health)
        {
            HealthUpdated?.Invoke(enemyAgent, maxHealth, health);
        }
    }
}