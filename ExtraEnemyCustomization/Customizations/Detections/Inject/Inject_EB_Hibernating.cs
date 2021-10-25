using Enemies;
using HarmonyLib;

namespace EECustom.Customizations.Detections.Inject
{
    //[HarmonyPatch(typeof(EB_Hibernating))]
    internal class Inject_EB_Hibernating
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(EB_Hibernating.UpdateDetection))]
        private static void Post_UpdateDetection(EB_Hibernating __instance)
        {
            if (__instance.m_inWakeup)
            {
                Logger.Log("Detected Wakeup!");
                __instance.m_ai.m_locomotion.ScoutScream.ActivateState(__instance.m_ai.Target);
                __instance.m_wakeUpTime = -1.0f;
            }
        }
    }
}