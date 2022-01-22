using EECustom.Customizations.Shared;
using EECustom.CustomSettings.DTO;
using EECustom.Events;
using EECustom.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EECustom.CustomSettings
{
    public static class CustomProjectileManager
    {
        public static bool AssetLoaded { get; internal set; } = false;

        private static readonly Dictionary<byte, ProjectileData> _projDataLookup = new();
        private static readonly Dictionary<int, ProjectileData> _instanceProjLookup = new();

        static CustomProjectileManager()
        {
            LevelEvents.LevelCleanup += () =>
            {
                _instanceProjLookup.Clear();
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

            var newPrefab = ProjectileManager.SpawnProjectileType(projInfo.BaseProjectile, Vector3.zero, Quaternion.identity);
            //UnityEngine.Object.DontDestroyOnLoad(newPrefab); //Doesn't needed, newPrefab will be generated under ProjectileManager so it will not be destroyed

            var projectileBase = newPrefab.GetComponent<ProjectileBase>();
            if (projectileBase != null)
            {
                projectileBase.m_maxDamage = projInfo.Damage.GetAbsValue(PlayerData.MaxHealth, projectileBase.m_maxDamage);
                projectileBase.m_maxInfection = projInfo.Infection.GetAbsValue(PlayerData.MaxInfection, projectileBase.m_maxInfection);
                var targeting = projectileBase.TryCast<ProjectileTargeting>();
                if (targeting != null)
                {
                    targeting.Speed = projInfo.Speed.GetAbsValue(targeting.Speed);
                    targeting.TargetStrength = projInfo.HomingStrength.GetAbsValue(targeting.TargetStrength);
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
            newPrefab.SetActive(false);
            newPrefab.name = "GeneratedProjectilePrefab_" + projInfo.ID;
            _projDataLookup.Add(projInfo.ID, new ProjectileData()
            {
                Prefab = newPrefab,
                Explosion = projInfo.Explosion,
                Knockback = projInfo.Knockback,
                Bleed = projInfo.Bleed
            });
            Logger.Debug($"Added Projectile!: {projInfo.ID} ({projInfo.DebugName})");
        }

        public static void DestroyAllProjectile()
        {
            foreach (var data in _projDataLookup.Values)
            {
                GameObject.Destroy(data.Prefab);
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
            public ExplosionSetting Explosion;
            public KnockbackSetting Knockback;
            public BleedSetting Bleed;

            public void RegisterInstance(GameObject gameObject)
            {
                var projectile = gameObject.GetComponent<ProjectileTargeting>();
                if (projectile != null)
                {
                    var instanceID = gameObject.GetInstanceID();

                    _instanceProjLookup[instanceID] = this;

                    MonoBehaviourEventHandler.AttatchToObject(gameObject, onDestroyed: (_) =>
                    {
                        RemoveInstanceLookup(instanceID);
                    });
                }
            }
        }
    }
}