using EEC.Utils.Json.Elements;
using GameData;
using GTFO.API;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public class WardenEventAbility : AbilityBase<WardenEventBehaviour>
    {
        public EventWrapper[] Events { get; set; } = Array.Empty<EventWrapper>();
        public List<uint> LevelLayoutIDs { get; set; } = new();
        public int TriggerCount { get; set; } = -1;
        public int GlobalTriggerCount { get; set; } = -1;

        private int _remainingGlobalTriggers = -1;

        public WardenEventAbility()
        {
            LevelAPI.OnLevelCleanup += OnLevelCleanup;
        }

        public override void OnAbilityLoaded()
        {
            foreach (var e in Events)
            {
                e.Cache();
            }
            _remainingGlobalTriggers = GlobalTriggerCount;
        }

        private void OnLevelCleanup()
        {
            _remainingGlobalTriggers = GlobalTriggerCount;
        }

        public override void OnAbilityUnloaded()
        {
            foreach (var e in Events)
            {
                e.Dispose();
            }
        }

        public bool TryConsumeGlobalTrigger()
        {
            if (_remainingGlobalTriggers == 0) return false;
            if (_remainingGlobalTriggers > 0) _remainingGlobalTriggers--;
            return true;
        }
    }

    public sealed class WardenEventBehaviour : AbilityBehaviour<WardenEventAbility>
    {
        public override bool RunUpdateOnlyWhileExecuting => true;
        public override bool AllowEABAbilityWhileExecuting => true;
        public override bool IsHostOnlyBehaviour => false;

        private int _remainingTriggers;

        protected override void OnSetup()
        {
            _remainingTriggers = Ability.TriggerCount;
        }

        protected override void OnEnter()
        {
            if (GameStateManager.CurrentStateName != eGameStateName.InLevel || Ability.Events == null)
                return;
            if (Ability.LevelLayoutIDs.Count > 0 && !Ability.LevelLayoutIDs.Contains(RundownManager.ActiveExpedition.LevelLayoutData))
                return;
            if (!Ability.TryConsumeGlobalTrigger() || _remainingTriggers == 0)
                return;

            foreach (var e in Ability.Events)
            {
                WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(e.ToEvent(), eWardenObjectiveEventTrigger.None, true, 0f);
            }

            if (_remainingTriggers > 0)
                _remainingTriggers--;
        }
    }
}
