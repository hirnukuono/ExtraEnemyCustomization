using EECustom.Events;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Utils
{
    public static class EnemyProperty<T> where T : class, new()
    {
        private static Dictionary<ushort, T> _Properties = new Dictionary<ushort, T>();

        static EnemyProperty()
        {
            LevelEvents.OnLevelCleanup += OnLevelCleanup;
        }

        static void OnLevelCleanup()
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

            var called = false;
            agent.add_OnDeadCallback(new Action(()=> {
                if (!called)
                {
                    _Properties.Remove(id);
                    called = true;
                }
            }));

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
