using EECustom.Events;
using EECustom.Extensions;
using Enemies;
using System.Collections.Generic;

namespace EECustom.Utils
{
    public static class EnemyProperty<T> where T : class, new()
    {
        private readonly static Dictionary<ushort, T> _Properties = new();

        static EnemyProperty()
        {
            LevelEvents.OnLevelCleanup += OnLevelCleanup;
        }

        private static void OnLevelCleanup()
        {
            _Properties.Clear();
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

            if (_Properties.ContainsKey(id))
                return null;

            var newProp = new T();
            _Properties.Add(id, newProp);

            agent.AddOnDeadOnce(() =>
            {
                _Properties.Remove(id);
            });

            return newProp;
        }

        public static T Get(EnemyAgent agent) => Get(agent.GlobalID);

        public static T Get(ushort id)
        {
            if (_Properties.ContainsKey(id))
                return _Properties[id];
            else
                return null;
        }
    }
}