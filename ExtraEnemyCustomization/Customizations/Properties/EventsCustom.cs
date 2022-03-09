using Agents;
using EECustom.Utils.JsonElements;
using Enemies;
using GameData;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Properties
{
    public sealed class EventsCustom : EnemyCustomBase, IEnemySpawnedEvent, IEnemyAgentModeEvent
    {
        public EventSetting OnSpawnedEvent { get; set; } = new();
        public EventSetting OnWakeupEvent { get; set; } = new();
        public EventSetting OnDeadEvent { get; set; } = new();
        public bool TriggerOnBossDeathEventOnDead { get; set; } = false;

        public override string GetProcessName()
        {
            return "Events";
        }

        public override void OnAssetLoaded()
        {
            OnSpawnedEvent.CacheAll();
            OnWakeupEvent.CacheAll();
            OnDeadEvent.CacheAll();
        }

        public override void OnConfigUnloaded()
        {
            OnSpawnedEvent.DisposeAll();
            OnWakeupEvent.DisposeAll();
            OnDeadEvent.DisposeAll();
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
                var spawnedNode = agent.GetSpawnedNode();
                if (spawnedNode == null)
                    return;

                var events = spawnedNode?.m_zone?.m_settings?.m_zoneData?.EventsOnBossDeath ?? null;
                if (events != null)
                {
                    agent.AddOnDeadOnce(() =>
                    {
                        WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(events, eWardenObjectiveEventTrigger.None, ignoreTrigger: true, 0f);
                    });
                }
            }
        }

        public void OnAgentModeChanged(EnemyAgent agent, AgentMode newMode)
        {
            if (newMode != AgentMode.Agressive)
                return;

            if (OnWakeupEvent?.Enabled ?? false)
            {
                OnWakeupEvent.FireEvents();
            }
        }

        public sealed class EventSetting
        {
            public bool Enabled { get; set; } = false;
            public EventWrapper[] Events { get; set; } = Array.Empty<EventWrapper>();

            public void CacheAll()
            {
                if (Events == null)
                    return;

                foreach (var e in Events)
                {
                    e.Cache();
                }
            }

            public void FireEvents()
            {
                if (Events == null)
                    return;

                foreach (var e in Events)
                {
                    WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(e.ToEvent(), eWardenObjectiveEventTrigger.None, ignoreTrigger: true, 0f);
                }
            }

            public void DisposeAll()
            {
                if (Events == null)
                    return;

                foreach (var e in Events)
                {
                    e.Dispose();
                }

                Events = null;
            }
        }
    }
}
