using EECustom.CustomAbilities.EMP.Handlers;
using HarmonyLib;
using LevelGeneration;

namespace EECustom.CustomAbilities.EMP.Inject
{
    [HarmonyPatch(typeof(LG_Light))]
    internal static class Inject_LG_Light
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(LG_Light.Start))]
        internal static void Pre_Start(LG_Light __instance)
        {
            EMPController controller = __instance.gameObject.AddComponent<EMPController>();
            controller.AssignHandler(new EMPLightHandler());
        }
    }
}