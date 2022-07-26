using EEC.Events;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EEC.Managers.Properties
{
    [CallConstructorOnLoad]
    public static class EnemySpawnDataManager
    {
        private static readonly Dictionary<ushort, pEnemySpawnData> _lookup = new(500);

        static EnemySpawnDataManager()
        {
            EnemyEvents.Spawn += Spawn;
            EnemyEvents.Despawn += Despawn;
            LevelEvents.LevelCleanup += OnLevelCleanup;
        }

        private static void Spawn(EnemyAgent agent, pEnemySpawnData spawnData)
        {
            _lookup[spawnData.replicationData.ReplicatorKey] = spawnData;
        }

        private static void Despawn(EnemyAgent agent)
        {
            _lookup.Remove(agent.GlobalID);
        }

        private static void OnLevelCleanup()
        {
            _lookup.Clear();
        }

        public static bool TryGet(ushort id, out pEnemySpawnData data)
        {
            return _lookup.TryGetValue(id, out data);
        }
    }
}
