using EECustom.Events;
using EECustom.Extensions;
using Enemies;
using System.Collections.Generic;

namespace EECustom.Utils
{
    public static class EnemyProperty<T> where T : class, new()
    {
        private readonly static Dictionary<ushort, T> _properties = new();

        static EnemyProperty()
        {
            LevelEvents.LevelCleanup += OnLevelCleanup;
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

            agent.AddOnDeadOnce(() =>
            {
                _properties.Remove(id);
            });

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
    }
}