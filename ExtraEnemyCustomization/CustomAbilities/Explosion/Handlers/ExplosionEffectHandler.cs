using EECustom.Attributes;
using EECustom.Utils;
using FX_EffectSystem;
using System;
using UnityEngine;

namespace EECustom.CustomAbilities.Explosion.Handlers
{
    [InjectToIl2Cpp]
    internal sealed class ExplosionEffectHandler : MonoBehaviour
    {
        public Action EffectDoneOnce;

        private bool _lightAllocated = false;
        private FX_PointLight _light;
        private Timer _timer;

        internal void DoEffect(ExplosionEffectData data)
        {
            if (FX_Manager.TryAllocateFXLight(out _light, important: false))
            {
                _light.SetColor(data.flashColor);
                _light.SetRange(data.range);
                _light.m_intensity = data.intensity;
                _light.m_position = data.position;
                _light.m_isOn = true;
                _light.UpdateData();
                _light.UpdateTransform();
                _lightAllocated = true;
                _timer.Reset(data.duration);
            }
            else
            {
                OnDone();
            }
        }

        internal void FixedUpdate()
        {
            if (_lightAllocated && _timer.TickAndCheckDone())
            {
                OnDone();
            }
        }

        private void OnDone()
        {
            if (_lightAllocated)
            {
                FX_Manager.DeallocateFXLight(_light);
            }

            EffectDoneOnce?.Invoke();
            EffectDoneOnce = null;
            _light = null;
            _lightAllocated = false;
        }
    }
}