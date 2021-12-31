using EECustom.Customizations.EnemyAbilities.Abilities.EMP.Inject;
using EECustom.Events;
using System.Collections.Generic;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP
{
    public static class EMPManager
    {
        private static readonly List<EMPController> _empTargets = new();
        private static readonly List<ActiveEMPs> _activeEMPs = new();
        private static uint _nextId;

        static EMPManager()
        {
            LevelEvents.LevelCleanup += () =>
            {
                _empTargets.Clear();
                EMPHandlerBase.Cleanup();
            };

            LevelEvents.BuildDone += () =>
            {
                Inject_PlayerBackpack.Setup();
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

            _activeEMPs.Add(new ActiveEMPs(position, range, duration));
            foreach (EMPController targetController in _empTargets)
            {
                float dist = Vector3.Distance(position, targetController.Position);
                if (dist < range)
                {
                    targetController.AddTime(duration);
                }
            }
        }

        public static float DurationFromPosition(Vector3 position)
        {
            _activeEMPs.RemoveAll(e => Mathf.Round(e.RemainingTime) <= 0);
            float totalDurationForPosition = 0;
            foreach (ActiveEMPs active in _activeEMPs)
            {
                if (active.InRange(position))
                {
                    totalDurationForPosition += active.RemainingTime;
                }
            }
            Logger.Debug("Duration of effect: {0}", totalDurationForPosition);
            return totalDurationForPosition;
        }

        public static uint GetID()
        {
            _nextId++;
            return _nextId;
        }

        private struct ActiveEMPs
        {
            readonly Vector3 _position;
            readonly float _range;
            readonly float _duration;
            public float RemainingTime => _duration - Clock.Time;

            public ActiveEMPs(Vector3 position, float range, float duration) : this()
            {
                _position = position;
                _range = range;
                _duration = Clock.Time + duration;
            }

            public bool InRange(Vector3 position)
            {
                float dist = Vector3.Distance(position, _position);
                return dist < _range;
            }
        }
    }
}
