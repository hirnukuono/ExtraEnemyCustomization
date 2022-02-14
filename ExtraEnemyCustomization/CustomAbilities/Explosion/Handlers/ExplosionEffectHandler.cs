using EECustom.Attributes;
using EECustom.Utils;
using FX_EffectSystem;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.CustomAbilities.Explosion.Handlers
{
    [InjectToIl2Cpp]
    public sealed class ExplosionEffectHandler : MonoBehaviour
    {
        public Color FlashColor;
        public float Range;
        public float Intensity;
        public float EffectDuration;

        private bool _lightAllocated = false;
        private FX_PointLight _light;
        private Timer _timer;

        internal void Start()
        {
            if (FX_Manager.TryAllocateFXLight(out _light, important: false))
            {
                _light.SetColor(FlashColor);
                _light.SetRange(Range);
                _light.m_intensity = Intensity;
                _light.m_position = transform.position;
                _light.m_isOn = true;
                _light.UpdateData();
                _light.UpdateTransform();
                _lightAllocated = true;
                _timer.Reset(EffectDuration);
            }
            else
            {
                Destroy(this);
            }
        }

        internal void Update()
        {
            if (_timer.TickAndCheckDone())
            {
                Destroy(this);
            }
        }

        internal void OnDestroy()
        {
            if (_lightAllocated)
            {
                FX_Manager.DeallocateFXLight(_light);
            }

            _light = null;
        }
    }
}
