using EECustom.Utils;
using EECustom.Utils.JsonElements;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public class ExplosionAbility : AbilityBase<ExplosionBehaviour>
    {
        public ValueBase Damage { get; set; } = ValueBase.Zero;
        public bool KillInflictor { get; set; } = true;
        public bool UseExplosionCounter { get; set; } = false;
        public int AllowedExplosionCount { get; set; } = 1;
        public float EnemyDamageMulti { get; set; } = 1.0f;
        public float MinRange { get; set; } = 2.0f;
        public float MaxRange { get; set; } = 5.0f;
        public float NoiseMinRange { get; set; } = 5.0f;
        public float NoiseMaxRange { get; set; } = 10.0f;
        public NM_NoiseType NoiseType { get; set; } = NM_NoiseType.Detectable;
    }

    public class ExplosionBehaviour : AbilityBehaviour<ExplosionAbility>
    {
        public override bool RunUpdateOnlyWhileExecuting => true;
        public override bool AllowEABAbilityWhileExecuting => true;
        public override bool IsHostOnlyBehaviour => true;

        protected override void OnEnter()
        {
            if (Ability.UseExplosionCounter)
            {
                var counter = EnemyProperty<ExplosionCounter>.RegisterOrGet(Agent);
                if (counter.Count >= Ability.AllowedExplosionCount)
                {
                    DoExit();
                    return;
                }

                counter.Count++;
            }

            if (Ability.KillInflictor)
            {
                Agent.Damage.ExplosionDamage(Agent.Damage.HealthMax, Vector3.zero, Vector3.zero);
            }

            var damage = Ability.Damage.GetAbsValue(PlayerData.MaxHealth);
            ExplosionUtil.MakeExplosion(Agent.EyePosition, damage, Ability.EnemyDamageMulti, Ability.MinRange, Ability.MaxRange);

            var noise = new NM_NoiseData()
            {
                noiseMaker = null,
                position = Agent.EyePosition,
                radiusMin = Ability.NoiseMinRange,
                radiusMax = Ability.NoiseMaxRange,
                yScale = 1,
                node = Agent.CourseNode,
                type = Ability.NoiseType,
                includeToNeightbourAreas = true,
                raycastFirstNode = false
            };
            NoiseManager.MakeNoise(noise);

            DoExit();
        }
    }

    public class ExplosionCounter
    {
        public int Count = 0;
    }
}