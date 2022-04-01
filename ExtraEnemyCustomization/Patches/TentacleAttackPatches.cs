using Enemies;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using EEC.Managers;
using System.Linq;
using EEC.EnemyCustomizations.Models;
using EEC.Configs.Customizations;
using EEC.API;

namespace EEC.Patches
{
    [HarmonyPatch(typeof(ES_EnemyAttackBase))]
    internal class TentacleAttackPatches // Need to know where to move this
    {
        [HarmonyPatch(nameof(ES_EnemyAttackBase.DoStartAttack))]
        [HarmonyPrefix]
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
        public static void OnEndAttack(ES_EnemyAttackBase __instance)
        {
            if (__instance.m_enemyAgent.TryGetProperty<TentacleAttackSpeedProperty>(out var attackSpeedProperty))
            {
                __instance.m_locomotion.m_animator.speed /= attackSpeedProperty.TentacleAttackSpeedAdjustmentMult;
            }
        }
    }
}
