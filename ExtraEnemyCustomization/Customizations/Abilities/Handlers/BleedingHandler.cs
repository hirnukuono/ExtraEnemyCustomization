using Player;
using System;
using System.Collections.Generic;
using System.Text;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EECustom.Customizations.Abilities.Handlers
{
    public class BleedingHandler : MonoBehaviour
    {
        public PlayerAgent Agent;

        private float _Damage;
        private float _Interval;
        private float _BleedingTimer;
        private float _BleedingIntervalTimer;
        private bool _Bleeding = false;

        public BleedingHandler(IntPtr ptr) : base(ptr)
        {

        }

        internal void Update()
        {
            if (!_Bleeding)
                return;

            if (!Agent.Alive)
                return;

            if (_BleedingTimer <= Clock.Time)
            {
                _Bleeding = false;
                return;
            }

            if (_BleedingIntervalTimer <= Clock.Time)
            {
                Agent.Damage.FireDamage(_Damage, Agent);
                _BleedingIntervalTimer = Clock.Time + _Interval;
            }
        }

        [HideFromIl2Cpp]
        public void DoBleed(float damage, float interval, float duration)
        {
            if (!Agent.Alive)
                return;

            _Damage = damage;
            _Interval = interval;
            _BleedingTimer = Clock.Time + duration;
            _Bleeding = true;
        }

        [HideFromIl2Cpp]
        public void StopBleed()
        {
            _Bleeding = false;
        }
    }
}
