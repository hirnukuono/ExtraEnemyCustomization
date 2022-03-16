using GameData;
using HarmonyLib;
using Localization;

namespace EECustom.CustomAbilities.Bleed.Inject
{
    [HarmonyPatch(typeof(PUI_LocalPlayerStatus), nameof(PUI_LocalPlayerStatus.UpdateHealth))]
    internal static class Inject_PUI_LocalPlayerStatus
    {
        public static bool IsBleeding = false;

        private static bool _hasOverrideText = false;
        private static string _overrideText = string.Empty;

        public static uint SpecialOverrideTextID
        {
            set
            {
                if (value == 0u)
                {
                    _hasOverrideText = false;
                    _overrideText = string.Empty;
                    return;
                }

                var text = Text.Get(value);
                if (!string.IsNullOrEmpty(text))
                {
                    _hasOverrideText = true;
                    _overrideText = string.Empty;
                }
                else
                {
                    _hasOverrideText = false;
                    _overrideText = string.Empty;
                }
            }
        }

        internal static void Postfix(PUI_LocalPlayerStatus __instance)
        {
            if (IsBleeding)
            {
                if (_hasOverrideText)
                {
                    __instance.m_healthText.text = string.Format(_overrideText, __instance.m_healthText.text);
                }
                else
                {
                    __instance.m_healthText.text = $"{__instance.m_healthText.text}\n<color=red><size=75%>BLEEDING</size></color>";
                }
            }
        }
    }
}