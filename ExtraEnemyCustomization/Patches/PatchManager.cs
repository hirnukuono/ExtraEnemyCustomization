using EECustom.Attributes;
using EECustom.Events;
using EECustom.Managers;
using EECustom.Patches.Handlers;
using Enemies;
using SNetwork;

namespace EECustom.Patches
{
    [CallConstructorOnLoad]
    public static class PatchManager
    {
        static PatchManager()
        {
            EnemyEvents.Spawned += Spawned_FlyerCheck;
        }

        private static void Spawned_FlyerCheck(EnemyAgent agent)
        {
            if (!ConfigManager.Global.UsingFlyerStuckCheck)
                return;

            if (!SNet.IsMaster)
                return;

            if (!agent.EnemyBehaviorData.IsFlyer)
                return;

            var flyerHandler = agent.gameObject.AddComponent<FlyerStuckHandler>();
            flyerHandler.Agent = agent;
            flyerHandler.UpdateInterval = ConfigManager.Global.FlyerStuck_Interval;
            flyerHandler.RetryCount = ConfigManager.Global.FlyerStuck_Retry;
        }
    }
}