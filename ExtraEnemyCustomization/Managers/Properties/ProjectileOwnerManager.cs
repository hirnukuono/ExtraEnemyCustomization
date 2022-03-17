using EECustom.Events;
using Enemies;
using System.Collections.Generic;

namespace EECustom.Managers.Properties
{
    public static class ProjectileOwnerManager
    {
        private static readonly Dictionary<int, EnemyAgent> _lookup = new();
        private static readonly Dictionary<int, uint> _enemyDataIDLookup = new();

        static ProjectileOwnerManager()
        {
            LevelEvents.LevelCleanup += OnLevelCleanup;
        }

        private static void OnLevelCleanup()
        {
            _lookup.Clear();
            _enemyDataIDLookup.Clear();
        }

        public static void Set(int instanceID, EnemyAgent agent)
        {
            _lookup[instanceID] = agent;
            _enemyDataIDLookup[instanceID] = agent.EnemyDataID;
        }

        public static void Remove(int instanceID)
        {
            _lookup.Remove(instanceID);
            _enemyDataIDLookup.Remove(instanceID);
        }

        public static bool TryGet(int instanceID, out EnemyAgent agent)
        {
            return _lookup.TryGetValue(instanceID, out agent);
        }

        public static bool TryGetDataID(int instanceID, out uint enemyDataID)
        {
            return _enemyDataIDLookup.TryGetValue(instanceID, out enemyDataID);
        }
    }
}