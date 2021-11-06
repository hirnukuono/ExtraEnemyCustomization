using HarmonyLib;

namespace EECustom.Customizations.EnemyAbilities.Events.Inject
{
    [HarmonyPatch(typeof(Enemies.EnemyAbilities))]
    internal class Inject_EnemyAbilities
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Enemies.EnemyAbilities.OnTakeDamage))]
        private static void Post_OnTakeDamage(float damage, Enemies.EnemyAbilities __instance)
        {
            EnemyAbilitiesEvents.OnTakeDamage(__instance, damage);
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Enemies.EnemyAbilities.OnHitreact))]
        private static void Post_OnHitreact(Enemies.EnemyAbilities __instance)
        {
            EnemyAbilitiesEvents.OnHitreact(__instance);
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Enemies.EnemyAbilities.OnDead))]
        private static void Post_OnDead(Enemies.EnemyAbilities __instance)
        {
            EnemyAbilitiesEvents.OnDead(__instance);
        }
    }
}
