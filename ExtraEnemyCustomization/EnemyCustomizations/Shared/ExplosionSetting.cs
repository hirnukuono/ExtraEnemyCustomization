using Agents;
using AIGraph;
using EEC.CustomAbilities.Explosion;
using EEC.Utils;
using EEC.Utils.Json.Elements;
using LevelGeneration;
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
        public float NoiseMinRange { get; set; }
        public float NoiseMaxRange { get; set; }
        public NM_NoiseType NoiseType { get; set; }
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
        public float NoiseMinRange { get; set; } = 5.0f;
        public float NoiseMaxRange { get; set; } = 10.0f;
        public NM_NoiseType NoiseType { get; set; } = NM_NoiseType.Detectable;
    }
}