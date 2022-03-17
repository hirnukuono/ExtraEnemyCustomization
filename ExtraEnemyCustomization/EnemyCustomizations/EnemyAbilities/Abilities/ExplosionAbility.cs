using EEC.CustomAbilities.Explosion;
using EEC.Utils;
using EEC.Utils.JsonElements;
using UnityEngine;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public class ExplosionAbility : AbilityBase<ExplosionBehaviour>
    {
        public ValueBase Damage { get; set; } = ValueBase.Zero;
        public Color LightColor { get; set; } = new(1, 0.2f, 0, 1);
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
            if (Agent.WasCollected)
            {
                DoExit();
                return;
            }

            if (Ability.UseExplosionCounter)
            {
                var counter = Agent.RegisterOrGetProperty<ExplosionCounter>();
                if (counter.Count >= Ability.AllowedExplosionCount)
                {
                    DoExit();
                    return;
                }

                counter.Count++;
            }

            if (Ability.KillInflictor && Agent.Alive)
            {
                Agent.Damage.ExplosionDamage(Agent.Damage.HealthMax, Vector3.zero, Vector3.zero);
            }

            var damage = Ability.Damage.GetAbsValue(PlayerData.MaxHealth);

            var position = Agent.EyePosition;
            GetRagdollPosition(ref position);
            ExplosionManager.DoExplosion(new ExplosionData()
            {
                position = position,
                damage = damage,
                enemyMulti = Ability.EnemyDamageMulti,
                minRange = Ability.MinRange,
                maxRange = Ability.MaxRange,
                lightColor = Ability.LightColor
            });

            var noise = new NM_NoiseData()
            {
                noiseMaker = null,
                position = position,
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

        private void GetRagdollPosition(ref Vector3 position)
        {
            if (Agent.Alive)
                return;

            if (Agent.RagdollInstance is null)
                return;

            if (Agent.EnemyMovementData.LocomotionDead != Enemies.ES_StateEnum.Dead)
                return;

            var bodyData = Agent.RagdollInstance.GetComponentInChildren<EnemyRagdollBodyData>();
            if (bodyData is null)
                return;

            position = bodyData.transform.position;
        }
    }

    public class ExplosionCounter
    {
        public int Count = 0;
    }
}