using Agents;
using EECustom.Customizations.EnemyAbilities.Abilities;
using EECustom.Events;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities
{
    public class BehaviourAbilityCustom : EnemyAbilityCustomBase<BehaviourAbilitySetting>, IEnemySpawnedEvent
    {
        public override string GetProcessName()
        {
            return "BehaviourAbility";
        }

        public override void OnSpawnedPost(EnemyAgent agent)
        {
            var unityEventHandler = agent.gameObject.AddComponent<MonoBehaviourEventHandler>();
            unityEventHandler.OnUpdate += (GameObject _) =>
            {
                OnUpdate(agent);
            };
        }

        public override void OnBehaviourAssigned(EnemyAgent agent, AbilityBehaviour behaviour, BehaviourAbilitySetting setting)
        {
            if (setting.Cooldown.InitCooldown > 0.0f)
            {
                behaviour.CooldownTimer = Clock.Time + setting.Cooldown.InitCooldown;
            }
        }

        private void OnUpdate(EnemyAgent agent)
        {
            foreach (var ab in Abilities)
            {
                if (Clock.Time < ab.UpdateTimer)
                    continue;

                ab.UpdateTimer = Clock.Time + ab.UpdateInterval;

                if (!ab.KeepOnDead && !agent.Alive)
                    continue;

                if (!ab.Ability.TryGetBehaviour(agent, out var behaviour))
                    continue;

                var canUseAbility = true;
                canUseAbility &= ab.ActiveType switch
                {
                    AbilityActiveType.Hibernate => agent.AI.Mode == AgentMode.Hibernate,
                    AbilityActiveType.Combat => agent.AI.Mode == AgentMode.Agressive,
                    AbilityActiveType.Scout => agent.AI.Mode == AgentMode.Scout,
                    AbilityActiveType.All => agent.AI.Mode != AgentMode.Off,
                    _ => false
                };
                canUseAbility &= ab.Cooldown.CanUseAbility(behaviour.CooldownTimer);
                switch (agent.AI.Mode)
                {
                    case AgentMode.Agressive:
                        canUseAbility &= ab.DistanceWithLOS.CanUseAbility(behaviour.Agent, shouldCheckLOS: true);
                        canUseAbility &= ab.DistanceWithoutLOS.CanUseAbility(behaviour.Agent, shouldCheckLOS: false);
                        break;
                }

                if (!canUseAbility)
                    continue;

                if (ab.Cooldown.Enabled)
                {
                    behaviour.CooldownTimer = Clock.Time + ab.Cooldown.Cooldown;
                }

                behaviour.DoTrigger();
            }
        }
    }

    public class BehaviourAbilitySetting : AbilitySettingBase
    {
        public float UpdateInterval { get; set; } = 0.15f;
        public AbilityActiveType ActiveType { get; set; } = AbilityActiveType.Combat;
        public bool KeepOnDead { get; set; } = false;
        public DistanceSetting DistanceWithLOS { get; set; } = new();
        public DistanceSetting DistanceWithoutLOS { get; set; } = new();
        public CooldownSetting Cooldown { get; set; } = new();

        public float UpdateTimer = 0.0f;
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
        public bool DoCheck { get; set; } = false;
        public float Min { get; set; } = 0.0f;
        public float Max { get; set; } = 1.0f;

        public bool CanUseAbility(EnemyAgent agent, bool shouldCheckLOS, bool mismatchingLOSsettingResult = true)
        {
            if (!DoCheck)
                return true;

            if (!agent.AI.IsTargetValid)
                return false;

            var hasLos = agent.AI.Target.m_hasLineOfSight;
            var distance = agent.AI.Target.m_distance;

            if (shouldCheckLOS != hasLos)
                return mismatchingLOSsettingResult;

            if (distance < Min)
                return false;

            if (distance > Max)
                return false;

            return true;
        }
    }
}
