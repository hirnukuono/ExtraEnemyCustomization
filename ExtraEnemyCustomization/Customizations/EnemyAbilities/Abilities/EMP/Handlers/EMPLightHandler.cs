using EECustom.Attributes;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers
{
    // TODO: Check if the level has a reactor and if that reactor affects the lights, if it does we'll have to a late update to stop it from flickering EMP'd lights
    public class EMPLightHandler : EMPHandlerBase
    {
        /// <summary>
        /// The light we're going to affect
        /// </summary>
        private LG_Light _light;

        /// <summary>
        /// The original intensity of the light
        /// </summary>
        private float _originalIntensity;

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            _light = gameObject.GetComponent<LG_Light>();
            if (_light == null)
            {
                Logger.Warning("No Light!");
                return;
            }
            _originalIntensity = _light.GetIntensity();
            _state = EMPState.On;
        }

        protected override void FlickerDevice()
        {
            _light?.ChangeIntensity(UnityEngine.Random.value * _originalIntensity);
        }

        protected override void DeviceOn()
        {
            _light?.ChangeIntensity(_originalIntensity);
        }

        protected override void DeviceOff()
        {
            _light?.ChangeIntensity(0);
        }
    }
}
