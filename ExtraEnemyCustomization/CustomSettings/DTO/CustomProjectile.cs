using EECustom.Utils;
using UnityEngine;

namespace EECustom.CustomSettings.DTO
{
    public class CustomProjectile
    {
        public string DebugName { get; set; } = string.Empty;
        public byte ID { get; set; } = 10;
        public ProjectileType BaseProjectile { get; set; } = ProjectileType.TargetingSmall;
        public ValueBase Speed { get; set; } = ValueBase.Unchanged;
        public ValueBase HomingStrength { get; set; } = ValueBase.Unchanged;
        public Color GlowColor { get; set; } = Color.yellow;
        public ValueBase GlowRange { get; set; } = ValueBase.Unchanged;
        public ValueBase Damage { get; set; } = ValueBase.Unchanged;
        public ValueBase Infection { get; set; } = ValueBase.Unchanged;
        public ValueBase ExplosionDamage { get; set; } = ValueBase.Zero;
        public float ExplosionEnemyDamageMulti { get; set; } = 1.0f;
        public float ExplosionMinRange { get; set; } = 2.0f;
        public float ExplosionMaxRange { get; set; } = 5.0f;
        public float ExplosionNoiseMinRange { get; set; } = 5.0f;
        public float ExplosionNoiseMaxRange { get; set; } = 10.0f;
    }
}