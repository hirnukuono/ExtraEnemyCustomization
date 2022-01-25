using HarmonyLib;
using static Dam_EnemyDamageBase;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(Dam_EnemyDamageBase))]
    internal static class Inject_Enemy_Limb
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Dam_EnemyDamageBase.ReceiveDestroyLimb))]
        internal static void Post_DestroyLimb(pDestroyLimbData data, Dam_EnemyDamageBase __instance)
        {
            EnemyLimbEvents.OnDestroyed(__instance.DamageLimbs[data.limbID]);
        }
    }
}