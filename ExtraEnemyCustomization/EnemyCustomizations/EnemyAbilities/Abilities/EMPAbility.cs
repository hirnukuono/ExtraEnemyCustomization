﻿using EEC.CustomAbilities.EMP;
using EEC.Utils;
using EEC.Utils.Unity;
using Timer = EEC.Utils.Unity.Timer;
using UnityEngine;
using UnityEngine.AI;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public sealed class EMPAbility : AbilityBase<EMPBehaviour>
    {
        public uint ChargeUpSoundId { get; set; } = 0u;
        public uint ActivateSoundId { get; set; } = 0u;
        public EnemyAnimType ChargeUpAnimation { get; set; } = EnemyAnimType.AbilityUse;
        public EnemyAnimType ActivateAnimation { get; set; } = EnemyAnimType.AbilityUseOut;
        public float ChargeUpDuration { get; set; } = 3;
        public float EffectDuration { get; set; } = 30;
        public float ActivateDuration { get; set; } = 5;
        public float EffectRange { get; set; } = 20;
        public bool InvincibleWhileCharging { get; set; } = true;
        public Color BuildupColor { get; set; } = new Color(0.525f, 0.956f, 0.886f, 1.0f) * 2.0f;
        public Color ScreamColor { get; set; } = new Color(0.525f, 0.956f, 0.886f, 1.0f) * 20.0f;
    }

    public sealed class EMPBehaviour : AbilityBehaviour<EMPAbility>
    {
        private EMPState _state = EMPState.None;
        private Timer _stateTimer;
        private INavigation _navAgent;

        public override bool RunUpdateOnlyWhileExecuting => true;
        public override bool AllowEABAbilityWhileExecuting => false;
        public override bool IsHostOnlyBehaviour => false;

        protected override void OnSetup()
        {
            _navAgent = Agent.AI.m_navMeshAgent;
        }

        protected override void OnEnter()
        {
            StandStill = true;

            _state = EMPState.BuildUp;
            _stateTimer.Reset(Ability.ChargeUpDuration);

            if (Ability.ChargeUpSoundId != 0u)
                Agent.Sound.Post(Ability.ChargeUpSoundId);

            if (Ability.InvincibleWhileCharging)
                Agent.Damage.IsImortal = true;

            Agent.Appearance.InterpolateGlow(Ability.BuildupColor, Ability.ChargeUpDuration);
            EnemyAnimUtil.DoAnimationLocal(Agent, Ability.ChargeUpAnimation, 0.15f, true);
        }

        protected override void OnUpdate()
        {
            switch (_state)
            {
                case EMPState.BuildUp:
                    if (_stateTimer.TickAndCheckDone())
                    {
                        Agent.Sound.Post(Ability.ActivateSoundId);
                        Agent.Appearance.InterpolateGlow(Ability.ScreamColor, 0.5f);
                        EnemyAnimUtil.DoAnimationLocal(Agent, Ability.ActivateAnimation, 0.15f, true);
                        EMPManager.Activate(Agent.Position, Ability.EffectRange, Ability.EffectDuration);

                        _state = EMPState.AbilityUsed;
                        _stateTimer.Reset(Ability.ActivateDuration);
                    }
                    break;

                case EMPState.AbilityUsed:
                    if (_stateTimer.TickAndCheckDone())
                    {
                        _state = EMPState.Done;
                        _stateTimer.Reset(0.0f);
                    }
                    break;

                case EMPState.Done:
                    DoExit();
                    break;
            }
        }

        protected override void OnExit()
        {
            StandStill = false;
            if (_navAgent.isOnNavMesh && !Agent.Damage.IsStuckInGlue)
                _navAgent.isStopped = false;

            if (Ability.InvincibleWhileCharging)
                Agent.Damage.IsImortal = false;
            Agent.Appearance.InterpolateGlow(Color.black, 0.5f);

            _state = EMPState.None;
            _stateTimer.Reset(0.0f);
        }

        protected override void OnDead()
        {
            DoExit();
        }

        private enum EMPState
        {
            None,
            BuildUp,
            AbilityUsed,
            Done
        }
    }
}