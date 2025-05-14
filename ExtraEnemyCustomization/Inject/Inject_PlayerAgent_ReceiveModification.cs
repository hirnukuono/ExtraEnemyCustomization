using HarmonyLib;
using Player;
using UnityEngine;

namespace ExtraEnemyCustomization.Inject
{
    [HarmonyPatch(typeof(PlayerAgent), nameof(PlayerAgent.ReceiveModification))]
    internal static class Inject_PlayerAgent_ReceiveModification
    {
        // Custom value for the amount the screen is flashed by damage.
        // Vanilla GTFO uses damage / 2f, but non-relativity is cringe.
        private const float FLASH_CONVERSION = 6f;

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

            var damBase = __instance.Damage;
            if (health > 0f)
                damBase.AddHealth(health, null);
            else
            {
                health = -health;
                if (__instance.IsLocallyOwned)
                    __instance.FPSCamera.AddHitReact(health / damBase.HealthMax * FLASH_CONVERSION, Vector3.up, 0f);
                damBase.OnIncomingDamage(health, health);
            }
        }
    }
}
