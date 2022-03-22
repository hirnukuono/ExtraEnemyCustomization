using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EEC.CustomSettings.CustomProjectiles.Inject
{
    [HarmonyPatch(typeof(ProjectileTargeting), nameof(ProjectileTargeting.OnFire))]
    internal static class Inject_ProjectileTargeting_OnFire
    {
        [HarmonyWrapSafe]
        internal static void Postfix(ProjectileTargeting __instance)
        {
            var instanceData = CustomProjectileManager.GetInstanceData(__instance.gameObject.GetInstanceID());
            if (instanceData != null)
            {
                var time = Clock.Time;
                var lifeTime = __instance.m_endLifeTime - time;
                __instance.m_endLifeTime = instanceData.Settings.LifeTime.GetAbsValue(lifeTime) + time;
            }
        }
    }
}
