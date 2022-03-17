using Agents;
using EECustom.Events;
using Enemies;
using Player;
using System.Text.Json.Serialization;

namespace EECustom.Customizations.Abilities
{
    public abstract class AttackCustomBase<T> : EnemyCustomBase where T : class, new()
    {
        [JsonIgnore] public abstract bool DisableProjectileDamageEvent { get; }
        public T MeleeData { get; set; } = new();
        public T TentacleData { get; set; } = new();
        public T ProjectileData { get; set; } = new();

        public override void OnConfigLoaded()
        {
            LocalPlayerDamageEvents.MeleeDamage += OnMelee;
            LocalPlayerDamageEvents.TentacleDamage += OnTentacle;
            if (!DisableProjectileDamageEvent)
                LocalPlayerDamageEvents.ProjectileDamage += OnProjectile;
        }

        public override void OnConfigUnloaded()
        {
            LocalPlayerDamageEvents.MeleeDamage -= OnMelee;
            LocalPlayerDamageEvents.TentacleDamage -= OnTentacle;
            if (!DisableProjectileDamageEvent)
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
            DoProjectile(player, inflictor, projectile, ProjectileData);
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

        private void DoProjectile(PlayerAgent player, Agent inflictor, ProjectileBase projectile, T setting)
        {
            if (inflictor.TryCastToEnemyAgent(out var enemy))
            {
                if (IsTarget(enemy))
                {
                    OnApplyProjectileEffect(setting, player, enemy, projectile);
                }
            }
        }

        protected abstract void OnApplyEffect(T setting, PlayerAgent player, EnemyAgent inflicator);
        protected virtual void OnApplyProjectileEffect(T setting, PlayerAgent player, EnemyAgent inflictor, ProjectileBase projectile)
        {

        }
    }
}