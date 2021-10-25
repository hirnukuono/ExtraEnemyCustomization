using EECustom.CustomSettings.Handlers;
using EECustom.Managers;
using HarmonyLib;
using System;
using UnityEngine;

namespace EECustom.CustomSettings.Inject
{
    [HarmonyPatch(typeof(ProjectileManager))]
    internal class Inject_ProjectileManager
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(ProjectileManager.LoadAssets))]
        private static void Post_LoadAsset()
        {
            foreach (var proj in ConfigManager.Current.ProjectileCustom.ProjectileDefinitions)
            {
                CustomProjectileManager.GenerateProjectile(proj);
            }
        }

        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(ProjectileManager.SpawnProjectileType))]
        private static bool Pre_SpawnProjectile(ref GameObject __result, ref ProjectileType type, Vector3 pos, Quaternion rot)
        {
            if (Enum.IsDefined(typeof(ProjectileType), (byte)type))
            {
                return true;
            }

            var projectilePrefab = CustomProjectileManager.GetProjectile((byte)type);
            if (projectilePrefab == null)
            {
                Logger.Error($"CANT FIND PREFAB WITH ID: {(int)type}");
                type = ProjectileType.TargetingSmall;
                return true;
            }

            var gameObject = GameObject.Instantiate(projectilePrefab, pos, rot, ProjectileManager.Current.m_root.transform);
            gameObject.SetActive(true);

            var baseExplosive = projectilePrefab.GetComponent<ExplosiveProjectileHandler>(); //MINOR: Maybe Unhollower Bug? Fields are reset when you Instantiate them
            if (baseExplosive != null)
            {
                var newExplosive = gameObject.GetComponent<ExplosiveProjectileHandler>();
                newExplosive.CopyValueFrom(baseExplosive);
            }

            __result = gameObject;
            return false;
        }
    }
}