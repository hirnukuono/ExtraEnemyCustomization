using EECustom.Events;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP
{
    public static class EmpManager
    {
        private static List<IEmpTarget> _empTargets = new();
        private static List<ActiveEmp> _activeTargets = new();

        static EmpManager()
        {
            LevelEvents.LevelCleanup += () =>
            {
                _empTargets.Clear();
                _activeTargets.Clear();
            };
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

                    target.Disable();
                    _activeTargets.Add(new ActiveEmp(target, Clock.Time + duration));
                }
            }
        }

        public static void Tick()
        {
            foreach (ActiveEmp activeEmp in _activeTargets)
            {
                if (activeEmp.Duration < Clock.Time)
                {
                    activeEmp.target.Enable();
                    _activeTargets.Remove(activeEmp);
                }
            }
        }

        private class ActiveEmp
        {
            public IEmpTarget target;
            public float Duration;
            public uint ID => target.ID;

            public ActiveEmp(IEmpTarget target, float duration)
            {
                this.target = target;
                Duration = duration;
            }
        }
    }
}
