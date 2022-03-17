using EECustom.Events;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Managers.Properties
{
    public static class ProjectileOwnerManager
    {
        private static readonly Dictionary<int, EnemyAgent> _lookup = new();

        static ProjectileOwnerManager()
        {
            LevelEvents.LevelCleanup += OnLevelCleanup;
        }

        private static void OnLevelCleanup()
        {
            _lookup.Clear();
        }

        public static void Set(int instanceID, EnemyAgent agent)
        {
            _lookup[instanceID] = agent;
        }

        public static void Remove(int instanceID)
        {
            _lookup.Remove(instanceID);
        }

        public static bool TryGet(int instanceID, out EnemyAgent agent)
        {
            return _lookup.TryGetValue(instanceID, out agent);
        }
    }
}
