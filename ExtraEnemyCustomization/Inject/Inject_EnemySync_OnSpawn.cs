using EECustom.Managers;
using Enemies;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Inject
{
    [HarmonyPatch(typeof(EnemySync), nameof(EnemySync.OnSpawn))]
    internal static class Inject_EnemySync_OnSpawn
    {
        [HarmonyWrapSafe]
        internal static void Postfix(EnemySync __instance)
        {
            ConfigManager.Current.FirePrefabBuiltEvent(__instance.m_agent);
        }
    }
}
