using EECustom.Attributes;
using EECustom.Utils;
using Enemies;
using UnityEngine;

namespace EECustom.EnemyCustomizations.Abilities.Handlers
{
    [InjectToIl2Cpp]
    internal sealed class ScoutFogSphereHandler : MonoBehaviour
    {
        public Color FogColor;
        public float FogIntensity;
        public ES_ScoutScream ScoutScream;
        public EV_Sphere EVSphere = null;

        private Timer _updateTimer = new(0.1f);
        private bool _isColorSet = false;

        private void Update()
        {
            if (ScoutScream.m_fogSphereAdd == null)
                return;

            if (!_isColorSet)
            {
                ScoutScream.m_fogSphereAdd.SetRadiance(FogColor, FogIntensity);
                _isColorSet = true;
            }

            if (EVSphere != null && _updateTimer.TickAndCheckDone())
            {
                if (ScoutScream.m_fogSphereAdd.IsAllocated)
                {
                    var sphere = ScoutScream.m_fogSphereAdd.m_sphere;
                    var range = Mathf.Sqrt(1.0f / sphere.m_data.InvRangeSqr);
                    EVSphere.minRadius = range * 0.8f;
                    EVSphere.maxRadius = range;
                    EVSphere.position = ScoutScream.m_fogSphereAdd.m_sphere.m_data.Position;
                    _updateTimer.Reset();
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
            if (EVSphere != null)
                EffectVolumeManager.UnregisterVolume(EVSphere);

            ScoutScream = null;
            EVSphere = null;
        }
    }
}