using EEC.Utils.Unity;
using FX_EffectSystem;
using System;
using UnityEngine;

namespace EEC.CustomAbilities.Explosion.Handlers
{
    [InjectToIl2Cpp]
    internal sealed class ExplosionEffectHandler : MonoBehaviour
    {
        public Action EffectDoneOnce;

        private EffectLight _light;
        private Timer _timer;
        private bool _effectOnGoing = false;

        internal void DoEffect(ExplosionEffectData data)
        {
            transform.position = data.position;

            if (_light == null)
            {
                _light = gameObject.GetComponent<EffectLight>();
                _light.Setup();
            }

            _light.UpdateVisibility(true);

            _light.Color = data.flashColor;
            _light.Range = data.range;
            _light.Intensity = data.intensity;
            _timer.Reset(data.duration);
            _effectOnGoing = true;
        }

        private void FixedUpdate()
        {
            if (_effectOnGoing && _timer.TickAndCheckDone())
            {
                OnDone();
            }
        }

        private void OnDone()
        {
            if (_light != null)
            {
                _light.UpdateVisibility(false);
            }

            EffectDoneOnce?.Invoke();
            EffectDoneOnce = null;
            _light = null;
            _effectOnGoing = false;
        }
    }
}