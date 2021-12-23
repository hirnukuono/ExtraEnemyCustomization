using EECustom.Utils;
using Enemies;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public class DoAnimAbility : AbilityBase<DoAnimBehaviour>
    {
        public EnemyAnimType Animation { get; set; } = EnemyAnimType.Screams;
        public uint SoundEvent { get; set; } = 0u;
        public uint VoiceEvent { get; set; } = 0u;
        public float Duration { get; set; } = 1.0f;
        public float CrossFadeTime { get; set; } = 0.0f;
        public bool AllowUsingEABWhileExecuting { get; set; } = false;
    }

    public class DoAnimBehaviour : AbilityBehaviour<DoAnimAbility>
    {
        public override bool AllowEABAbilityWhileExecuting => Ability.AllowUsingEABWhileExecuting;
        public override bool IsHostOnlyBehaviour => false;

        private Animator _animator;
        private NavMeshAgent _navAgent;
        private float _exitTimer = 0.0f;

        protected override void OnSetup()
        {
            _animator = Agent.Locomotion.m_animator;
            _navAgent = Agent.AI.m_navMeshAgent.Cast<NavMeshAgent>();
        }

        protected override void OnDead()
        {
            DoExit();
        }

        protected override void OnEnter()
        {
            _exitTimer = Clock.Time + Ability.Duration;

            EnemyAnimUtil.DoAnimationLocal(Agent, Ability.Animation, Ability.CrossFadeTime, true);

            if (Ability.SoundEvent != 0u)
            {
                Agent.Sound.Post(Ability.SoundEvent);
            }

            if (Ability.VoiceEvent != 0u)
            {
                Agent.Voice.PlayVoiceEvent(Ability.VoiceEvent);
            }
        }

        protected override void OnUpdate()
        {
            if (Clock.Time >= _exitTimer)
            {
                DoExit();
            }
        }

        protected override void OnExit()
        {
            _animator.applyRootMotion = false;
            if (_navAgent.isOnNavMesh)
            {
                _navAgent.isStopped = false;
            }
        }
    }
}
