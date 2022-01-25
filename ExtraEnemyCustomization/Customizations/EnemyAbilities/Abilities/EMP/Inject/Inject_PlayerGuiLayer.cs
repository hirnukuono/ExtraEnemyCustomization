using EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers;
using HarmonyLib;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Inject
{
    [HarmonyPatch(typeof(PlayerGuiLayer))]
    internal static class Inject_PlayerGuiLayer
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerGuiLayer.UpdateGUIElementsVisibility))]
        internal static bool Pre_UpdateGUIElementsVisibility()
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