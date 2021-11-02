﻿using Enemies;
using HarmonyLib;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.SyncPlaceNavMarkerTag))]
    internal class Inject_Enemy_Marked
    {
        [HarmonyWrapSafe]
        public static void Postfix(EnemyAgent __instance)
        {
            EnemyMarkerEvents.OnMarked?.Invoke(__instance, __instance.m_tagMarker);
        }
    }
}