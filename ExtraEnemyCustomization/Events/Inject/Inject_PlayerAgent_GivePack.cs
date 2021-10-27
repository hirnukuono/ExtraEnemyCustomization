using Gear;
using HarmonyLib;
using Player;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(PlayerAgent))]
    internal class Inject_PlayerAgent_GivePack
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerAgent.GiveHealth))]
        private static void Post_Health(PlayerAgent __instance)
        {
            ResourcePackEvents.OnReceiveMedi?.Invoke(__instance.Cast<iResourcePackReceiver>());
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerAgent.GiveAmmoRel))]
        private static void Post_Ammo(float ammoStandardRel, float ammoSpecialRel, float ammoClassRel, PlayerAgent __instance)
        {
            if (ammoStandardRel > 0.0f || ammoSpecialRel > 0.0f)
                ResourcePackEvents.OnReceiveAmmo?.Invoke(__instance.Cast<iResourcePackReceiver>());
            if (ammoClassRel > 0.0f)
                ResourcePackEvents.OnReceiveTool?.Invoke(__instance.Cast<iResourcePackReceiver>());
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerAgent.GiveDisinfection))]
        private static void Post_Disinect(PlayerAgent __instance)
        {
            ResourcePackEvents.OnReceiveDisinfect?.Invoke(__instance.Cast<iResourcePackReceiver>());
        }
    }
}