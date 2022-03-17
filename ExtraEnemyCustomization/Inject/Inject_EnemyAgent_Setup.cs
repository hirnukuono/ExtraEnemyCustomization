using EEC.Events;
using EEC.Managers;
using Enemies;
using HarmonyLib;

namespace EEC.Inject
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.Setup))]
    internal static class Inject_EnemyAgent_Setup
    {
        [HarmonyWrapSafe]
        internal static void Prefix(EnemyAgent __instance)
        {
            if (!__instance.m_isSetup)
            {
                EnemyEvents.OnSpawn(__instance);
            }
            else
            {
                EnemyEvents.OnSpawned(__instance);
                ConfigManager.FireSpawnedEvent(__instance);

                __instance.AddOnDeadOnce(() =>
                {
                    ConfigManager.FireDeadEvent(__instance);
                });
            }
        }
    }
}