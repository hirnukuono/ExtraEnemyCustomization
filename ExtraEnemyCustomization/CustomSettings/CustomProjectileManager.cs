using EECustom.CustomSettings.DTO;
using EECustom.CustomSettings.Handlers;
using EECustom.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EECustom.CustomSettings
{
    public static class CustomProjectileManager
    {
        private static readonly Dictionary<byte, GameObject> _projectilePrefabs = new();

        public static void GenerateProjectile(CustomProjectile projInfo)
        {
            if (Enum.IsDefined(typeof(ProjectileType), projInfo.ID))
            {
                Logger.Error($"ProjectileID Conflict with Official ID!, ProjID: {projInfo.ID}");
                return;
            }

            if (_projectilePrefabs.ContainsKey(projInfo.ID))
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
            var newPrefab = GameObject.Instantiate(basePrefab);
            UnityEngine.Object.DontDestroyOnLoad(newPrefab);
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

                var explosiveDamage = projInfo.ExplosionDamage.GetAbsValue(PlayerData.MaxHealth);
                if (explosiveDamage > 0.0f)
                {
                    Logger.Debug($"Adding Explosive Effect!  Dmg: {explosiveDamage}");
                    var explosive = newPrefab.gameObject.AddComponent<ExplosiveProjectileHandler>();
                    explosive.Damage = explosiveDamage;
                    explosive.EnemyMulti = projInfo.ExplosionEnemyDamageMulti;
                    explosive.MinRange = projInfo.ExplosionMinRange;
                    explosive.MaxRange = projInfo.ExplosionMaxRange;
                    explosive.NoiseMinRange = projInfo.ExplosionNoiseMinRange;
                    explosive.NoiseMaxRange = projInfo.ExplosionNoiseMaxRange;
                }
            }
            else
            {
                Logger.Error($"Projectile Base Prefab Doesn't have ProjectileBase, Are you sure?, ProjID: {projInfo.ID}, Name: {projInfo.DebugName}");
            }
            newPrefab.SetActive(false);
            newPrefab.name = "GeneratedProjectilePrefab_" + projInfo.ID;
            _projectilePrefabs.Add(projInfo.ID, newPrefab);
            Logger.Debug($"Added Projectile!: {projInfo.ID} ({projInfo.DebugName})");
        }

        public static GameObject GetProjectile(byte id)
        {
            if (_projectilePrefabs.TryGetValue(id, out var prefab))
            {
                return prefab;
            }
            return null;
        }
    }
}