using HarmonyLib;

namespace EEC.Events.Inject
{
    [HarmonyPatch(typeof(PLOC_Downed))]
    internal static class Inject_PLOC_Downed
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PLOC_Downed.Enter))]
        private static void Post_Downed(PLOC_Downed __instance)
        {
            LocalPlayerAliveEvents.OnDown(__instance.m_owner);
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PLOC_Downed.Exit))]
        private static void Post_Revived(PLOC_Downed __instance)
        {
            LocalPlayerAliveEvents.OnRevive(__instance.m_owner);
        }
    }
}