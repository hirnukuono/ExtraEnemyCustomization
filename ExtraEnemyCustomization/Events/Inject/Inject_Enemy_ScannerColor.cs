using Enemies;
using HarmonyLib;
using UnityEngine;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(MethodType.Setter)]
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.ScannerColor))]
    internal class Inject_Enemy_ScannerColor
    {
        [HarmonyWrapSafe]
        public static void Postfix(EnemyAgent __instance, Color value)
        {
            EnemyScannerColorEvents.OnChanged?.Invoke(__instance, value);
        }
    }
}