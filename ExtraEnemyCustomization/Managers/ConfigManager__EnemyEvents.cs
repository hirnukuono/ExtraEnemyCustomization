using Agents;
using EECustom.API;
using EECustom.Customizations;
using EECustom.Utils;
using Enemies;
using GameData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EECustom.Managers
{
    public partial class ConfigManager
    {
        public class EnemyEventHolder<T> where T : class, IEnemyEvent
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
                _eventList.Clear();
                _hasDirty = true;
            }

            internal void FireEventPreSpawn(EnemyAgent agent, Action<T> doAction)
            {
                var handlers = Events;
                var enemyBlock = GameDataBlockBase<EnemyDataBlock>.GetBlock(agent.EnemyDataID);

                for (int i = 0; i < handlers.Length; i++)
                {
                    var handler = handlers[i];
                    var custom = handler.Base;

                    if (!custom.Enabled)
                        return;

                    if (custom.Target.IsMatch(enemyBlock))
                    {
                        if (!IgnoreLogs) custom.LogDev($"Apply {EventName} Event: {agent.name}");
                        doAction?.Invoke(handler);
                        if (!IgnoreLogs) custom.LogVerbose($"Finished!");
                    }
                }
            }

            internal void FireEvent(EnemyAgent agent, Action<T> doAction)
            {
                var handlers = Events;

                for (int i = 0; i < handlers.Length; i++)
                {
                    var handler = handlers[i];
                    var custom = handler.Base;

                    if (!custom.Enabled)
                        return;

                    if (custom.IsTarget(agent))
                    {
                        if (!IgnoreLogs) custom.LogDev($"Apply {EventName} Event: {agent.name}");
                        doAction?.Invoke(handler);
                        if (!IgnoreLogs) custom.LogVerbose($"Finished!");
                    }
                }
            }
        }

        private readonly EnemyEventHolder<IEnemyPrefabBuiltEvent> _enemyPrefabBuiltHolder = new("PrefabBuilt");
        private readonly EnemyEventHolder<IEnemySpawnedEvent> _enemySpawnedHolder = new("Spawned");
        private readonly EnemyEventHolder<IEnemyAgentModeEvent> _enemyModeChangedHolder = new("AgentModeChange", ignoreLogs: true);
        private readonly EnemyEventHolder<IEnemyGlowEvent> _enemyGlowHolder = new("Glow", ignoreLogs: true);

        private void GenerateEventBuffer()
        {
            _enemyPrefabBuiltHolder.Clear();
            _enemySpawnedHolder.Clear();
            _enemyModeChangedHolder.Clear();
            _enemyGlowHolder.Clear();

            foreach (var custom in _customizationBuffer)
            {
                _enemyPrefabBuiltHolder.TryAdd(custom);
                _enemySpawnedHolder.TryAdd(custom);
                _enemyModeChangedHolder.TryAdd(custom);
                _enemyGlowHolder.TryAdd(custom);
            }
        }

        internal void FirePrefabBuildEventAll()
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

                var enemyAgentComp = prefab.GetComponentInChildren<EnemyAgent>(true);
                if (enemyAgentComp is null)
                {
                    Logger.Error($"EnemyData id: {block.persistentID} prefab doesn't have EnemyAgent!");
                    continue;
                }

                RegisterTargetEnemyLookup(block);
                FirePrefabBuiltEvent(enemyAgentComp);
            }
        }

        internal void FirePrefabBuiltEvent(EnemyAgent agent)
        {
            _enemyPrefabBuiltHolder.FireEvent(agent, (e) =>
            {
                e.OnPrefabBuilt(agent);
            });
        }

        internal void FireSpawnedEvent(EnemyAgent agent)
        {
            _enemySpawnedHolder.FireEvent(agent, (e) =>
            {
                e.OnSpawned(agent);
            });

            CustomizationAPI.OnSpawnCustomizationDone_Internal(agent);
        }

        internal void FireAgentModeChangedEvent(EnemyAgent agent, AgentMode newMode)
        {
            _enemyModeChangedHolder.FireEvent(agent, (e) =>
            {
                e.OnAgentModeChanged(agent, newMode);
            });
        }

        internal bool FireGlowEvent(EnemyAgent agent, ref GlowInfo glowInfo)
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