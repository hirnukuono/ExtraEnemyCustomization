using EECustom.Events;
using System.Collections.Generic;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP
{
    public static class EMPManager
    {
        private static readonly List<EMPController> _empTargets = new();
        private static readonly List<EMPController> _activeTargets = new();
        private static uint _nextId;
        private static bool _initialized = false;
        private static bool _adding = false;

        static EMPManager()
        {
            LevelEvents.LevelCleanup += () =>
            {
                _empTargets.Clear();
            };
            _nextId = 0;
        }

        public static void Initialize()
        {
            //if (_initialized)
            //    return;
            //
            //var newUpdater = new GameObject();
            //MonoBehaviourEventHandler.AttatchToObject(newUpdater, onUpdate: (_) =>
            //{
            //    Tick();
            //});
            //GameObject.DontDestroyOnLoad(newUpdater);
            //
            //_initialized = true;
        }

        public static void AddTarget(EMPController target) => _empTargets.Add(target);
        public static void RemoveTarget(EMPController target) => _empTargets.Remove(target);

        public static void Activate(Vector3 position, float range, float duration)
        {
            foreach (EMPController targetController in _empTargets)
            {
                float dist = Vector3.Distance(position, targetController.Position);
                if (dist < range)
                {
                    targetController.AddTime(duration);
                }
            }
        }

        public static uint GetID()
        {
            _nextId++;
            return _nextId;
        }
    }
}
