using EEC.CustomAbilities.Explosion;
using EEC.EnemyCustomizations.Shared;
using EEC.Utils;
using EEC.Utils.Json.Elements;
using UnityEngine;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public sealed class ExplosionAbility : AbilityBase<ExplosionBehaviour>, IExplosionSetting
    {
        public ValueBase Damage { get; set; } = ValueBase.Zero;
        public Color LightColor { get; set; } = new(1, 0.2f, 0, 1);
        public bool KillInflictor { get; set; } = true;
        public bool UseRagdollPosition { get; set; } = true;
        public bool UseExplosionCounter { get; set; } = false;
        public int AllowedExplosionCount { get; set; } = 1;
        public float EnemyDamageMulti { get; set; } = 1.0f;
        public float MinRange { get; set; } = 2.0f;
        public float MaxRange { get; set; } = 5.0f;
        public float NoiseMinRange { get; set; } = 5.0f;
        public float NoiseMaxRange { get; set; } = 10.0f;
        public NM_NoiseType NoiseType { get; set; } = NM_NoiseType.Detectable;
    }

    public sealed class ExplosionBehaviour : AbilityBehaviour<ExplosionAbility>
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

            var position = Agent.EyePosition;
            if (Ability.UseRagdollPosition)
            {
                GetRagdollPosition(ref position);
            }
            Ability.DoExplode(Agent.CourseNode, position);
            Ability.TryKillInflictor(Agent);
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

    public sealed class ExplosionCounter
    {
        public int Count = 0;
    }
}