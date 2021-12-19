using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.CustomSettings.Inject
{
    [HarmonyPatch(typeof(ProjectileBase), nameof(ProjectileBase.Collision))]
    internal class Inject_ProjectileBase
    {
        public static void Prefix()
        {
            //MAJOR: PROBLEM
        }
    }
}
