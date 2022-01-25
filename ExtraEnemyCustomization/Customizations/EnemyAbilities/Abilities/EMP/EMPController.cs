using EECustom.Attributes;
using UnhollowerBaseLib.Attributes;
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
        private bool IsEMPActive => _duration > Clock.Time;

        public Vector3 Position => transform.position;

        [HideFromIl2Cpp]
        public void AddTime(float time)
        {
            _duration = Clock.Time + time;
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

#pragma warning disable IDE0051 // Remove unused private members

        private void Awake()
        {
            EMPManager.AddTarget(this);
        }

        private void OnDestroy()
        {
            EMPManager.RemoveTarget(this);
            _handler.OnDespawn();
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

#pragma warning restore IDE0051 // Remove unused private members
    }
}