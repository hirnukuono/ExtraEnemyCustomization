using Agents;
using EECustom.Customizations.EnemyAbilities.Abilities;
using EECustom.Customizations.Shared;
using EECustom.Events;
using EECustom.Utils;
using Enemies;
using Player;
using SNetwork;
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

            var unityEventHandler = agent.gameObject.AddComponent<MonoBehaviourEventHandler>();
            unityEventHandler.OnUpdate += (GameObject _) =>
            {
                OnUpdate(data);
            };

            if (setting.Cooldown.InitCooldown > 0.0f)
            {
                data.CooldownTimer = Clock.Time + setting.Cooldown.InitCooldown;
            }
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

            if (!data.Setting.KeepOnDead && !data.Agent.Alive)
                return;

            var isMatchingMode = data.Setting.ActiveType switch
            {
                AbilityActiveType.Hibernate => agent.AI.Mode == AgentMode.Hibernate,
                AbilityActiveType.Combat => agent.AI.Mode == AgentMode.Agressive,
                AbilityActiveType.Scout => agent.AI.Mode == AgentMode.Scout,
                AbilityActiveType.All => true,
                _ => false
            };

            if (!isMatchingMode)
                return;

            var canUseAbility = true;
            canUseAbility &= setting.Cooldown.CanUseAbility(data.CooldownTimer);

            var hasLos = false;
            var distance = float.MaxValue;
            for (int i = 0; i < SNet.Slots.SlottedPlayers.Count; i++)
            {
                SNet_Player snet_Player = SNet.Slots.SlottedPlayers[i];

                var iPlayerAgent = snet_Player.PlayerAgent;
                if (iPlayerAgent == null)
                    continue;

                var playerAgent = iPlayerAgent.Cast<PlayerAgent>();
                var tempDistance = Vector3.Distance(agent.EyePosition, playerAgent.EyePosition);
                if (distance >= tempDistance)
                {
                    distance = tempDistance;
                    hasLos = !Physics.Linecast(agent.EyePosition, playerAgent.EyePosition, LayerManager.MASK_WORLD);
                }
            }

            if (distance == float.MaxValue)
            {
                hasLos = false;
            }

            canUseAbility &= setting.DistanceWithLOS.CanUseAbility(hasLos, distance);
            canUseAbility &= setting.DistanceWithoutLOS.CanUseAbility(hasLos, distance);

            if (!canUseAbility)
                return;

            if (setting.Cooldown.Enabled)
            {
                data.CooldownTimer = Clock.Time + setting.Cooldown.Cooldown;
            }

            behaviour.DoTriggerSync();
        }
    }

    public class BehaviourEnemyData
    {
        public EnemyAgent Agent;
        public AbilityBehaviour Behaviour;
        public BehaviourAbilitySetting Setting;
        public float UpdateTimer = 0.0f;
        public float CooldownTimer = 0.0f;
    }

    public class BehaviourAbilitySetting : AbilitySettingBase
    {
        public float UpdateInterval { get; set; } = 0.15f;
        public AbilityActiveType ActiveType { get; set; } = AbilityActiveType.Combat;
        public bool KeepOnDead { get; set; } = false;
        public DistanceSetting DistanceWithLOS { get; set; } = new();
        public DistanceSetting DistanceWithoutLOS { get; set; } = new();
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
