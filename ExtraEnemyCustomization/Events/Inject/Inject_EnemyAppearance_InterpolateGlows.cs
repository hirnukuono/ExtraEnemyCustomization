using HarmonyLib;
using System;
using UnityEngine;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(EnemyAppearance))]
    internal class Inject_EnemyAppearance_InterpolateGlows
    {
        [HarmonyPatch(nameof(EnemyAppearance.InterpolateGlow), new Type[] { typeof(Color), typeof(Vector4), typeof(float) })]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        private static void Pre_InterpolateGlow(ref Color col, Vector4 pos, float transitionTime, EnemyAppearance __instance)
        {
            col = EnemyGlowEvents.FireEvent(__instance.m_owner, col, pos);
        }
    }
}