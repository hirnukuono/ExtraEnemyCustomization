using EECustom.Managers;
using Enemies;
using HarmonyLib;

namespace EECustom.Inject
{
    [HarmonyPatch(typeof(EnemySync), nameof(EnemySync.OnSpawn))]
    internal static class Inject_EnemySync_OnSpawn
    {
        [HarmonyWrapSafe]
        internal static void Postfix(EnemySync __instance)
        {
            ConfigManager.Current.FireSyncSpawnedEvent(__instance.m_agent);
        }
    }
}