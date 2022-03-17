using Enemies;
using HarmonyLib;

namespace EEC.Events.Inject
{
    [HarmonyPatch(typeof(ScoutAntenna), nameof(ScoutAntenna.Init))]
    internal static class Inject_ScoutAntenna
    {
        [HarmonyWrapSafe]
        internal static void Postfix(ScoutAntennaDetection detection, ScoutAntenna __instance)
        {
            ScoutAntennaSpawnEvent.OnAntennaSpawn(detection.m_owner, detection, __instance);
        }
    }
}