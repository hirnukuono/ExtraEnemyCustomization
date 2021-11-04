using EECustom.Events;
using EECustom.Managers;
using Enemies;
using HarmonyLib;

namespace EECustom.Inject
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.Setup))]
    internal static class Inject_EnemyAgent_Setup
    {
        [HarmonyWrapSafe]
        public static void Prefix(EnemyAgent __instance)
        {
            EnemyEvents.OnSpawn(__instance);
        }

        [HarmonyWrapSafe]
        public static void Postfix(EnemyAgent __instance)
        {
            if (__instance.name.EndsWith(")")) //No Replicator Number = Fake call
            {
                return;
            }
            ConfigManager.Current.RegisterTargetLookup(__instance);
            ConfigManager.Current.FireSpawnedEvent(__instance);
            EnemyEvents.OnSpawned(__instance);
        }
    }
}