using EEC.CustomAbilities.EMP.Handlers;
using HarmonyLib;

namespace EEC.CustomAbilities.EMP.Inject
{
    [HarmonyPatch(typeof(SentryGunInstance))]
    internal static class Inject_SentryGunInstance
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(SentryGunInstance.Setup))]
        internal static void Post_Setup(SentryGunInstance __instance)
        {
            EMPController controller = __instance.gameObject.AddComponent<EMPController>();
            controller.AssignHandler(new EMPSentryHandler());
        }
    }
}