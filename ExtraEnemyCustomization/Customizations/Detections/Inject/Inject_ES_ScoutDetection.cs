using EECustom.Utils;
using HarmonyLib;
using SNetwork;

namespace EECustom.Customizations.Detections.Inject
{
    [HarmonyPatch(typeof(ES_ScoutDetection), nameof(ES_ScoutDetection.CommonEnter))]
    internal static class Inject_ES_ScoutDetection
    {
        public static void Postfix(ES_ScoutDetection __instance)
        {
            var data = EnemyProperty<ScoutAnimOverrideData>.Get(__instance.m_enemyAgent);
            if (data == null)
                return;

            if (SNet.IsMaster)
                ScoutAnimCustom._animSync.DoRandom(__instance.m_owner);

            if (data.AnimDetermined)
            {
                data.DoAnim(data.NextAnim);
                data.AnimDetermined = false;
            }

            data.EnterWasCalled = true;
        }
    }
}