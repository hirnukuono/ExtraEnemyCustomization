using HarmonyLib;
using Player;
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
            if (data != null)
            {
                if (data.Explosion.Enabled)
                    data.Explosion.DoExplode(__instance.transform.position);

                var baseAgent = hit.collider?.GetComponent<IDamageable>()?.GetBaseAgent() ?? null;
                if (baseAgent == null)
                    return;

                if (baseAgent.TryCastToPlayerAgent(out var agent))
                    return;

                if (data.Knockback.Enabled)
                    data.Knockback.DoKnockbackIgnoreDistance(__instance.transform.position, agent);

                if (data.Bleed.Enabled)
                    data.Bleed.DoBleed(agent);
            }
        }
    }
}