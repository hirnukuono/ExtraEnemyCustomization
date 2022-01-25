using HarmonyLib;

namespace EECustom.Customizations.EnemyAbilities.Events.Inject
{
    [HarmonyPatch(typeof(Enemies.EnemyAbilities))]
    internal static class Inject_EnemyAbilities
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Enemies.EnemyAbilities.OnTakeDamage))]
        internal static void Post_OnTakeDamage(float damage, Enemies.EnemyAbilities __instance)
        {
            EnemyAbilitiesEvents.OnTakeDamage(__instance, damage);
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Enemies.EnemyAbilities.OnHitreact))]
        internal static void Post_OnHitreact(Enemies.EnemyAbilities __instance)
        {
            EnemyAbilitiesEvents.OnHitreact(__instance);
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Enemies.EnemyAbilities.OnDead))]
        internal static void Post_OnDead(Enemies.EnemyAbilities __instance)
        {
            EnemyAbilitiesEvents.OnDead(__instance);
        }
    }
}