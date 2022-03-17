using EEC.Attributes;
using EEC.Managers;
using EEC.Patches.Handlers;
using Enemies;
using GameData;
using SNetwork;

namespace EEC.Patches
{
    [CallConstructorOnLoad]
    public static class PatchManager
    {
        static PatchManager()
        {
            ConfigManager.EnemyPrefabBuilt += PrefabBuilt;
        }

        private static void PrefabBuilt(EnemyAgent agent, EnemyDataBlock enemyData)
        {
            if (!ConfigManager.Global.UsingFlyerStuckCheck)
                return;

            var enemyBehaviourBlock = EnemyBehaviorDataBlock.GetBlock(enemyData.BehaviorDataId);
            if (enemyBehaviourBlock == null)
                return;

            if (!enemyBehaviourBlock.IsFlyer)
                return;

            agent.gameObject.AddComponent<FlyerStuckHandler>();
            Logger.Debug($"Added Flyer Check to {enemyData.persistentID}");
        }
    }
}