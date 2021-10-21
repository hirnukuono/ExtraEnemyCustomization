using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(EnemyAppearance))]
    class Inject_EnemyAppearance_InterpolateGlows
    {
        [HarmonyPatch(nameof(EnemyAppearance.InterpolateGlow), new Type[] { typeof(Color), typeof(Vector4), typeof(float) })]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        static void Pre_InterpolateGlow(ref Color col, Vector4 pos, float transitionTime, EnemyAppearance __instance)
        {
            col = EnemyGlowEvents.FireEvent(__instance.m_owner, col, pos);
        }
    }
}
