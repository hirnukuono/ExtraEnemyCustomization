using EECustom.Utils;
using EECustom.Utils.JsonElements;

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
    }

    public class ExplosionBehaviour : AbilityBehaviour<ExplosionAbility>
    {
        public override bool AllowEABAbilityWhileExecuting => true;
        public override bool IsHostOnlyBehaviour => true;

        protected override void OnEnter()
        {

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
                type = NM_NoiseType.Detectable,
                includeToNeightbourAreas = true,
                raycastFirstNode = false
            };
            NoiseManager.MakeNoise(noise);

            DoExit();
        }
    }
}
