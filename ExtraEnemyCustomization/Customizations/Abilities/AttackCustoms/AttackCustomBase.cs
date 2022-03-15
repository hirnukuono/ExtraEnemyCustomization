using Agents;
using EECustom.Events;
using Enemies;
using Player;

namespace EECustom.Customizations.Abilities
{
    public abstract class AttackCustomBase<T> : EnemyCustomBase where T : class, new()
    {
        public T MeleeData { get; set; } = new();
        public T TentacleData { get; set; } = new();
        public T ProjectileData { get; set; } = null;

        public override void OnConfigLoaded()
        {
            LocalPlayerDamageEvents.MeleeDamage += OnMelee;
            LocalPlayerDamageEvents.TentacleDamage += OnTentacle;
#warning Remove null check after 2.x.x
            if (ProjectileData != null)
                LocalPlayerDamageEvents.ProjectileDamage += OnProjectile;
        }

        public override void OnConfigUnloaded()
        {
            LocalPlayerDamageEvents.MeleeDamage -= OnMelee;
            LocalPlayerDamageEvents.TentacleDamage -= OnTentacle;
            if (ProjectileData != null)
                LocalPlayerDamageEvents.ProjectileDamage -= OnProjectile;
        }

        private void OnMelee(PlayerAgent player, Agent inflictor, float damage)
        {
            Do(player, inflictor, MeleeData);
        }

        private void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            Do(player, inflictor, TentacleData);
        }

        private void OnProjectile(PlayerAgent player, Agent inflictor, ProjectileBase projectile, float damage)
        {
            Do(player, inflictor, ProjectileData);
        }

        private void Do(PlayerAgent player, Agent inflictor, T setting)
        {
            if (inflictor.TryCastToEnemyAgent(out var enemy))
            {
                if (IsTarget(enemy))
                {
                    OnApplyEffect(setting, player, enemy);
                }
            }
        }

        protected abstract void OnApplyEffect(T setting, PlayerAgent player, EnemyAgent inflicator);
    }
}