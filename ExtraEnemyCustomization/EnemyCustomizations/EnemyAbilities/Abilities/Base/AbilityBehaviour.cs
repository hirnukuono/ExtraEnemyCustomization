﻿using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Logging;
using EEC.EnemyCustomizations.EnemyAbilities.Events;
using EEC.Events;
using EEC.Utils.Unity;
using Timer = EEC.Utils.Unity.Timer;
using Enemies;
using SNetwork;
using System.Collections;
using UnityEngine;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public abstract class AbilityBehaviour<AB> : AbilityBehaviour where AB : class, IAbility
    {
        public AB Ability
        {
            get
            {
                return BaseAbility as AB;
            }
        }
    }

    public abstract class AbilityBehaviour
    {
        public const float LAZYUPDATE_DELAY = 0.15f;

        public IAbility BaseAbility { get; private set; }
        public EnemyAgent Agent { get; private set; }
        public bool AgentDestroyed { get; private set; } = false;

        public bool IsMasterOnlyAndClient
        {
            get
            {
                return IsHostOnlyBehaviour && !SNet.IsMaster;
            }
        }


        public bool Executing
        {
            get => _executing;
            private set
            {
                var newValue = value;
                if (newValue == _executing)
                    return;

                _executing = newValue;
                if (!AllowEABAbilityWhileExecuting)
                {
                    Agent.Abilities.CanTriggerAbilities = !_executing && Agent.CanUseAbilities();
                }
            }
        }

        public bool StandStill
        {
            get => _standStillStack > 0;
            set
            {
                var oldValue = StandStill;
                _standStillStack = Math.Max(0, _standStillStack + (value ? 1 : -1));
                if (oldValue == StandStill)
                    return;

                if (StandStill)
                {
                    _prevState = Agent.Locomotion.CurrentStateEnum;
                    _navAgent.isStopped = true;
                    _navAgent.velocity = Vector3.zero;
                    Agent.Locomotion.ChangeState(ES_StateEnum.StandStill);
                }
                else if (_prevState != ES_StateEnum.StandStill)
                {
                    switch (Agent.Locomotion.CurrentStateEnum)
                    {
                        //When in Specific State it should not change state
                        case ES_StateEnum.Hitreact:
                        case ES_StateEnum.HitReactFlyer:
                        case ES_StateEnum.Dead:
                        case ES_StateEnum.DeadFlyer:
                        case ES_StateEnum.DeadSquidBoss:
                        case ES_StateEnum.ScoutScream:
                        case ES_StateEnum.HibernateWakeUp:
                        case ES_StateEnum.StuckInGlue:
                            break;

                        default:
                            goto RevertState;
                    }
                    return;

                RevertState:
                    // If enemy was asleep before StandStill, check if they should revert to sleeping
                    if (_prevState == ES_StateEnum.Hibernate)
                    {
                        // Can't check enum since it doesn't get updated by EB_Hibernate switching to combat
                        if (Agent.AI.m_behaviour.CurrentState.TryCast<EB_Hibernating>() != null)
                        {
                            Agent.Locomotion.ChangeState(ES_StateEnum.Hibernate);
                            return;
                        }
                    }
                    Agent.Locomotion.ChangeState(Agent.Locomotion.PathMove);
                }
            }
        }

        public abstract bool RunUpdateOnlyWhileExecuting { get; }
        public abstract bool AllowEABAbilityWhileExecuting { get; }
        public abstract bool IsHostOnlyBehaviour { get; }
        public virtual bool IsHostOnlySetup => IsHostOnlyBehaviour;

        private Timer _lazyUpdateTimer = new(LAZYUPDATE_DELAY);
        private bool _executing = false;
        private int _standStillStack = 0;
        private ES_StateEnum _prevState;
        private UnityEngine.AI.INavigation _navAgent;

        public void Setup(IAbility baseAbility, EnemyAgent agent)
        {
            BaseAbility = baseAbility;
            Agent = agent;
            _navAgent = Agent.AI.m_navMeshAgent;

            Agent.AI.StartCoroutine(Update());
            Agent.Locomotion.AddState(ES_StateEnum.StandStill, new ES_StandStill());

            EnemyEvents.Despawn += Despawn_Del;
            EnemyAbilitiesEvents.TakeDamage += TakeDamage_Del;
            EnemyAbilitiesEvents.Dead += Dead_Del;
            EnemyAbilitiesEvents.Hitreact += Hitreact_Del;

            DoSetup();
        }

        public void Unload()
        {
            EnemyEvents.Despawn -= Despawn_Del;
            EnemyAbilitiesEvents.TakeDamage -= TakeDamage_Del;
            EnemyAbilitiesEvents.Dead -= Dead_Del;
            EnemyAbilitiesEvents.Hitreact -= Hitreact_Del;
        }

        #region EVENT DELEGATES

        private IEnumerator Update()
        {
            while (true)
            {
                DoUpdate();
                yield return null;
            }
        }

        private void Despawn_Del(EnemyAgent agent)
        {
            if (agent.GlobalID == Agent.GlobalID)
                AgentDestroyed = true;
        }

        private void TakeDamage_Del(Enemies.EnemyAbilities abilities, float damage)
        {
            if (AgentDestroyed)
                return;

            if (abilities.m_agent.GlobalID == Agent.GlobalID)
            {
                DoTakeDamage(damage);
            }
        }

        private void Dead_Del(Enemies.EnemyAbilities abilities)
        {
            if (AgentDestroyed)
                return;

            if (abilities.m_agent.GlobalID == Agent.GlobalID)
            {
                DoDead();

                EnemyAbilitiesEvents.TakeDamage -= TakeDamage_Del;
                EnemyAbilitiesEvents.Dead -= Dead_Del;
                EnemyAbilitiesEvents.Hitreact -= Hitreact_Del;
                EnemyLimbEvents.Destroyed -= LimbDestroyed_Del;
            }
        }

        private void LimbDestroyed_Del(Dam_EnemyDamageLimb limb)
        {
            if (limb.GetBaseAgent().GlobalID == Agent.GlobalID)
            {
                DoLimbDestroyed(limb);
            }
        }

        private void Hitreact_Del(Enemies.EnemyAbilities abilities)
        {
            if (abilities.m_agent.GlobalID == Agent.GlobalID)
            {
                DoHitreact();
            }
        }

        #endregion EVENT DELEGATES

        #region EVENT CALLERS

        private void DoSetup()
        {
            if (IsHostOnlySetup && !SNet.IsMaster)
                return;

            OnSetup();
        }

        private void DoDead()
        {
            if (IsMasterOnlyAndClient)
                return;

            OnDead();
        }

        private void DoUpdate()
        {
            if (IsMasterOnlyAndClient)
                return;

            if (RunUpdateOnlyWhileExecuting && !Executing)
                return;

            OnUpdate();
            DoAbilityUpdate();
        }

        private void DoAbilityUpdate()
        {
            if (Executing && !AgentDestroyed)
            {
                OnAbilityUpdate();
                DoAbilityLazyUpdate();
            }
        }

        private void DoAbilityLazyUpdate()
        {
            if (_lazyUpdateTimer.TickAndCheckDone())
            {
                _lazyUpdateTimer.Reset();
                OnAbilityLazyUpdate();
            }
        }

        public void DoTriggerSync()
        {
            EnemyAbilityManager.SendEvent(BaseAbility.SyncID, Agent.GlobalID, AbilityPacketType.DoTrigger);
        }

        public void DoTrigger()
        {
            DoEnter();
        }

        public void DoEnterSync()
        {
            EnemyAbilityManager.SendEvent(BaseAbility.SyncID, Agent.GlobalID, AbilityPacketType.DoTrigger);
        }

        public void DoEnter()
        {
            if (IsMasterOnlyAndClient)
                return;

            if (Executing || AgentDestroyed)
                return;

            Executing = true;
            OnEnter();
        }

        public void DoExitSync()
        {
            EnemyAbilityManager.SendEvent(BaseAbility.SyncID, Agent.GlobalID, AbilityPacketType.DoExit);
        }

        public void DoExit()
        {
            if (IsMasterOnlyAndClient)
                return;

            if (!Executing || AgentDestroyed)
                return;

            OnExit();
            Executing = false;
        }

        private void DoTakeDamage(float damage)
        {
            if (IsMasterOnlyAndClient)
                return;

            OnTakeDamage(damage);
        }

        private void DoHitreact()
        {
            if (IsMasterOnlyAndClient)
                return;

            OnHitreact();
        }

        private void DoLimbDestroyed(Dam_EnemyDamageLimb limb)
        {
            if (IsMasterOnlyAndClient)
                return;

            OnLimbDestroyed(limb);
        }

        #endregion EVENT CALLERS

        #region OVERRIDES

        protected virtual void OnAbilityUpdate()
        {
        }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnAbilityLazyUpdate()
        {
        }

        protected virtual void OnExit()
        {
        }

        protected virtual void OnSetup()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnDead()
        {
        }

        protected virtual void OnTakeDamage(float damage)
        {
        }

        protected virtual void OnHitreact()
        {
        }

        protected virtual void OnLimbDestroyed(Dam_EnemyDamageLimb limb)
        {
        }

        #endregion OVERRIDES

        #region LOGGING

        public void LogVerbose(string str)
        {
            LogFormatDebug(str, true);
        }

        public void LogDev(string str)
        {
            LogFormatDebug(str, false);
        }

        public void LogError(string str)
        {
            LogFormat(LogLevel.Error, str);
        }

        public void LogWarning(string str)
        {
            LogFormat(LogLevel.Warning, str);
        }

        private void LogFormat(LogLevel level, string str)
        {
            Logger.LogInstance.Log(level, $"[{BaseAbility.Name}] [{Agent.name}] {str}");
        }

        private void LogFormatDebug(string str, bool verbose)
        {
            if (verbose)
                Logger.Verbose($"[{BaseAbility.Name}] [{Agent.name}] {str}");
            else
                Logger.Debug($"[{BaseAbility.Name}] [{Agent.name}] {str}");
        }

        #endregion LOGGING
    }
}