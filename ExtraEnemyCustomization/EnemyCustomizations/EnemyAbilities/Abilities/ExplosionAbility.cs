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
        public ValueBase EnemyMinRange { get; set; } = ValueBase.Unchanged;
        public ValueBase EnemyMaxRange { get; set; } = ValueBase.Unchanged;
        public float NoiseMinRange { get; set; } = 5.0f;
        public float NoiseMaxRange { get; set; } = 10.0f;
        public NM_NoiseType NoiseType { get; set; } = NM_NoiseType.Detectable;
        public KnockbackSetting Knockback { get; set; } = new();
        public BleedSetting Bleed { get; set; } = new();
        public DrainStaminaSetting DrainStamina { get; set; } = new();
    }

    public sealed class ExplosionBehaviour : AbilityBehaviour<ExplosionAbility>
    {
        public override bool RunUpdateOnlyWhileExecuting => true;
        public override bool AllowEABAbilityWhileExecuting => true;
        public override bool IsHostOnlyBehaviour => true;

        protected override void OnEnterUseClientPos()
        {
            DoExplosion(true);
        }

        protected override void OnEnter()
        {
            DoExplosion(false);
        }

        private void DoExplosion(bool useClientPos)
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

            Ability.DoExplode(Agent, Ability.UseRagdollPosition, useClientPos);
            Ability.TryKillInflictor(Agent);
            DoExit();
        }
    }

    public sealed class ExplosionCounter
    {
        public int Count = 0;
    }
}