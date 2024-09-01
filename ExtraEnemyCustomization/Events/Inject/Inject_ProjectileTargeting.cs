using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EEC.Events.Inject
{
    [HarmonyPatch(typeof(ProjectileTargeting), nameof(ProjectileTargeting.OnDestroy))]
    internal static class Inject_ProjectileTargeting
    {
        [HarmonyWrapSafe]
        internal static void Prefix(ProjectileTargeting __instance)
        {
            if (!__instance.m_active && __instance.m_endLifeTime < Clock.Time)
            {
                ProjectileEvents.OnLifeTimeDone(__instance);
            }
        }
    }
}
