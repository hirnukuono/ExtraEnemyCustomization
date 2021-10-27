using EECustom.Customizations;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Managers
{
    public partial class ConfigManager
    {
        private EnemyCustomBase[] _EnemyPrefabBuiltEvents = new EnemyCustomBase[0];
        private EnemyCustomBase[] _EnemySpawnedEvents = new EnemyCustomBase[0];
        private EnemyCustomBase[] _EnemyDespawnedEvents = new EnemyCustomBase[0];
        private EnemyCustomBase[] _EnemyGlowEvents = new EnemyCustomBase[0];

        private void GenerateEventBuffer()
        {
            List<EnemyCustomBase>
                prefabBuilt = new List<EnemyCustomBase>(),
                spawned = new List<EnemyCustomBase>(),
                despawned = new List<EnemyCustomBase>(),
                glow = new List<EnemyCustomBase>();

            foreach (var custom in _CustomizationBuffer)
            {
                if (custom is IEnemyPrefabBuiltEvent)
                    prefabBuilt.Add(custom);

                if (custom is IEnemySpawnedEvent)
                    spawned.Add(custom);

                if (custom is IEnemyDespawnedEvent)
                    despawned.Add(custom);

                if (custom is IEnemyGlowEvent)
                    glow.Add(custom);
            }

            _EnemyPrefabBuiltEvents = prefabBuilt.ToArray();
            _EnemySpawnedEvents = spawned.ToArray();
            _EnemyDespawnedEvents = despawned.ToArray();
            _EnemyGlowEvents = glow.ToArray();
        }

        internal void FirePrefabBuiltEvent(EnemyAgent agent)
        {
            FireEventPreSpawn<IEnemyPrefabBuiltEvent>(_EnemyPrefabBuiltEvents, agent, "PrefabBuilt", (e) =>
            {
                e.OnPrefabBuilt(agent);
            });
        }

        internal void FireSpawnedEvent(EnemyAgent agent)
        {
            FireEvent<IEnemySpawnedEvent>(_EnemySpawnedEvents, agent, "Spawned", (e) =>
            {
                e.OnSpawned(agent);
            });
        }

        internal void FireDespawnedEvent(EnemyAgent agent)
        {
            FireEvent<IEnemyDespawnedEvent>(_EnemyDespawnedEvents, agent, "Despawned", (e) =>
            {
                e.OnDespawned(agent);
            });
        }

        internal bool FireGlowEvent(EnemyAgent agent, ref GlowInfo glowInfo)
        {
            bool altered = false;

            for (int i = 0; i < _EnemyGlowEvents.Length; i++)
            {
                var custom = _EnemyGlowEvents[i];

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

        private void FireEventPreSpawn<T>(EnemyCustomBase[] handlers, EnemyAgent agent, string eventName, Action<T> doAction) where T : class
        {
            for (int i = 0; i < handlers.Length; i++)
            {
                var custom = handlers[i];

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

        private void FireEvent<T>(EnemyCustomBase[] handlers, EnemyAgent agent, string eventName, Action<T> doAction) where T : class
        {
            for (int i = 0; i < handlers.Length; i++)
            {
                var custom = handlers[i];

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
