using EECustom.Customizations.EnemyAbilities.Events;
using EECustom.Events;
using Enemies;
using SNetwork;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public abstract class AbilityBehaviour<AB> : AbilityBehaviour where AB : IAbility
    {
        public new AB Ability { get; private set; }
    }

    public abstract class AbilityBehaviour
    {
        public const float LAZYUPDATE_DELAY = 0.15f;

        public IAbility Ability { get; private set; }
        public EnemyAgent Agent { get; private set; }

        public bool Executing
        {
            get => _executing;
            private set
            {
                var newValue = value;
                if (newValue == _executing)
                    return;

                _executing = newValue;
                if (AllowEABAbilityWhileExecuting)
                {
                    Agent.Abilities.CanTriggerAbilities = !_executing;
                }
            }
        }

        public abstract bool AllowEABAbilityWhileExecuting { get; }
        public abstract bool IsHostOnlyBehaviour { get; }

        private float _lazyUpdateTimer = 0.0f;
        private bool _executing = false;

        public void Setup(IAbility baseAbility, EnemyAgent agent)
        {
            Ability = baseAbility;
            Agent = agent;

            var mbEventHandler = Agent.gameObject.AddComponent<MonoBehaviourEventHandler>();

            mbEventHandler.OnUpdate += Update_Del;
            EnemyAbilitiesEvents.TakeDamage += TakeDamage_Del;
            EnemyAbilitiesEvents.Dead += Dead_Del;
            EnemyAbilitiesEvents.Hitreact += Hitreact_Del;

            DoSetup();
        }

        public void Unload()
        {
            EnemyAbilitiesEvents.TakeDamage -= TakeDamage_Del;
            EnemyAbilitiesEvents.Dead -= Dead_Del;
            EnemyAbilitiesEvents.Hitreact -= Hitreact_Del;
        }

        #region EVENT DELEGATES

        private void Update_Del(GameObject _)
        {
            DoUpdate();
        }

        private void TakeDamage_Del(Enemies.EnemyAbilities abilities, float damage)
        {
            if (abilities.m_agent.GlobalID == Agent.GlobalID)
            {
                DoTakeDamage(damage);
            }
        }

        private void Dead_Del(Enemies.EnemyAbilities abilities)
        {
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
            if (IsHostOnlyBehaviour && !SNet.IsMaster)
                return;

            OnSetup();
        }

        private void DoDead()
        {
            if (IsHostOnlyBehaviour && !SNet.IsMaster)
                return;

            OnDead();
        }

        private void DoUpdate()
        {
            if (IsHostOnlyBehaviour && !SNet.IsMaster)
                return;

            OnUpdate();
            DoAbilityUpdate();
        }

        private void DoAbilityUpdate()
        {
            if (Executing)
            {
                OnAbilityUpdate();
                DoAbilityLazyUpdate();
            }
        }

        private void DoAbilityLazyUpdate()
        {
            if (_lazyUpdateTimer <= Clock.Time)
            {
                _lazyUpdateTimer = Clock.Time + LAZYUPDATE_DELAY;
                OnAbilityLazyUpdate();
            }
        }

        public void DoTriggerSync()
        {
            EnemyAbilityManager.SendEvent(Ability.SyncID, Agent.GlobalID, AbilityPacketType.DoTrigger);
        }

        public void DoTrigger()
        {
            DoEnter();
        }

        public void DoEnterSync()
        {
            EnemyAbilityManager.SendEvent(Ability.SyncID, Agent.GlobalID, AbilityPacketType.DoTrigger);
        }

        public void DoEnter()
        {
            if (IsHostOnlyBehaviour && !SNet.IsMaster)
                return;

            if (Executing)
                return;

            Executing = true;
            OnEnter();
        }
        
        public void DoExitSync()
        {
            EnemyAbilityManager.SendEvent(Ability.SyncID, Agent.GlobalID, AbilityPacketType.DoExit);
        }

        public void DoExit()
        {
            if (IsHostOnlyBehaviour && !SNet.IsMaster)
                return;

            if (!Executing)
                return;

            Executing = false;
            OnExit();
        }

        private void DoTakeDamage(float damage)
        {
            if (IsHostOnlyBehaviour && !SNet.IsMaster)
                return;

            OnTakeDamage(damage);
        }

        private void DoHitreact()
        {
            if (IsHostOnlyBehaviour && !SNet.IsMaster)
                return;

            OnHitreact();
        }

        private void DoLimbDestroyed(Dam_EnemyDamageLimb limb)
        {
            if (IsHostOnlyBehaviour && !SNet.IsMaster)
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
    }
}