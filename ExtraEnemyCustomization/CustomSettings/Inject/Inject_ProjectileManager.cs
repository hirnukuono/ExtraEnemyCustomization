using EECustom.Managers;
using HarmonyLib;
using System;
using UnityEngine;

namespace EECustom.CustomSettings.Inject
{
    [HarmonyPatch(typeof(ProjectileManager))]
    internal static class Inject_ProjectileManager
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(ProjectileManager.LoadAssets))]
        internal static void Post_LoadAsset()
        {
            CustomProjectileManager.AssetLoaded = true;

            foreach (var proj in ConfigManager.Current.ProjectileCustom.ProjectileDefinitions)
            {
                CustomProjectileManager.GenerateProjectile(proj);
            }
        }

        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(ProjectileManager.SpawnProjectileType))]
        internal static bool Pre_SpawnProjectile(ref GameObject __result, ref ProjectileType type, Vector3 pos, Quaternion rot)
        {
            if (Enum.IsDefined(typeof(ProjectileType), (byte)type))
            {
                return true;
            }

            var projInfo = CustomProjectileManager.GetProjectileData((byte)type);
            if (projInfo == null)
            {
                Logger.Error($"CANT FIND PROJECTILE DATA WITH ID: {(byte)type}");
                type = ProjectileType.TargetingSmall;
                return true;
            }

            var gameObject = GameObject.Instantiate(projInfo.Prefab, pos, rot, ProjectileManager.Current.m_root.transform);
            gameObject.SetActive(true);
            projInfo.RegisterInstance(gameObject);

            __result = gameObject;
            return false;
        }
    }
}