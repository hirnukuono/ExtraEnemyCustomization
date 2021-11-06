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
            var damage = data.damage.Get(__instance.HealthMax);

            EnemyDamageEvents.OnDamage(__instance.Owner, agent, damage);
            EnemyDamageEvents.OnBulletDamage(__instance.Owner, agent, damage);
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Dam_EnemyDamageBase.ReceiveMeleeDamage))]
        public static void Post_MeleeDamage(pFullDamageData data, Dam_EnemyDamageBase __instance)
        {
            data.source.TryGet(out var agent);
            var damage = data.damage.Get(__instance.HealthMax);

            EnemyDamageEvents.OnDamage(__instance.Owner, agent, damage);
            EnemyDamageEvents.OnMeleeDamage(__instance.Owner, agent, damage);
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Dam_EnemyDamageBase.ReceiveExplosionDamage))]
        public static void Post_ExplosionDamage(pFullDamageData data, Dam_EnemyDamageBase __instance)
        {
            data.source.TryGet(out var agent);
            var damage = data.damage.Get(__instance.HealthMax);

            EnemyDamageEvents.OnDamage(__instance.Owner, agent, damage);
            EnemyDamageEvents.OnExplosionDamage(__instance.Owner, agent, damage);
        }
    }
}