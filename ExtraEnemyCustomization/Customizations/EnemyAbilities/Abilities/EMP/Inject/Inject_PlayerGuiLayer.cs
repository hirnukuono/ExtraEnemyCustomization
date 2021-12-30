using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Inject
{
    [HarmonyPatch(typeof(PlayerGuiLayer))]
    internal class Inject_PlayerGuiLayer
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerGuiLayer.UpdateGUIElementsVisibility))]
        public static bool Pre_UpdateGUIElementsVisibility(PlayerGuiLayer __instance)
        {
            if (!__instance.m_compass.Visible && EMPHandlerBase.IsLocalPlayerDisabled) return false;
            return true;
        }
    }
}
