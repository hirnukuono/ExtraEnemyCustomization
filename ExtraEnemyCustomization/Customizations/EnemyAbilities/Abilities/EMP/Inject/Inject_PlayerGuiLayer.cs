using EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers;
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
        public static bool Pre_UpdateGUIElementsVisibility()
        {
            if (EMPHandlerBase.IsLocalPlayerDisabled)
            {
                EMPPlayerHudHandler.Instance.ForceState(EMPState.Off);
                return false;
            }
            if (GameStateManager.CurrentStateName == eGameStateName.InLevel)
            {
                EMPPlayerHudHandler.Instance.ForceState(EMPState.On);
            }
            return true;
        }
    }
}
