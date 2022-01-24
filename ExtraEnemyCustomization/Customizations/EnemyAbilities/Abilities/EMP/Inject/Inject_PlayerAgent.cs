using EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers;
using HarmonyLib;
using Player;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Inject
{
    [HarmonyPatch(typeof(PlayerAgent))]
    internal class Inject_PlayerAgent
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerAgent.Setup))]
        public static void Post_Setup(PlayerAgent __instance)
        {
            if (!__instance.IsLocallyOwned) return;
            EMPController flashLightController = __instance.gameObject.AddComponent<EMPController>();
            flashLightController.AssignHandler(new EMPPlayerFlashLightHandler());

            EMPController hudController = __instance.gameObject.AddComponent<EMPController>();
            hudController.AssignHandler(new EMPPlayerHudHandler());
        }
    }
}
