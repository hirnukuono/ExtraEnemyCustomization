using EECustom.Managers;
using EECustom.Patches.Handlers;
using Enemies;
using HarmonyLib;
using SNetwork;

namespace EECustom.Patches.Inject
{
    [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.Setup))]
    internal static class Inject_Patch_FlyerStuck
    {
        [HarmonyWrapSafe]
        internal static void Postfix(EnemyAgent __instance)
        {
            if (!ConfigManager.Global.UsingFlyerStuckCheck)
                return;

            if (!SNet.IsMaster)
                return;

            if (!__instance.EnemyBehaviorData.IsFlyer)
                return;

            var flyerHandler = __instance.gameObject.AddComponent<FlyerStuckHandler>();
            flyerHandler.Agent = __instance;
            flyerHandler.UpdateInterval = ConfigManager.Global.FlyerStuck_Interval;
            flyerHandler.RetryCount = ConfigManager.Global.FlyerStuck_Retry;
        }
    }
}