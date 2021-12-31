﻿using EECustom.Attributes;
using System;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP
{
    [InjectToIl2Cpp]
    public class EMPController : MonoBehaviour
    {
        private IEMPHandler _handler = null;
        private bool _hasHandler = false;
        private float _duration;
        private bool _setup = false;
        private bool _isEMPActive => _duration > Clock.Time;

        public EMPController(IntPtr ptr) : base(ptr)
        {
        }

        public Vector3 Position => transform.position;


        public void AddTime(float time)
        {
            _duration = Clock.Time + time;
        }

        public void AssignHandler(IEMPHandler handler)
        {
            if (_handler != null)
            {
                Logger.Warning("Tried to assign a handler to a controller that already had one!");
                return;
            }

            _handler = handler;
            _handler.Setup(gameObject, this);
            _hasHandler = true;
            _setup = true;
        }

        void Awake()
        {
            EMPManager.AddTarget(this);
        }

        void OnDestroy()
        {
            EMPManager.RemoveTarget(this);
            _handler.OnDespawn();
        }

        void OnEnable()
        {
            if (GameStateManager.CurrentStateName != eGameStateName.InLevel ) return;
            if (!_setup) return;
            _duration = Clock.Time + EMPManager.DurationFromPosition(transform.position);
            if (_duration > Clock.Time)
            {
                Logger.Debug("FORCE STATE -> OFF, IsDeviceActive: {0}", _isEMPActive);
                _handler.ForceState(EMPState.Off);
                _handler.Tick(_isEMPActive);
            } else
            {
                Logger.Debug("FORCE STATE -> ON, IsEMPActive: {0}", _isEMPActive);
                _handler.ForceState(EMPState.On);
                _handler.Tick(_isEMPActive);
            }
        }

        void Update()
        {
            if (!_hasHandler) return;
            _handler.Tick(_isEMPActive);
        }
    }
}
