using Agents;
using Enemies;

namespace EECustom.Events
{
    public delegate void EnemyTakeDamageHandler(EnemyAgent enemyAgent, Agent inflictor, float damage);
    public static class EnemyDamageEvents
    {
        public static event EnemyTakeDamageHandler Damage;
        public static event EnemyTakeDamageHandler MeleeDamage;
        public static event EnemyTakeDamageHandler BulletDamage;
        public static event EnemyTakeDamageHandler ExplosionDamage;

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
    }
}