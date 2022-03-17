using EECustom.EnemyCustomizations;
using EECustom.Managers;
using HarmonyLib;
using System;
using UnityEngine;

namespace EECustom.Inject
{
    [HarmonyPatch(typeof(EnemyAppearance))]
    internal static class Inject_EnemyAppearance_InterpolateGlows
    {
        [HarmonyPatch(nameof(EnemyAppearance.InterpolateGlow), new Type[] { typeof(Color), typeof(Vector4), typeof(float) })]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        internal static void Pre_InterpolateGlow(ref Color col, ref Vector4 pos, EnemyAppearance __instance)
        {
            var glowInfo = new GlowInfo(col, pos);
            if (ConfigManager.FireGlowEvent(__instance.m_owner, ref glowInfo))
            {
                col = glowInfo.Color;
                pos = glowInfo.Position;
            }
        }
    }
}