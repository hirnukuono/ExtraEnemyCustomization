using Agents;
using AIGraph;
using EEC.CustomAbilities.Explosion;
using EEC.Utils;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EEC.EnemyCustomizations.Shared
{
    public static class ExplosionExtension
    {
        public static void TryKillInflictor(this IExplosionSetting setting, Agent inflictor)
        {
            if (setting.KillInflictor)
            {
                if (inflictor.TryCastToEnemyAgent(out var enemy))
                {
                    var damage = enemy.Damage;
                    damage.ExplosionDamage(damage.HealthMax, Vector3.zero, Vector3.zero);
                }
                else if (inflictor.TryCastToPlayerAgent(out var player))
                {
                    var damage = player.Damage;
                    damage.ExplosionDamage(damage.HealthMax, Vector3.zero, Vector3.zero);
                }
            }
        }

        public static void DoExplode(this IExplosionSetting setting, Agent from, bool useRagdoll = false, bool useClientPos = true)
        {
            var position = from.EyePosition;
            if (useRagdoll)
                from.GetRagdollPosition(ref position);
            if (useClientPos)
                MakeExplosion(setting, from, useRagdoll);
            else
                MakeExplosion(setting, position);
            MakeNoise(setting, from.CourseNode, position);
        }

        public static void DoLocalExplode(this IExplosionSetting setting, Vector3 position)
        {
            MakeLocalExplosion(setting, position);
            if (SNetwork.SNet.IsMaster)
                TryMakeNoise(setting, position);
        }

        public static void MakeExplosion(this IExplosionSetting setting, Agent agent, bool useRagdoll = false)
        {
            var maxDamage = setting.Damage.GetAbsValue(PlayerData.MaxHealth);
            if (maxDamage != 0.0f)
            {
                var data = new ExplosionAgentData()
                {
                    useRagdoll = useRagdoll,
                    damage = maxDamage,
                    enemyMulti = setting.EnemyDamageMulti,
                    minRange = setting.MinRange,
                    maxRange = setting.MaxRange,
                    enemyMinRange = setting.EnemyMinRange.GetAbsValue(setting.MinRange),
                    enemyMaxRange = setting.EnemyMaxRange.GetAbsValue(setting.MaxRange),
                    lightColor = setting.LightColor
                };
                data.agent.Set(agent);

                ExplosionManager.DoExplosion(data);
            }
        }

        public static void MakeExplosion(this IExplosionSetting setting, Vector3 position)
        {
            var maxDamage = setting.Damage.GetAbsValue(PlayerData.MaxHealth);
            if (maxDamage != 0.0f)
            {
                ExplosionManager.DoExplosion(new ExplosionData()
                {
                    position = position,
                    damage = maxDamage,
                    enemyMulti = setting.EnemyDamageMulti,
                    minRange = setting.MinRange,
                    maxRange = setting.MaxRange,
                    enemyMinRange = setting.EnemyMinRange.GetAbsValue(setting.MinRange),
                    enemyMaxRange = setting.EnemyMaxRange.GetAbsValue(setting.MaxRange),
                    lightColor = setting.LightColor
                });
            }
        }

        public static void MakeLocalExplosion(this IExplosionSetting setting, Vector3 position)
        {
            var maxDamage = setting.Damage.GetAbsValue(PlayerData.MaxHealth);
            if (maxDamage != 0.0f)
            {
                ExplosionManager.DoLocalExplosion(new ExplosionData()
                {
                    position = position,
                    damage = maxDamage,
                    enemyMulti = setting.EnemyDamageMulti,
                    minRange = setting.MinRange,
                    maxRange = setting.MaxRange,
                    enemyMinRange = setting.EnemyMinRange.GetAbsValue(setting.MinRange),
                    enemyMaxRange = setting.EnemyMaxRange.GetAbsValue(setting.MaxRange),
                    lightColor = setting.LightColor
                });
            }
        }

        public static void TryMakeNoise(this IExplosionSetting setting, Vector3 position)
        {
            var newPos = position;
            if (!PhysicsUtil.SlamPos(ref newPos, Vector3.down, 64.0f, LayerManager.MASK_LEVELGEN, false, 0.0f, 0.0f))
            {
                return;
            }

            if (AIG_GeomorphNodeVolume.TryGetCourseNode(Dimension.GetDimensionFromPos(newPos).DimensionIndex, newPos, out var courseNode))
            {
                MakeNoise(setting, courseNode, position);
            }
        }

        public static void MakeNoise(this IExplosionSetting setting, AIG_CourseNode node, Vector3 position)
        {
            if (node == null)
                return;

            var noise = new NM_NoiseData()
            {
                noiseMaker = null,
                position = position,
                radiusMin = setting.NoiseMinRange,
                radiusMax = setting.NoiseMaxRange,
                yScale = 1,
                node = node,
                type = setting.NoiseType,
                includeToNeightbourAreas = true,
                raycastFirstNode = false
            };
            NoiseManager.MakeNoise(noise);
        }
    }
}
