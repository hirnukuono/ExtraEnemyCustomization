using HarmonyLib;
using LevelGeneration;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(Builder))]
    internal class Inject_Builder
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Builder.Build))]
        public static void Pre_BuildStart()
        {
            LevelEvents.OnBuildStart();
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Builder.OnFactoryDone))]
        public static void Post_BuildDone()
        {
            LevelEvents.OnBuildDone();
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(Builder.OnLevelCleanup))]
        public static void Post_LevelCleanup()
        {
            LevelEvents.OnLevelCleanup();
        }
    }
}