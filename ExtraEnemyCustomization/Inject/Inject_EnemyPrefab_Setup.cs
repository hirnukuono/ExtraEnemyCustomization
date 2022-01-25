using EECustom.Managers;
using Enemies;
using HarmonyLib;
using UnityEngine;

namespace EECustom.Inject
{
    [HarmonyPatch(typeof(EnemyPrefabManager), nameof(EnemyPrefabManager.BuildEnemyPrefab))]
    internal static class Inject_EnemyPrefab_Setup
    {
        [HarmonyWrapSafe]
        internal static void Postfix(GameObject __result)
        {
            var agent = __result.GetComponent<EnemyAgent>();
            if (agent is null)
            {
                Logger.Error($"Agent is null! : {__result.name}");
                return;
            }
            ConfigManager.Current.FirePrefabBuiltEvent(__result.GetComponent<EnemyAgent>());
        }
    }
}