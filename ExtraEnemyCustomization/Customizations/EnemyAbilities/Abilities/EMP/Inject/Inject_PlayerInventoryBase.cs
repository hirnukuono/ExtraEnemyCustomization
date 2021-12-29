using EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers;
using HarmonyLib;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Inject
{
    [HarmonyPatch(typeof(PlayerInventoryBase))]
    internal class Inject_PlayerInventoryBase
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerInventoryBase.Setup))]
        public static void Pre_Setup(PlayerInventoryBase __instance)
        {
            EMPController controller = __instance.gameObject.AddComponent<EMPController>();
            controller.AssignHandler(new EMPPlayerFlashLightHandler());
        }
    }
}
