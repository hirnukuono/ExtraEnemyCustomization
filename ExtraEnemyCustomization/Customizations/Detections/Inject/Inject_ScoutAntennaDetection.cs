using EECustom.Utils;
using Enemies;
using HarmonyLib;

namespace EECustom.Customizations.Detections.Inject
{
    [HarmonyPatch(typeof(ScoutAntennaDetection), nameof(ScoutAntennaDetection.PlayAbilityOutAnimation))]
    internal static class Inject_ScoutAntennaDetection
    {
        public static void Postfix(ScoutAntennaDetection __instance)
        {
            var data = EnemyProperty<ScoutAnimOverrideData>.Get(__instance.m_owner);
            if (data == null)
                return;

            if (data.OverridePullingAnimation)
            {
                data.DoPullingAnim();
            }
        }
    }
}