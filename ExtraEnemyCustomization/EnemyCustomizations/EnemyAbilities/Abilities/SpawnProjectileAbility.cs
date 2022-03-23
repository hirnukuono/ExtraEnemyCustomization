using EEC.EnemyCustomizations.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public sealed class SpawnProjectileAbility : AbilityBase<SpawnProjectileBehaviour>, ISpawnProjectileSetting
    {
        public ProjectileType ProjectileType { get; set; } = ProjectileType.TargetingSmall;
        public bool BackwardDirection { get; set; } = false;
        public int Count { get; set; } = 1;
        public int BurstCount { get; set; } = 1;
        public float Delay { get; set; } = 0.1f;
        public float BurstDelay { get; set; } = 0.05f;
        public float ShotSpreadXMin { get; set; } = 0.0f;
        public float ShotSpreadXMax { get; set; } = 0.0f;
        public float ShotSpreadYMin { get; set; } = 0.0f;
        public float ShotSpreadYMax { get; set; } = 0.0f;
    }

    public sealed class SpawnProjectileBehaviour : AbilityBehaviour<SpawnProjectileAbility>
    {
        public override bool RunUpdateOnlyWhileExecuting => true;
        public override bool AllowEABAbilityWhileExecuting => true;
        public override bool IsHostOnlyBehaviour => true;

        protected override void OnEnter()
        {
            var target = Agent.AI.IsTargetValid ? Agent.AI.Target.m_agent : null;
            Ability.DoSpawn(Agent, target, Agent.ModelRef.m_shooterFireAlign, true);
            DoExit();
        }
    }
}
