using EECustom.Events;
using System.Collections.Generic;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP
{
    public static class EMPManager
    {
        private static readonly List<EMPController> _empTargets = new();
        private static uint _nextId;

        static EMPManager()
        {
            LevelEvents.LevelCleanup += () =>
            {
                _empTargets.Clear();
                EMPHandlerBase.Cleanup();
            };
            _nextId = 0;
        }

        public static void AddTarget(EMPController target) => _empTargets.Add(target);
        public static void RemoveTarget(EMPController target) => _empTargets.Remove(target);

        public static void Activate(Vector3 position, float range, float duration)
        {
            if (!GameStateManager.IsInExpedition)
            {
                Logger.Error("Tried to activate an EMP when not in level, this shouldn't happen!");
                return;
            }
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
