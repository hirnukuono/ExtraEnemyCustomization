using EECustom.Attributes;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EECustom.CustomAbilities.EMP
{
    [InjectToIl2Cpp]
    public sealed class EMPController : MonoBehaviour
    {
        private IEMPHandler _handler = null;
        private bool _hasHandler = false;
        private float _duration;
        private bool _setup = false;

        [HideFromIl2Cpp]
        private bool IsEMPActive => _duration > Clock.Time;

        [HideFromIl2Cpp]
        public Vector3 Position => transform.position;

        private void Awake()
        {
            EMPManager.AddTarget(this);
        }

        private void OnEnable()
        {
            if (GameStateManager.CurrentStateName != eGameStateName.InLevel) return;
            if (!_setup) return;
            _duration = Clock.Time + EMPManager.DurationFromPosition(transform.position);
            if (_duration > Clock.Time)
            {
                _handler.ForceState(EMPState.Off);
            }
            else
            {
                _handler.ForceState(EMPState.On);
            }
        }

        private void Update()
        {
            if (!_hasHandler) return;
            _handler.Tick(IsEMPActive);
        }

        [HideFromIl2Cpp]
        public void AddTime(float time)
        {
            _duration = Clock.Time + time;
        }

        [HideFromIl2Cpp]
        public void ClearTime()
        {
            _duration = Clock.Time - 1.0f;
        }

        [HideFromIl2Cpp]
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

        [HideFromIl2Cpp]
        public void ForceState(EMPState state)
        {
            if (_handler != null)
            {
                _handler.ForceState(state);
            }
        }

        private void OnDestroy()
        {
            EMPManager.RemoveTarget(this);
            _handler.OnDespawn();

            _handler = null;
        }
    }
}