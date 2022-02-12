﻿using Agents;
using AIGraph;
using EECustom.CustomAbilities.Explosion;
using EECustom.Utils;
using EECustom.Utils.JsonElements;
using LevelGeneration;
using UnityEngine;

namespace EECustom.Customizations.Shared
{
    public sealed class ExplosionSetting
    {
        public bool Enabled { get; set; } = false;
        public ValueBase Damage { get; set; } = ValueBase.Zero;
        public bool KillInflictor { get; set; } = true;
        public float EnemyDamageMulti { get; set; } = 1.0f;
        public float MinRange { get; set; } = 2.0f;
        public float MaxRange { get; set; } = 5.0f;
        public float NoiseMinRange { get; set; } = 5.0f;
        public float NoiseMaxRange { get; set; } = 10.0f;
        public NM_NoiseType NoiseType { get; set; } = NM_NoiseType.Detectable;

        public void DoExplode(Agent from)
        {
            var maxDamage = Damage.GetAbsValue(PlayerData.MaxHealth);
            if (maxDamage > 0.0f)
            {
                ExplosionManager.DoExplosion(new ExplosionData()
                {
                    position = from.EyePosition,
                    damage = maxDamage,
                    enemyMulti = EnemyDamageMulti,
                    minRange = MinRange,
                    maxRange = MaxRange
                });

                var noise = new NM_NoiseData()
                {
                    noiseMaker = null,
                    position = from.EyePosition,
                    radiusMin = NoiseMinRange,
                    radiusMax = NoiseMaxRange,
                    yScale = 1,
                    node = from.CourseNode,
                    type = NoiseType,
                    includeToNeightbourAreas = true,
                    raycastFirstNode = false
                };
                NoiseManager.MakeNoise(noise);
            }
        }

        public void DoExplode(Vector3 position)
        {
            var maxDamage = Damage.GetAbsValue(PlayerData.MaxHealth);
            if (maxDamage > 0.0f)
            {
                ExplosionManager.DoExplosion(new ExplosionData()
                {
                    position = position,
                    damage = maxDamage,
                    enemyMulti = EnemyDamageMulti,
                    minRange = MinRange,
                    maxRange = MaxRange
                });

                var newPos = position;
                if (!PhysicsUtil.SlamPos(ref newPos, Vector3.down, 64.0f, LayerManager.MASK_LEVELGEN, false, 0.0f, 0.0f))
                {
                    return;
                }

                if (AIG_GeomorphNodeVolume.TryGetCourseNode(Dimension.GetDimensionFromPos(newPos).DimensionIndex, newPos, out var courseNode))
                {
                    var noise = new NM_NoiseData()
                    {
                        noiseMaker = null,
                        position = position,
                        radiusMin = NoiseMinRange,
                        radiusMax = NoiseMaxRange,
                        yScale = 1,
                        node = courseNode,
                        type = NoiseType,
                        includeToNeightbourAreas = true,
                        raycastFirstNode = false
                    };
                    NoiseManager.MakeNoise(noise);
                }
            }
        }
    }
}