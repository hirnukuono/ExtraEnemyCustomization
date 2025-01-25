using AK;
using Enemies;
using HarmonyLib;

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

            if (SharedRoarData.Dict.TryGetValue(agent.EnemyData.persistentID, out var roarData))
            {
                switchID = roarData.SwitchID;
                SharedRoarData.Dict[agent.EnemyData.persistentID].IsInWave = true;
                __result = true;
            }
                
            return switchID == 0u;   
        }

        [HarmonyPatch(typeof(EnemyGroup), nameof(EnemyGroup.GetByteFromEnemyType))]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        private static bool AppendAKEnemyTypes(ref byte __result, uint enemyType)
        {
            if (enemyType == SWITCHES.ENEMY_TYPE.SWITCH.POUNCER)
            {
                __result = 8;
                return false;
            }
            if (enemyType == SWITCHES.ENEMY_TYPE.SWITCH.STRIKER_BERSERK)
            {
                __result = 9;
                return false;
            }
            if (enemyType == SWITCHES.ENEMY_TYPE.SWITCH.SHOOTER_SPREAD)
            {
                __result = 10;
                return false;
            }
            if (enemyType == 9900u)
            {
                __result = 11;
                return false;
            }
            if (enemyType == 9901u)
            {
                __result = 12;
                return false;
            }
            if (enemyType == 9902u)
            {
                __result = 13;
                return false;
            }

            return true;
        }
    }
}
