using HarmonyLib;
using EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Inject
{
    [HarmonyPatch(typeof(SentryGunInstance))]
    internal class Inject_SentryGunInstance
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(SentryGunInstance.Setup))]
        public static void Post_Setup(SentryGunInstance __instance)
        {
            EMPController controller = __instance.gameObject.AddComponent<EMPController>();
            controller.AssignHandler(new EMPSentryHandler());
        }
    }
}
