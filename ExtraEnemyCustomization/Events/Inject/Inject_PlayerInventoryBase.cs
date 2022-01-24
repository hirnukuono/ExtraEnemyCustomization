using Gear;
using HarmonyLib;
using System;

namespace EECustom.Events.Inject
{
    [HarmonyPatch(typeof(PlayerInventoryBase))]
    internal class Inject_PlayerInventoryBase
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerInventoryBase.OnItemEquippableFlashlightWielded))]
        public static void Post_DoWieldItem(GearPartFlashlight flashlight)
        {
            InventoryEvents.OnWieldItem(flashlight);
        }
    }
}
