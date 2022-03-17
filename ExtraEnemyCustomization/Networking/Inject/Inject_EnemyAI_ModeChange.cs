using Enemies;
using HarmonyLib;
using SNetwork;

namespace EECustom.Networking.Inject
{
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.ModeChange))]
    internal static class Inject_EnemyAI_ModeChange
    {
        [HarmonyWrapSafe]
        internal static void Postfix(EnemyAI __instance)
        {
            if (SNet.IsMaster)
            {
                NetworkManager.EnemyAgentModeState.SetState(__instance.m_enemyAgent.GlobalID, __instance.Mode);
            }
        }
    }
}