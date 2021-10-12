using AIGraph;
using EECustom.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EECustom.CustomSettings.Handlers
{
    public class ExplosiveProjectileHandler : MonoBehaviour
    {
        public float Damage;
        public float MinRange;
        public float MaxRange;
        public float NoiseMinRange;
        public float NoiseMaxRange;

        public ExplosiveProjectileHandler(IntPtr ptr) : base(ptr)
        {

        }

        private void Awake()
        {
            Logger.Debug($"Damage is {Damage}");
        }

        private void OnDisable()
        {
            ExplosionUtil.TriggerExplodion(transform.position, Damage, MinRange, MaxRange);

            var newPos = transform.position;
            if (!PhysicsUtil.SlamPos(ref newPos, Vector3.down, 64.0f, LayerManager.MASK_LEVELGEN, false, 0.0f, 0.0f))
            {
                return;
            }

            if (AIG_GeomorphNodeVolume.TryGetCourseNode(newPos, out var courseNode))
            {
                Logger.Debug("Found CourseNode, Making Noise");
                Logger.Debug("Info: " + courseNode.m_zone.name + " Area:" + courseNode.m_area.name);
                var noise = new NM_NoiseData()
                {
                    noiseMaker = null,
                    position = transform.position,
                    radiusMin = NoiseMinRange,
                    radiusMax = NoiseMaxRange,
                    yScale = 1,
                    node = courseNode,
                    type = NM_NoiseType.Detectable,
                    includeToNeightbourAreas = true,
                    raycastFirstNode = false
                };
                NoiseManager.MakeNoise(noise);
            }
        }

        private void OnDestroy()
        {
            
        }
    }
}
