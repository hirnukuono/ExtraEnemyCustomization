using EECustom.Customizations.EnemyAbilities.Abilities.EMP.Targets;
using HarmonyLib;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Inject
{
    [HarmonyPatch(typeof(LG_Light))]
    internal class Inject_LG_Light
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(LG_Light.Start))]
        public static void Pre_Start(LG_Light __instance)
        {
            __instance.gameObject.AddComponent<EMPLight>();
            Logger.Debug($"Added EmpLight to {__instance.m_name}");
        }
    }
}
