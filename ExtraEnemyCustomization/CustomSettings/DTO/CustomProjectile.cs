using EECustom.Customizations.Shared;
using EECustom.Utils.JsonElements;
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
        [SuppressMessage("Type Safety", "UNT0014:Invalid type for call to GetComponent", Justification = "IDamagable IS Unity Component Interface")]
        public void Collision(Vector3 projectilePosition, RaycastHit hit)
        {
            if (Explosion?.Enabled ?? false)
                Explosion.DoExplode(projectilePosition);

            if (hit.collider == null)
                return;

            if (!hit.collider.gameObject.TryGetComp<IDamageable>(out var damagable))
                return;

            var baseAgent = damagable.GetBaseAgent();
            if (baseAgent == null)
                return;

            if (!baseAgent.TryCastToPlayerAgent(out var agent))
                return;

            if (Knockback?.Enabled ?? false)
                Knockback.DoKnockbackIgnoreDistance(projectilePosition, agent);

            if (Bleed?.Enabled ?? false)
                Bleed.DoBleed(agent);

            if (DrainStamina?.Enabled ?? false)
                DrainStamina.DoDrain(agent);
        }
    }
}