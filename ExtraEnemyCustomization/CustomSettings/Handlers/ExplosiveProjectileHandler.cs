using AIGraph;
using EECustom.Attributes;
using EECustom.Utils;
using System;
using UnityEngine;

namespace EECustom.CustomSettings.Handlers
{
    [InjectToIl2Cpp]
    public class ExplosiveProjectileHandler : MonoBehaviour
    {
        public float Damage;
        public float EnemyMulti;
        public float MinRange;
        public float MaxRange;
        public float NoiseMinRange;
        public float NoiseMaxRange;

        public ExplosiveProjectileHandler(IntPtr ptr) : base(ptr)
        {
        }

        private void OnDestroy()
        {
            ExplosionUtil.TriggerExplodion(transform.position, Damage, EnemyMulti, MinRange, MaxRange);

            var newPos = transform.position;
            if (!PhysicsUtil.SlamPos(ref newPos, Vector3.down, 64.0f, LayerManager.MASK_LEVELGEN, false, 0.0f, 0.0f))
            {
                return;
            }

            if (AIG_GeomorphNodeVolume.TryGetCourseNode(newPos, out var courseNode))
            {
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
    }
}