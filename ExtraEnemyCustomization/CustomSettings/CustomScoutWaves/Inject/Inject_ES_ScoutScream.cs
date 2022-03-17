using Enemies;
using HarmonyLib;
using SNetwork;

namespace EECustom.CustomSettings.CustomScoutWaves.Inject
{
    [HarmonyPatch(typeof(ES_ScoutScream), nameof(ES_ScoutScream.CommonUpdate))]
    internal static class Inject_ES_ScoutScream
    {
        [HarmonyWrapSafe]
        internal static void Postfix(ES_ScoutScream __instance)
        {
            if (__instance.m_state == ES_ScoutScream.ScoutScreamState.Done)
            {
                if (__instance.m_stateDoneTimer >= 0.0f)
                {
                    __instance.m_stateDoneTimer = -1.0f;
                    if (SNet.IsMaster)
                    {
                        CustomScoutWaveManager.TriggerScoutWave(__instance.m_enemyAgent);
                    }
                }
            }
        }
    }
}