using Enemies;
using HarmonyLib;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(ScoutAntennaDetection), nameof(ScoutAntennaDetection.OnSpawn))]
    internal class Inject_ScoutAntennaDetection
    {
        [HarmonyWrapSafe]
        internal static void Prefix(pScoutAntennaDetectionSpawnData spawnData, ScoutAntennaDetection __instance)
        {
            if (spawnData.owner.TryGet(out var owner))
            {
                ScoutAntennaSpawnEvent.OnDetectionSpawn(owner, __instance);
            }
        }
    }
}