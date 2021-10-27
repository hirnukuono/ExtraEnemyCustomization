using HarmonyLib;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(Dam_PlayerDamageLocal))]
    internal class Inject_PlayerAgent_ReceiveDamages
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Dam_PlayerDamageLocal.ReceiveMeleeDamage))]
        public static void Post_Melee(pFullDamageData data, Dam_PlayerDamageBase __instance)
        {
            if (data.source.TryGet(out var inflictor))
            {
                var damage = data.damage.Get(__instance.HealthMax);
                LocalPlayerDamageEvents.OnDamage?.Invoke(__instance.Owner, inflictor, damage);
                LocalPlayerDamageEvents.OnMeleeDamage?.Invoke(__instance.Owner, inflictor, damage);
            }
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Dam_PlayerDamageLocal.ReceiveTentacleAttackDamage))]
        public static void Post_Tentacle(pMediumDamageData data, Dam_PlayerDamageLocal __instance)
        {
            if (data.source.TryGet(out var inflictor))
            {
                var damage = data.damage.Get(__instance.HealthMax);
                LocalPlayerDamageEvents.OnDamage?.Invoke(__instance.Owner, inflictor, damage);
                LocalPlayerDamageEvents.OnTentacleDamage?.Invoke(__instance.Owner, inflictor, damage);
            }
        }
    }
}