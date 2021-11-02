using Enemies;
using HarmonyLib;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(ScoutAntenna), nameof(ScoutAntenna.Init))]
    internal class Inject_ScoutAntenna
    {
        [HarmonyWrapSafe]
        public static void Postfix(ScoutAntennaDetection detection, ScoutAntenna __instance)
        {
            ScoutAntennaSpawnEvent.OnAntennaSpawn?.Invoke(detection.m_owner, detection, __instance);
        }
    }
}