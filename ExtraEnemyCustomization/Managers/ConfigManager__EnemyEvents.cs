using Agents;
using EEC.API;
using EEC.EnemyCustomizations;
using EEC.Utils.Unity;
using Enemies;
using GameData;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EEC.Managers
{
    public static partial class ConfigManager
    {
        public static event Action<EnemyAgent, EnemyDataBlock> EnemyPrefabBuilt;

        public sealed class EnemyEventHolder<T> where T : class, IEnemyEvent
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
            private readonly List<T> _enemyCacheTempList = new();
            private readonly Dictionary<uint, T[]> _eventsEnemyCache = new();
            private T _handlerTemp;
            private EnemyCustomBase _customTemp;
            private T[] _events = Array.Empty<T>();
            private bool _hasDirty = false;

            public EnemyEventHolder(string eventName, bool ignoreLogs = false)
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
                _eventsEnemyCache.Clear();
                _eventList.Clear();
                _hasDirty = true;
            }

            internal void RegisterCache(uint enemyID, bool forceRebuild)
            {
                if (forceRebuild || !_eventsEnemyCache.ContainsKey(enemyID))
                {
                    _enemyCacheTempList.Clear();

                    var handlers = Events;
                    var length = handlers.Length;
                    for (int i = 0; i < length; i++)
                    {
                        _handlerTemp = handlers[i];
                        _customTemp = _handlerTemp.Base;

                        if (_customTemp.IsTarget(enemyID))
                        {
                            _enemyCacheTempList.Add(_handlerTemp);
                        }
                    }

                    _eventsEnemyCache[enemyID] = _enemyCacheTempList.ToArray();
                }
            }

            internal void FireEvent(EnemyAgent agent, Action<T> doAction)
            {
                if (doAction == null)
                    return;
                if (!_eventsEnemyCache.TryGetValue(agent.EnemyDataID, out var handlers))
                    return;

                var length = handlers.Length;
                for (int i = 0; i < length; i++)
                {
                    _handlerTemp = handlers[i];
                    _customTemp = _handlerTemp.Base;

                    if (!IgnoreLogs && Logger.DevLogAllowed) _customTemp.LogDev($"Apply {EventName} Event: {agent.name}");
                    doAction.Invoke(_handlerTemp);
                    if (!IgnoreLogs && Logger.VerboseLogAllowed) _customTemp.LogVerbose($"Finished!");
                }
            }
        }

        private static readonly EnemyEventHolder<IEnemyPrefabBuiltEvent> _enemyPrefabBuiltHolder = new("PrefabBuilt");
        private static readonly EnemyEventHolder<IEnemySpawnedEvent> _enemySpawnedHolder = new("Spawned");
        private static readonly EnemyEventHolder<IEnemyDeadEvent> _enemyDeadHolder = new("Dead");
        private static readonly EnemyEventHolder<IEnemyDespawnedEvent> _enemyDespawnedHolder = new("Despawned");
        private static readonly EnemyEventHolder<IEnemyAgentModeEvent> _enemyModeChangedHolder = new("AgentModeChange", ignoreLogs: true);
        private static readonly EnemyEventHolder<IEnemyGlowEvent> _enemyGlowHolder = new("Glow", ignoreLogs: true);

        private static void GenerateEventBuffer()
        {
            _enemyPrefabBuiltHolder.Clear();
            _enemySpawnedHolder.Clear();
            _enemyDeadHolder.Clear();
            _enemyDespawnedHolder.Clear();
            _enemyModeChangedHolder.Clear();
            _enemyGlowHolder.Clear();

            foreach (var custom in _customizationBuffer)
            {
                _enemyPrefabBuiltHolder.TryAdd(custom);
                _enemySpawnedHolder.TryAdd(custom);
                _enemyDeadHolder.TryAdd(custom);
                _enemyDespawnedHolder.TryAdd(custom);
                _enemyModeChangedHolder.TryAdd(custom);
                _enemyGlowHolder.TryAdd(custom);
            }
        }

        private static void CacheEnemyEventBuffer(uint enemyID)
        {
            _enemyPrefabBuiltHolder.RegisterCache(enemyID, forceRebuild: false);
            _enemySpawnedHolder.RegisterCache(enemyID, forceRebuild: false);
            _enemyDeadHolder.RegisterCache(enemyID, forceRebuild: false);
            _enemyDespawnedHolder.RegisterCache(enemyID, forceRebuild: false);
            _enemyModeChangedHolder.RegisterCache(enemyID, forceRebuild: false);
            _enemyGlowHolder.RegisterCache(enemyID, forceRebuild: false);
        }

        internal static void FirePrefabBuildEventAll(bool rebuildPrefabs)
        {
            EnemyDataBlock[] allBlocks = GameDataBlockBase<EnemyDataBlock>.GetAllBlocks();
            foreach (var block in allBlocks)
            {
                var prefab = EnemyPrefabManager.GetEnemyPrefab(block.persistentID);
                if (prefab is null)
                {
                    Logger.Error($"EnemyData id: {block.persistentID} doesn't have EnemyPrefab!");
                    continue;
                }

                if (rebuildPrefabs)
                {
                    UnityEngine.Object.Destroy(prefab);
                    EnemyPrefabManager.GenerateEnemy(block);

                    prefab = EnemyPrefabManager.GetEnemyPrefab(block.persistentID);
                }

                var enemyAgentComp = prefab.GetComponentInChildren<EnemyAgent>(true);
                if (enemyAgentComp is null)
                {
                    Logger.Error($"EnemyData id: {block.persistentID} prefab doesn't have EnemyAgent!");
                    continue;
                }

                RegisterTargetEnemyLookup(block);
                CacheEnemyEventBuffer(block.persistentID);
                FirePrefabBuiltEvent(enemyAgentComp);

                EnemyPrefabBuilt?.Invoke(enemyAgentComp, block);
            }

            TargetEnemyLookupFullyBuilt();

            if (rebuildPrefabs)
            {
                EnemyAllocator.Current.m_enemyReplicationManager.ClearPrefabs();
                EnemyAllocator.Current.GetEnemyPrefabs();
            }
        }

        internal static void FirePrefabBuiltEvent(EnemyAgent agent)
        {
            _enemyPrefabBuiltHolder.FireEvent(agent, (e) =>
            {
                e.OnPrefabBuilt(agent);
            });
        }

        internal static void FireSpawnedEvent(EnemyAgent agent)
        {
            _enemySpawnedHolder.FireEvent(agent, (e) =>
            {
                e.OnSpawned(agent);
            });

            CustomizationAPI.OnSpawnCustomizationDone_Internal(agent);
            return;
        }

        internal static void FireDeadEvent(EnemyAgent agent)
        {
            _enemyDeadHolder.FireEvent(agent, (e) =>
            {
                e.OnDead(agent);
            });
        }

        internal static void FireDespawnedEvent(EnemyAgent agent)
        {
            _enemyDespawnedHolder.FireEvent(agent, (e) =>
            {
                e.OnDespawned(agent);
            });
        }

        internal static void FireAgentModeChangedEvent(EnemyAgent agent, AgentMode newMode)
        {
            _enemyModeChangedHolder.FireEvent(agent, (e) =>
            {
                e.OnAgentModeChanged(agent, newMode);
            });
        }

        internal static bool FireGlowEvent(EnemyAgent agent, ref GlowInfo glowInfo)
        {
            bool altered = false;
            var newGlowInfo = new GlowInfo(glowInfo.Color, glowInfo.Position);

            _enemyGlowHolder.FireEvent(agent, (e) =>
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