using HarmonyLib;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(Dam_EnemyDamageBase))]
    internal class Inject_Enemy_RecieveDamages
    {
        //NOTE: Hooking Dam_EnemyDamageBase.ProcessReceivedDamage will cause various problem, such as unwanted hitreact.

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Dam_EnemyDamageBase.ReceiveBulletDamage))]
        public static void Post_BulletDamage(pBulletDamageData data, Dam_EnemyDamageBase __instance)
        {
            data.source.TryGet(out var agent);
            EnemyDamageEvents.OnDamage?.Invoke(__instance.Owner, agent);
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Dam_EnemyDamageBase.ReceiveMeleeDamage))]
        public static void Post_MeleeDamage(pFullDamageData data, Dam_EnemyDamageBase __instance)
        {
            data.source.TryGet(out var agent);
            EnemyDamageEvents.OnDamage?.Invoke(__instance.Owner, agent);
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Dam_EnemyDamageBase.ReceiveExplosionDamage))]
        public static void Post_ExplosionDamage(pFullDamageData data, Dam_EnemyDamageBase __instance)
        {
            data.source.TryGet(out var agent);
            EnemyDamageEvents.OnDamage?.Invoke(__instance.Owner, agent);
        }
    }
}