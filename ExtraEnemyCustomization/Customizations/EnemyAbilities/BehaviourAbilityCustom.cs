using EECustom.Customizations.EnemyAbilities.Abilities;
using EECustom.Events;
using EECustom.Utils.JsonElements;
using Enemies;
using Player;
using SNetwork;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities
{
    public class BehaviourAbilityCustom : EnemyAbilityCustomBase<BehaviourAbilitySetting>, IEnemySpawnedEvent
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
            var data = new BehaviourEnemyData()
            {
                Agent = agent,
                Behaviour = behaviour,
                Setting = setting,
                UpdateTimer = 0.0f,
                CooldownTimer = 0.0f
            };

            MonoBehaviourEventHandler.AttatchToObject(agent.gameObject, onUpdate: (GameObject _) =>
            {
                OnUpdate(data);
            });
        }

        private void OnUpdate(BehaviourEnemyData data)
        {
            if (Clock.Time < data.UpdateTimer)
                return;

            if (!SNet.IsMaster)
                return;

            var agent = data.Agent;
            var setting = data.Setting;
            var behaviour = data.Behaviour;

            data.UpdateTimer = Clock.Time + setting.UpdateInterval;

            var canUseAbility = true;
            canUseAbility &= data.Setting.KeepOnDead || data.Agent.Alive;
            canUseAbility &= data.Setting.AllowedMode.IsMatch(agent);

            var hasLos = false;
            var sqrDistance = float.MaxValue;
            for (int i = 0; i < PlayerManager.PlayerAgentsInLevel.Count; i++)
            {
                var playerAgent = PlayerManager.PlayerAgentsInLevel[i];
                var tempDistance = (agent.EyePosition - playerAgent.EyePosition).sqrMagnitude;
                if (sqrDistance >= tempDistance)
                {
                    sqrDistance = tempDistance;
                    hasLos = !Physics.Linecast(agent.EyePosition, playerAgent.EyePosition, LayerManager.MASK_WORLD);
                }
            }

            if (sqrDistance == float.MaxValue)
            {
                hasLos = false;
            }

            var distance = Mathf.Sqrt(sqrDistance);
            var distSettingToUse = hasLos ? setting.DistanceWithLOS : setting.DistanceWithoutLOS;
            canUseAbility &= distSettingToUse.CanUseAbility(hasLos, distance);

            if (!canUseAbility)
            {
                if (setting.ForceExitOnConditionMismatch || behaviour.Executing)
                {
                    behaviour.DoExitSync();
                }
                return;
            }

            if (setting.Cooldown.Enabled && setting.Cooldown.CanUseAbility(data.CooldownTimer))
            {
                if (!data.HasInitialTimerDone)
                {
                    data.CooldownTimer = Clock.Time + setting.Cooldown.InitCooldown;
                    data.HasInitialTimerDone = true;
                    return;
                }
                else
                    data.CooldownTimer = Clock.Time + setting.Cooldown.Cooldown;

                behaviour.DoTriggerSync();
            }
        }
    }

    public class BehaviourEnemyData
    {
        public EnemyAgent Agent;
        public AbilityBehaviour Behaviour;
        public BehaviourAbilitySetting Setting;
        public float UpdateTimer = 0.0f;
        public float CooldownTimer = 0.0f;
        public bool HasInitialTimerDone = false;
    }

    public class BehaviourAbilitySetting : AbilitySettingBase
    {
        public float UpdateInterval { get; set; } = 0.15f;
        public AgentModeTarget AllowedMode { get; set; } = AgentModeTarget.None;
        public bool KeepOnDead { get; set; } = false;
        public DistanceSetting DistanceWithLOS { get; set; } = new();
        public DistanceSetting DistanceWithoutLOS { get; set; } = new();
        public bool ForceExitOnConditionMismatch { get; set; } = false;
        public CooldownSetting Cooldown { get; set; } = new();
    }

    public enum AbilityActiveType
    {
        Hibernate,
        Scout,
        Combat,
        All
    }

    public class CooldownSetting
    {
        public bool Enabled { get; set; } = false;
        public float InitCooldown { get; set; } = 0.0f;
        public float Cooldown { get; set; } = 5.0f;

        public bool CanUseAbility(float cooldownTimer)
        {
            if (!Enabled)
                return true;

            return Clock.Time >= cooldownTimer;
        }
    }

    public class DistanceSetting
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
    }

    public enum DistanceCheckingBehaviour
    {
        AlwaysAllow,
        AlwaysDisallow,
        UsingDistance
    }
}
