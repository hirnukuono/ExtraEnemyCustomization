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
        /// If the device is currently active 
        /// </summary>
        protected bool _isDeviceOn = false;

        /// <summary>
        /// The current EMP State
        /// </summary>
        protected EMPState _state;

        /// <summary>
        /// The time remaining in the current state
        /// </summary>
        protected float _stateTimer;

        /// <summary>
        /// Used to give random delay to how long it takes the device to turn back on, if desired
        /// </summary>
        protected float _delayTimer;

        /// <summary>
        /// How long the flickering effect should last
        /// </summary>
        protected const float _flickerDuration = 0.2f;


        public abstract void Setup(GameObject gameObject, EMPController controller);
        public void Tick(bool isEMPD)
        {
            if (isEMPD && _state == EMPState.On)
            {
                float delay = GetRandomDelay(0, 1.5f);
                _state = EMPState.FlickerOff;
                _delayTimer = Clock.Time + delay;
                _stateTimer = Clock.Time + _flickerDuration + delay;
            }

            if (!isEMPD && _state == EMPState.Off)
            {
                float delay = GetRandomDelay(0, 1.5f);
                _state = EMPState.FlickerOn;
                _delayTimer = Clock.Time + delay;
                _stateTimer = Clock.Time + _flickerDuration + delay;
            }

            switch (_state)
            {
                case EMPState.On:
                    if (_isDeviceOn) return;
                    DeviceOn();
                    _isDeviceOn = true;
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
