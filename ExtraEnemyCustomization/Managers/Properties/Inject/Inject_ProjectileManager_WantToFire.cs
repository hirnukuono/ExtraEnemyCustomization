using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Managers.Properties.Inject
{
    [HarmonyPatch(typeof(ProjectileManager), nameof(ProjectileManager.WantToFireTargeting))]
    internal static class Inject_ProjectileManager_WantToFire
    {
        internal static void Prefix(ref int burstSize)
        {
            if (Inject_EAB_Shooter.LastAgent != null)
                burstSize = Inject_EAB_Shooter.LastAgent.GlobalID;
        }
    }
}
