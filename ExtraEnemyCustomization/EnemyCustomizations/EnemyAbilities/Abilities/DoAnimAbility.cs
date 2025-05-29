using EEC.Utils;
using EEC.Utils.Unity;
using Timer = EEC.Utils.Unity.Timer;
using UnityEngine;
using UnityEngine.AI;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public sealed class DoAnimAbility : AbilityBase<DoAnimBehaviour>
    {
        public EnemyAnimType Animation { get; set; } = EnemyAnimType.Screams;
        public uint SoundEvent { get; set; } = 0u;
        public uint VoiceEvent { get; set; } = 0u;
        public float Duration { get; set; } = 1.0f;
        public float CrossFadeTime { get; set; } = 0.0f;
        public bool AllowUsingEABWhileExecuting { get; set; } = false;
        public bool StandStill { get; set; } = true;
    }

    public sealed class DoAnimBehaviour : AbilityBehaviour<DoAnimAbility>
    {
        public override bool RunUpdateOnlyWhileExecuting => true;
        public override bool AllowEABAbilityWhileExecuting => Ability.AllowUsingEABWhileExecuting;
        public override bool IsHostOnlyBehaviour => false;

        private Animator _animator;
        private INavigation _navAgent;
        private Timer _exitTimer;

        protected override void OnSetup()
        {
            _animator = Agent.Locomotion.m_animator;
            _navAgent = Agent.AI.m_navMeshAgent;
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
            if (_navAgent.isOnNavMesh && !Agent.Damage.IsStuckInGlue)
            {
                _navAgent.isStopped = false;
            }
        }
    }
}