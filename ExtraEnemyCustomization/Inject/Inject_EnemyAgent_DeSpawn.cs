using EECustom.Managers;
using Enemies;
using HarmonyLib;

namespace EECustom.Inject
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.OnDeSpawn))]
    internal static class Inject_EnemyAgent_DeSpawn
    {
        [HarmonyWrapSafe]
        private static void Prefix(EnemyAgent __instance)
        {
            ConfigManager.Current.FireDespawnedEvent(__instance);
        }
    }
}