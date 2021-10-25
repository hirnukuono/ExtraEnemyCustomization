using EECustom.Customizations;
using EECustom.Managers;
using HarmonyLib;
using System;
using UnityEngine;

namespace EECustom.Inject
{
    [HarmonyPatch(typeof(EnemyAppearance))]
    internal class Inject_EnemyAppearance_InterpolateGlows
    {
        [HarmonyPatch(nameof(EnemyAppearance.InterpolateGlow), new Type[] { typeof(Color), typeof(Vector4), typeof(float) })]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        private static void Pre_InterpolateGlow(ref Color col, ref Vector4 pos, float transitionTime, EnemyAppearance __instance)
        {
            var glowInfo = new GlowInfo(col, pos);
            if (ConfigManager.Current.FireGlowEvent(__instance.m_owner, ref glowInfo))
            {
                col = glowInfo.Color;
                pos = glowInfo.Position;
            }
        }
    }
}