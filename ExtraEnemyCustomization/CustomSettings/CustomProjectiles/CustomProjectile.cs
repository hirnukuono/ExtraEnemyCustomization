using EEC.EnemyCustomizations.Shared;
using EEC.Utils.Json.Elements;
using Player;
using System.Text.Json.Serialization;
using UnityEngine;

namespace EEC.CustomSettings.CustomProjectiles
{
    public sealed class CustomProjectile
    {
        public string DebugName { get; set; } = string.Empty;
        public byte ID { get; set; } = 10;
        public ProjectileType BaseProjectile { get; set; } = ProjectileType.TargetingSmall;
        public ValueBase Speed { get; set; } = ValueBase.Unchanged;
        public MultiplierShiftSetting SpeedChange { get; set; } = new();
        public ValueBase CheckEvasiveDistance { get; set; } = ValueBase.Unchanged;
        public ValueBase InitialHomingDuration { get; set; } = ValueBase.Unchanged;
        public ValueBase InitialHomingStrength { get; set; } = ValueBase.Unchanged;
        public ValueBase HomingDelay { get; set; } = ValueBase.Unchanged;
        public ValueBase HomingStrength { get; set; } = ValueBase.Unchanged;
        public MultiplierShiftSetting HomingStrengthChange { get; set; } = new();
        public ValueBase LifeTime { get; set; } = ValueBase.Unchanged;
        public Color TrailColor { get; set; } = Color.yellow;
        public ValueBase TrailTime { get; set; } = ValueBase.Unchanged;
        public ValueBase TrailWidth { get; set; } = ValueBase.Unchanged;
        public Color GlowColor { get; set; } = Color.yellow;
        public ValueBase GlowRange { get; set; } = ValueBase.Unchanged;
        public ValueBase Damage { get; set; } = ValueBase.Unchanged;
        public ValueBase Infection { get; set; } = ValueBase.Unchanged;
        [JsonPropertyName("SpawnProjectileOnCollideWorld")]
        public SpawnProjectileSetting SpawnProjColWorld { get; set; } = new();
        [JsonPropertyName("SpawnProjectileOnCollidePlayer")]
        public SpawnProjectileSetting SpawnProjColPlayer { get; set; } = new();
        [JsonPropertyName("SpawnProjectileOnLifeTimeDone")]
        public SpawnProjectileSetting SpawnProjLifeDone { get; set; } = new();
        public ExplosionSetting Explosion { get; set; } = new();
        public KnockbackSetting Knockback { get; set; } = new();
        public BleedSetting Bleed { get; set; } = new();
        public DrainStaminaSetting DrainStamina { get; set; } = new();

        public void DoCollisionEffect(Vector3 projectilePosition, PlayerAgent player = null)
        {
            if (Explosion?.Enabled ?? false)
                Explosion.DoExplode(projectilePosition);

            if (player == null)
                return;

            if (Knockback?.Enabled ?? false)
                Knockback.DoKnockbackIgnoreDistance(projectilePosition, player);

            if (Bleed?.Enabled ?? false)
                Bleed.DoBleed(player);

            if (DrainStamina?.Enabled ?? false)
                DrainStamina.DoDrain(player);
        }

        public void DoDestroyEffect(ProjectileBase projectile, ProjectileDestroyedReason reason)
        {
            if (projectile.TryGetOwner(out var owner))
            {
                switch (reason)
                {
                    case ProjectileDestroyedReason.CollideWorld:
                        SpawnProjColWorld?.DoSpawn(owner, projectile.m_targetAgent, projectile.transform, keepTrack: false);
                        break;

                    case ProjectileDestroyedReason.CollidePlayer:
                        SpawnProjColPlayer?.DoSpawn(owner, projectile.m_targetAgent, projectile.transform, keepTrack: false);
                        break;

                    case ProjectileDestroyedReason.LifeTimeDone:
                        SpawnProjLifeDone?.DoSpawn(owner, projectile.m_targetAgent, projectile.transform, keepTrack: false);
                        break;
                }
            }
        }
    }

    public enum ProjectileDestroyedReason
    {
        CollideWorld,
        CollidePlayer,
        LifeTimeDone
    }
}