using Agents;
using EEC.EnemyCustomizations.EnemyAbilities.Abilities;
using EEC.Utils.Unity;
using Enemies;
using Player;
using SNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EEC.EnemyCustomizations.EnemyAbilities.Handlers
{
    public sealed class BehaviourUpdateRoutine
    {
        public EnemyAgent Agent;
        public AbilityBehaviour Behaviour;
        public BehaviourAbilitySetting Setting;

        private AgentMode _latestMode;
        private LazyTimer _modeTransitionTimer;
        private LazyTimer _cooldownTimer;
        private bool _hasInitialTimerDone = false;

        public IEnumerator Routine()
        {
            _latestMode = Agent.AI.Mode;

            var yielder = WaitFor.Seconds[Setting.UpdateInterval];
            while (true)
            {
                DoUpdate();
                yield return yielder;
            }
        }

        private void DoUpdate()
        {
            if (!SNet.IsMaster)
                return;

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
                if (!_cooldownTimer.TickAndCheckDone())
                    return;

                if (!_hasInitialTimerDone)
                {
                    _cooldownTimer.Reset(Setting.Cooldown.InitCooldown);
                    _hasInitialTimerDone = true;
                    return;
                }
                else
                    _cooldownTimer.Reset(Setting.Cooldown.Cooldown);
            }

            Behaviour.DoTriggerSync();
        }

        private bool CheckAllowedModeCondition()
        {
            if (_latestMode != Agent.AI.Mode)
            {
                _latestMode = Agent.AI.Mode;
                _modeTransitionTimer.Reset(Setting.AllowedModeTransitionTime);
            }

            if (!_modeTransitionTimer.TickAndCheckDone())
            {
                return false;
            }
            return Setting.AllowedMode.IsMatch(Agent);
        }

        private bool CheckAllowedStateCondition()
        {
            return Setting.State.CanUseAbility(Agent.Locomotion.CurrentStateEnum);
        }

        private bool CheckAttackingCondition()
        {
            return Setting.AllowWhileAttack || (!Agent.Locomotion.IsAttacking());
        }

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
