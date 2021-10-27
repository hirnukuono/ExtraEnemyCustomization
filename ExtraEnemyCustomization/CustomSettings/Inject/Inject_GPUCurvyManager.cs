using EECustom.Managers;
using HarmonyLib;
using System;

namespace EECustom.CustomSettings.Inject
{
    [HarmonyPatch(typeof(GPUCurvyManager))]
    internal class Inject_GPUCurvyManager
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(GPUCurvyManager.Setup))]
        public static void Post_Setup()
        {
            foreach (var tentDef in ConfigManager.Current.TentacleCustom.TentacleDefinitions)
            {
                CustomTentacleManager.GenerateTentacle(tentDef);
            }
        }

        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(GPUCurvyManager.TryAllocateGPUCurvy))]
        public static bool Pre_Allocate(GPUCurvyType type, ref GPUCurvy gpuCurvy)
        {
            var id = (int)type;
            if (Enum.IsDefined(typeof(GPUCurvyType), id))
            {
                return true;
            }

            var setup = CustomTentacleManager.GetTentacle(id);
            if (setup.TryCanAllocate(out gpuCurvy))
            {
                Logger.Error($"ALLOC {id}");
                return false;
            }
            Logger.Error($"CANT FIND TENTACLE SETTING WITH ID: {id}");
            return true;
        }
    }
}