using EEC.Managers;
using Enemies;
using HarmonyLib;
using UnityEngine;

namespace EEC.EnemyCustomizations.Properties.Inject
{
    [HarmonyPatch(typeof(EnemyGroup), nameof(EnemyGroup.TryGetAKSwitchIDFromEnemyType))]
    internal static class Inject_EnemyGroup
    {
        private static readonly Dictionary<string, uint> v_WaveRoars = new()
        {
            { "None", 0u },
            { "Striker", 3129078391u },
            { "Shooter", 2586696975u },
            { "Bullrush", 2175837293u },
            { "Shadow", 3140781661u },
            { "Flyer", 918167457u },
            { "Tank", 3206747537u },
            { "Birther", 2995743377u },
            { "Pouncer", 4217125911u }
        };

        [HarmonyWrapSafe]
        public static bool Prefix(EnemyAgent agent, out uint switchID)
        {
            try
            {
                var roar = ConfigManager.PropertyCustom.DistantRoarCustom
                    .FirstOrDefault(q => q.IsTarget(agent.EnemyData.persistentID));

                if (roar != null && v_WaveRoars.TryGetValue(roar.WaveRoarOverride.ToString(), out switchID))
                {
                    Debug.Log($"[EEC] - Custom EnemyAgent: {agent.ToString()}, new switchID: {switchID}");
                    return switchID == 0u; 
                }

                switchID = 0u;
                return true; 
            }
            catch (Exception e)
            {
                Debug.LogError($"[EEC] - Something somewhere went wrong...\n{e}");
                switchID = 0u;
                return true; 
            }
        }
    }
}
