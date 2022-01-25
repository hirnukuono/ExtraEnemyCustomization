using Enemies;
using HarmonyLib;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.SyncPlaceNavMarkerTag))]
    internal static class Inject_Enemy_Marked
    {
        [HarmonyWrapSafe]
        internal static void Postfix(EnemyAgent __instance)
        {
            EnemyMarkerEvents.OnMarked(__instance, __instance.m_tagMarker);
        }
    }
}