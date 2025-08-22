using Enemies;
using HarmonyLib;
using SWITCH_ID = AK.SWITCHES.ENEMY_TYPE.SWITCH;

namespace EEC.EnemyCustomizations.Properties.Inject
{
    [HarmonyPatch]
    internal static class Inject_EnemyGroup
    {
        [HarmonyPatch(typeof(EnemyGroup), nameof(EnemyGroup.TryGetAKSwitchIDFromEnemyType))]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        private static bool OverrideSwitchID(ref bool __result, EnemyAgent agent, out uint switchID)
        {
            switchID = 0u;

            if (DistantRoarCustom.SharedRoarData.TryGetValue(agent.EnemyData.persistentID, out var roarData))
            {
                switchID = roarData.SwitchID;
                roarData.IsInWave = true;
                __result = true;
            }
                
            return switchID == 0u;   
        }

        [HarmonyPatch(typeof(EnemyGroup), nameof(EnemyGroup.GetByteFromEnemyType))]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        private static bool AppendAKEnemyTypes(ref byte __result, uint enemyType)
        {
            if (enemyType == SWITCH_ID.POUNCER)
            {
                __result = 8;
                return false;
            }
            if (enemyType == SWITCH_ID.STRIKER_BERSERK)
            {
                __result = 9;
                return false;
            }
            if (enemyType == SWITCH_ID.SHOOTER_SPREAD)
            {
                __result = 10;
                return false;
            }
            if (enemyType >= 11u && enemyType <= 14u)
            {
                __result = (byte)enemyType;
                return false;
            }

            return true;
        }
    }
}
