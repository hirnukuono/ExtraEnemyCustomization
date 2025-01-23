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
        public uint SoundID { get; set; } = 0;
        public bool FindTargetIfInvalid { get; set; } = false;
    }

    public sealed class SpawnProjectileBehaviour : AbilityBehaviour<SpawnProjectileAbility>
    {
        public override bool RunUpdateOnlyWhileExecuting => true;
        public override bool AllowEABAbilityWhileExecuting => true;
        public override bool IsHostOnlyBehaviour => true;

        protected override void OnEnter()
        {
            Agents.Agent? target = null;
            if (Agent.AI.IsTargetValid)
            {
                target = Agent.AI.Target.m_agent;
            }
            else if (Ability.FindTargetIfInvalid)
            {
                float sqrDistance = float.MaxValue;
                foreach (var playerAgent in Player.PlayerManager.PlayerAgentsInLevel)
                {
                    var tempDistance = (Agent.EyePosition - playerAgent.EyePosition).sqrMagnitude;
                    if (sqrDistance >= tempDistance && !UnityEngine.Physics.Linecast(Agent.EyePosition, playerAgent.EyePosition, LayerManager.MASK_WORLD))
                    {
                        sqrDistance = tempDistance;
                        target = playerAgent;
                    }
                }
            }

            Ability.DoSpawn(Agent, target, Agent.ModelRef.m_shooterFireAlign, true);

            if (Ability.SoundID != 0u)
            {
                Agent.Sound.Post(Ability.SoundID);
            }

            DoExit();
        }
    }
}
