using EECustom.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace EECustom.EnemyCustomizations.EnemyAbilities.Abilities
{
    public class DoAnimAbility : AbilityBase<DoAnimBehaviour>
    {
        public EnemyAnimType Animation { get; set; } = EnemyAnimType.Screams;
        public uint SoundEvent { get; set; } = 0u;
        public uint VoiceEvent { get; set; } = 0u;
        public float Duration { get; set; } = 1.0f;
        public float CrossFadeTime { get; set; } = 0.0f;
        public bool AllowUsingEABWhileExecuting { get; set; } = false;
        public bool StandStill { get; set; } = true;
    }

    public class DoAnimBehaviour : AbilityBehaviour<DoAnimAbility>
    {
        public override bool RunUpdateOnlyWhileExecuting => true;
        public override bool AllowEABAbilityWhileExecuting => Ability.AllowUsingEABWhileExecuting;
        public override bool IsHostOnlyBehaviour => false;

        private Animator _animator;
        private NavMeshAgent _navAgent;
        private Timer _exitTimer;

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
            StandStill = Ability.StandStill;

            _exitTimer.Reset(Ability.Duration);

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
            if (_exitTimer.TickAndCheckDone())
            {
                DoExit();
            }
        }

        protected override void OnExit()
        {
            StandStill = false;

            _animator.applyRootMotion = false;
            if (_navAgent.isOnNavMesh)
            {
                _navAgent.isStopped = false;
            }
        }
    }
}