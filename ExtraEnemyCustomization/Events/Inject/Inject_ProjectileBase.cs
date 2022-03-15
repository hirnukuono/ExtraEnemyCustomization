using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(ProjectileBase), nameof(ProjectileBase.Collision))]
    internal static class Inject_ProjectileBase
    {
        [HarmonyWrapSafe]
        internal static void Prefix(ProjectileBase __instance, RaycastHit hit)
        {
            var baseAgent = hit.collider?.GetComponent<IDamageable>()?.GetBaseAgent() ?? null;
            if (baseAgent == null)
                return;

            if (!baseAgent.TryCastToPlayerAgent(out var playerAgent))
                return;

            if (__instance.TryGetOwner(out var owner))
            {
                LocalPlayerDamageEvents.OnProjectileDamage(playerAgent, owner, __instance, __instance.m_maxDamage);
            }
        }
    }
}
