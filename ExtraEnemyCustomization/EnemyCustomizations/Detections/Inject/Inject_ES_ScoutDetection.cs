using HarmonyLib;
using SNetwork;

namespace EEC.EnemyCustomizations.Detections.Inject
{
    [HarmonyPatch(typeof(ES_ScoutDetection), nameof(ES_ScoutDetection.CommonEnter))]
    internal static class Inject_ES_ScoutDetection
    {
        [HarmonyWrapSafe]
        internal static void Postfix(ES_ScoutDetection __instance)
        {
            if (!__instance.m_enemyAgent.TryGetProperty<ScoutAnimOverrideProperty>(out var data))
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