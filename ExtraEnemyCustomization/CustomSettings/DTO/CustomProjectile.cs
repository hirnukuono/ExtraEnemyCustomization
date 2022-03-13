using EECustom.Customizations.Shared;
using EECustom.Utils.JsonElements;
using UnityEngine;

namespace EECustom.CustomSettings.DTO
{
    public class CustomProjectile
    {
        public string DebugName { get; set; } = string.Empty;
        public byte ID { get; set; } = 10;
        public ProjectileType BaseProjectile { get; set; } = ProjectileType.TargetingSmall;
        public ValueBase Speed { get; set; } = ValueBase.Unchanged;
        public SpeedChangeSetting SpeedChange { get; set; } = new();
        public ValueBase HomingStrength { get; set; } = ValueBase.Unchanged;
        public Color GlowColor { get; set; } = Color.yellow;
        public ValueBase GlowRange { get; set; } = ValueBase.Unchanged;
        public ValueBase Damage { get; set; } = ValueBase.Unchanged;
        public ValueBase Infection { get; set; } = ValueBase.Unchanged;
        public ExplosionSetting Explosion { get; set; } = new();
        public KnockbackSetting Knockback { get; set; } = new();
        public BleedSetting Bleed { get; set; } = new();
        public DrainStaminaSetting DrainStamina { get; set; } = new();

        public void Collision(Vector3 projectilePosition, RaycastHit hit)
        {
            if (Explosion?.Enabled ?? false)
                Explosion.DoExplode(projectilePosition);

            var baseAgent = hit.collider?.GetComponent<IDamageable>()?.GetBaseAgent() ?? null;
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

        public sealed class SpeedChangeSetting
        {
            public bool Enabled { get; set; } = false;
            public float MinMulti { get; set; } = 1.0f;
            public float MaxMulti { get; set; } = 1.0f;
            public float Duration { get; set; } = 1.0f;
            public eEasingType EasingMode { get; set; } = eEasingType.Linear;
            public RepeatMode Mode { get; set; } = RepeatMode.Clamped;

            private float _durationInv = 0.0f;

            public void CalcInv()
            {
                _durationInv = 1.0f / Duration;
            }

            public float EvaluateMultiplier(float progress)
            {
                return Mode switch
                {
                    RepeatMode.Unclamped => Mathf.LerpUnclamped(MinMulti, MaxMulti, Ease(progress * _durationInv)),
                    RepeatMode.PingPong => Mathf.Lerp(MinMulti, MaxMulti, Ease(Mathf.PingPong(progress * _durationInv, 1.0f))),
                    RepeatMode.Repeat => Mathf.Lerp(MinMulti, MaxMulti, Ease(Mathf.Repeat(progress * _durationInv, 1.0f))),
                    _ => Mathf.LerpUnclamped(MinMulti, MaxMulti, Mathf.Clamp01(Ease(progress * _durationInv))) //Clamped, Etc
                };
            }

            private float Ease(float p)
            {
                return Easing.GetEasingValue(EasingMode, p, false);
            }

            public enum RepeatMode
            {
                Clamped,
                Unclamped,
                PingPong,
                Repeat
            }
        }
    }
}