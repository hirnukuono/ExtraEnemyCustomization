using Gear;
using HarmonyLib;

namespace EEC.Events.Inject
{
    [HarmonyPatch(typeof(PlayerInventoryBase))]
    internal static class Inject_PlayerInventoryBase
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(nameof(PlayerInventoryBase.OnItemEquippableFlashlightWielded))]
        internal static void Post_DoWieldItem(GearPartFlashlight flashlight)
        {
            InventoryEvents.OnWieldItem(flashlight);
        }
    }
}