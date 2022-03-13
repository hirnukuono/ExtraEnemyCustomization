using HarmonyLib;
using UnityEngine;

namespace EECustom.CustomSettings.Inject
{
    [HarmonyPatch(typeof(ProjectileBase))]
    internal static class Inject_ProjectileBase
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(ProjectileBase.Collision))]
        internal static void Pre_Collision(ProjectileBase __instance, RaycastHit hit)
        {
            var instanceID = __instance.gameObject.GetInstanceID();
            var data = CustomProjectileManager.GetInstanceData(instanceID);
            data?.Settings?.Collision(__instance.transform.position, hit);
        }
    }
}