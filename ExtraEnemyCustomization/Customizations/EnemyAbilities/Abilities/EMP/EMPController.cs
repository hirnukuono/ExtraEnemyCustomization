using EECustom.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP
{
    [InjectToIl2Cpp]
    public class EMPController : MonoBehaviour
    {
        private IEMPHandler _handler = null;
        private bool _hasHandler = false;
        private float _duration;

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

        void Update()
        {
            if (!_hasHandler) return;
            _handler.Tick(_duration > Clock.Time);
        }
    }
}
