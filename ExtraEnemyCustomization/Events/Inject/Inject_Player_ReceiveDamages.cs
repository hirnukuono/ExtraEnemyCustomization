using HarmonyLib;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(Dam_PlayerDamageLocal))]
    internal static class Inject_Player_ReceiveDamages
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Dam_PlayerDamageLocal.ReceiveMeleeDamage))]
        internal static void Post_Melee(pFullDamageData data, Dam_PlayerDamageBase __instance)
        {
            if (data.source.TryGet(out var inflictor))
            {
                var damage = data.damage.Get(__instance.HealthMax);
                LocalPlayerDamageEvents.OnDamage(__instance.Owner, inflictor, damage);
                LocalPlayerDamageEvents.OnMeleeDamage(__instance.Owner, inflictor, damage);
            }
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Dam_PlayerDamageLocal.ReceiveTentacleAttackDamage))]
        internal static void Post_Tentacle(pMediumDamageData data, Dam_PlayerDamageLocal __instance)
        {
            if (data.source.TryGet(out var inflictor))
            {
                var damage = data.damage.Get(__instance.HealthMax);
                LocalPlayerDamageEvents.OnDamage(__instance.Owner, inflictor, damage);
                LocalPlayerDamageEvents.OnTentacleDamage(__instance.Owner, inflictor, damage);
            }
        }
    }
}