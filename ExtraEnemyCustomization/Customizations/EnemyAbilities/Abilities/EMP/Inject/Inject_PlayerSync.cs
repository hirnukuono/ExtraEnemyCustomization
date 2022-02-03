using HarmonyLib;
using Player;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Inject
{
    [HarmonyPatch(typeof(PlayerSync))]
    internal static class Inject_PlayerSync
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerSync.WantsToSetFlashlightEnabled))]
        internal static bool Pre_WantsToSetFlashlightEnabled(PlayerSync __instance, bool enable)
        {
            if (EMPHandlerBase.IsLocalPlayerDisabled)
            {
                return false;
            }

            // This is so fucking stupid but if I don't do this "enable" will always be true for some god-forsaken reason
            if (enable == __instance.m_agent.Inventory.FlashlightEnabled)
            {
                __instance.WantsToSetFlashlightEnabled(!__instance.m_agent.Inventory.FlashlightEnabled);
                return false;
            }
            return true;
        }
    }
}