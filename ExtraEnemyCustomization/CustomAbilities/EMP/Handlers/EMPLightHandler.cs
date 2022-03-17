using LevelGeneration;
using UnityEngine;

namespace EEC.CustomAbilities.EMP.Handlers
{
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

        /// <summary>
        /// The original color of the light
        /// </summary>
        private Color _originalColor;

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            if (!gameObject.TryGetComp(out _light))
            {
                Logger.Warning("No Light!");
                return;
            }
            _originalIntensity = _light.GetIntensity();
            _originalColor = _light.m_color;
            _state = EMPState.On;
        }

        protected override void FlickerDevice()
        {
            if (_light != null)
            {
                _light.ChangeIntensity(GetRandom01() * _originalIntensity);
            }
        }

        protected override void DeviceOn()
        {
            if (_light != null)
            {
                _light.ChangeIntensity(_originalIntensity);
                _light.ChangeColor(_originalColor);
            }
        }

        protected override void DeviceOff()
        {
            if (_light != null)
            {
                _light.ChangeIntensity(0);
                _light.ChangeColor(Color.black);
            }
        }
    }
}