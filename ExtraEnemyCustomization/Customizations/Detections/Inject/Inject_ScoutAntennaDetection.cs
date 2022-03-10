using Enemies;
using HarmonyLib;

namespace EECustom.Customizations.Detections.Inject
{
    [HarmonyPatch(typeof(ScoutAntennaDetection), nameof(ScoutAntennaDetection.PlayAbilityOutAnimation))]
    internal static class Inject_ScoutAntennaDetection
    {
        [HarmonyWrapSafe]
        internal static void Postfix(ScoutAntennaDetection __instance)
        {
            if (!__instance.m_owner.TryGetProperty<ScoutAnimOverrideProperty>(out var data))
                return;

            if (data.OverridePullingAnimation)
            {
                data.DoPullingAnim();
            }
        }
    }
}