using EECustom.Utils;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public class ExplosionAbility : AbilityBase<ExplosionBehaviour>
    {
        public ValueBase Damage { get; set; } = ValueBase.Zero;
        public float EnemyDamageMulti { get; set; } = 1.0f;
        public float MinRange { get; set; } = 2.0f;
        public float MaxRange { get; set; } = 5.0f;
        public float NoiseMinRange { get; set; } = 5.0f;
        public float NoiseMaxRange { get; set; } = 10.0f;

        public override void OnBehaviourAssigned(EnemyAgent agent, ExplosionBehaviour behaviour)
        {
            behaviour.Damage = Damage.GetAbsValue(PlayerData.MaxHealth);
            behaviour.EnemyMulti = EnemyDamageMulti;
            behaviour.MinRange = MinRange;
            behaviour.MaxRange = MaxRange;
            behaviour.NoiseMinRange = NoiseMinRange;
            behaviour.NoiseMaxRange = NoiseMaxRange;
        }
    }

    public class ExplosionBehaviour : AbilityBehaviour
    {
        public float Damage;
        public float EnemyMulti;
        public float MinRange;
        public float MaxRange;
        public float NoiseMinRange;
        public float NoiseMaxRange;

        public override bool AllowEABAbilityWhileExecuting => true;
        public override bool IsHostOnlyBehaviour => true;

        protected override void OnEnter()
        {
            ExplosionUtil.MakeExplosion(Agent.EyePosition, Damage, EnemyMulti, MinRange, MaxRange);

            var noise = new NM_NoiseData()
            {
                noiseMaker = null,
                position = Agent.EyePosition,
                radiusMin = NoiseMinRange,
                radiusMax = NoiseMaxRange,
                yScale = 1,
                node = Agent.CourseNode,
                type = NM_NoiseType.Detectable,
                includeToNeightbourAreas = true,
                raycastFirstNode = false
            };
            NoiseManager.MakeNoise(noise);

            DoExit();
        }
    }
}
