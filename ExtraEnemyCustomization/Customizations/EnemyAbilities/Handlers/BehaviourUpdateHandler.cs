using Agents;
using EECustom.Attributes;
using EECustom.Customizations.EnemyAbilities.Abilities;
using Enemies;
using Player;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Handlers
{
    [InjectToIl2Cpp]
    public sealed class BehaviourUpdateHandler : MonoBehaviour
    {
        public EnemyAgent Agent;
        public AbilityBehaviour Behaviour;
        public BehaviourAbilitySetting Setting;

        private AgentMode _latestMode;
        private float _modeTransitionTimer = 0.0f;
        private float _updateTimer = 0.0f;
        private float _cooldownTimer = 0.0f;
        private bool _hasInitialTimerDone = false;

        internal void Start()
        {
            _latestMode = Agent.AI.Mode;
        }

        internal void Update()
        {
            if (Clock.Time < _updateTimer)
                return;

            if (!SNet.IsMaster)
                return;

            _updateTimer = Clock.Time + Setting.UpdateInterval;

            var canUseAbility = true;
            canUseAbility &= Setting.KeepOnDead || Agent.Alive;
            canUseAbility &= CheckAllowedModeCondition();
            canUseAbility &= CheckAllowedStateCondition();
            canUseAbility &= CheckAttackingCondition();
            canUseAbility &= CheckDistanceCondition();

            if (!canUseAbility)
            {
                if (Setting.ForceExitOnConditionMismatch && Behaviour.Executing)
                {
                    Behaviour.DoExitSync();
                }
                return;
            }

            if (Setting.Cooldown.Enabled)
            {
                if (!Setting.Cooldown.CanUseAbility(_cooldownTimer))
                    return;

                if (!_hasInitialTimerDone)
                {
                    _cooldownTimer = Clock.Time + Setting.Cooldown.InitCooldown;
                    _hasInitialTimerDone = true;
                    return;
                }
                else
                    _cooldownTimer = Clock.Time + Setting.Cooldown.Cooldown;
            }

            Behaviour.DoTriggerSync();
        }

        internal void OnDestroy()
        {
            Agent = null;
            Behaviour = null;
            Setting = null;
        }

        [HideFromIl2Cpp]
        private bool CheckAllowedModeCondition()
        {
            if (_latestMode != Agent.AI.Mode)
            {
                _latestMode = Agent.AI.Mode;
                _modeTransitionTimer = Clock.Time + Setting.AllowedModeTransitionTime;
            }

            if (_modeTransitionTimer > Clock.Time)
            {
                return false;
            }
            return Setting.AllowedMode.IsMatch(Agent);
        }

        [HideFromIl2Cpp]
        private bool CheckAllowedStateCondition()
        {
            return Setting.State.CanUseAbility(Agent.Locomotion.CurrentStateEnum);
        }

        [HideFromIl2Cpp]
        private bool CheckAttackingCondition()
        {
            return Setting.AllowWhileAttack || (!Agent.Locomotion.IsAttacking());
        }

        [HideFromIl2Cpp]
        private bool CheckDistanceCondition()
        {
            var hasLos = false;
            float distance;
            float sqrDistance = float.MaxValue;
            if (Agent.AI.IsTargetValid)
            {
                distance = Agent.AI.Target.m_distance;
                hasLos = Agent.AI.Target.m_hasLineOfSight;
            }
            else
            {
                for (int i = 0; i < PlayerManager.PlayerAgentsInLevel.Count; i++)
                {
                    var playerAgent = PlayerManager.PlayerAgentsInLevel[i];
                    var tempDistance = (Agent.EyePosition - playerAgent.EyePosition).sqrMagnitude;
                    if (sqrDistance >= tempDistance)
                    {
                        sqrDistance = tempDistance;
                        hasLos = !Physics.Linecast(Agent.EyePosition, playerAgent.EyePosition, LayerManager.MASK_WORLD);
                    }
                }

                if (sqrDistance == float.MaxValue)
                {
                    hasLos = false;
                    distance = float.MaxValue;
                }
                else
                {
                    distance = Mathf.Sqrt(sqrDistance);
                }
            }

            var distSettingToUse = hasLos ? Setting.DistanceWithLOS : Setting.DistanceWithoutLOS;
            return distSettingToUse.CanUseAbility(hasLos, distance);
        }
    }
}
