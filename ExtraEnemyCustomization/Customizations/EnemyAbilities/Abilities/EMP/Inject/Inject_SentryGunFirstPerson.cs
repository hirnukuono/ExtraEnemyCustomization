using HarmonyLib;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Inject
{
    [HarmonyPatch(typeof(SentryGunFirstPerson))]
    internal static class Inject_SentryGunFirstPerson
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(SentryGunFirstPerson.CheckCanPlace))]
        internal static bool Pre_CheckCanPlace(ref bool __result)
        {
            if (EMPHandlerBase.IsLocalPlayerDisabled)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}