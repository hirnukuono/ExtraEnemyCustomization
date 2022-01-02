using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Gear;
using AK;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Inject
{
    [HarmonyPatch(typeof(EnemyScanner))]
    internal class Inject_EnemyScanner
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(EnemyScanner.UpdateDetectedEnemies))]
        public static bool Pre_UpdateDetectedEnemies()
        {
            return !EMPHandlerBase.IsLocalPlayerDisabled;
        }

        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(EnemyScanner.UpdateTagProgress))]
        public static bool Pre_UpdateTagProgress(EnemyScanner __instance)
        { 
            if (EMPHandlerBase.IsLocalPlayerDisabled)
            {
                __instance.m_screen.SetStatusText("ERROR");
                return false;
            }
            __instance.m_screen.SetStatusText("Ready to tag");
            return true;
        }
    }
}
