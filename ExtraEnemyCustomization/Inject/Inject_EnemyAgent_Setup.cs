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
        internal static void Prefix(EnemyAgent __instance)
        {
            if (!__instance.m_isSetup)
            {
                EnemyEvents.OnSpawn(__instance);
            }
            else
            {
                EnemyEvents.OnSpawned(__instance);
                ConfigManager.Current.FireSpawnedEvent(__instance);

                __instance.AddOnDeadOnce(() =>
                {
                    ConfigManager.Current.FireDeadEvent(__instance);
                });
            }
        }
    }
}