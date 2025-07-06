using Agents;
using EEC.Utils.Json.Elements;
using Enemies;
using GameData;
using System;

namespace EEC.EnemyCustomizations.Properties
{
    public sealed class EventsCustom : EnemyCustomBase, IEnemySpawnedEvent, IEnemyDeadEvent, IEnemyAgentModeEvent
    {
        public EventSetting OnSpawnedEvent { get; set; } = new();
        public EventSetting OnWakeupEvent { get; set; } = new();
        public EventSetting OnDeadEvent { get; set; } = new();
        public List<uint> LevelLayoutIDs { get; set; } = new();
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
            if (LevelLayoutIDs.Count > 0 && !LevelLayoutIDs.Contains(RundownManager.ActiveExpedition.LevelLayoutData))
                return;

            if (OnSpawnedEvent?.Enabled ?? false)
            {
                OnSpawnedEvent.FireEvents();
            }

            if (TriggerOnBossDeathEventOnDead)
            {
                var spawnedNode = agent.GetSpawnedNode();
                if (spawnedNode == null)
                    return;

                if (spawnedNode.m_zone == null)
                    return;

                if (spawnedNode.m_zone.m_settings == null)
                    return;

                var zoneSetting = spawnedNode.m_zone.m_settings;
                var events = zoneSetting?.m_zoneData.EventsOnBossDeath ?? null;
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

        public void OnDead(EnemyAgent agent)
        {
            if (OnDeadEvent?.Enabled ?? false)
            {
                OnDeadEvent.FireEvents();
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