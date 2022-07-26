using EEC.CustomAbilities.EMP.Inject;
using EEC.Events;
using EEC.Networking;
using System.Collections.Generic;
using UnityEngine;

namespace EEC.CustomAbilities.EMP
{
    [CallConstructorOnLoad]
    public static class EMPManager
    {
        private static readonly List<EMPController> _empTargets = new();
        private static readonly List<ActiveEMPs> _activeEMPs = new();

        static EMPManager()
        {
            LevelEvents.LevelCleanup += () =>
            {
                _empTargets.Clear();
                EMPHandlerBase.Cleanup();
            };

            SNetEvents.PrepareRecall += (bufferType) =>
            {
                foreach (var target in _empTargets)
                {
                    target.ClearTime();
                    target.ForceState(EMPState.On);
                }

                EMPHandlerBase.Cleanup();
            };

            LevelEvents.BuildDone += () =>
            {
                Inject_PlayerBackpack.Setup();
            };
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
            return totalDurationForPosition;
        }

        private struct ActiveEMPs
        {
            private readonly Vector3 _position;
            private readonly float _range;
            private readonly float _duration;
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