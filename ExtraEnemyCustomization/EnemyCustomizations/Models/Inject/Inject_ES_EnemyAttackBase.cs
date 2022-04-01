using Enemies;
using HarmonyLib;

namespace EEC.EnemyCustomizations.Models.Inject
{
    [HarmonyPatch(typeof(ES_EnemyAttackBase))]
    internal static class Inject_ES_EnemyAttackBase
    {
        [HarmonyPatch(nameof(ES_EnemyAttackBase.DoStartAttack))]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void OnStartAttack(ES_EnemyAttackBase __instance)
        {
            if (__instance.m_enemyAgent.TryGetProperty<TentacleAttackSpeedProperty>(out var attackSpeedProperty))
            {
                __instance.m_locomotion.m_animator.speed *= attackSpeedProperty.TentacleAttackSpeedAdjustmentMult;
            }

            if (!SNetwork.SNet.IsMaster) // bug fix for clients not seeing tentacles
                __instance.m_attackWindupDuration = __instance.m_enemyAgent.Locomotion.AnimHandle.TentacleAttackWindUpLen / __instance.m_enemyAgent.Locomotion.AnimSpeedOrg;
        }

        [HarmonyPatch(nameof(ES_EnemyAttackBase.SyncExit))]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void OnEndAttack(ES_EnemyAttackBase __instance)
        {
            if (__instance.m_enemyAgent.TryGetProperty<TentacleAttackSpeedProperty>(out var attackSpeedProperty))
            {
                __instance.m_locomotion.m_animator.speed /= attackSpeedProperty.TentacleAttackSpeedAdjustmentMult;
            }
        }
    }
}
