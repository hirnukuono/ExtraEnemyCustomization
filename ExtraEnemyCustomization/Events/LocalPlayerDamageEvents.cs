using Agents;
using Player;

namespace EECustom.Events
{
    public delegate void PlayerTakeDamageHandler(PlayerAgent playerAgent, Agent inflictor, float damage);

    public delegate void PlayerTakeDamageFromProjectileHandler(PlayerAgent playerAgent, Agent inflictor, ProjectileBase projectile, float damage);

    public static class LocalPlayerDamageEvents
    {
        public static event PlayerTakeDamageHandler Damage;

        public static event PlayerTakeDamageHandler MeleeDamage;

        public static event PlayerTakeDamageHandler TentacleDamage;

        public static event PlayerTakeDamageFromProjectileHandler ProjectileDamage;

        internal static void OnDamage(PlayerAgent playerAgent, Agent inflictor, float damage)
        {
            Damage?.Invoke(playerAgent, inflictor, damage);
        }

        internal static void OnMeleeDamage(PlayerAgent playerAgent, Agent inflictor, float damage)
        {
            MeleeDamage?.Invoke(playerAgent, inflictor, damage);
        }

        internal static void OnTentacleDamage(PlayerAgent playerAgent, Agent inflictor, float damage)
        {
            TentacleDamage?.Invoke(playerAgent, inflictor, damage);
        }

        internal static void OnProjectileDamage(PlayerAgent playerAgent, Agent inflictor, ProjectileBase projectile, float damage)
        {
            ProjectileDamage?.Invoke(playerAgent, inflictor, projectile, damage);
        }
    }
}