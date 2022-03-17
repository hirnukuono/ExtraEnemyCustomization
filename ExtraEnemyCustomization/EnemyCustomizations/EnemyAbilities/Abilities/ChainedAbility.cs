using EEC.Utils;
using System;
using System.Collections.Generic;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public class ChainedAbility : AbilityBase<ChainedBehaviour>
    {
        public EventBlock[] Abilities { get; set; } = Array.Empty<EventBlock>();

        public float ExitDelay { get; set; } = 0.0f;
        public bool ExitAllInForceExit { get; set; } = true;
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
            public Timer TriggerTimer;
            public bool Triggered = false;
        }
    }

    public class ChainedBehaviour : AbilityBehaviour<ChainedAbility>
    {
        public override bool RunUpdateOnlyWhileExecuting => true;
        public override bool AllowEABAbilityWhileExecuting => true;
        public override bool IsHostOnlyBehaviour => true;

        private Timer _endTimer;
        private bool _waitingEndTimer = false;

        protected override void OnSetup()
        {
            foreach (var abSetting in Ability.Abilities)
            {
                _ = abSetting.Ability.RegisterBehaviour(Agent);
            }
        }

        protected override void OnEnter()
        {
            foreach (var abSetting in Ability.Abilities)
            {
                abSetting.TriggerTimer.Reset(abSetting.Delay);
                abSetting.Triggered = false;
            }

            _waitingEndTimer = false;
        }

        protected override void OnUpdate()
        {
            var isAllDone = true;

            foreach (var abSetting in Ability.Abilities)
            {
                if (!abSetting.Triggered)
                {
                    if (abSetting.TriggerTimer.TickAndCheckDone())
                    {
                        abSetting.Ability.TriggerSync(Agent);
                        abSetting.Triggered = true;
                    }
                    else
                    {
                        isAllDone = false;
                    }
                }
            }

            if (isAllDone)
            {
                if (!_waitingEndTimer)
                {
                    _endTimer.Reset(Ability.ExitDelay);
                    _waitingEndTimer = true;
                }
                else if (_waitingEndTimer && _endTimer.TickAndCheckDone())
                {
                    DoExit();
                }
            }
        }

        protected override void OnExit()
        {
            if (Ability.ExitAllInForceExit)
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
    }
}