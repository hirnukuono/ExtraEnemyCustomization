using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UnityEngine;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(ProjectileBase), nameof(ProjectileBase.Collision))]
    internal static class Inject_ProjectileBase
    {
        [HarmonyWrapSafe]
        [SuppressMessage("Type Safety", "UNT0014:Invalid type for call to GetComponent", Justification = "IDamagable IS Unity Component Interface")]
        internal static void Prefix(ProjectileBase __instance, RaycastHit hit)
        {
            if (hit.collider == null)
                return;

            if (!hit.collider.TryGetComponent<IDamageable>(out var damagable))
                return;

            var baseAgent = damagable.GetBaseAgent();
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
