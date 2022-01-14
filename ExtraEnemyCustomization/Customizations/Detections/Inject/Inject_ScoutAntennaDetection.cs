using EECustom.Networking.Events;
using EECustom.Utils;
using Enemies;
using HarmonyLib;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Detections.Inject
{
    [HarmonyPatch(typeof(ScoutAntennaDetection), nameof(ScoutAntennaDetection.SetState))]
    internal static class Inject_ScoutAntennaDetection
    {
        public static void Postfix(ScoutAntennaDetection __instance, eDetectionState state)
        {
            var data = EnemyProperty<ScoutAnimOverrideData>.Get(__instance.m_owner);
            if (data == null)
                return;

            if (state != eDetectionState.WaitingOut)
                return;

            if (data.AnimDetermined)
            {
                data.DoAnim(data.NextAnim);

                data.NextAnim = ScoutAnimType.Unknown;
                data.BendingWasCalled = false;
                data.AnimDetermined = false;
            }
            else
            {
                data.BendingWasCalled = true;
            }
        }
    }
}
