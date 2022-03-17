using HarmonyLib;
using SNetwork;
using System;

namespace EEC.Patches.Inject
{
    [HarmonyPatch(typeof(Dam_SyncedDamageBase))]
    internal static class Inject_Patch_HealerEnemies
    {
        [HarmonyPatch(nameof(Dam_SyncedDamageBase.TentacleAttackDamage))]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        internal static bool Pre_TentacleDamage(float dam, Dam_SyncedDamageBase __instance) => DoHealer(dam, __instance);

        [HarmonyPatch(nameof(Dam_SyncedDamageBase.MeleeDamage))]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        internal static bool Pre_PunchDamage(float dam, Dam_SyncedDamageBase __instance) => DoHealer(dam, __instance);

        [HarmonyPatch(nameof(Dam_SyncedDamageBase.ExplosionDamage))]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        internal static bool Pre_ExplosionDamage(float dam, Dam_SyncedDamageBase __instance) => DoHealer(dam, __instance);

        [HarmonyPatch(nameof(Dam_SyncedDamageBase.ShooterProjectileDamage))]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        internal static bool Pre_ShooterProjectileDamage(float dam, Dam_SyncedDamageBase __instance) => DoHealer(dam, __instance);

        private static bool DoHealer(float dam, Dam_SyncedDamageBase damBase)
        {
            if (dam >= 0.0f)
                return true; //Run Original Code

            if (SNet.IsMaster)
            {
                var healAmount = Math.Abs(dam);
                damBase.SendSetHealth(Math.Min(damBase.Health + healAmount, damBase.HealthMax));
            }

            return false; //Skip Original Code
        }
    }
}