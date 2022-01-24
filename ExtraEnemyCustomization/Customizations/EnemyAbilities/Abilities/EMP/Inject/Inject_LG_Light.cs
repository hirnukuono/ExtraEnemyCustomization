using EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers;
using HarmonyLib;
using LevelGeneration;

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
            EMPController controller = __instance.gameObject.AddComponent<EMPController>();
            controller.AssignHandler(new EMPLightHandler());
        }
    }
}
