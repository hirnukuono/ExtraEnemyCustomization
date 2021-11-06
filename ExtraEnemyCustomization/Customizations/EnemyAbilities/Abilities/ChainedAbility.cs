using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public class ChainedAbility : AbilityBase<ExplosionBehaviour>
    {
        public EventBlock[] Abilities { get; set; } = new EventBlock[0];
        public bool WaitForLastAbilityDone { get; set; } = false;

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
                    LogVerbose($"Ability was assigned! name: {ab.AbilityName} mode: {ab.Mode} delay: {ab.Delay}");
                }
            }

            Abilities = tempList.ToArray();
        }

        public class EventBlock : AbilitySettingBase
        {
            public WaitingBehaviour Mode { get; set; } = WaitingBehaviour.WaitForDelay;
            public float Delay { get; set; } = 0.0f;
        }

        public enum WaitingBehaviour
        {
            Instantly,
            WaitForDelay,
            WaitForPreviousAbility,
            WaitForPreviousAbilityThenDelay
        }
    }

    public class ChainedBehaviour : AbilityBehaviour<ChainedAbility>
    {
        public override bool AllowEABAbilityWhileExecuting => false;
        public override bool IsHostOnlyBehaviour => true;

        private ChainedAbility.EventBlock _currentAbility = null;
        private ChainedAbility.EventBlock _previousAbility = null;
        private AbilityBehaviour _currentBehaviour = null;
        private AbilityBehaviour _previousBehaviour = null;
        private int _currentIdx = 0;

        protected override void OnSetup()
        {
            foreach (var abSetting in Ability.Abilities)
            {
                _ = abSetting.Ability.RegisterBehaviour(Agent);
            }
        }

        protected override void OnEnter()
        {
            _previousBehaviour = null;
            _currentIdx = 0;

            if (Ability.Abilities.Length < 1)
            {
                LogError("Ability Count was zero! Unable to start ChainedAbility!");
                DoExit();
                return;
            }

            if (TryGetCurrentBehaviour(out var behaviour))
            {
                //They immediately triggered on host's side, it's safe to check Executing Property!
                behaviour.DoTriggerSync();
                _previousBehaviour = behaviour;
                _currentBehaviour = behaviour;
            }
            else
            {
                LogError($"AbilityBehaviour was not assigned! : {Ability.Abilities[_currentIdx].AbilityName}");
                DoExit();
                return;
            }

            _currentIdx++;
        }

        protected override void OnUpdate()
        {
            if (_currentIdx < Ability.Abilities.Length)
            {

            }

            switch (_currentAbility)
            {

            }
            DoExit();
        }

        private bool TryGetCurrentBehaviour(out AbilityBehaviour behaviour)
        {
            return Ability.Abilities[_currentIdx].Ability.TryGetBehaviour(Agent, out behaviour);
        }
    }
}
