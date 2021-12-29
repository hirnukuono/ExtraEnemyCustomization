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
        protected bool _isDeviceOn = true;

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

        public abstract void Setup(GameObject gameObject, EMPController controller);
        public void Tick(bool isEMPD)
        {
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
                    if (_isDeviceOn) return;
                    DeviceOn();
                    _isDeviceOn = true;
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
                    if (!_isDeviceOn) return;
                    DeviceOff();
                    _isDeviceOn = false;
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

        protected abstract void FlickerDevice();
        protected abstract void DeviceOn();
        protected abstract void DeviceOff();

        protected float GetRandomDelay(float min, float max)
        {
            return UnityEngine.Random.RandomRange(min, max);
        }

        //protected float FlickerUtil()
        //{
        //    this.m_timePassed += Time.deltaTime;
        //    if (this.m_stableTime < this.m_timePassed)
        //    {
        //        this.m_stable = !this.m_stable;
        //        this.m_stableTime = UnityEngine.Random.Range(this.m_stableTimeMin, this.m_stableTimeMax);
        //        this.m_timePassed = 0f;
        //    }
        //    if (!this.m_stable)
        //    {
        //        this.m_lt.intensity = Mathf.Lerp(this.m_lastIntensity, this.m_targetIntensity, this.m_timePassed / this.m_accelerateTime);
        //        if ((double)Mathf.Abs(this.m_lt.intensity - this.m_targetIntensity) < 0.0001)
        //        {
        //            this.m_lastIntensity = this.m_lt.intensity;
        //            this.m_maxLightIntensity = this.m_startIntensity + 0.2f;
        //            this.m_targetIntensity = UnityEngine.Random.Range(this.m_minLightIntensity, this.m_maxLightIntensity);
        //        }
        //    }
        //}
    }
}
