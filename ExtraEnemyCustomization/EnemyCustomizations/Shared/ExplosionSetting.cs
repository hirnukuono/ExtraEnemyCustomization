using EEC.CustomAbilities.Explosion;
using EEC.Utils;
using EEC.Utils.Json.Elements;
using UnityEngine;

namespace EEC.EnemyCustomizations.Shared
{
    public interface IExplosionSetting
    {
        public ValueBase Damage { get; set; }
        public Color LightColor { get; set; }
        public bool KillInflictor { get; set; }
        public float EnemyDamageMulti { get; set; }
        public float MinRange { get; set; }
        public float MaxRange { get; set; }
        public ValueBase EnemyMinRange { get; set; }
        public ValueBase EnemyMaxRange { get; set; }
        public float NoiseMinRange { get; set; }
        public float NoiseMaxRange { get; set; }
        public NM_NoiseType NoiseType { get; set; }
        public KnockbackSetting Knockback { get; set; }
        public BleedSetting Bleed { get; set; }
        public DrainStaminaSetting DrainStamina { get; set; }
        public InfectionSetting Infection { get; set; }

        public ExplosionData ToPacket(Vector3 position)
        {
            return new()
            {
                damage = Damage.GetAbsValue(PlayerData.MaxHealth),
                enemyMulti = EnemyDamageMulti,
                minRange = MinRange,
                maxRange = MaxRange,
                enemyMinRange = EnemyMinRange.GetAbsValue(MinRange),
                enemyMaxRange = EnemyMaxRange.GetAbsValue(MaxRange),
                lightColor = LightColor,
                bleeding = new()
                {
                    enabled = Bleed.Enabled,
                    packet = Bleed.ToPacket()
                },
                drainStamina = new()
                {
                    enabled = DrainStamina.Enabled,
                    packet = DrainStamina.ToPacket()
                },
                knockback = new()
                {
                    enabled = Knockback.Enabled,
                    packet = Knockback.ToPacket(position)
                },
                infection = new()
                {
                    enabled = Infection.Enabled,
                    packet = ((IInfectionSetting)Infection).ToPacket()
                }
            };
        }
    }

    public sealed class ExplosionSetting : IExplosionSetting
    {
        public bool Enabled { get; set; } = false;
        public ValueBase Damage { get; set; } = ValueBase.Zero;
        public Color LightColor { get; set; } = new(1, 0.2f, 0, 1);
        public bool KillInflictor { get; set; } = true;
        public float EnemyDamageMulti { get; set; } = 1.0f;
        public float MinRange { get; set; } = 2.0f;
        public float MaxRange { get; set; } = 5.0f;
        public ValueBase EnemyMinRange { get; set; } = ValueBase.Unchanged;
        public ValueBase EnemyMaxRange { get; set; } = ValueBase.Unchanged;
        public float NoiseMinRange { get; set; } = 5.0f;
        public float NoiseMaxRange { get; set; } = 10.0f;
        public NM_NoiseType NoiseType { get; set; } = NM_NoiseType.Detectable;
        public KnockbackSetting Knockback { get; set; } = new();
        public BleedSetting Bleed { get; set; } = new();
        public DrainStaminaSetting DrainStamina { get; set; } = new();
        public InfectionSetting Infection { get; set; } = new();
    }
}