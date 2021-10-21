using Enemies;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.Abilities.Handlers
{
    public class ScoutEffectFogSphereHandler : MonoBehaviour
    {
        public ES_ScoutScream ScoutScream;
        public EV_Sphere EVSphere;

        private float _updateTimer = 0.0f;

        public ScoutEffectFogSphereHandler(IntPtr ptr) : base(ptr)
        {
        }

        private void Update()
        {
            if (ScoutScream.m_fogSphereAdd != null && _updateTimer < Clock.Time)
            {
                if (ScoutScream.m_fogSphereAdd.IsAllocated)
                {
                    var sphere = ScoutScream.m_fogSphereAdd.m_sphere;
                    var range = Mathf.Sqrt(1.0f / sphere.m_data.invRangeSqr);
                    EVSphere.minRadius = range * 0.8f;
                    EVSphere.maxRadius = range;
                    EVSphere.position = ScoutScream.m_fogSphereAdd.m_sphere.m_data.position;
                    _updateTimer = Clock.Time + 0.1f;
                }
                else
                {
                    EVSphere.minRadius = 0.0f;
                    EVSphere.maxRadius = 0.0f;
                }
            }
        }

        private void OnDestroy()
        {
            EffectVolumeManager.UnregisterVolume(EVSphere);
        }
    }
}
