using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Shared.Inject
{
    [HarmonyPatch(typeof(PUI_LocalPlayerStatus), nameof(PUI_LocalPlayerStatus.UpdateHealth))]
    internal static class Inject_PUI_LocalPlayerStatus
    {
        public static bool IsBleeding = false;

        internal static void Postfix(PUI_LocalPlayerStatus __instance)
        {
            if (IsBleeding)
            {
                __instance.m_healthText.text = $"{__instance.m_healthText.text}\n<color=red><size=75%>BLEEDING</size></color>";
            }
        }
    }
}
