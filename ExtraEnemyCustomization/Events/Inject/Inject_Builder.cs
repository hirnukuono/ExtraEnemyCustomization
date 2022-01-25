using HarmonyLib;
using LevelGeneration;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(Builder))]
    internal static class Inject_Builder
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Builder.Build))]
        internal static void Pre_BuildStart()
        {
            LevelEvents.OnBuildStart();
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Builder.OnFactoryDone))]
        internal static void Post_BuildDone()
        {
            LevelEvents.OnBuildDone();
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Builder.OnLevelCleanup))]
        internal static void Post_LevelCleanup()
        {
            LevelEvents.OnLevelCleanup();
        }
    }
}