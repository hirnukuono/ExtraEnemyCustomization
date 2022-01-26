using AssetShards;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(AssetShardManager), nameof(AssetShardManager.LoadAllShardsForBundleAsync))]
    internal static class Inject_AssetShardManager
    {
        //necessary evil: This ensures the timing of assetLoaded callback always after base-game callback has finished.
        [HarmonyWrapSafe]
        internal static void Prefix(AssetBundleName name, ref Il2CppSystem.Action callback)
        {
            switch (name)
            {
                case AssetBundleName.Enemies:
                    callback += (Action)AssetEvents.OnEnemyAssetLoaded;
                    break;

                case AssetBundleName.Complex_Shared:
                    callback += (Action)AssetEvents.OnShardAssetLoaded;
                    break;

                case AssetBundleName.Startup:
                    callback += (Action)AssetEvents.OnStartupAssetLoaded;
                    break;
            }
        }
    }
}
