using EECustom.Attributes;
using UnityEngine;

namespace EECustom.Customizations.Shared.Handlers
{
    [InjectToIl2Cpp]
    internal sealed class EffectFogSphereHandler : MonoBehaviour
    {
        public FogSphereHandler Handler;
        public EV_Sphere EVSphere;

        private float _updateTimer = 0.0f;

        internal void Update()
        {
            if (Handler.m_sphere != null && _updateTimer < Clock.Time)
            {
                var sphere = Handler.m_sphere;
                var range = Mathf.Sqrt(1.0f / sphere.m_data.InvRangeSqr);
                EVSphere.minRadius = range * 0.8f;
                EVSphere.maxRadius = range;
                _updateTimer = Clock.Time + 0.1f;
            }

            if (Handler.m_sphere == null && _updateTimer > 0.0f)
            {
                GameObject.Destroy(this);
            }
        }

        internal void OnDestroy()
        {
            EffectVolumeManager.UnregisterVolume(EVSphere);

            Handler = null;
            EVSphere = null;
        }
    }
}