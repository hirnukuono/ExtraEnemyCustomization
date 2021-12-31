using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP
{
    /// <summary>
    /// Contains utility methods and variables useful to EMPHandler's
    /// </summary>
    public abstract class EMPHandlerBase : IEMPHandler
    {
        /// <summary>
        /// Tracks if the local player is under the effects of an EMP
        /// </summary>
        public static bool IsLocalPlayerDisabled => _isLocalPlayerDisabled;

        /// <summary>
        /// How long the flickering effect should last
        /// </summary>
        protected virtual float FlickerDuration { get => 0.2f; }

        /// <summary>
        /// The minimum delay before the device turns back on
        /// </summary>
        protected virtual float MinDelay { get => 0f; }

        /// <summary>
        /// The maximum delay before the device turns back on
        /// </summary>
        protected virtual float MaxDelay { get => 1.5f; }

        /// <summary>
        /// Set to true if the controller is attached to the player agent, used to tell if the player is EMP'd
        /// </summary>
        protected virtual bool IsDeviceOnPlayer { get => false; }

        /// <summary>
        /// If the device is currently active 
        /// </summary>
        protected DeviceState _deviceState = DeviceState.Unknown;

        /// <summary>
        /// The current EMP State
        /// </summary>
        protected EMPState _state;

        /// <summary>
        /// The time remaining in the current state
        /// </summary>
        protected float _stateTimer;

        /// <summary>
        /// Internally tracks if the player is under the effects of an EMP
        /// </summary>
        private static bool _isLocalPlayerDisabled;

        /// <summary>
        /// Stores the random delay before the device turns back on
        /// </summary>
        private float _delayTimer;

        private bool _destroyed = false;

        public abstract void Setup(GameObject gameObject, EMPController controller);

        public static void Cleanup()
        {
            _isLocalPlayerDisabled = false;
        }

        public void ForceState(EMPState state)
        {
            if (_state == state) return;
            _state = state;
            _delayTimer = Clock.Time - 1;
            _stateTimer = Clock.Time - 1;
            switch (state)
            {
                case EMPState.On:
                    _deviceState = DeviceState.On;
                    DeviceOn();
                    break;
                case EMPState.Off:
                    _deviceState = DeviceState.Off;
                    DeviceOff();
                    break;
                default:
                    _deviceState = DeviceState.Unknown;
                    break;
            }
        }

        public void Tick(bool isEMPD)
        {
            if (_destroyed) return;
            if (isEMPD && _state == EMPState.On)
            {
                float delay = GetRandomDelay(MinDelay, MaxDelay);
                _state = EMPState.FlickerOff;
                _delayTimer = Clock.Time + delay;
                _stateTimer = Clock.Time + FlickerDuration + delay;
            }

            if (!isEMPD && _state == EMPState.Off)
            {
                float delay = GetRandomDelay(0, 1.5f);
                _state = EMPState.FlickerOn;
                _delayTimer = Clock.Time + delay;
                _stateTimer = Clock.Time + FlickerDuration + delay;
            }

            switch (_state)
            {
                case EMPState.On:
                    if (_deviceState == DeviceState.On) return;
                    DeviceOn();
                    _deviceState = DeviceState.On;
                    if (IsDeviceOnPlayer) _isLocalPlayerDisabled = false;
                    break;
                case EMPState.FlickerOff:
                    if (_delayTimer > Clock.Time) return;
                    if (Clock.Time < _stateTimer)
                    {
                        FlickerDevice();
                        break;
                    }
                    _state = EMPState.Off;
                    break;
                case EMPState.Off:
                    if (_deviceState == DeviceState.Off) return;
                    DeviceOff();
                    _deviceState = DeviceState.Off;
                    if (IsDeviceOnPlayer) _isLocalPlayerDisabled = true;
                    break;
                case EMPState.FlickerOn:
                    if (_delayTimer > Clock.Time) return;
                    if (Clock.Time < _stateTimer)
                    {
                        FlickerDevice();
                        break;
                    }
                    _state = EMPState.On;
                    break;
                default:
                    break;
            }
        }

        public void OnDespawn()
        {
            _destroyed = true;
        }

        /// <summary>
        /// This method should handle the flickering effect of the device
        /// </summary>
        protected abstract void FlickerDevice();

        /// <summary>
        /// This method should reset the device to its original state
        /// </summary>
        protected abstract void DeviceOn();

        /// <summary>
        /// This method should shut the device off
        /// </summary>
        protected abstract void DeviceOff();

        /// <summary>
        /// Returns a random delay between the min and max values
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns></returns>
        protected float GetRandomDelay(float min, float max)
        {
            return UnityEngine.Random.RandomRange(min, max);
        }

        protected bool FlickerUtil(int oneInX = 2)
        {
            return UnityEngine.Random.RandomRange(0, oneInX) == 0;
        }

        protected enum DeviceState
        {
            On,
            Off,
            Unknown
        }
    }
}
