using EECustom.Attributes;
using Enemies;
using System;
using UnityEngine;

namespace EECustom.Customizations.Abilities.Handlers
{
    [InjectToIl2Cpp]
    internal class ScoutFogSphereHandler : MonoBehaviour
    {
        public Color FogColor;
        public float FogIntensity;
        public ES_ScoutScream ScoutScream;
        public EV_Sphere EVSphere = null;

        private float _updateTimer = 0.0f;
        private bool _isColorSet = false;

        public ScoutFogSphereHandler(IntPtr ptr) : base(ptr)
        {
        }

        public void Update()
        {
            if (ScoutScream.m_fogSphereAdd == null)
                return;

            if (!_isColorSet)
            {
                ScoutScream.m_fogSphereAdd.SetRadiance(FogColor, FogIntensity);
                _isColorSet = true;
            }

            if (EVSphere != null && _updateTimer < Clock.Time)
            {
                if (ScoutScream.m_fogSphereAdd.IsAllocated)
                {
                    var sphere = ScoutScream.m_fogSphereAdd.m_sphere;
                    var range = Mathf.Sqrt(1.0f / sphere.m_data.InvRangeSqr);
                    EVSphere.minRadius = range * 0.8f;
                    EVSphere.maxRadius = range;
                    EVSphere.position = ScoutScream.m_fogSphereAdd.m_sphere.m_data.Position;
                    _updateTimer = Clock.Time + 0.1f;
                }
                else
                {
                    EVSphere.minRadius = 0.0f;
                    EVSphere.maxRadius = 0.0f;
                }
            }
        }

        public void OnDestroy()
        {
            if (EVSphere != null)
                EffectVolumeManager.UnregisterVolume(EVSphere);
        }
    }
}