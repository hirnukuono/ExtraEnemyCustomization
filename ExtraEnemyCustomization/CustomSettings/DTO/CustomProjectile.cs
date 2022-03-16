using EECustom.Customizations.Shared;
using EECustom.Utils.JsonElements;
using Player;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace EECustom.CustomSettings.DTO
{
    public class CustomProjectile
    {
        public string DebugName { get; set; } = string.Empty;
        public byte ID { get; set; } = 10;
        public ProjectileType BaseProjectile { get; set; } = ProjectileType.TargetingSmall;
        public ValueBase Speed { get; set; } = ValueBase.Unchanged;
        public MultiplierShiftSetting SpeedChange { get; set; } = new();
        public ValueBase HomingStrength { get; set; } = ValueBase.Unchanged;
        public MultiplierShiftSetting HomingStrengthChange { get; set; } = new();
        public Color GlowColor { get; set; } = Color.yellow;
        public ValueBase GlowRange { get; set; } = ValueBase.Unchanged;
        public ValueBase Damage { get; set; } = ValueBase.Unchanged;
        public ValueBase Infection { get; set; } = ValueBase.Unchanged;
        [Obsolete("Will be merged to AttackCustoms")] public ExplosionSetting Explosion { get; set; } = new();
        [Obsolete("Will be merged to AttackCustoms")] public KnockbackSetting Knockback { get; set; } = new();
        [Obsolete("Will be merged to AttackCustoms")] public BleedSetting Bleed { get; set; } = new();
        [Obsolete("Will be merged to AttackCustoms")] public DrainStaminaSetting DrainStamina { get; set; } = new();

        [Obsolete("Will be merged to AttackCustoms")]

        public void Collision(Vector3 projectilePosition, PlayerAgent player = null)
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
    }
}