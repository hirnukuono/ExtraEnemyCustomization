using Agents;
using EECustom.Customizations.Models.Handlers;
using Enemies;
using HarmonyLib;

namespace EECustom.Customizations.Models.Inject
{
    [HarmonyPatch(typeof(ES_PathMove), nameof(ES_PathMove.CommonEnter))]
    internal static class Inject_ES_PathMove
    {
        [HarmonyWrapSafe]
        internal static void Prefix(ES_PathMove __instance)
        {
            if (__instance.m_enemyAgent.gameObject.TryGetComponent<ScannerHandler>(out var scannerManager))
            {
                scannerManager.UpdateAgentMode(AgentMode.Agressive);
                __instance.m_enemyAgent.ScannerData.m_soundIndex = 0;
            }
        }
    }

    [HarmonyPatch(typeof(ES_PathMoveFlyer), nameof(ES_PathMoveFlyer.CommonEnter))]
    internal static class Inject_ES_PathMove_Flyer
    {
        [HarmonyWrapSafe]
        internal static void Prefix(ES_PathMoveFlyer __instance)
        {
            if (__instance.m_enemyAgent.gameObject.TryGetComponent<ScannerHandler>(out var scannerManager))
            {
                scannerManager.UpdateAgentMode(AgentMode.Agressive);
                __instance.m_enemyAgent.ScannerData.m_soundIndex = 0;
            }
        }
    }

    [HarmonyPatch(typeof(ES_Hibernate), nameof(ES_Hibernate.CommonExit))]
    internal static class Inject_Hibernate
    {
        [HarmonyWrapSafe]
        internal static void Prefix(ES_HibernateWakeUp __instance)
        {
            if (__instance.m_enemyAgent.gameObject.TryGetComponent<ScannerHandler>(out var scannerManager))
            {
                scannerManager.UpdateAgentMode(AgentMode.Agressive);
                __instance.m_enemyAgent.ScannerData.m_soundIndex = 0;
            }
        }
    }
}