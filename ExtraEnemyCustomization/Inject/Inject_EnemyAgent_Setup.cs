using EECustom.Events;
using EECustom.Managers;
using EECustom.Utils;
using Enemies;
using HarmonyLib;

namespace EECustom.Inject
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.Setup))]
    internal static class Inject_EnemyAgent_Setup
    {
        [HarmonyWrapSafe]
        internal static void Prefix(EnemyAgent __instance)
        {
            EnemyEvents.OnSpawn(__instance);
        }

        [HarmonyWrapSafe]
        internal static void Postfix(EnemyAgent __instance)
        {
            if (__instance.name.InvariantEndsWith(")")) //No Replicator Number = Fake call
            {
                return;
            }

            EnemyEvents.OnSpawned(__instance);
            ConfigManager.Current.FireSpawnedEvent(__instance);
        }
    }
}