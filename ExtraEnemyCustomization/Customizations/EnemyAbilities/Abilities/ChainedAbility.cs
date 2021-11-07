using System.Collections.Generic;
using System.Linq;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public class ChainedAbility : AbilityBase<ExplosionBehaviour>
    {
        public const string DefaultGroupName = "$__DefaultGroup__";

        public EventBlock[] Abilities { get; set; } = new EventBlock[0];

        //TODO: Implement This ffs
        public bool WaitForLastAbilityDone { get; set; } = false;
        public bool ExitAllInForceExit { get; set; } = true;

        public EventGroup[] Groups = new EventGroup[0];

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
                    LogVerbose($"Ability was assigned! name: {ab.AbilityName} group: {ab.Group} waitfor: {ab.GroupToWait} delay: {ab.Delay}");
                }
            }


            var tempGroupList = new List<EventGroup>();
            tempGroupList.Add(new EventGroup()
            {
                EventsToWait = new(),
                Events = new(),
                Group = DefaultGroupName,
                GroupToWait = string.Empty
            });
            foreach (var ab in tempList)
            {
                List<EventBlock> tempWaitingGroupEvents;
                if (string.IsNullOrEmpty(ab.GroupToWait))
                {
                    ab.Group = DefaultGroupName;
                    tempWaitingGroupEvents = new List<EventBlock>();
                }
                else
                {
                    tempWaitingGroupEvents = tempList.Where(x => ab.GroupToWait == x.Group)
                    .ToList();

                    if (tempWaitingGroupEvents.Count <= 0)
                    {
                        LogError($"There were no group data with: [{ab.GroupToWait}] assigning it to Default!");
                        ab.Group = DefaultGroupName;
                    }
                }
                

                var entry = tempGroupList.SingleOrDefault(x => x.Group == ab.Group && x.GroupToWait == ab.GroupToWait);
                if (entry == null)
                {
                    tempGroupList.Add(new EventGroup()
                    {
                        Events = new(),
                        EventsToWait = tempWaitingGroupEvents,
                        Group = ab.Group,
                        GroupToWait = ab.GroupToWait
                    });
                }
                else
                {
                    entry.Events.Add(ab);
                }
            }

            Abilities = tempList.ToArray();
            Groups = tempGroupList.ToArray();
        }

        public class EventBlock : AbilitySettingBase
        {
            public string Group { get; set; } = string.Empty;
            public string GroupToWait { get; set; } = string.Empty;
            public float Delay { get; set; } = 0.0f;

            public EventState State = EventState.None;
            public float TriggerTimer = 0.0f;
            public AbilityBehaviour RunningBehaviour = null;

            public enum EventState
            {
                None,
                WaitingForDelay,
                Executing,
                Done
            }
        }

        public class EventGroup
        {
            public string Group;
            public string GroupToWait;
            public List<EventBlock> Events;
            public List<EventBlock> EventsToWait;

            public bool IsDoneExcuting = false;

            public bool CanExecute
            {
                get
                {
                    if (EventsToWait.Count <= 0)
                        return true;

                    return EventsToWait.TrueForAll(x => x.State == EventBlock.EventState.Done);
                }
            }
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

        protected override void OnSetup()
        {
            foreach (var abSetting in Ability.Abilities)
            {
                _ = abSetting.Ability.RegisterBehaviour(Agent);
            }

            foreach (var abGroup in Ability.Groups)
            {
                abGroup.IsDoneExcuting = false;

                foreach (var e in abGroup.Events)
                {
                    e.State = ChainedAbility.EventBlock.EventState.None;
                    e.TriggerTimer = 0.0f;
                }
            }
        }

        protected override void OnEnter()
        {
            if (Ability.Abilities.Length < 1)
            {
                LogError("Ability Count was zero! Unable to start ChainedAbility!");
                DoExit();
                return;
            }
        }

        protected override void OnUpdate()
        {
            var isAllDone = true;
            foreach (var group in Ability.Groups)
            {
                var isGroupDone = false;
                if (group.IsDoneExcuting)
                {
                    isGroupDone = true;
                    continue;
                }

                if (!group.CanExecute)
                {
                    isAllDone = false;
                    continue;
                }

                foreach (var e in group.Events)
                {
                    var isDone = false;
                    switch (e.State)
                    {
                        case ChainedAbility.EventBlock.EventState.None:
                            e.TriggerTimer = Clock.Time + e.Delay;
                            e.State = ChainedAbility.EventBlock.EventState.WaitingForDelay;
                            break;

                        case ChainedAbility.EventBlock.EventState.WaitingForDelay:
                            if (Clock.Time <= e.TriggerTimer)
                            {
                                if (e.Ability.TryGetBehaviour(Agent, out var behaviour))
                                {
                                    behaviour.DoEnterSync();
                                    e.RunningBehaviour = behaviour;
                                    e.State = ChainedAbility.EventBlock.EventState.Executing;
                                }
                                else
                                {
                                    LogWarning($"Unable to get Behaviour from [{e.Ability}]");
                                    e.State = ChainedAbility.EventBlock.EventState.Done;
                                }
                            }
                            break;

                        case ChainedAbility.EventBlock.EventState.Executing:
                            if (!e.RunningBehaviour.Executing)
                            {
                                e.State = ChainedAbility.EventBlock.EventState.Done;
                            }
                            break;

                        case ChainedAbility.EventBlock.EventState.Done:
                            isDone = true;
                            break;
                    }
                    isGroupDone &= isDone;
                }

                if (isGroupDone)
                {
                    group.IsDoneExcuting = true;
                }

                isAllDone &= isGroupDone;
            }

            if (isAllDone)
            {
                DoExit();
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
    }
}
