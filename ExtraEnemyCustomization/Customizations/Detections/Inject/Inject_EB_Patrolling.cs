using Enemies;
using HarmonyLib;

namespace EECustom.Customizations.Detections.Inject
{
    //[HarmonyPatch(typeof(EB_Patrolling))]
    internal class Inject_EB_Patrolling
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(EB_Patrolling.UpdateDetection))]
        private static void Post_UpdateDetection(EB_Patrolling __instance)
        {
            if (__instance.m_waitingForAbilityDone)
            {
            }
        }
    }
}