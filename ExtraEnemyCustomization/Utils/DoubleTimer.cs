using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Utils
{
    public struct DoubleTimer
    {
        public double Timer { get; private set; }
        public double Duration { get; private set; }

        public double Progress => Math.Clamp(0.0, 1.0, ProgressUnclamped);
        public double ProgressUnclamped
        {
            get
            {
                if (Duration != 0.0)
                {
                    return (Timer / Duration);
                }
                return 1.0;
            }
        }

        public float FloatProgress => Mathf.Clamp01(FloatProgressUnclamped);
        public float FloatProgressUnclamped => (float)ProgressUnclamped;

        public DoubleTimer(double duration)
        {
            Timer = 0.0;
            Duration = duration;
        }

        public void Reset(float newDuration = -1.0f) => Reset((double)newDuration);
        public void Reset(double newDuration = -1.0)
        {
            Timer = 0.0;
            if (newDuration >= 0.0)
            {
                Duration = newDuration;
            }
        }

        public void Tick()
        {
            Timer += Clock.Delta;
        }
    }
}
