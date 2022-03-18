using EEC.Events;
using Enemies;
using System.Collections.Generic;
using UnityEngine;

namespace EEC.Utils
{
    public static class EnemyProperty<T> where T : class, new()
    {
        private static readonly Dictionary<ushort, T> _properties = new();

        public static IEnumerable<T> Properties => _properties.Values;

        static EnemyProperty()
        {
            EnemyEvents.Despawn += EnemyDespawn;
            LevelEvents.LevelCleanup += OnLevelCleanup;
        }

        private static void EnemyDespawn(EnemyAgent agent)
        {
            _properties.Remove(agent.GlobalID);
        }

        private static void OnLevelCleanup()
        {
            _properties.Clear();
        }

        public static T RegisterOrGet(EnemyAgent agent)
        {
            var result = RegisterEnemy(agent);
            if (result != null)
            {
                return result;
            }

            return Get(agent);
        }

        public static T RegisterEnemy(EnemyAgent agent)
        {
            var id = agent.GlobalID;

            if (_properties.ContainsKey(id))
                return null;

            var newProp = new T();
            _properties.Add(id, newProp);
            return newProp;
        }

        public static T Get(EnemyAgent agent) => Get(agent.GlobalID);

        public static T Get(ushort id)
        {
            if (_properties.ContainsKey(id))
                return _properties[id];
            else
                return null;
        }

        public static bool TryGet(EnemyAgent agent, out T property) => TryGet(agent.GlobalID, out property);

        public static bool TryGet(ushort id, out T property)
        {
            return _properties.TryGetValue(id, out property);
        }
    }
}