using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Utils
{
    public struct LazyTimer
    {
        public float PassedTime { get; private set; }
        public float Duration { get; private set; }
        private float _lastTickTime;

        public bool Done => Progress >= 1.0f;
        public float Progress => Mathf.Clamp01(ProgressUnclamped);
        public float ProgressUnclamped
        {
            get
            {
                if (Duration != 0.0f)
                {
                    return PassedTime * _durationInv;
                }
                return 1.0f;
            }
        }

        private float _durationInv;

        public LazyTimer(float duration)
        {
            PassedTime = 0.0f;
            Duration = duration;
            _durationInv = 1.0f / duration;

            _lastTickTime = GetTime();
        }

        public void Reset(float newDuration = -1.0f)
        {
            PassedTime = 0.0f;
            if (newDuration >= 0.0f)
            {
                Duration = newDuration;
                _durationInv = 1.0f / newDuration;
            }

            _lastTickTime = GetTime();
        }

        public void Tick()
        {
            var time = GetTime();
            var delta = time - _lastTickTime;

            _lastTickTime = time;
            PassedTime += delta;
        }

        public bool TickAndCheckDone()
        {
            Tick();
            return Done;
        }

        private static float GetTime()
        {
            return GameStateManager.CurrentStateName switch
            {
                eGameStateName.InLevel => Clock.ExpeditionProgressionTime,
                _ => Clock.Time,
            };
        }
    }

    public struct Timer
    {
        public float PassedTime { get; private set; }
        public float Duration { get; private set; }

        public bool Done => Progress >= 1.0f;
        public float Progress => Mathf.Clamp01(ProgressUnclamped);
        public float ProgressUnclamped
        {
            get
            {
                if (Duration != 0.0f)
                {
                    return PassedTime * _durationInv;
                }
                return 1.0f;
            }
        }

        private float _durationInv;

        public Timer(float duration)
        {
            PassedTime = 0.0f;
            Duration = duration;
            _durationInv = 1.0f / duration;
        }

        public void Reset(float newDuration = -1.0f)
        {
            PassedTime = 0.0f;
            if (newDuration >= 0.0f)
            {
                Duration = newDuration;
                _durationInv = 1.0f / newDuration;
            }
        }

        public void Tick()
        {
            PassedTime += Clock.Delta;
        }

        public bool TickAndCheckDone()
        {
            Tick();
            return Done;
        }
    }

    public struct DoubleTimer
    {
        public double PassedTime { get; private set; }
        public double Duration { get; private set; }

        public bool Done => Progress >= 1.0;
        public double Progress => Math.Clamp(0.0, 1.0, ProgressUnclamped);
        public double ProgressUnclamped
        {
            get
            {
                if (Duration != 0.0)
                {
                    return PassedTime * _durationInv;
                }
                return 1.0;
            }
        }

        private double _durationInv;

        public DoubleTimer(double duration)
        {
            PassedTime = 0.0;
            Duration = duration;
            _durationInv = 1.0 / duration;
        }

        public void Reset(double newDuration = -1.0)
        {
            PassedTime = 0.0;
            if (newDuration >= 0.0)
            {
                Duration = newDuration;
                _durationInv = 1.0 / newDuration;
            }
        }

        public void Tick()
        {
            PassedTime += Clock.Delta;
        }

        public bool TickAndCheckDone()
        {
            Tick();
            return Done;
        }
    }
}
