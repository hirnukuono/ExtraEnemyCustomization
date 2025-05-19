﻿using EEC.Events;
using EEC.Utils;
using Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EEC.CustomSettings.CustomProjectiles
{
    [CallConstructorOnLoad]
    public static class CustomProjectileManager
    {
        public static bool AssetLoaded { get; internal set; } = false;

        private static readonly Dictionary<byte, ProjectileData> _projDataLookup = new(200);
        private static readonly Dictionary<int, ProjectileData> _instanceProjLookup = new(200);

        static CustomProjectileManager()
        {
            LevelEvents.LevelCleanup += () =>
            {
                _instanceProjLookup.Clear();
            };

            ProjectileEvents.CollidedWorld += (ProjectileBase proj, GameObject obj) =>
            {
                var instanceID = proj.gameObject.GetInstanceID();
                var data = GetInstanceData(instanceID);
                if (data == null || data.Settings == null)
                    return;

                var settings = data.Settings;
                settings.DoCollisionEffect(proj.transform.position);
                settings.DoDestroyEffect(proj, ProjectileDestroyedReason.CollideWorld);
            };

            ProjectileEvents.CollidedPlayer += (ProjectileBase proj, PlayerAgent agent) =>
            {
                var instanceID = proj.gameObject.GetInstanceID();
                var data = GetInstanceData(instanceID);
                if (data == null || data.Settings == null)
                    return;

                var settings = data.Settings;
                settings.DoCollisionEffect(proj.transform.position, agent);
                settings.DoDestroyEffect(proj, ProjectileDestroyedReason.CollidePlayer);
            };

            ProjectileEvents.LifeTimeDone += (ProjectileTargeting proj) =>
            {
                var instanceID = proj.gameObject.GetInstanceID();
                var data = GetInstanceData(instanceID);
                data?.Settings?.DoDestroyEffect(proj, ProjectileDestroyedReason.LifeTimeDone);
            };
        }

        public static void GenerateProjectile(CustomProjectile projInfo)
        {
            Logger.Verbose($"Trying to Add Projectile... {projInfo.DebugName}");

            if (!AssetLoaded)
            {
                Logger.Error($"Cannot Create CustomProjectile before asset fully loaded!");
                return;
            }

            if (Enum.IsDefined(typeof(ProjectileType), projInfo.ID))
            {
                Logger.Error($"ProjectileID Conflict with Official ID!, ProjID: {projInfo.ID}");
                return;
            }

            if (_projDataLookup.ContainsKey(projInfo.ID))
            {
                Logger.Error($"ProjectileID Conflict!, ProjID: {projInfo.ID}");
                return;
            }

            if (!Enum.IsDefined(typeof(ProjectileType), projInfo.BaseProjectile))
            {
                Logger.Error($"BaseProjectile should be one of the from official!, ProjID: {projInfo.ID}");
                return;
            }

            var basePrefab = ProjectileManager.Current.m_projectilePrefabs[(int)projInfo.BaseProjectile];
            var newPrefab = UnityEngine.Object.Instantiate(basePrefab);
            UnityEngine.Object.DontDestroyOnLoad(newPrefab);

            if (newPrefab.TryGetComp<ProjectileBase>(out var projectileBase))
            {
                projectileBase.m_maxDamage = projInfo.Damage.GetAbsValue(PlayerData.MaxHealth, projectileBase.m_maxDamage);
                projectileBase.m_maxInfection = projInfo.Infection.GetAbsValue(PlayerData.MaxInfection, projectileBase.m_maxInfection);
                var targeting = projectileBase.TryCast<ProjectileTargeting>();
                if (targeting != null)
                {
                    targeting.Speed = projInfo.Speed.GetAbsValue(targeting.Speed);
                    targeting.m_checkEvasiveDis = projInfo.CheckEvasiveDistance.GetAbsValue(targeting.m_checkEvasiveDis);
                    targeting.TargetStrength = projInfo.HomingStrength.GetAbsValue(targeting.TargetStrength);
                    targeting.m_targetingDelay = projInfo.HomingDelay.GetAbsValue(targeting.m_targetingDelay);
                    targeting.m_initialTargetStrength = projInfo.InitialHomingStrength.GetAbsValue(targeting.m_initialTargetStrength);
                    targeting.m_initialTargetingDuration = projInfo.InitialHomingDuration.GetAbsValue(targeting.m_initialTargetingDuration);
                    targeting.LightColor = projInfo.GlowColor;
                    targeting.LightRange = projInfo.GlowRange.GetAbsValue(targeting.LightRange);
                }
                else
                {
                    Logger.Warning($"ProjectileBase is not a ProjectileTargeting, Ignore few settings, ProjID: {projInfo.ID}, Name: {projInfo.DebugName}");
                }
            }
            else
            {
                Logger.Error($"Projectile Base Prefab Doesn't have ProjectileBase, Are you sure?, ProjID: {projInfo.ID}, Name: {projInfo.DebugName}");
            }

            var trail = newPrefab.GetComponentInChildren<TrailRenderer>();
            if (trail != null)
            {
                trail.startColor = projInfo.TrailColor;
                trail.endColor = Color.clear;
                trail.time = projInfo.TrailTime.GetAbsValue(trail.time);
                trail.widthMultiplier = projInfo.TrailWidth.GetAbsValue(trail.widthMultiplier);
            }
            newPrefab.SetActive(false);
            newPrefab.name = "GeneratedProjectilePrefab_" + projInfo.ID;
            _projDataLookup.Add(projInfo.ID, new ProjectileData()
            {
                Prefab = newPrefab,
                Settings = projInfo
            });
            Logger.Debug($"Added Projectile!: {projInfo.ID} ({projInfo.DebugName})");
        }

        public static void DestroyAllProjectile()
        {
            foreach (var data in _projDataLookup.Values)
            {
                UnityEngine.Object.Destroy(data.Prefab);
            }
            Logger.Debug("Custom Projectile has Cleaned up!");

            _projDataLookup.Clear();
        }

        public static ProjectileData GetProjectileData(byte id)
        {
            if (_projDataLookup.TryGetValue(id, out var data))
            {
                return data;
            }
            return null;
        }

        public static ProjectileData GetInstanceData(int id)
        {
            if (_instanceProjLookup.TryGetValue(id, out var data))
            {
                return data;
            }
            return null;
        }

        public static void RemoveInstanceLookup(int id)
        {
            _instanceProjLookup.Remove(id);
        }

        public class ProjectileData
        {
            public GameObject Prefab;
            public CustomProjectile Settings;

            public void RegisterInstance(GameObject gameObject)
            {
                if (gameObject.TryGetComp<ProjectileTargeting>(out var projectile))
                {
                    var instanceID = gameObject.GetInstanceID();

                    _instanceProjLookup[instanceID] = this;

                    UnityEventHandler update = null;
                    if (Settings?.SpeedChange?.Enabled ?? false)
                    {
                        var speedChange = Settings.SpeedChange;
                        var inv = 1.0f / speedChange.Duration;
                        var progress = 0.0f;
                        var originalSpeed = projectile.Speed;
                        var enabled = true;
                        update += (_) =>
                        {
                            if (!enabled)
                                return;

                            if (speedChange.StopAfterDuration)
                            {
                                if (progress >= speedChange.Duration)
                                {
                                    projectile.Speed = originalSpeed * speedChange.StopMulti;
                                    enabled = false;
                                    return;
                                }
                            }

                            var multi = speedChange.EvaluateMultiplier(progress * inv);
                            projectile.Speed = originalSpeed * multi;
                            progress += Time.deltaTime;
                        };
                    }

                    if (Settings?.HomingStrengthChange?.Enabled ?? false)
                    {
                        var homingChange = Settings.HomingStrengthChange;
                        var inv = 1.0f / homingChange.Duration;
                        var progress = 0.0f;
                        var originalHoming = projectile.TargetStrength;
                        var enabled = true;
                        update += (_) =>
                        {
                            if (!enabled)
                                return;

                            if (homingChange.StopAfterDuration)
                            {
                                if (progress >= homingChange.Duration)
                                {
                                    projectile.Speed = originalHoming * homingChange.StopMulti;
                                    enabled = false;
                                    return;
                                }
                            }

                            var multi = homingChange.EvaluateMultiplier(progress * inv);
                            projectile.TargetStrength = originalHoming * multi;
                            progress += Time.deltaTime;
                        };
                    }

                    if (Settings?.HitEnemies ?? false)
                    {
                        Vector3 moveVector = default;
                        Ray moveRay = default;
                        update += (_) =>
                        {
                            moveVector = projectile.m_myPos - projectile.m_lastPos;
                            float maxDistance = Mathf.Max(0.5f, ProjectileBase.s_tempMove.magnitude);
                            moveRay.origin = projectile.m_lastPos;
                            moveVector.Normalize();
                            moveRay.direction = moveVector;
                            if (Physics.Raycast(moveRay, out var rayHit, maxDistance, LayerManager.MASK_ENEMY_DAMAGABLE))
                            {
                                // Cancel hit if projectile hit the owner
                                if (projectile.TryGetOwner(out var owner))
                                {
                                    var agent = rayHit.collider.GetComponent<IDamageable>().GetBaseAgent();
                                    if (agent.Pointer == owner.Pointer)
                                        return;
                                }
                                projectile.Collision(moveRay, rayHit);
                            }
                        };
                    }

                    MonoBehaviourEventHandler.AttatchToObject(gameObject, onUpdate: update, onDestroyed: (_) =>
                    {
                        RemoveInstanceLookup(instanceID);
                    });
                }
            }
        }
    }
}