using EECustom.Networking.Events;
using EECustom.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Detections.Inject
{
    [HarmonyPatch(typeof(ES_ScoutDetection), nameof(ES_ScoutDetection.CommonEnter))]
    internal static class Inject_ES_ScoutDetection
    {
        public static bool Prefix(ES_ScoutDetection __instance)
        {
            var data = EnemyProperty<ScoutAnimOverrideData>.Get(__instance.m_enemyAgent);
            if (data == null)
                return true;

            ScoutAnimCustom._animSync.DoRandom(__instance.m_owner);
            __instance.m_enemyAgent.MovingCuller.UpdatePosition(__instance.m_enemyAgent.DimensionIndex, __instance.m_enemyAgent.Position);
            __instance.m_enemyAgent.Locomotion.m_animator.applyRootMotion = true;
            return false;
        }
    }
}
