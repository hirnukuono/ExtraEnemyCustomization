using EEC.Utils.Unity;
using Timer = EEC.Utils.Unity.Timer;
using System;
using System.Collections.Generic;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public sealed class ChainedAbility : AbilityBase<ChainedBehaviour>
    {
        public EventBlock[] Abilities { get; set; } = Array.Empty<EventBlock>();

        public float ExitDelay { get; set; } = 0.0f;
        public bool ExitWhenAllFinished { get; set; } = true;
        public bool ExitAllInForceExit { get; set; } = true;
        public bool ExitAllInForceExitOnly { get; set; } = false;
        public bool ForceExitOnHitreact { get; set; } = false;
        public bool ForceExitOnDead { get; set; } = false;
        public bool ForceExitOnLimbDestroy { get; set; } = false;

        public override void OnAbilityLoaded()
        {
            var tempList = new List<EventBlock>(Abilities);
            foreach (var ab in Abilities)
            {
                if (!ab.TryCache())
                {
                    LogError($"Key: [{ab.AbilityName}] was missing, unable to apply ability!");
                    tempList.Remove(ab);
                    continue;
                }
                else
                {
                    LogVerbose($"Ability was assigned! name: {ab.AbilityName} delay: {ab.Delay}");
                }
            }

            Abilities = tempList.ToArray();
        }

        public class EventBlock : AbilitySettingBase
        {
            public float Delay { get; set; } = 0.0f;
        }
    }

    public sealed class ChainedBehaviour : AbilityBehaviour<ChainedAbility>
    {
        public override bool RunUpdateOnlyWhileExecuting => true;
        public override bool AllowEABAbilityWhileExecuting => true;
        public override bool IsHostOnlyBehaviour => true;
        public override bool IsHostOnlySetup => false;

        private EventBlockBehaviour[] _blockBehaviours = Array.Empty<EventBlockBehaviour>();
        private Timer _endTimer;
        private bool _waitingEndTimer = false;
        private bool _allFinished = false;
        private bool _forceExit = false;

        protected override void OnSetup()
        {
            if (SNetwork.SNet.IsMaster)
            {
                _blockBehaviours = new EventBlockBehaviour[Ability.Abilities.Length];
                for (int i = 0; i < Ability.Abilities.Length; i++)
                {
                    var ability = Ability.Abilities[i];
                    _blockBehaviours[i] = new(ability, ability.Ability.RegisterBehaviour(Agent));
                }
            }
            else // Clients need to register the behaviours, but only host needs to cache them
            {
                foreach (var abSetting in Ability.Abilities)
                {
                    abSetting.Ability.RegisterBehaviour(Agent);
                }
            }
            
        }

        protected override void OnEnter()
        {
            foreach (var block in _blockBehaviours)
            {
                block.TriggerTimer.Reset(block.AbSetting.Delay);
                block.Triggered = false;
            }

            _waitingEndTimer = false;
            _forceExit = true;
        }

        protected override void OnUpdate()
        {
            var isAllDone = true;

            foreach (var block in _blockBehaviours)
            {
                if (!block.Triggered)
                {
                    if (block.TriggerTimer.TickAndCheckDone())
                    {
                        block.AbSetting.Ability.TriggerSync(Agent);
                        block.Triggered = true;
                    }
                    else
                    {
                        isAllDone = false;
                    }
                }
            }

            if (isAllDone)
            {
                if (!_allFinished && Ability.ExitWhenAllFinished)
                {
                    foreach (var block in _blockBehaviours)
                    {
                        if (block.Behaviour.Executing)
                            return;
                    }
                    _allFinished = true;
                }

                if (!_waitingEndTimer)
                {
                    _endTimer.Reset(Ability.ExitDelay);
                    _waitingEndTimer = true;
                }
                else if (_waitingEndTimer && _endTimer.TickAndCheckDone())
                {
                    _forceExit = false;
                    DoExit();
                }
            }
        }

        protected override void OnExit()
        {
            if ((Ability.ExitAllInForceExit && !Ability.ExitAllInForceExitOnly) || (_forceExit && Ability.ExitAllInForceExitOnly))
            {
                foreach (var abSetting in Ability.Abilities)
                {
                    abSetting.Ability.ExitSync(Agent);
                }
            }
        }

        protected override void OnHitreact()
        {
            if (Ability.ForceExitOnHitreact)
            {
                DoExit();
            }
        }

        protected override void OnLimbDestroyed(Dam_EnemyDamageLimb _)
        {
            if (Ability.ForceExitOnLimbDestroy)
            {
                DoExit();
            }
        }

        protected override void OnDead()
        {
            if (Ability.ForceExitOnDead)
            {
                DoExit();
            }
        }

        public class EventBlockBehaviour
        {
            public readonly ChainedAbility.EventBlock AbSetting;
            public readonly AbilityBehaviour Behaviour;
            public Timer TriggerTimer;
            public bool Triggered = false;

            public EventBlockBehaviour(ChainedAbility.EventBlock ability, AbilityBehaviour behaviour)
            {
                AbSetting = ability;
                Behaviour = behaviour;
            }
        }
    }
}