using EECustom.Customizations.EnemyAbilities.Abilities;
using EECustom.Customizations.EnemyAbilities.Handlers;
using EECustom.Utils.JsonElements;
using Enemies;
using System;
using System.Linq;

namespace EECustom.Customizations.EnemyAbilities
{
    public sealed class BehaviourAbilityCustom : EnemyAbilityCustomBase<BehaviourAbilitySetting>, IEnemySpawnedEvent
    {
        public override string GetProcessName()
        {
            return "BehaviourAbility";
        }

        public override void OnConfigLoadedPost()
        {
            foreach (var abSetting in Abilities)
            {
                abSetting.DistanceWithLOS.ShouldCheckLOS = true;
                abSetting.DistanceWithoutLOS.ShouldCheckLOS = false;
            }
        }

        public override void OnSpawnedPost(EnemyAgent agent)
        {
        }

        public override void OnBehaviourAssigned(EnemyAgent agent, AbilityBehaviour behaviour, BehaviourAbilitySetting setting)
        {
            var updater = agent.gameObject.AddComponent<BehaviourUpdateHandler>();
            updater.Agent = agent;
            updater.Behaviour = behaviour;
            updater.Setting = setting;
        }
    }

    public sealed class BehaviourAbilitySetting : AbilitySettingBase
    {
        public float UpdateInterval { get; set; } = 0.15f;
        public bool ForceExitOnConditionMismatch { get; set; } = false;
        public AgentModeTarget AllowedMode { get; set; } = AgentModeTarget.Agressive;
        public float AllowedModeTransitionTime { get; set; } = 0.0f;
        public bool KeepOnDead { get; set; } = false;
        public bool AllowWhileAttack { get; set; } = false;
        public EnemyStateSetting State { get; set; } = new();
        public DistanceSetting DistanceWithLOS { get; set; } = new();
        public DistanceSetting DistanceWithoutLOS { get; set; } = new();
        public CooldownSetting Cooldown { get; set; } = new();
    }

    public sealed class EnemyStateSetting
    {
        public StateCheckingBehaviour Mode { get; set; } = StateCheckingBehaviour.None;
        public ES_StateEnum[] States { get; set; } = Array.Empty<ES_StateEnum>();

        public bool CanUseAbility(ES_StateEnum currentState)
        {
            return Mode switch
            {
                StateCheckingBehaviour.AllowStates => States.Contains(currentState),
                StateCheckingBehaviour.DisallowStates => !States.Contains(currentState),
                _ => true,
            };
        }

        public enum StateCheckingBehaviour
        {
            None,
            AllowStates,
            DisallowStates
        }
    }

    public sealed class CooldownSetting
    {
        public bool Enabled { get; set; } = false;
        public float InitCooldown { get; set; } = 0.0f;
        public float Cooldown { get; set; } = 5.0f;
    }

    public sealed class DistanceSetting
    {
        public DistanceCheckingBehaviour Mode { get; set; } = DistanceCheckingBehaviour.AlwaysAllow;
        public float Min { get; set; } = 0.0f;
        public float Max { get; set; } = 1.0f;

        public bool ShouldCheckLOS = false;

        public bool CanUseAbility(bool hasLosOnTarget, float distanceToClosestTarget)
        {
            switch (Mode)
            {
                case DistanceCheckingBehaviour.AlwaysAllow:
                    return true;

                case DistanceCheckingBehaviour.AlwaysDisallow:
                    return false;
            }

            if (ShouldCheckLOS != hasLosOnTarget)
                return true;

            if (distanceToClosestTarget < Min)
                return false;

            if (distanceToClosestTarget > Max)
                return false;

            return true;
        }

        public enum DistanceCheckingBehaviour
        {
            AlwaysAllow,
            AlwaysDisallow,
            UsingDistance
        }
    }
}