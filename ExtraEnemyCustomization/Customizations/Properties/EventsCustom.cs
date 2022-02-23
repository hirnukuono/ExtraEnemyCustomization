using Enemies;
using GameData;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Properties
{
    public sealed class EventsCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public EventSetting OnSpawnedEvent { get; set; } = new();
        public EventSetting OnDeadEvent { get; set; } = new();
        public bool TriggerOnBossDeathEventOnDead { get; set; } = false;

        public override string GetProcessName()
        {
            return "Events";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            if (OnSpawnedEvent?.Enabled ?? false)
            {
                OnSpawnedEvent.FireEvents();
            }

            if (OnDeadEvent?.Enabled ?? false)
            {
                agent.AddOnDeadOnce(OnDeadEvent.FireEvents);
            }

            if (TriggerOnBossDeathEventOnDead)
            {
                var events = agent.CourseNode.m_zone.m_settings.m_zoneData.EventsOnBossDeath;
                agent.AddOnDeadOnce(() =>
                {
                    WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(events, eWardenObjectiveEventTrigger.None, ignoreTrigger: true, 0f);
                });
            }
        }
    }

    public class EventSetting
    {
        public bool Enabled { get; set; } = false;
        public WardenObjectiveEventData[] Events { get; set; } = Array.Empty<WardenObjectiveEventData>();

        public void FireEvents()
        {
            foreach (var e in Events)
            {
                WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(e, eWardenObjectiveEventTrigger.None, ignoreTrigger: true, 0f);
            }
        }
    }
}
