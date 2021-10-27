using EECustom.Customizations;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Managers
{
    public partial class ConfigManager
    {
        private IEnemyPrefabBuiltEvent[] _EnemyPrefabBuiltEvents = new IEnemyPrefabBuiltEvent[0];
        private IEnemySpawnedEvent[] _EnemySpawnedEvents = new IEnemySpawnedEvent[0];
        private IEnemyDespawnedEvent[] _EnemyDespawnedEvents = new IEnemyDespawnedEvent[0];
        private IEnemyGlowEvent[] _EnemyGlowEvents = new IEnemyGlowEvent[0];

        private void GenerateEventBuffer()
        {
            var prefabBuilt = new List<IEnemyPrefabBuiltEvent>();
            var spawned = new List<IEnemySpawnedEvent>();
            var despawned = new List<IEnemyDespawnedEvent>();
            var glow = new List<IEnemyGlowEvent>();

            foreach (var custom in _CustomizationBuffer)
            {
                if (custom is IEnemyPrefabBuiltEvent e1)
                    prefabBuilt.Add(e1);

                if (custom is IEnemySpawnedEvent e2)
                    spawned.Add(e2);

                if (custom is IEnemyDespawnedEvent e3)
                    despawned.Add(e3);

                if (custom is IEnemyGlowEvent e4)
                    glow.Add(e4);
            }

            _EnemyPrefabBuiltEvents = prefabBuilt.ToArray();
            _EnemySpawnedEvents = spawned.ToArray();
            _EnemyDespawnedEvents = despawned.ToArray();
            _EnemyGlowEvents = glow.ToArray();
        }

        internal void FirePrefabBuiltEvent(EnemyAgent agent)
        {
            FireEventPreSpawn(_EnemyPrefabBuiltEvents, agent, "PrefabBuilt", (e) =>
            {
                e.OnPrefabBuilt(agent);
            });
        }

        internal void FireSpawnedEvent(EnemyAgent agent)
        {
            FireEvent(_EnemySpawnedEvents, agent, "Spawned", (e) =>
            {
                e.OnSpawned(agent);
            });
        }

        internal void FireDespawnedEvent(EnemyAgent agent)
        {
            FireEvent(_EnemyDespawnedEvents, agent, "Despawned", (e) =>
            {
                e.OnDespawned(agent);
            });
        }

        internal bool FireGlowEvent(EnemyAgent agent, ref GlowInfo glowInfo)
        {
            bool altered = false;

            for (int i = 0; i < _EnemyGlowEvents.Length; i++)
            {
                var custom = _EnemyGlowEvents[i].Base;

                if (!custom.Enabled)
                    continue;

                if (custom.IsTarget(agent))
                {
                    var newGlowInfo = new GlowInfo(glowInfo.Color, glowInfo.Position);
                    if (((IEnemyGlowEvent)custom).OnGlow(agent, ref newGlowInfo))
                    {
                        glowInfo = newGlowInfo;
                        altered = true;
                    }
                }
            }

            return altered;
        }

        private void FireEventPreSpawn<T>(T[] handlers, EnemyAgent agent, string eventName, Action<T> doAction) where T : class, IEnemyEvent
        {
            for (int i = 0; i < handlers.Length; i++)
            {
                var custom = handlers[i].Base;

                if (!custom.Enabled)
                    continue;

                if (custom.Target.IsMatch(agent))
                {
                    custom.LogDev($"Apply {eventName} Event: {agent.name}");
                    doAction?.Invoke(custom as T);
                    custom.LogVerbose($"Finished!");
                }
            }
        }

        private void FireEvent<T>(T[] handlers, EnemyAgent agent, string eventName, Action<T> doAction) where T : class, IEnemyEvent
        {
            for (int i = 0; i < handlers.Length; i++)
            {
                var custom = handlers[i].Base;

                if (!custom.Enabled)
                    continue;

                if (custom.IsTarget(agent))
                {
                    custom.LogDev($"Apply {eventName} Event: {agent.name}");
                    doAction?.Invoke(custom as T);
                    custom.LogVerbose($"Finished!");
                }
            }
        }
    }
}
