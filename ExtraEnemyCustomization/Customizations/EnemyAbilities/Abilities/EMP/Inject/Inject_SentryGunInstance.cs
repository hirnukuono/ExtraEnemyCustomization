using EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers;
using HarmonyLib;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Inject
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