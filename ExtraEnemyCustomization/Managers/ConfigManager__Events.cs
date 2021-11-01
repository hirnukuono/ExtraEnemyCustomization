using EECustom.Customizations;
using Enemies;
using GameData;
using System;
using System.Collections.Generic;

namespace EECustom.Managers
{
    public partial class ConfigManager
    {
        public class EventHolder<T> where T : class, IEnemyEvent
        {
            public string EventName { get; set; } = string.Empty;
            public bool IgnoreLogs { get; set; } = false;

            public T[] Events
            {
                get
                {
                    if (_hasDirty)
                    {
                        _events = _eventList.ToArray();
                        _hasDirty = false;
                    }

                    return _events;
                }
            }

            private readonly List<T> _eventList = new();
            private T[] _events = new T[0];
            private bool _hasDirty = false;

            public EventHolder(string eventName, bool ignoreLogs = false)
            {
                EventName = eventName;
                IgnoreLogs = ignoreLogs;
            }

            public void TryAdd(EnemyCustomBase custom)
            {
                if (custom is T e)
                {
                    _eventList.Add(e);
                    _hasDirty = true;
                }
            }

            public void Clear()
            {
                _eventList.Clear();
                _hasDirty = true;
            }

            internal void FireEventPreSpawn(EnemyAgent agent, Action<T> doAction)
            {
                var handlers = Events;
                var enemyBlock = GameDataBlockBase<EnemyDataBlock>.GetBlock(agent.EnemyDataID);

                for (int i = 0; i < handlers.Length; i++)
                {
                    var custom = handlers[i].Base;

                    if (!custom.Enabled)
                        continue;

                    if (custom.Target.IsMatch(enemyBlock))
                    {
                        if (!IgnoreLogs) custom.LogDev($"Apply {EventName} Event: {agent.name}");
                        doAction?.Invoke(custom as T);
                        if (!IgnoreLogs) custom.LogVerbose($"Finished!");
                    }
                }
            }

            internal void FireEvent(EnemyAgent agent, Action<T> doAction)
            {
                var handlers = Events;

                for (int i = 0; i < handlers.Length; i++)
                {
                    var custom = handlers[i].Base;

                    if (!custom.Enabled)
                        continue;

                    if (custom.IsTarget(agent))
                    {
                        if (!IgnoreLogs) custom.LogDev($"Apply {EventName} Event: {agent.name}");
                        doAction?.Invoke(custom as T);
                        if (!IgnoreLogs) custom.LogVerbose($"Finished!");
                    }
                }
            }
        }

        private readonly EventHolder<IEnemyPrefabBuiltEvent> _enemyPrefabBuiltHolder = new("PrefabBuilt");
        private readonly EventHolder<IEnemySpawnedEvent> _enemySpawnedHolder = new("Spawned");
        private readonly EventHolder<IEnemyDespawnedEvent> _enemyDespawnedHolder = new("Despawned");
        private readonly EventHolder<IEnemyGlowEvent> _enemyGlowHolder = new("Glow", true);

        private void GenerateEventBuffer()
        {
            foreach (var custom in _CustomizationBuffer)
            {
                _EnemyPrefabBuiltHolder.TryAdd(custom);
                _EnemySpawnedHolder.TryAdd(custom);
                _EnemyDespawnedHolder.TryAdd(custom);
                _EnemyGlowHolder.TryAdd(custom);
            }
        }

        internal void FirePrefabBuiltEvent(EnemyAgent agent)
        {
            _EnemyPrefabBuiltHolder.FireEventPreSpawn(agent, (e) =>
            {
                e.OnPrefabBuilt(agent);
            });
        }

        internal void FireSpawnedEvent(EnemyAgent agent)
        {
            _EnemySpawnedHolder.FireEvent(agent, (e) =>
            {
                e.OnSpawned(agent);
            });
        }

        internal void FireDespawnedEvent(EnemyAgent agent)
        {
            _EnemyDespawnedHolder.FireEvent(agent, (e) =>
            {
                e.OnDespawned(agent);
            });
        }

        internal bool FireGlowEvent(EnemyAgent agent, ref GlowInfo glowInfo)
        {
            bool altered = false;
            var newGlowInfo = new GlowInfo(glowInfo.Color, glowInfo.Position);

            _EnemyGlowHolder.FireEvent(agent, (e) =>
            {
                var copyedGlowInfo = new GlowInfo(newGlowInfo.Color, newGlowInfo.Position);
                if (e.OnGlow(agent, ref copyedGlowInfo))
                {
                    newGlowInfo = copyedGlowInfo;
                    altered = true;
                }
            });

            if (altered)
            {
                glowInfo = newGlowInfo;
            }

            return altered;
        }
    }
}