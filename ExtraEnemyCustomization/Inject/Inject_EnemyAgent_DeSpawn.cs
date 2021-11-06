using EECustom.Events;
using Enemies;
using HarmonyLib;

namespace EECustom.Inject
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.OnDeSpawn))]
    internal static class Inject_EnemyAgent_DeSpawn
    {
        [HarmonyWrapSafe]
        public static void Prefix(EnemyAgent __instance)
        {
            EnemyEvents.OnDespawn(__instance);
        }

        [HarmonyWrapSafe]
        public static void Postfix(EnemyAgent __instance)
        {
            EnemyEvents.OnDespawned(__instance);
        }
    }
}
