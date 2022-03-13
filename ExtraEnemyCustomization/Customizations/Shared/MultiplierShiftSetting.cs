using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.Shared
{
    public sealed class MultiplierShiftSetting
    {
        public bool Enabled { get; set; } = false;
        public float MinMulti { get; set; } = 1.0f;
        public float MaxMulti { get; set; } = 1.0f;
        public float Duration { get; set; } = 1.0f;
        public eEasingType EasingMode { get; set; } = eEasingType.Linear;
        public RepeatMode Mode { get; set; } = RepeatMode.Clamped;

        private float _durationInv = 0.0f;

        public void CalcInv()
        {
            _durationInv = 1.0f / Duration;
        }

        public float EvaluateMultiplier(float progress)
        {
            return Mode switch
            {
                RepeatMode.Unclamped => Mathf.LerpUnclamped(MinMulti, MaxMulti, Ease(progress * _durationInv)),
                RepeatMode.PingPong => Mathf.Lerp(MinMulti, MaxMulti, Ease(Mathf.PingPong(progress * _durationInv, 1.0f))),
                RepeatMode.Repeat => Mathf.Lerp(MinMulti, MaxMulti, Ease(Mathf.Repeat(progress * _durationInv, 1.0f))),
                _ => Mathf.LerpUnclamped(MinMulti, MaxMulti, Mathf.Clamp01(Ease(progress * _durationInv))) //Clamped, Etc
            };
        }

        private float Ease(float p)
        {
            return Easing.GetEasingValue(EasingMode, p, false);
        }

        public enum RepeatMode
        {
            Clamped,
            Unclamped,
            PingPong,
            Repeat
        }
    }
}
