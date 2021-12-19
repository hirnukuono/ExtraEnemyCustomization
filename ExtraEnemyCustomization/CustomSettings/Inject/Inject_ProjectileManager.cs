using EECustom.Managers;
using HarmonyLib;
using LevelGeneration;
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
        public static void Post_LoadAsset()
        {
            foreach (var proj in ConfigManager.Current.ProjectileCustom.ProjectileDefinitions)
            {
                CustomProjectileManager.GenerateProjectile(proj);
            }
        }

        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(ProjectileManager.SpawnProjectileType))]
        public static bool Pre_SpawnProjectile(ref GameObject __result, ref ProjectileType type, Vector3 pos, Quaternion rot)
        {
            if (Enum.IsDefined(typeof(ProjectileType), (byte)type))
            {
                return true;
            }

            var projInfo = CustomProjectileManager.GetProjectileData((byte)type);
            if (projInfo == null)
            {
                Logger.Error($"CANT FIND PROJECTILE DATA WITH ID: {(int)type}");
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