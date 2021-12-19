using EECustom.Attributes;
using Player;
using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EECustom.Customizations.Shared.Handlers
{
    [InjectToIl2Cpp]
    public class BleedingHandler : MonoBehaviour
    {
        public PlayerAgent Agent;

        private float _damage;
        private float _interval;
        private float _bleedingTimer;
        private float _bleedingIntervalTimer;
        private bool _bleeding = false;

        public BleedingHandler(IntPtr ptr) : base(ptr)
        {
        }

        internal void Update()
        {
            if (!_bleeding)
                return;

            if (!Agent.Alive)
                return;

            if (_bleedingTimer <= Clock.Time)
            {
                StopBleed();
                return;
            }

            if (_bleedingIntervalTimer <= Clock.Time)
            {
                Agent.Damage.FireDamage(_damage, Agent);
                _bleedingIntervalTimer = Clock.Time + _interval;
            }
        }

        [HideFromIl2Cpp]
        public void DoBleed(float damage, float interval, float duration)
        {
            if (!Agent.Alive)
                return;

            _damage = damage;
            _interval = interval;
            _bleedingTimer = Clock.Time + duration;
            _bleeding = true;
        }

        [HideFromIl2Cpp]
        public void StopBleed()
        {
            _bleeding = false;
        }
    }
}