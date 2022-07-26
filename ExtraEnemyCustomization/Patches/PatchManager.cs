using EEC.Managers;
using EEC.Patches.Handlers;
using Enemies;
using GameData;

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

            if (!enemyData.TryGetBehaviourBlock(out var enemyBehaviourBlock))
                return;

            if (!enemyBehaviourBlock.IsFlyer)
                return;

            var handler = agent.gameObject.AddComponent<FlyerStuckHandler>();
            handler.Agent.Value = agent;

            Logger.Debug($"Added Flyer Check to {enemyData.persistentID}");
        }
    }
}