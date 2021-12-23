using EECustom.Events;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP
{
    public static class EMPManager
    {
        private static readonly List<IEmpTarget> _empTargets = new();
        private static readonly List<ActiveEmp> _activeTargets = new();
        private static uint _nextId;
        private static bool _initialized = false;

        static EMPManager()
        {
            LevelEvents.LevelCleanup += () =>
            {
                _empTargets.Clear();
                _activeTargets.Clear();
            };
            _nextId = 0;
        }

        public static void Initialize()
        {
            if (_initialized)
                return;

            var newUpdater = new GameObject();
            MonoBehaviourEventHandler.AttatchToObject(newUpdater, onUpdate: (_) =>
            {
                Tick();
            });
            GameObject.DontDestroyOnLoad(newUpdater);

            _initialized = true;
        }

        public static void AddTarget(IEmpTarget target) => _empTargets.Add(target);
        public static void RemoveTarget(IEmpTarget target) => _empTargets.Remove(target);

        public static void Activate(Vector3 position, float range, float duration)
        {
            foreach (IEmpTarget target in _empTargets)
            {
                float dist = Vector3.Distance(position, target.Position);
                if (dist < range)
                {
                    ActiveEmp instance = _activeTargets.Find((ActiveEmp e) => { return target.ID == e.ID; });
                    if (instance != null)
                    {
                        instance.Duration += duration;
                        continue;
                    }

                    target.EnableEmp();
                    _activeTargets.Add(new ActiveEmp(target, Clock.Time + duration));
                }
            }
        }

        private static void Tick()
        {
            foreach (ActiveEmp activeEmp in _activeTargets)
            {
                if (activeEmp.Duration < Clock.Time)
                {
                    activeEmp.DisableEmp();
                }
            }

            _activeTargets.RemoveAll(t => t.Duration < Clock.Time);
        }

        public static uint GetID()
        {
            _nextId++;
            return _nextId;
        }

        private class ActiveEmp
        {
            public IEmpTarget target;
            public float Duration;
            public uint ID => target.ID;

            public void DisableEmp()
            {
                if (target != null)
                {
                    target.DisableEmp();
                }
            }

            public ActiveEmp(IEmpTarget target, float duration)
            {
                this.target = target;
                Duration = duration;
            }
        }
    }
}
