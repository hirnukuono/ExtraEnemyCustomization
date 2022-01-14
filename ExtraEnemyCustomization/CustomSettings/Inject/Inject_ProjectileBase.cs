using HarmonyLib;
using Player;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.CustomSettings.Inject
{
    [HarmonyPatch(typeof(ProjectileBase))]
    internal class Inject_ProjectileBase
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(ProjectileBase.Collision))]
        public static void Pre_Collision(ProjectileBase __instance, RaycastHit hit)
        {
            var instanceID = __instance.gameObject.GetInstanceID();
            var data = CustomProjectileManager.GetInstanceData(instanceID);
            if (data != null)
            {
                if (data.Explosion.Enabled)
                    data.Explosion.DoExplode(__instance.transform.position);

                var baseAgent = hit.collider?.GetComponent<IDamageable>()?.GetBaseAgent() ?? null;
                if (baseAgent == null)
                    return;

                var agent = baseAgent.Cast<PlayerAgent>();
                if (agent == null)
                    return;

                if (data.Knockback.Enabled)
                    data.Knockback.DoKnockbackIgnoreDistance(__instance.transform.position, agent);

                if (data.Bleed.Enabled)
                    data.Bleed.DoBleed(agent);
            }
        }
    }
}
