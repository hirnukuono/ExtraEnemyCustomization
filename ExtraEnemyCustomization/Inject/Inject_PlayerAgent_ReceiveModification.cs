using EEC.CustomAbilities.FogHealth;
using HarmonyLib;
using Player;

namespace ExtraEnemyCustomization.Inject
{
    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.ReceiveModification))]
    internal static class Inject_PlayerAgent_ReceiveModification
    {
        // Health change is not implemented at all in vanilla and prints an error log.
        [HarmonyWrapSafe]
        [HarmonyPriority(Priority.Low)]
        private static void Prefix(PlayerAgent __instance, ref EV_ModificationData data)
        {
            // If no health modifications, let it proceed as normal
            float health = data.health;
            if (health == 0f) return;

            // Otherwise, set health change to 0 to avoid ugly error log
            data.health = 0f;

            FogHealthManager.DoHealthChange(__instance, health);
        }
    }
}
